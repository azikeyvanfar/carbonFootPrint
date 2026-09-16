using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Log;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.Persistence.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ContractorBackend.WebApiClient.Filters
{
    // 1. Global Exception Handler Middleware
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Dictionary<Type, Func<HttpContext, Exception, IActionResult>> ExceptionHandlers = new()
        {
            [typeof(ValidationException)] = HandleValidation,
            [typeof(BadRequestException)] = HandleBadRequest,
            [typeof(NotFoundException)] = HandleNotFound,
            [typeof(UnauthorizedAccessException)] = HandleUnauthorized,
            [typeof(ForbiddenAccessException)] = HandleForbidden,
            [typeof(SecurityTokenExpiredException)] = HandleTokenExpired,
            [typeof(DbException)] = HandleDb,
            [typeof(DBConcurrencyException)] = HandleConcurrency,
            [typeof(DbUpdateException)] = HandleDbUpdate,
            [typeof(DbUpdateConcurrencyException)] = HandleDbUpdateConcurrency,
            [typeof(AggregateException)] = HandleAggregate,
            [typeof(CustomException)] = HandleCustom,
            [typeof(Exception)] = HandleUnknown
        };

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            IActionResult result;

            // ModelState validation
            var modelState = context.Features.Get<IValidationProblemDetailsFactory>();
            if (modelState != null && !context.GetEndpoint()?.Metadata.GetMetadata<DisableRequestSizeLimitAttribute>()?.DisableRequestSizeLimit == true)
            {
                result = HandleInvalidModelState(context);
            }
            else
            {
                var exceptionType = exception.GetType();
                result = ExceptionHandlers.TryGetValue(exceptionType, out var handler)
                    ? handler(context, exception)
                    : HandleUnknown(context, exception);
            }

            // Queue error for background processing
            _ = Task.Run(() => QueueErrorForBackgroundProcessing(context, exception, result));

            var objectResult = result as ObjectResult;
            context.Response.StatusCode = objectResult?.StatusCode ?? 500;

            await context.Response.WriteAsync(objectResult?.Value?.ToString() ?? "{}");
        }

        // Background Error Queue
        private static readonly Queue<ErrorQueueItem> ErrorQueue = new();
        private static readonly SemaphoreSlim QueueSemaphore = new(1, 1);
        private static readonly AutoResetEvent QueueSignal = new(false);

        private static async Task QueueErrorForBackgroundProcessing(HttpContext context, Exception exception, IActionResult result)
        {
            var errorItem = new ErrorQueueItem
            {
                ContextData = new ErrorContextData
                {
                    UserId = context.GetUserId(),
                    Route = string.Join(":", context.Request.RouteValues.Values),
                    ActionName = context.GetEndpoint()?.DisplayName,
                    Timestamp = DateTime.UtcNow,
                    Exception = exception,
                    ErrorCode = context.GetEndpoint()?.Metadata.GetMetadata<ErrorCodeAttribute>()?.ErrorCode ?? "",
                    ClientIP = context.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = context.Request.Headers["User-Agent"].FirstOrDefault()
                }
            };

            await QueueSemaphore.WaitAsync();
            try
            {
                ErrorQueue.Enqueue(errorItem);
                QueueSignal.Set(); // Signal background job
            }
            finally
            {
                QueueSemaphore.Release();
            }
        }

        // Handler Methods (مشابه قبلی)
        private static IActionResult HandleValidation(HttpContext context, Exception ex)
        {
            var validationEx = (ValidationException)ex;
            return Results.ValidationProblem(validationEx.Errors);
        }

        private static IActionResult HandleInvalidModelState(HttpContext context)
        {
            var modelStateDict = new Dictionary<string, string[]>
            {
                [""] = new[] { "خطا در فرمت ورودی" }
            };
            return Results.ValidationProblem(modelStateDict);
        }

        private static IActionResult HandleBadRequest(HttpContext _, Exception __) =>
            Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.5.1", statusCode: 400);

        private static IActionResult HandleNotFound(HttpContext _, Exception __) =>
            Results.NotFound();

        private static IActionResult HandleUnauthorized(HttpContext _, Exception __) =>
            Results.Unauthorized();

        private static IActionResult HandleForbidden(HttpContext _, Exception __) =>
            Results.Forbidden();

        private static IActionResult HandleTokenExpired(HttpContext _, Exception __) =>
            Results.Unauthorized();

        private static IActionResult HandleDb(HttpContext _, Exception __) =>
            Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.6.1", statusCode: 500);

        private static IActionResult HandleConcurrency(HttpContext _, Exception __) =>
            Results.Conflict();

        private static IActionResult HandleDbUpdate(HttpContext _, Exception __) =>
            Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.6.1", statusCode: 500);

        private static IActionResult HandleDbUpdateConcurrency(HttpContext _, Exception __) =>
            Results.Conflict();

        private static IActionResult HandleAggregate(HttpContext _, Exception ex)
        {
            var aggEx = (AggregateException)ex;
            return Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.6.1",
                statusCode: 500, extensions: new { InnerExceptions = aggEx.InnerExceptions.Count });
        }

        private static IActionResult HandleCustom(HttpContext _, Exception ex)
        {
            var customEx = (CustomException)ex;
            return Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.6.1",
                statusCode: 500, extensions: new { Errors = customEx.Errors });
        }

        private static IActionResult HandleUnknown(HttpContext _, Exception __) =>
            Results.Problem("https://tools.ietf.org/html/rfc7231#section-6.6.1", statusCode: 500);
    }

    // 2. Background Error Processing Service
    public class BackgroundErrorProcessor : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<BackgroundErrorProcessor> _logger;
        private readonly ErrorLoggingOptions _options;

        public BackgroundErrorProcessor(
            IServiceProvider serviceProvider,
            ILogger<BackgroundErrorProcessor> logger,
            IOptions<ErrorLoggingOptions> options)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessErrorQueueAsync(stoppingToken);
                    await Task.Delay(1000, stoppingToken); // 1 second poll
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in background error processor");
                    await Task.Delay(5000, stoppingToken); // Backoff
                }
            }
        }

        private async Task ProcessErrorQueueAsync(CancellationToken cancellationToken)
        {
            if (!_options.EnableErrorLogging) return;

            var signalled = GlobalExceptionHandlerMiddleware.QueueSignal.WaitOne(1000);
            if (!signalled) return;

            await GlobalExceptionHandlerMiddleware.QueueSemaphore.WaitAsync(cancellationToken);
            try
            {
                while (GlobalExceptionHandlerMiddleware.ErrorQueue.Count > 0 && !cancellationToken.IsCancellationRequested)
                {
                    var errorItem = GlobalExceptionHandlerMiddleware.ErrorQueue.Dequeue();
                    await SaveErrorToDatabaseAsync(errorItem);
                }
            }
            finally
            {
                GlobalExceptionHandlerMiddleware.QueueSemaphore.Release();
            }
        }

        private async Task SaveErrorToDatabaseAsync(ErrorQueueItem errorItem)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<LogDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BackgroundErrorProcessor>>();

            try
            {
                var errorHistory = new ErrorHistory
                {
                    Message = $"{errorItem.ContextData.Exception.InnerException?.Message} | {errorItem.ContextData.Exception.Message}",
                    Exception = errorItem.ContextData.Exception.StackTrace ?? "",
                    LogEvent = errorItem.ContextData.ErrorCode,
                    Route = errorItem.ContextData.Route,
                    Description = errorItem.ContextData.ActionName,
                    ProjectType = LogType.Admin,
                    TimeStamp = errorItem.ContextData.Timestamp,
                    UserId = errorItem.ContextData.UserId,
                    ClientIP = errorItem.ContextData.ClientIP,
                    UserAgent = errorItem.ContextData.UserAgent
                };

                dbContext.ErrorHistories.Add(errorHistory);
                await dbContext.SaveChangesAsync();

                errorItem.ErrorId = errorHistory.Id;
                logger.LogInformation("Error logged: {ErrorId}", errorHistory.Id);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save error to database");
            }
        }
    }

    // 3. Data Models
    public class ErrorQueueItem
    {
        public ErrorContextData ContextData { get; set; } = null!;
        public long ErrorId { get; set; }
    }

    public class ErrorContextData
    {
        public long? UserId { get; set; }
        public string Route { get; set; } = "";
        public string? ActionName { get; set; }
        public DateTime Timestamp { get; set; }
        public Exception Exception { get; set; } = null!;
        public string ErrorCode { get; set; } = "";
        public string? ClientIP { get; set; }
        public string? UserAgent { get; set; }
    }
}
