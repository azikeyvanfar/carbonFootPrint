using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Services;
using ContractorBackend.Domain.Entities.Ghg;
using Gridify;
using Gridify.EntityFramework;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg.Formulas
{
    // ---------------- Queries ----------------

    /// <summary>
    /// لیست فرمول‌های محاسبه
    /// </summary>
    public class GetAllFormulasQuery : SearchQueryRequest, IRequest<SearchQueryResponse<CalculationFormulaDto>>
    {
        public int? Category { get; set; }
    }

    public class GetAllFormulasQueryHandler : IRequestHandler<GetAllFormulasQuery, SearchQueryResponse<CalculationFormulaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllFormulasQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<SearchQueryResponse<CalculationFormulaDto>> Handle(GetAllFormulasQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CalculationFormulas.AsNoTracking().Where(x => x.IsActive);
            if (request.Category.HasValue)
                query = query.Where(x => (int?)x.Category == request.Category);

            var items = await query.OrderByDescending(x => x.Category).ThenBy(x => x.Code)
                .ToListAsyncSafe(cancellationToken);

            var dtos = items.Select(x => FormulaMappings.MapToDto(x)).ToList();
            var paged = dtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            var paging = new Paging<CalculationFormulaDto>(dtos.Count, paged);
            return new SearchQueryResponse<CalculationFormulaDto>(request, paging);
        }
    }

    /// <summary>
    /// دریافت فرمول بر اساس شناسه
    /// </summary>
    public class GetFormulaByIdQuery : IRequest<CalculationFormulaDto>
    {
        public Guid Id { get; set; }
    }

    public class GetFormulaByIdQueryHandler : IRequestHandler<GetFormulaByIdQuery, CalculationFormulaDto>
    {
        private readonly IApplicationDbContext _context;

        public GetFormulaByIdQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<CalculationFormulaDto> Handle(GetFormulaByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _context.CalculationFormulas.AsNoTracking()
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(CalculationFormula), request.Id);
            return FormulaMappings.MapToDto(entity);
        }
    }

    // ---------------- Commands ----------------

    /// <summary>
    /// افزودن فرمول محاسبه
    /// </summary>
    public class CreateFormulaCommand : AddCalculationFormulaDto, IRequest<CalculationFormulaDto>
    {
    }

    public class CreateFormulaCommandHandler : IRequestHandler<CreateFormulaCommand, CalculationFormulaDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateFormulaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<CalculationFormulaDto> Handle(CreateFormulaCommand request, CancellationToken cancellationToken)
        {
            if (await _context.CalculationFormulas.AnyAsyncSafe(x => x.Code == request.Code, cancellationToken))
                throw new Common.Exceptions.BadRequestException($"فرمولی با کد '{request.Code}' از قبل تعریف شده است.");

            FormulaValidation.ValidateExpression(request);

            var entity = new CalculationFormula
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Code = request.Code,
                Name = request.Name,
                FaName = request.FaName,
                Category = request.Category,
                Expression = request.Expression,
                VariablesJson = FormulaMappings.SerializeVariables(request.Variables),
                OutputUnit = request.OutputUnit,
                Standard = request.Standard,
                Reference = request.Reference,
                Notes = request.Notes,
                IsEnabled = request.IsEnabled,
                Version = request.Version
            };
            _context.CalculationFormulas.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return FormulaMappings.MapToDto(entity);
        }
    }

    /// <summary>
    /// ویرایش فرمول محاسبه
    /// </summary>
    public class UpdateFormulaCommand : UpdateCalculationFormulaDto, IRequest<CalculationFormulaDto>
    {
    }

    public class UpdateFormulaCommandHandler : IRequestHandler<UpdateFormulaCommand, CalculationFormulaDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateFormulaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<CalculationFormulaDto> Handle(UpdateFormulaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CalculationFormulas
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(CalculationFormula), request.Id);

            FormulaValidation.ValidateExpression(request);

            entity.Code = request.Code;
            entity.Name = request.Name;
            entity.FaName = request.FaName;
            entity.Category = request.Category;
            entity.Expression = request.Expression;
            entity.VariablesJson = FormulaMappings.SerializeVariables(request.Variables);
            entity.OutputUnit = request.OutputUnit;
            entity.Standard = request.Standard;
            entity.Reference = request.Reference;
            entity.Notes = request.Notes;
            entity.IsEnabled = request.IsEnabled;
            entity.Version = request.Version;
            entity.IsActive = true;
            await _context.SaveChangesAsync(cancellationToken);
            return FormulaMappings.MapToDto(entity);
        }
    }

    /// <summary>
    /// حذف فرمول محاسبه
    /// </summary>
    public class DeleteFormulaCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteFormulaCommandHandler : IRequestHandler<DeleteFormulaCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteFormulaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteFormulaCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.CalculationFormulas
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(CalculationFormula), request.Id);

            entity.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// آزمایش فرمول با مقادیر نمونه
    /// </summary>
    public class TestFormulaCommand : TestFormulaRequestDto, IRequest<TestFormulaResultDto>
    {
    }

    public class TestFormulaCommandHandler : IRequestHandler<TestFormulaCommand, TestFormulaResultDto>
    {
        private readonly IApplicationDbContext _context;

        public TestFormulaCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<TestFormulaResultDto> Handle(TestFormulaCommand request, CancellationToken cancellationToken)
        {
            CalculationFormula? formula = null;
            if (request.FormulaId.HasValue)
                formula = await _context.CalculationFormulas.AsNoTracking()
                    .FirstOrDefaultAsyncSafe(x => x.Id == request.FormulaId, cancellationToken);
            else if (!string.IsNullOrEmpty(request.Code))
                formula = await _context.CalculationFormulas.AsNoTracking()
                    .FirstOrDefaultAsyncSafe(x => x.Code == request.Code, cancellationToken);

            var expression = request.Expression ?? formula?.Expression;
            if (string.IsNullOrEmpty(expression))
                return new TestFormulaResultDto { IsSuccess = false, Error = "عبارت فرمول مشخص نشده است." };

            var variables = formula != null ? GhgCalculationService.ParseVariables(formula.VariablesJson) : new List<FormulaVariableDto>();
            var declared = variables.Where(v => v.Type == Domain.Enums.Ghg.FormulaVariableType.Input).Select(v => v.Name).ToList();

            try
            {
                var result = MathExpressionEvaluator.Evaluate(expression!, request.Values);
                return new TestFormulaResultDto { IsSuccess = true, Result = result, Expression = expression, Variables = variables };
            }
            catch (Exception ex)
            {
                return new TestFormulaResultDto { IsSuccess = false, Error = ex.Message, Expression = expression, Variables = variables };
            }
        }
    }

    // ---------------- helpers ----------------

    internal static class FormulaMappings
    {
        public static CalculationFormulaDto MapToDto(CalculationFormula entity) => new()
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            Code = entity.Code,
            Name = entity.Name,
            FaName = entity.FaName,
            Category = entity.Category,
            Expression = entity.Expression,
            Variables = GhgCalculationService.ParseVariables(entity.VariablesJson),
            OutputUnit = entity.OutputUnit,
            Standard = entity.Standard,
            Reference = entity.Reference,
            Notes = entity.Notes,
            IsEnabled = entity.IsEnabled,
            Version = entity.Version
        };

        public static string SerializeVariables(List<FormulaVariableDto> variables) =>
            JsonSerializer.Serialize(variables ?? new List<FormulaVariableDto>(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    internal static class FormulaValidation
    {
        public static void ValidateExpression(AddCalculationFormulaDto request)
        {
            try
            {
                var testValues = request.Variables.ToDictionary(v => v.Name, v => v.SampleValue ?? 1.0);
                MathExpressionEvaluator.Evaluate(request.Expression, testValues);
            }
            catch (Exception ex)
            {
                throw new Common.Exceptions.BadRequestException($"فرمول نامعتبر است: {ex.Message}");
            }
        }
    }
}
