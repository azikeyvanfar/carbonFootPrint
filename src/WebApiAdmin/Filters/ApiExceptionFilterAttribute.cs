using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using ContractorBackend.Application.Common.Exceptions;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using ContractorBackend.Persistence.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace ContractorBackend.WebApiAdmin.Filters
{
    public class ApiExceptionFilterAttribute : ExceptionFilterAttribute
    {

        private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
        private readonly LogDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ApiExceptionFilterAttribute(LogDbContext dbContext, IWebHostEnvironment env, IConfiguration configuration)
        {
            _dbContext = dbContext;
            // Register known exception types and handlers.
            _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
            {
                { typeof(ValidationException), HandleValidationException },
                { typeof(BadRequestException), HandleBadRequestException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
                { typeof(ForbiddenAccessException), HandleForbiddenAccessException },
                { typeof(SecurityTokenExpiredException), HandleSecurityTokenExpiredException },
                { typeof(DbException), HandleDbException },
                { typeof(DBConcurrencyException), HandleDBConcurrencyException },
                { typeof(DbUpdateException), HandleDbUpdateException },  // validation PK/FK
                { typeof(DbUpdateConcurrencyException), HandleDbUpdateConcurrencyException },
                { typeof(AggregateException),HandleAggregateException},
                { typeof(CustomException),HandleCustomException },
                { typeof(Exception),HandleOtherException },

            };
            _env = env;
            _configuration = configuration;
        }

        public override void OnException(ExceptionContext context)
        {

            HandleException(context);
            // AddErrorCodeAttributeToExceptionResult(context);
            //if (showFullError)
            //{
            //    AttachFullErrorTextToExceptionResult(context);
            //}
            base.OnException(context);
        }

        private void HandleException(ExceptionContext context)
        {
            Type type = context.Exception.GetType();

            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context);
                if (type == typeof(CustomException) || type == typeof(ValidationException))
                {
                    AttachFullErrorTextToExceptionResult(context);
                }
                else
                {
                    Save(context);

                }

                return;
            }

            if (!context.ModelState.IsValid)
            {
                HandleInvalidModelStateException(context);
                Save(context);
                return;
            }

            HandleUnknownException(context);
            Save(context);
        }

        private void HandleValidationException(ExceptionContext context)
        {
            var exception = context.Exception as ValidationException;

            var details = new ValidationProblemDetails(exception?.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status400BadRequest,


            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleInvalidModelStateException(ExceptionContext context)
        {
            // if Exception root of this Unhandeled Error is Null considered as Json Parse Error and Rewrite Message 
            foreach (var item in context.ModelState)
            {
                var errors = item.Value.Errors.ToList();
                foreach (var error in errors)
                {
                    if (error.Exception is null)
                    {
                        context.ModelState[item.Key].Errors.Remove(error);
                        context.ModelState.AddModelError(item.Key, "خطا در فرمت ورودی");
                    }
                }
            }


            var details = new ValidationProblemDetails(context.ModelState)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleBadRequestException(ExceptionContext context)
        {
            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Status = StatusCodes.Status500InternalServerError,
                // Detail=context.Exception.Message
            };

            context.Result = new BadRequestObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleNotFoundException(ExceptionContext context)
        {
            var details = new ProblemDetails()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new NotFoundObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleUnauthorizedAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleForbiddenAccessException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleSecurityTokenExpiredException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleDbException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleDBConcurrencyException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,

            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleDbUpdateException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleDbUpdateConcurrencyException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleUnknownException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        public void HandleAggregateException(ExceptionContext context)
        {
            var exceptions = context.Exception as AggregateException;

            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void HandleCustomException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError,
                Detail = context.Exception != null ? JsonConvert.SerializeObject((context.Exception as CustomException).Errors) : null
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }
        private void HandleOtherException(ExceptionContext context)
        {
            var details = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = StatusCodes.Status500InternalServerError
            };

            context.Result = new ObjectResult(details);

            context.ExceptionHandled = true;
        }

        private void AddErrorCodeAttributeToExceptionResult(ExceptionContext context)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata.OfType<ErrorCodeAttribute>().SingleOrDefault()?.ErrorCode ?? "";

            ((context.Result as ObjectResult).Value as ProblemDetails).Detail = attribute;
        }

        private void AttachFullErrorTextToExceptionResult(ExceptionContext context)
        {
            string errorText = /*context.Exception.InnerException?.Message +*/ context.Exception?.Message ?? "";
            ((context.Result as ObjectResult).Value as ProblemDetails).Title = errorText;
        }
        private void Save(ExceptionContext context)
        {
            long? userId = null;
            try
            {
                userId = context.HttpContext.GetUserId();
            }
            catch (Exception)
            {
            }

            var errorHistory = new ErrorHistory
            {
                Message = context.Exception.InnerException?.Message + context.Exception?.Message ?? "",
                Exception = context.Exception?.StackTrace ?? "",
                LogEvent = context.ActionDescriptor.EndpointMetadata.OfType<ErrorCodeAttribute>().SingleOrDefault()?.ErrorCode ?? "",
                Route = string.Join(":", context.RouteData.Values.Values),
                Description = context.ActionDescriptor.DisplayName,
                ProjectType = LogType.Admin,
                TimeStamp = DateTime.Now,
                UserId = userId

            };

            var setting = _dbContext.ErrorHistories.Add(errorHistory);
            _dbContext.SaveChanges();
            ((context.Result as ObjectResult).Value as ProblemDetails).Title =
                string.Format("خطای کد {0} رخ داده است. لطفا با پشتیبانی تماس بگیرید.", errorHistory.Id.ToString());


        }
    }
}
