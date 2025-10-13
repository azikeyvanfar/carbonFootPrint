using System;
using System.Linq;
using System.Linq.Expressions;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class IQueryableExtensions
    {
        /// <summary>
        /// Get List of Entity and filter based on Role Scopes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">DB List Of Entity</param>
        /// <param name="roleScopes">List of Scopes That user has</param>
        /// <param name="scopeIdName">name of Property Scope Id in Entity Table in Case it is NOT ScopeId</param>
        /// <returns></returns>
        public static IQueryable<T> GetAllByCurrentUserScope<T>(this IQueryable<T> source, IQueryable<Guid?> roleScopes, string? scopeIdName = "ScopeId") where T : class
        {
            var parameter = Expression.Parameter(typeof(T), "e");

            var property = Expression.Property(parameter, scopeIdName);

            //var propertyType = property.GetType();

            var containsMethod = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Guid?));

            var containsExpression = Expression.Call(
                containsMethod,
                Expression.Constant(roleScopes),
                property
                );

            var lambda = Expression.Lambda<Func<T, bool>>(containsExpression, parameter);

            return source.Where(lambda);
        }



        //public static IQueryable<BusinessUnit> FilterAccessToRisk(this IQueryable<BusinessUnit> source,
        //    IHttpContextAccessor accessor,
        //    IApplicationDbContext dbContext)
        //{
        //    var currentUserId = accessor.HttpContext.GetUserId();
        //    var accessBusinessUnitIds = dbContext.RiskFacilitationAccesses
        //                .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.BusinessUnitId != null)
        //                .Distinct()
        //                .Select(x => x.BusinessUnitId)
        //                ;

        //    return source.Where(x => accessBusinessUnitIds.Any(l => l == x.Id));
        //}

        //[Obsolete]
        //public static IQueryable<Section> FilterAccessToRisk(this IQueryable<Section> source,
        //    IHttpContextAccessor accessor,
        //    IApplicationDbContext dbContext)
        //{
        //    var currentUserId = accessor.HttpContext.GetUserId();
        //    var accessSectionIds = dbContext.RiskFacilitationAccesses
        //                .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.SectionId != null)
        //                .Distinct()
        //                .Select(x => x.SectionId)
        //                ;

        //    return source.Where(x => accessSectionIds.Any(l => l == x.Id));
        //}

        //[Obsolete]
        //public static IQueryable<Section> FilterAccessToRisk(this IQueryable<Section> source,
        //    IEnumerable<Guid> childrenBusinessUnitIds,
        //    IHttpContextAccessor accessor, IApplicationDbContext dbContext)
        //{
        //    var currentUserId = accessor.HttpContext.GetUserId();
        //    var accessSectionIds = dbContext.RiskFacilitationAccesses
        //                .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.SectionId != null)
        //                .Where(x => childrenBusinessUnitIds.Any(c => c == x.Id))
        //                .Distinct()
        //                .Select(x => x.SectionId)
        //                ;

        //    return source.Where(x => accessSectionIds.Any(l => l == x.Id));
        //}

        //[Obsolete]
        //public static IQueryable<ChanceManagement> FilterAccessToRisk(this IQueryable<ChanceManagement> source,
        //    IHttpContextAccessor accessor,
        //    IApplicationDbContext dbContext)
        //{
        //    var currentUserId = accessor.HttpContext.GetUserId();
        //    var accessBusinessUnitIds = dbContext.RiskFacilitationAccesses
        //             .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.BusinessUnitId != null)
        //             .Distinct()
        //             .Select(x => x.BusinessUnitId)
        //             ;
        //    var accessSectionIds = dbContext.RiskFacilitationAccesses
        //             .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.SectionId != null)
        //             .Distinct()
        //             .Select(x => x.SectionId)
        //             ;

        //    return source.Where(x => accessBusinessUnitIds.Any(l => l == x.BusinessUnitId || l == x.Section.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId));
        //}

        //[Obsolete]
        //public static IQueryable<RiskManagement> FilterAccessToRisk(this IQueryable<RiskManagement> source,
        //    IHttpContextAccessor accessor,
        //    IApplicationDbContext dbContext)
        //{
        //    var currentUserId = accessor.HttpContext.GetUserId();
        //    var accessBusinessUnitIds = dbContext.RiskFacilitationAccesses
        //             .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.BusinessUnitId != null)
        //             .Distinct()
        //             .Select(x => x.BusinessUnitId)
        //             ;
        //    var accessSectionIds = dbContext.RiskFacilitationAccesses
        //             .Where(x => x.IsActive && x.EndDate == null && x.UserId == currentUserId && x.SectionId != null)
        //             .Distinct()
        //             .Select(x => x.SectionId)
        //             ;

        //    return source.Where(x => accessBusinessUnitIds.Any(l => l == x.BusinessUnitId || l == x.Section.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId));
        //}


        //#region Filter By Parent BusinessUnit
        //[Obsolete]
        //public static IQueryable<RiskManagement> FilterByParentBusinessUnit(this IQueryable<RiskManagement> source,
        //    IHttpContextAccessor accessor,
        //    IBusinessUnitService businessUnitService,
        //    Guid? parentBusinessUnitId,
        //    IApplicationDbContext dbContext
        //    )
        //{
        //    #region Filter By Parent  BusinessUnit
        //    if (parentBusinessUnitId is null)
        //    {
        //        return source;
        //    }
        //    var childrenBusinessUnits = businessUnitService.GetCurrentUserChildrenBusinessUnits(parentBusinessUnitId.Value, containsCurrentBusinessUnit: true);
        //    var childrenBusinessUnitIds = childrenBusinessUnits.Select(x => x.Id);
        //    var accessSectionIds = dbContext.Sections.FilterAccessToRisk(childrenBusinessUnitIds, accessor, dbContext).Select(x => x.Id);

        //    return source.Where(x => childrenBusinessUnitIds.Any(l => l ==  x.BusinessUnitId || l == x.Section.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId));
        //    #endregion

        //    //#region table function
        //    //if (parentBusinessUnitId is null)
        //    //{
        //    //    return source;
        //    //}
        //    //var parentBusinessUnitIsSuiteId = dbContext.BusinessUnits
        //    //    .Where(x => x.Id ==  parentBusinessUnitId)
        //    //.Select(x => x.IsSuiteId)
        //    //.FirstOrDefault();
        //    //var userId = accessor.HttpContext.GetUserId();

        //    //var childrenBusinessUnitIds = dbContext.GetCurrentUserChildrenBusinessUnits(parentBusinessUnitIsSuiteId.Value, userId).Select(x => x.Id);
        //    //var accessSectionIds = dbContext.Sections.FilterAccessToRisk(childrenBusinessUnitIds, accessor, dbContext).Select(x => x.Id);

        //    //return source.Where(x => childrenBusinessUnitIds.Any(l => l ==  x.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId));

        //    //#endregion


        //}

        //[Obsolete]
        //public static IQueryable<RiskManagement> FilterByParentBusinessUnits(this IQueryable<RiskManagement> source,
        //   IHttpContextAccessor accessor,
        //   IBusinessUnitService businessUnitService,
        //   IEnumerable<Guid> parentBusinessUnitIds,
        //   IApplicationDbContext dbContext
        //   )
        //{
        //    #region Filter By Parent  BusinessUnit
        //    if (parentBusinessUnitIds is null || !parentBusinessUnitIds.Any())
        //    {
        //        return source;
        //    }

        //    IQueryable<BusinessUnit> childrenBusinessUnits = dbContext.BusinessUnits.AsQueryable();
        //    IQueryable<Guid> childrenBusinessUnitIds = dbContext.BusinessUnits.Select(x => x.Id).AsQueryable();
        //    IQueryable<Guid> accessSectionIds = dbContext.Sections.Select(x => x.Id).AsQueryable();

        //    //List<BusinessUnit> childrenBusinessUnits = new List<BusinessUnit>();
        //    //List<Guid> childrenBusinessUnitIds = new List<Guid>();
        //    //List<Guid> accessSectionIds = new List<Guid>();


        //    foreach (var parentBusinessUnitId in parentBusinessUnitIds)
        //    {
        //        var tmpchildrenBusinessUnits = businessUnitService.GetCurrentUserChildrenBusinessUnits(parentBusinessUnitId, containsCurrentBusinessUnit: true);
        //        var tmpchildrenBusinessUnitIds = tmpchildrenBusinessUnits.Select(x => x.Id);
        //        var tmpaccessSectionIds = dbContext.Sections.FilterAccessToRisk(tmpchildrenBusinessUnitIds, accessor, dbContext).Select(x => x.Id);

        //        childrenBusinessUnitIds  = childrenBusinessUnitIds.Concat(tmpchildrenBusinessUnitIds);
        //        accessSectionIds = accessSectionIds.Concat(tmpaccessSectionIds);
        //    }

        //    childrenBusinessUnitIds=childrenBusinessUnitIds.Distinct();
        //    accessSectionIds=accessSectionIds.Distinct();



        //    var res = source
        //      .Where(x => childrenBusinessUnitIds.Any(l => l ==  x.BusinessUnitId || l == x.Section.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId))
        //      ;
        //    return res
        //        .AsQueryable()
        //        ;
        //    #endregion

        //}


        //[Obsolete]
        //public static IQueryable<ChanceManagement> FilterByParentBusinessUnit(this IQueryable<ChanceManagement> source,
        //    IHttpContextAccessor accessor,
        //    IBusinessUnitService businessUnitService,
        //    Guid? parentBusinessUnitId,
        //    IApplicationDbContext dbContext
        //    )
        //{
        //    #region Filter By Parent  BusinessUnit
        //    if (parentBusinessUnitId is null)
        //    {
        //        return source;
        //    }
        //    var childrenBusinessUnits = businessUnitService.GetCurrentUserChildrenBusinessUnits(parentBusinessUnitId.Value, containsCurrentBusinessUnit: true);
        //    var childrenBusinessUnitIds = childrenBusinessUnits.Select(x => x.Id);
        //    var accessSectionIds = dbContext.Sections.FilterAccessToRisk(childrenBusinessUnitIds, accessor, dbContext).Select(x => x.Id);

        //    return source.Where(x => childrenBusinessUnitIds.Any(l => l ==  x.BusinessUnitId || l == x.Section.BusinessUnitId) || accessSectionIds.Any(l => l == x.SectionId));
        //    #endregion
        //}

        //#endregion


    }
}
