using System;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.IsSuiteEntities;
using ContractorBackend.Domain.Entities.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {

        #region System Authorization and Access Tables
        DbSet<Domain.Entities.Core.GeneralClaims> GeneralClaims { get; set; }
        DbSet<PageRoute> PageRoutes { get; set; }
        //DbSet<UserDetail> UserDetails { get; set; }
        DbSet<Menu> Menus { get; set; }
        DbSet<MenuItem> MenuItems { get; set; }
        DbSet<RolePageRouteAccess> RolePageRouteAccesses { get; set; }
        DbSet<PageRouteClaim> PageRouteClaims { get; set; }
        DbSet<OtpSetting> OtpSettings { get; set; }
        #endregion


        #region Core
        DbSet<Lookup> Lookups { get; set; }
        #endregion



        #region Shared
        DbSet<Document> Documents { get; set; }
        DbSet<FileExtension> FileExtensions { get; set; }
        DbSet<Captcha> Captchas { get; set; }
        DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        DbSet<RateLimitCustomRule> RateLimitCustomRules { get; set; }
        DbSet<SmsHistory> SmsHistories { get; set; }

        DbSet<QASubject> QASubjects { get; set; }
        DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        DbSet<NewsCategory> NewsCategories { get; set; }
        DbSet<News> News { get; set; }
        DbSet<NewsCategoryOrgUnit> NewsCategoryOrgUnits { get; set; }
        #endregion



        #region fr-sr
        DbSet<Contractor> Contractors { get; set; }
        #endregion


        #region Ghg (ردپای کربن - ISO 14064-1 / ISO 14067)
        DbSet<Domain.Entities.Ghg.GhgArea> GhgAreas { get; set; }
        DbSet<Domain.Entities.Ghg.CostCenter> CostCenters { get; set; }
        DbSet<Domain.Entities.Ghg.GhgGas> GhgGases { get; set; }
        DbSet<Domain.Entities.Ghg.GlobalWarmingPotential> GlobalWarmingPotentials { get; set; }
        DbSet<Domain.Entities.Ghg.EmissionCategory> EmissionCategories { get; set; }
        DbSet<Domain.Entities.Ghg.GhgParameter> GhgParameters { get; set; }
        DbSet<Domain.Entities.Ghg.Fuel> Fuels { get; set; }
        DbSet<Domain.Entities.Ghg.EmissionFactor> EmissionFactors { get; set; }
        DbSet<Domain.Entities.Ghg.CalculationFormula> CalculationFormulas { get; set; }
        DbSet<Domain.Entities.Ghg.GhgPeriod> GhgPeriods { get; set; }
        DbSet<Domain.Entities.Ghg.ActivityDataEntry> ActivityDataEntries { get; set; }
        DbSet<Domain.Entities.Ghg.EmissionResult> EmissionResults { get; set; }
        DbSet<Domain.Entities.Ghg.ProductFootprint> ProductFootprints { get; set; }
        #endregion




        #region BusinessUnitChildren
        //DbSet<BusinessUnitChildren> businessUnitChildrens { get; set; }
        //IQueryable<BusinessUnitChildren> GetCurrentUserChildrenBusinessUnits(long? businessUnitId, long currentUserId);

        // IQueryable<RiskCountReportDtoTVF> GetRiskCount(long? currentUserId, Guid businessUnitId);
        #endregion









        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;


        #region BaseClass
        void Migrate();

        T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible;
        object GetShadowPropertyValue(object entity, string propertyName);

        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();

        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new());

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());

        DatabaseFacade Database { get; }
        #endregion



    }
}
