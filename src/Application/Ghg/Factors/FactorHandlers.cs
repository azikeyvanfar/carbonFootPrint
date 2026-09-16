using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using Gridify;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Domain.Entities.Ghg;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg.Factors
{
    // ---------------- Queries ----------------

    /// <summary>
    /// لیست ضرایب انتشار - قابل فیلتر بر اساس دسته و کلید
    /// </summary>
    public class GetAllEmissionFactorsQuery : SearchQueryRequest, IRequest<SearchQueryResponse<EmissionFactorDto>>
    {
        public int? Category { get; set; }
        public string? RefKey { get; set; }
        public string? Gas { get; set; }
        public string? Search { get; set; }
    }

    public class GetAllEmissionFactorsQueryHandler : IRequestHandler<GetAllEmissionFactorsQuery, SearchQueryResponse<EmissionFactorDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllEmissionFactorsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<SearchQueryResponse<EmissionFactorDto>> Handle(GetAllEmissionFactorsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.EmissionFactors.AsNoTracking().Where(x => x.IsActive);
            if (request.Category.HasValue)
                query = query.Where(x => (int?)x.Category == request.Category);
            if (!string.IsNullOrEmpty(request.RefKey))
                query = query.Where(x => x.RefKey == request.RefKey);
            if (!string.IsNullOrEmpty(request.Gas))
                query = query.Where(x => x.Gas == request.Gas);
            if (!string.IsNullOrEmpty(request.Search))
                query = query.Where(x => x.RefKey.Contains(request.Search) || x.RefLabel!.Contains(request.Search));

            var items = await query.OrderBy(x => x.Category).ThenBy(x => x.RefKey).ThenBy(x => x.Gas)
                .ToListAsyncSafe(cancellationToken);

            var dtos = items.Select(x => FactorMappings.MapToDto(x)).ToList();
            var paged = dtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return new SearchQueryResponse<EmissionFactorDto>(request, new Paging<EmissionFactorDto>(dtos.Count, paged));
        }
    }

    // ---------------- Commands ----------------

    /// <summary>
    /// افزودن ضریب انتشار
    /// </summary>
    public class CreateEmissionFactorCommand : AddEmissionFactorDto, IRequest<EmissionFactorDto>
    {
    }

    public class CreateEmissionFactorCommandHandler : IRequestHandler<CreateEmissionFactorCommand, EmissionFactorDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateEmissionFactorCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<EmissionFactorDto> Handle(CreateEmissionFactorCommand request, CancellationToken cancellationToken)
        {
            var entity = new EmissionFactor
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Category = request.Category,
                RefKey = request.RefKey,
                RefLabel = request.RefLabel,
                SubKey = request.SubKey,
                Gas = request.Gas,
                Value = request.Value,
                Unit = request.Unit,
                Standard = request.Standard,
                Source = request.Source,
                ValidFromYear = request.ValidFromYear
            };
            _context.EmissionFactors.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return FactorMappings.MapToDto(entity);
        }
    }

    /// <summary>
    /// ویرایش ضریب انتشار
    /// </summary>
    public class UpdateEmissionFactorCommand : UpdateEmissionFactorDto, IRequest<EmissionFactorDto>
    {
    }

    public class UpdateEmissionFactorCommandHandler : IRequestHandler<UpdateEmissionFactorCommand, EmissionFactorDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateEmissionFactorCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<EmissionFactorDto> Handle(UpdateEmissionFactorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.EmissionFactors
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(EmissionFactor), request.Id);

            entity.Category = request.Category;
            entity.RefKey = request.RefKey;
            entity.RefLabel = request.RefLabel;
            entity.SubKey = request.SubKey;
            entity.Gas = request.Gas;
            entity.Value = request.Value;
            entity.Unit = request.Unit;
            entity.Standard = request.Standard;
            entity.Source = request.Source;
            entity.ValidFromYear = request.ValidFromYear;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return FactorMappings.MapToDto(entity);
        }
    }

    /// <summary>
    /// حذف ضریب انتشار
    /// </summary>
    public class DeleteEmissionFactorCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteEmissionFactorCommandHandler : IRequestHandler<DeleteEmissionFactorCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteEmissionFactorCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteEmissionFactorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.EmissionFactors
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(EmissionFactor), request.Id);
            entity.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    internal static class FactorMappings
    {
        public static EmissionFactorDto MapToDto(EmissionFactor entity) => new()
        {
            Id = entity.Id,
            IsActive = entity.IsActive,
            Category = entity.Category,
            RefKey = entity.RefKey,
            RefLabel = entity.RefLabel,
            SubKey = entity.SubKey,
            Gas = entity.Gas,
            Value = entity.Value,
            Unit = entity.Unit,
            Standard = entity.Standard,
            Source = entity.Source,
            ValidFromYear = entity.ValidFromYear
        };
    }
}
