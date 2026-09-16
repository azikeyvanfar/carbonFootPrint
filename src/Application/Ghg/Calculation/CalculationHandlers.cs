using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Services;
using ContractorBackend.Domain.Entities.Ghg;
using ContractorBackend.Domain.Enums.Ghg;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg.Calculation
{
    // ---------------- Queries ----------------

    /// <summary>
    /// لیست دوره‌های گزارش‌دهی
    /// </summary>
    public class GetPeriodsQuery : IRequest<System.Collections.Generic.List<GhgPeriodDto>>
    {
    }

    public class GetPeriodsQueryHandler : IRequestHandler<GetPeriodsQuery, System.Collections.Generic.List<GhgPeriodDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetPeriodsQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<System.Collections.Generic.List<GhgPeriodDto>> Handle(GetPeriodsQuery request, CancellationToken cancellationToken)
        {
            var items = await _context.GhgPeriods.AsNoTracking().Where(x => x.IsActive)
                .OrderByDescending(x => x.PersianYear).ToListAsyncSafe(cancellationToken);
            return items.Select(x => new GhgPeriodDto
            {
                Id = x.Id, IsActive = x.IsActive, PersianYear = x.PersianYear, GregorianYear = x.GregorianYear,
                Title = x.Title, Status = x.Status, Description = x.Description
            }).ToList();
        }
    }

    /// <summary>
    /// خلاصه نتایج محاسبه دوره (موجودی انتشار - Tables & Graphs)
    /// </summary>
    public class GetPeriodSummaryQuery : IRequest<GhgCalculationSummaryDto>
    {
        public Guid PeriodId { get; set; }
    }

    public class GetPeriodSummaryQueryHandler : IRequestHandler<GetPeriodSummaryQuery, GhgCalculationSummaryDto>
    {
        private readonly IGhgCalculationService _calculationService;

        public GetPeriodSummaryQueryHandler(IGhgCalculationService calculationService)
            => _calculationService = calculationService;

        public async Task<GhgCalculationSummaryDto> Handle(GetPeriodSummaryQuery request, CancellationToken cancellationToken)
            => await _calculationService.BuildSummaryAsync(request.PeriodId);
    }

    // ---------------- Commands ----------------

    /// <summary>
    /// ایجاد دوره گزارش‌دهی جدید
    /// </summary>
    public class CreatePeriodCommand : IRequest<GhgPeriodDto>
    {
        public int PersianYear { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
    }

    public class CreatePeriodCommandHandler : IRequestHandler<CreatePeriodCommand, GhgPeriodDto>
    {
        private readonly IApplicationDbContext _context;

        public CreatePeriodCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<GhgPeriodDto> Handle(CreatePeriodCommand request, CancellationToken cancellationToken)
        {
            if (await _context.GhgPeriods.AnyAsyncSafe(x => x.PersianYear == request.PersianYear, cancellationToken))
                throw new Common.Exceptions.BadRequestException($"دوره‌ای برای سال {request.PersianYear} از قبل وجود دارد.");

            var entity = new GhgPeriod
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                PersianYear = request.PersianYear,
                GregorianYear = request.PersianYear + 621,
                Title = request.Title,
                Status = GhgPeriodStatus.Draft,
                Description = request.Description
            };
            _context.GhgPeriods.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new GhgPeriodDto
            {
                Id = entity.Id, IsActive = true, PersianYear = entity.PersianYear,
                GregorianYear = entity.GregorianYear, Title = entity.Title, Status = entity.Status
            };
        }
    }

    /// <summary>
    /// محاسبه مجدد موجودی انتشار دوره - اجرای فرمول‌های تنظیمات روی داده‌های فعالیت
    /// </summary>
    public class RecalculatePeriodCommand : IRequest<GhgCalculationSummaryDto>
    {
        public Guid PeriodId { get; set; }
    }

    public class RecalculatePeriodCommandHandler : IRequestHandler<RecalculatePeriodCommand, GhgCalculationSummaryDto>
    {
        private readonly IGhgCalculationService _calculationService;

        public RecalculatePeriodCommandHandler(IGhgCalculationService calculationService)
            => _calculationService = calculationService;

        public async Task<GhgCalculationSummaryDto> Handle(RecalculatePeriodCommand request, CancellationToken cancellationToken)
            => await _calculationService.RecalculatePeriodAsync(request.PeriodId);
    }

    /// <summary>
    /// دریافت داده‌های اولیه از منبع داده (فایل‌های ارجاع / آینده: سرویس وب) و ورود آنها به دوره
    /// </summary>
    public class ImportActivityDataCommand : IRequest<int>
    {
        public Guid PeriodId { get; set; }
    }

    public class ImportActivityDataCommandHandler : IRequestHandler<ImportActivityDataCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IActivityDataProvider _provider;

        public ImportActivityDataCommandHandler(IApplicationDbContext context, IActivityDataProvider provider)
        {
            _context = context;
            _provider = provider;
        }

        public async Task<int> Handle(ImportActivityDataCommand request, CancellationToken cancellationToken)
        {
            var period = await _context.GhgPeriods
                .FirstOrDefaultAsyncSafe(x => x.Id == request.PeriodId, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(GhgPeriod), request.PeriodId);

            var items = await _provider.GetActivityDataAsync(period.PersianYear);
            if (items.Count == 0) return 0;

            var areas = await _context.GhgAreas.AsNoTracking().ToListAsyncSafe(cancellationToken);
            var categories = await _context.EmissionCategories.AsNoTracking().ToListAsyncSafe(cancellationToken);
            var costCenters = await _context.CostCenters.AsNoTracking().ToListAsyncSafe(cancellationToken);
            var fuels = await _context.Fuels.AsNoTracking().Where(x => x.IsActive)
                .OrderByDescending(x => x.ValidFromYear).ToListAsyncSafe(cancellationToken);

            var existing = _context.ActivityDataEntries.Where(x => x.PeriodId == request.PeriodId);
            _context.ActivityDataEntries.RemoveRange(existing);

            var created = 0;
            foreach (var item in items)
            {
                var category = categories.FirstOrDefault(c => c.Sign == item.CategorySign);
                if (category == null) continue;

                _context.ActivityDataEntries.Add(new ActivityDataEntry
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    PeriodId = request.PeriodId,
                    CategoryId = category.Id,
                    AreaId = areas.FirstOrDefault(a => a.Code == item.AreaCode)?.Id,
                    CostCenterId = costCenters.FirstOrDefault(c => c.Code == item.CostCenterCode)?.Id,
                    FuelId = fuels.FirstOrDefault(f => f.Name == item.FuelName)?.Id,
                    FactorRefKey = item.FactorRefKey ?? item.FuelName,
                    FactorSubKey = item.FactorSubKey,
                    EmissionSource = item.EmissionSource,
                    Quantity = item.Quantity,
                    Quantity2 = item.Quantity2,
                    Quantity3 = item.Quantity3,
                    Unit = item.Unit,
                    ControlEfficiency = item.ControlEfficiency,
                    DataSource = item.DataSource,
                    Description = item.Description
                });
                created++;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return created;
        }
    }

    /// <summary>
    /// ورود ردپای کربن محصولات (بخش 2) از منبع داده
    /// </summary>
    public class ImportProductFootprintsCommand : IRequest<int>
    {
        public Guid PeriodId { get; set; }
    }

    public class ImportProductFootprintsCommandHandler : IRequestHandler<ImportProductFootprintsCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly IActivityDataProvider _provider;

        public ImportProductFootprintsCommandHandler(IApplicationDbContext context, IActivityDataProvider provider)
        {
            _context = context;
            _provider = provider;
        }

        public async Task<int> Handle(ImportProductFootprintsCommand request, CancellationToken cancellationToken)
        {
            var period = await _context.GhgPeriods
                .FirstOrDefaultAsyncSafe(x => x.Id == request.PeriodId, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(GhgPeriod), request.PeriodId);

            var items = await _provider.GetProductFootprintsAsync(period.PersianYear);
            if (items.Count == 0) return 0;

            var old = _context.ProductFootprints.Where(x => x.PeriodId == request.PeriodId);
            _context.ProductFootprints.RemoveRange(old);

            foreach (var item in items)
            {
                _context.ProductFootprints.Add(new ProductFootprint
                {
                    Id = Guid.NewGuid(),
                    IsActive = true,
                    PeriodId = request.PeriodId,
                    Name = item.Name,
                    FaName = item.FaName,
                    AreaName = item.AreaName,
                    CarbonFootprint = item.CarbonFootprint,
                    UpstreamSharePct = item.UpstreamSharePct,
                    AnnualProduction = item.AnnualProduction,
                    Boundary = item.Boundary,
                    Standard = item.Standard,
                    Description = item.Description
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return items.Count;
        }
    }
}
