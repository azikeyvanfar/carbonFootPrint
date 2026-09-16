using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using Gridify;
using ContractorBackend.Application.Dtos.Ghg;
using ContractorBackend.Application.Ghg.Services;
using ContractorBackend.Domain.Entities.Ghg;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Ghg.ActivityData
{
    // ---------------- Queries ----------------

    /// <summary>
    /// لیست داده‌های فعالیت یک دوره
    /// </summary>
    public class GetActivityDataQuery : SearchQueryRequest, IRequest<SearchQueryResponse<ActivityDataEntryDto>>
    {
        public Guid PeriodId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? AreaId { get; set; }
    }

    public class GetActivityDataQueryHandler : IRequestHandler<GetActivityDataQuery, SearchQueryResponse<ActivityDataEntryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetActivityDataQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<SearchQueryResponse<ActivityDataEntryDto>> Handle(GetActivityDataQuery request, CancellationToken cancellationToken)
        {
            var query = from e in _context.ActivityDataEntries.AsNoTracking()
                        join c in _context.EmissionCategories.AsNoTracking() on e.CategoryId equals c.Id
                        join a in _context.GhgAreas.AsNoTracking() on e.AreaId equals a.Id into ga
                        from area in ga.DefaultIfEmpty()
                        join cc in _context.CostCenters.AsNoTracking() on e.CostCenterId equals cc.Id into gcc
                        from costCenter in gcc.DefaultIfEmpty()
                        join f in _context.Fuels.AsNoTracking() on e.FuelId equals f.Id into gf
                        from fuel in gf.DefaultIfEmpty()
                        where e.IsActive && e.PeriodId == request.PeriodId
                        select new { e, c, area, costCenter, fuel };

            if (request.CategoryId.HasValue)
                query = query.Where(x => x.e.CategoryId == request.CategoryId);
            if (request.AreaId.HasValue)
                query = query.Where(x => x.e.AreaId == request.AreaId);

            var rows = await query.OrderByDescending(x => x.e.Id).ToListAsyncSafe(cancellationToken);
            var dtos = rows.Select(x => new ActivityDataEntryDto
            {
                Id = x.e.Id,
                PeriodId = x.e.PeriodId,
                CategoryId = x.e.CategoryId,
                CategoryName = x.c.FaName ?? x.c.Name,
                AreaId = x.e.AreaId,
                AreaName = x.area != null ? x.area.FaName : null,
                CostCenterId = x.e.CostCenterId,
                CostCenterName = x.costCenter != null ? x.costCenter.Name : null,
                FuelId = x.e.FuelId,
                FuelName = x.fuel != null ? x.fuel.Name : null,
                FactorRefKey = x.e.FactorRefKey,
                FactorSubKey = x.e.FactorSubKey,
                EmissionSource = x.e.EmissionSource,
                Quantity = x.e.Quantity,
                Quantity2 = x.e.Quantity2,
                Quantity3 = x.e.Quantity3,
                Unit = x.e.Unit,
                ControlEfficiency = x.e.ControlEfficiency,
                DataSource = x.e.DataSource,
                Description = x.e.Description
            }).ToList();

            var paged = dtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
            return new SearchQueryResponse<ActivityDataEntryDto>(request, new Paging<ActivityDataEntryDto>(dtos.Count, paged));
        }
    }

    /// <summary>
    /// ساختار فرم گام‌به‌گام ورود داده - بر اساس دسته‌ها و فرمول‌های تعریف شده در تنظیمات
    /// </summary>
    public class GetActivityWizardQuery : IRequest<ActivityWizardDto>
    {
        public Guid PeriodId { get; set; }
    }

    public class GetActivityWizardQueryHandler : IRequestHandler<GetActivityWizardQuery, ActivityWizardDto>
    {
        private readonly IApplicationDbContext _context;

        public GetActivityWizardQueryHandler(IApplicationDbContext context) => _context = context;

        public async Task<ActivityWizardDto> Handle(GetActivityWizardQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.EmissionCategories.AsNoTracking()
                .Where(x => x.IsActive).OrderBy(x => x.Priority).ToListAsyncSafe(cancellationToken);

            var formulas = await _context.CalculationFormulas.AsNoTracking()
                .Where(x => x.IsActive && x.IsEnabled).ToListAsyncSafe(cancellationToken);

            var fuels = await _context.Fuels.AsNoTracking().Where(x => x.IsActive)
                .OrderByDescending(x => x.ValidFromYear).ToListAsyncSafe(cancellationToken);

            var steps = new List<WizardStepDto>();
            foreach (var cat in categories)
            {
                var step = new WizardStepDto
                {
                    CategoryId = cat.Id,
                    Name = cat.Name,
                    FaName = cat.FaName,
                    Sign = cat.Sign,
                    Scope = cat.Scope,
                    Priority = cat.Priority,
                    Description = cat.Description,
                    Fields = new List<WizardFieldDto>
                    {
                        new() { Name = "AreaId", Label = "ناحیه", Type = "area", IsRequired = true },
                        new() { Name = "CostCenterId", Label = "مرکز هزینه", Type = "costcenter" },
                        new() { Name = "EmissionSource", Label = "شرح منبع انتشار", Type = "text" }
                    }
                };

                // فیلدهای اختصاصی هر دسته بر اساس منطق ورک‌بوک
                switch (cat.Sign)
                {
                    case "C":
                        step.Fields.Add(new WizardFieldDto { Name = "FuelId", Label = "نوع سوخت", Type = "fuel", IsRequired = true });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مصرف سالانه", Type = "number", IsRequired = true, Unit = "بسته به سوخت" });
                        break;
                    case "V":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "فرآیند", Type = "text", IsRequired = true, HelpText = "مثل Lime Making / EAFs / Direct Reduction" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "تولید سالانه (تن)", Type = "number", Unit = "ton/y" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity2", Label = "خوراک گاز (kNm3)", Type = "number", Unit = "kNm3" });
                        break;
                    case "F":
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "تعداد شیرها", Type = "number", IsRequired = true });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity2", Label = "تعداد فلنج‌ها", Type = "number" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity3", Label = "تعداد PSV ها", Type = "number" });
                        step.Fields.Add(new WizardFieldDto { Name = "ControlEfficiency", Label = "راندمان کنترل (٪)", Type = "number" });
                        break;
                    case "E":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "نوع برق", Type = "text", IsRequired = true, HelpText = "Total Average / Direct Contract / Solar / MSC Production" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مصرف برق", Type = "number", IsRequired = true, Unit = "kWh/y" });
                        break;
                    case "W":
                    case "Z":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "نوع پسماند", Type = "text", IsRequired = true, HelpText = "Metal / Non-Metal / Plastic" });
                        step.Fields.Add(new WizardFieldDto { Name = "FactorSubKey", Label = "روش مدیریت", Type = "text", IsRequired = true, HelpText = "Sale / Landfill" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مقدار پسماند", Type = "number", IsRequired = true, Unit = "ton" });
                        break;
                    case "Y":
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "دبی فاضلاب", Type = "number", IsRequired = true, Unit = "m3/y" });
                        break;
                    case "M":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "نوع سوخت", Type = "text", IsRequired = true, HelpText = "Petrol / Gas oil / CNG" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مصرف سوخت", Type = "number", IsRequired = true, Unit = "Lit" });
                        break;
                    case "T":
                    case "D":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "نوع حمل", Type = "text", IsRequired = true, HelpText = "Road / Rail / Marine / Air" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "وزن بار (تن)", Type = "number", IsRequired = true, Unit = "ton" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity2", Label = "مسافت (کیلومتر)", Type = "number", IsRequired = true, Unit = "km" });
                        break;
                    case "P":
                    case "B":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "وسیله نقلیه", Type = "text", IsRequired = true, HelpText = "Car / Bus / Air - Short Haul / Train / ..." });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مسافت (کیلومتر)", Type = "number", IsRequired = true, Unit = "km" });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity2", Label = "تعداد افراد / سفر", Type = "number", Unit = "person" });
                        break;
                    case "R":
                    case "L":
                    case "X":
                        step.Fields.Add(new WizardFieldDto { Name = "FactorRefKey", Label = "نام ماده / خدمت", Type = "text", IsRequired = true, HelpText = "مثل DRI, Lime, Scrap, ..." });
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مقدار خریداری شده", Type = "number", IsRequired = true, Unit = "ton" });
                        break;
                    default:
                        step.Fields.Add(new WizardFieldDto { Name = "Quantity", Label = "مقدار", Type = "number", IsRequired = true });
                        break;
                }

                // افزودن متغیرهای فرمول مرتبط به عنوان راهنما
                var relatedFormulas = formulas.Where(f => f.Category == (Domain.Enums.Ghg.EmissionFactorCategory?)null || IsFormulaForCategory(f, cat)).ToList();
                foreach (var f in relatedFormulas.Take(3))
                    step.Description = $"{step.Description}\nفرمول: {f.Code} → {f.Expression}";

                steps.Add(step);
            }

            return new ActivityWizardDto { Steps = steps };
        }

        private static bool IsFormulaForCategory(CalculationFormula f, EmissionCategory cat) =>
            f.Category.HasValue && EmissionFactorCategoryForSign(cat.Sign) == f.Category.Value;

        private static Domain.Enums.Ghg.EmissionFactorCategory? EmissionFactorCategoryForSign(string sign) => sign switch
        {
            "C" => Domain.Enums.Ghg.EmissionFactorCategory.FuelCombustion,
            "E" => Domain.Enums.Ghg.EmissionFactorCategory.ElectricityGeneration,
            "F" => Domain.Enums.Ghg.EmissionFactorCategory.FugitiveEquipment,
            "W" or "Z" => Domain.Enums.Ghg.EmissionFactorCategory.WasteManagement,
            "Y" => Domain.Enums.Ghg.EmissionFactorCategory.Wastewater,
            "M" => Domain.Enums.Ghg.EmissionFactorCategory.OnSiteTransportation,
            "T" or "D" => Domain.Enums.Ghg.EmissionFactorCategory.MaterialTransportation,
            "P" => Domain.Enums.Ghg.EmissionFactorCategory.EmployeeCommuting,
            "B" => Domain.Enums.Ghg.EmissionFactorCategory.BusinessTravel,
            "R" or "L" or "X" => Domain.Enums.Ghg.EmissionFactorCategory.PurchasedMaterial,
            "N" => Domain.Enums.Ghg.EmissionFactorCategory.EnergyRelated,
            _ => null
        };
    }

    // ---------------- Commands ----------------

    /// <summary>
    /// افزودن ردیف داده فعالیت
    /// </summary>
    public class CreateActivityDataCommand : AddActivityDataEntryDto, IRequest<ActivityDataEntryDto>
    {
    }

    public class CreateActivityDataCommandHandler : IRequestHandler<CreateActivityDataCommand, ActivityDataEntryDto>
    {
        private readonly IApplicationDbContext _context;

        public CreateActivityDataCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<ActivityDataEntryDto> Handle(CreateActivityDataCommand request, CancellationToken cancellationToken)
        {
            var entity = new ActivityDataEntry
            {
                Id = Guid.NewGuid(),
                IsActive = true,
                PeriodId = request.PeriodId,
                CategoryId = request.CategoryId,
                AreaId = request.AreaId,
                CostCenterId = request.CostCenterId,
                FuelId = request.FuelId,
                FactorRefKey = request.FactorRefKey,
                FactorSubKey = request.FactorSubKey,
                EmissionSource = request.EmissionSource,
                Quantity = request.Quantity,
                Quantity2 = request.Quantity2,
                Quantity3 = request.Quantity3,
                Unit = request.Unit,
                ControlEfficiency = request.ControlEfficiency,
                DataSource = request.DataSource,
                Description = request.Description
            };
            _context.ActivityDataEntries.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return new ActivityDataEntryDto
            {
                Id = entity.Id, PeriodId = entity.PeriodId, CategoryId = entity.CategoryId,
                AreaId = entity.AreaId, Quantity = entity.Quantity, Unit = entity.Unit
            };
        }
    }

    /// <summary>
    /// ویرایش ردیف داده فعالیت
    /// </summary>
    public class UpdateActivityDataCommand : UpdateActivityDataEntryDto, IRequest<bool>
    {
    }

    public class UpdateActivityDataCommandHandler : IRequestHandler<UpdateActivityDataCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateActivityDataCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<bool> Handle(UpdateActivityDataCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ActivityDataEntries
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(ActivityDataEntry), request.Id);

            entity.PeriodId = request.PeriodId;
            entity.CategoryId = request.CategoryId;
            entity.AreaId = request.AreaId;
            entity.CostCenterId = request.CostCenterId;
            entity.FuelId = request.FuelId;
            entity.FactorRefKey = request.FactorRefKey;
            entity.FactorSubKey = request.FactorSubKey;
            entity.EmissionSource = request.EmissionSource;
            entity.Quantity = request.Quantity;
            entity.Quantity2 = request.Quantity2;
            entity.Quantity3 = request.Quantity3;
            entity.Unit = request.Unit;
            entity.ControlEfficiency = request.ControlEfficiency;
            entity.DataSource = request.DataSource;
            entity.Description = request.Description;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// حذف ردیف داده فعالیت
    /// </summary>
    public class DeleteActivityDataCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteActivityDataCommandHandler : IRequestHandler<DeleteActivityDataCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteActivityDataCommandHandler(IApplicationDbContext context) => _context = context;

        public async Task<bool> Handle(DeleteActivityDataCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.ActivityDataEntries
                .FirstOrDefaultAsyncSafe(x => x.Id == request.Id, cancellationToken)
                ?? throw new Common.Exceptions.NotFoundException(nameof(ActivityDataEntry), request.Id);
            entity.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    /// <summary>
    /// پیش‌نمایش محاسبه (فرم گام‌به‌گام)
    /// </summary>
    public class PreviewActivityCommand : ActivityPreviewRequestDto, IRequest<ActivityPreviewDto>
    {
    }

    public class PreviewActivityCommandHandler : IRequestHandler<PreviewActivityCommand, ActivityPreviewDto>
    {
        private readonly IGhgCalculationService _calculationService;

        public PreviewActivityCommandHandler(IGhgCalculationService calculationService)
            => _calculationService = calculationService;

        public async Task<ActivityPreviewDto> Handle(PreviewActivityCommand request, CancellationToken cancellationToken)
            => await _calculationService.PreviewActivityAsync(request);
    }
}
