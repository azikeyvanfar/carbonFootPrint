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

namespace ContractorBackend.Application.Ghg.Settings
{
    // ---------------- Reference Queries ----------------

    /// <summary>
    /// لیست نواحی
    /// </summary>
    public class GetAllAreasQuery : IRequest<System.Collections.Generic.List<GhgAreaDto>>
    {
    }

    public class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, System.Collections.Generic.List<GhgAreaDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllAreasQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<System.Collections.Generic.List<GhgAreaDto>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.GhgAreas.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Code)
                .ToListAsyncSafe(cancellationToken);
            return items.Select(x => new GhgAreaDto
            {
                Id = x.Id, IsActive = x.IsActive, Code = x.Code, Name = x.Name, FaName = x.FaName,
                CommitteeCode = x.CommitteeCode, EmployeeCount = x.EmployeeCount, ContractorCount = x.ContractorCount
            }).ToList();
        }
    }

    /// <summary>
    /// لیست مراکز هزینه (با نام ناحیه)
    /// </summary>
    public class GetAllCostCentersQuery : SearchQueryRequest, IRequest<SearchQueryResponse<CostCenterDto>>
    {
        public Guid? AreaId { get; set; }
    }

    public class GetAllCostCentersQueryHandler : IRequestHandler<GetAllCostCentersQuery, SearchQueryResponse<CostCenterDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllCostCentersQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<SearchQueryResponse<CostCenterDto>> Handle(GetAllCostCentersQuery request, CancellationToken cancellationToken)
        {
            var query = from cc in _context.CostCenters.AsNoTracking()
                        join a in _context.GhgAreas.AsNoTracking() on cc.AreaId equals a.Id into ga
                        from area in ga.DefaultIfEmpty()
                        where cc.IsActive
                        select new CostCenterDto
                        {
                            Id = cc.Id, IsActive = cc.IsActive, Code = cc.Code, Name = cc.Name,
                            UnitProcess = cc.UnitProcess, AreaId = cc.AreaId, AreaName = area != null ? area.Name : null
                        };

            if (request.AreaId.HasValue)
                query = query.Where(x => x.AreaId == request.AreaId);

            var items = await query.OrderBy(x => x.Code).ToListAsyncSafe(cancellationToken);
            var paged = items.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return new SearchQueryResponse<CostCenterDto>(request, new Paging<CostCenterDto>(items.Count, paged));
        }
    }

    /// <summary>
    /// لیست دسته‌های انتشار
    /// </summary>
    public class GetAllEmissionCategoriesQuery : IRequest<System.Collections.Generic.List<EmissionCategoryDto>>
    {
    }

    public class GetAllEmissionCategoriesQueryHandler : IRequestHandler<GetAllEmissionCategoriesQuery, System.Collections.Generic.List<EmissionCategoryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllEmissionCategoriesQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<System.Collections.Generic.List<EmissionCategoryDto>> Handle(GetAllEmissionCategoriesQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.EmissionCategories.AsNoTracking().Where(x => x.IsActive).OrderBy(x => x.Priority)
                .ToListAsyncSafe(cancellationToken);
            return items.Select(x => new EmissionCategoryDto
            {
                Id = x.Id, IsActive = x.IsActive, Name = x.Name, FaName = x.FaName, Sign = x.Sign,
                CategoryNo = x.CategoryNo, Scope = x.Scope, Description = x.Description, Priority = x.Priority
            }).ToList();
        }
    }

    /// <summary>
    /// لیست سوخت‌ها و LHV
    /// </summary>
    public class GetAllFuelsQuery : IRequest<System.Collections.Generic.List<FuelDto>>
    {
    }

    public class GetAllFuelsQueryHandler : IRequestHandler<GetAllFuelsQuery, System.Collections.Generic.List<FuelDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllFuelsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<System.Collections.Generic.List<FuelDto>> Handle(GetAllFuelsQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.Fuels.AsNoTracking().Where(x => x.IsActive)
                .OrderByDescending(x => x.ValidFromYear).ToListAsyncSafe(cancellationToken);
            return items.Select(x => new FuelDto
            {
                Id = x.Id, IsActive = x.IsActive, Name = x.Name, FaName = x.FaName, Lhv = x.Lhv,
                LhvUnit = x.LhvUnit, ConsumptionUnit = x.ConsumptionUnit, Source = x.Source, ValidFromYear = x.ValidFromYear
            }).ToList();
        }
    }

    /// <summary>
    /// لیست ضرایب GWP فعال
    /// </summary>
    public class GetAllGwpsQuery : IRequest<System.Collections.Generic.List<GlobalWarmingPotentialDto>>
    {
    }

    public class GetAllGwpsQueryHandler : IRequestHandler<GetAllGwpsQuery, System.Collections.Generic.List<GlobalWarmingPotentialDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllGwpsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<System.Collections.Generic.List<GlobalWarmingPotentialDto>> Handle(GetAllGwpsQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.GlobalWarmingPotentials.AsNoTracking().Where(x => x.IsActive)
                .ToListAsyncSafe(cancellationToken);
            return items.Select(x => new GlobalWarmingPotentialDto
            {
                Id = x.Id, IsActive = x.IsActive, GasKey = x.GasKey, Value = x.Value,
                AssessmentReport = x.AssessmentReport, TimeHorizon = x.TimeHorizon, ValidFromYear = x.ValidFromYear
            }).ToList();
        }
    }

    // ---------------- Parameters ----------------

    /// <summary>
    /// لیست پارامترهای محاسبات
    /// </summary>
    public class GetAllParametersQuery : SearchQueryRequest, IRequest<SearchQueryResponse<GhgParameterDto>>
    {
        public int? Year { get; set; }
    }

    public class GetAllParametersQueryHandler : IRequestHandler<GetAllParametersQuery, SearchQueryResponse<GhgParameterDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllParametersQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<SearchQueryResponse<GhgParameterDto>> Handle(GetAllParametersQuery request, CancellationToken cancellationToken)
        {
            var query = _context.GhgParameters.AsNoTracking().Where(x => x.IsActive);
            if (request.Year.HasValue) query = query.Where(x => x.Year == request.Year);

            var items = await query.OrderByDescending(x => x.Year).ThenBy(x => x.Key).ToListAsyncSafe(cancellationToken);
            var dtos = items.Select(x => ParameterMappings.MapToDto(x)).ToList();
            var paged = dtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return new SearchQueryResponse<GhgParameterDto>(request, new Paging<GhgParameterDto>(dtos.Count, paged));
        }
    }

    /// <summary>
    /// افزودن پارامتر
    /// </summary>
    public class CreateParameterCommand : AddGhgParameterDto, IRequest<GhgParameterDto>
    {
    }

    public class CreateParameterCommandHandler : IRequestHandler<CreateParameterCommand, GhgParameterDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateParameterCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<GhgParameterDto> Handle(CreateParameterCommand request, CancellationToken cancellationToken)
        {
            var entity = new GhgParameter
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                Key = request.Key,
                Name = request.Name,
                FaName = request.FaName,
                Value = request.Value,
                Unit = request.Unit,
                Source = request.Source,
                Year = request.Year
            };
            _context.GhgParameters.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return ParameterMappings.MapToDto(entity);
        }
    }

    /// <summary>
    /// ویرایش پارامتر
    /// </summary>
    public class UpdateParameterCommand : UpdateGhgParameterDto, IRequest<GhgParameterDto>
    {
    }

    public class UpdateParameterCommandHandler : IRequestHandler<UpdateParameterCommand, GhgParameterDto>
    {
        private readonly IApplicationDbContext _context;

        public UpdateParameterCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<GhgParameterDto> Handle(UpdateParameterCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.GhgParameters
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(GhgParameter), request.Id);

            entity.Key = request.Key;
            entity.Name = request.Name;
            entity.FaName = request.FaName;
            entity.Value = request.Value;
            entity.Unit = request.Unit;
            entity.Source = request.Source;
            entity.Year = request.Year;
            entity.IsActive = request.IsActive;
            await _context.SaveChangesAsync(cancellationToken);
            return ParameterMappings.MapToDto(entity);
        }
    }

    internal static class ParameterMappings
    {
        public static GhgParameterDto MapToDto(GhgParameter x) => new()
        {
            Id = x.Id, IsActive = x.IsActive, Key = x.Key, Name = x.Name, FaName = x.FaName,
            Value = x.Value, Unit = x.Unit, Source = x.Source, Year = x.Year
        };
    }
}
