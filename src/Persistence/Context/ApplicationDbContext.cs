using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Extensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.IsSuiteEntities;
using ContractorBackend.Domain.Entities.Shared;
using ContractorBackend.Persistence.Configurations;
using ContractorBackend.Persistence.Configurations.Identity;
using ContractorBackend.Persistence.Extensions;
using ContractorBackend.Persistence.Toolkit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Serilog;

namespace ContractorBackend.Persistence.Context
{
    public class ApplicationDbContext :
       IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>,
        IApplicationDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// soft delete navigations bug fix 
        /// </summary>
        public string SoftDeleteNavigationsCheck()
        {
            var resultErrors = string.Empty;

            var deletedEntries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Deleted && e.Entity is ISoftDeleteEntity)
                .ToList()
                ;

            foreach (var entry in deletedEntries)
            {

                var entity = entry.Entity;
                var entityType1 = entity.GetType();

                if (entityType1 == typeof(GeneralClaims))
                {
                    continue;
                }

                Type realtype = this.Entry(entity).Metadata.ClrType;
                var primaryKeyVaue = entry.Properties.First(x => x.Metadata.IsPrimaryKey()).CurrentValue;


                #region Inside Method
                var entityId = primaryKeyVaue;

                // Get the entity type
                var entityType = ((DbContext)this).Model.FindEntityType(entityType1);

                // Get navigation properties
                var navigationProperties = entityType.GetNavigations().Where(x => x.IsCollection);

                foreach (var navigation in navigationProperties)
                {
                    // Get the related entity type
                    var relatedEntityType = navigation.TargetEntityType;

                    // Create a parameter expression for the related entity
                    var parameter = Expression.Parameter(relatedEntityType.ClrType, "e");

                    // Create a property expression for the foreign key
                    var foreignKeyProperty = relatedEntityType.GetForeignKeys()
                        .Where(fk => fk.PrincipalEntityType == entityType)
                        .FirstOrDefault()?
                        .Properties
                        .FirstOrDefault();

                    if (foreignKeyProperty != null)
                    {
                        // Create a lambda expression to check if any record exists
                        var foreignKeyIdExpression = Expression.Property(parameter, foreignKeyProperty.Name);
                        var entityIdExpression = Expression.Constant(entityId);
                        var equalityExpression = Expression.Equal(foreignKeyIdExpression, entityIdExpression);

                        var lambda = Expression.Lambda(equalityExpression, parameter);
                        // Use reflection to create the DbSet for the related entity
                        var dbSetMethod = typeof(DbContext).GetMethod("Set", Type.EmptyTypes)
                            .MakeGenericMethod(relatedEntityType.ClrType);
                        var dbSet = dbSetMethod.Invoke(this, null);

                        var anyMethod = typeof(Queryable).GetMethods()
                            .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(relatedEntityType.ClrType);

                        // Check if any records exist referencing the entityId
                        var exists = (bool)anyMethod.Invoke(null, new object[] { dbSet, lambda });

                        if (exists)
                        {
                            Log.Warning($"Entity with ID {entityId} is used by other records in navigation '{navigation.Name}'.");
                            entry.State = EntityState.Unchanged;
                            resultErrors += $"Entity with ID {entityId} is used by other records in navigation '{navigation.Name}'. ";
                            //throw new CustomException("سطر مورد نظر داای رکورد فعال است.");

                        }
                    }
                }
                #endregion

                //var method = typeof(IApplicationDbContextExtensions)
                //    .GetMethod(nameof(CheckEntityUsageAsync))
                //    .MakeGenericMethod(entityType);
                //await (Task)method.Invoke(null, new object[] { this, primaryKeyVaue, entry });

            }
            return resultErrors;
        }








        #region System Authorization and Access Tables
        public DbSet<GeneralClaims> GeneralClaims { get; set; }
        public DbSet<PageRoute> PageRoutes { get; set; }
        //public DbSet<UserDetail> UserDetails { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<RolePageRouteAccess> RolePageRouteAccesses { get; set; }
        public DbSet<PageRouteClaim> PageRouteClaims { get; set; }
        public DbSet<OtpSetting> OtpSettings { get; set; }

        #endregion



        #region Core
        public DbSet<Lookup> Lookups { get; set; }
        #endregion



        #region Shared
        public DbSet<Document> Documents { get; set; }
        public DbSet<FileExtension> FileExtensions { get; set; }
        public DbSet<Captcha> Captchas { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }
        public DbSet<RateLimitCustomRule> RateLimitCustomRules { get; set; }
        public DbSet<SmsHistory> SmsHistories { get; set; }
        public DbSet<QASubject> QASubjects { get; set; }
        public DbSet<QuestionAnswer> QuestionAnswers { get; set; }
        public DbSet<NewsCategory> NewsCategories { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsCategoryOrgUnit> NewsCategoryOrgUnits { get; set; }
        #endregion






        #region fr-sr
        public DbSet<Contractor> Contractors { get; set; }

        #region Ghg (Carbon Footprint)
        public DbSet<Domain.Entities.Ghg.GhgArea> GhgAreas { get; set; }
        public DbSet<Domain.Entities.Ghg.CostCenter> CostCenters { get; set; }
        public DbSet<Domain.Entities.Ghg.GhgGas> GhgGases { get; set; }
        public DbSet<Domain.Entities.Ghg.GlobalWarmingPotential> GlobalWarmingPotentials { get; set; }
        public DbSet<Domain.Entities.Ghg.EmissionCategory> EmissionCategories { get; set; }
        public DbSet<Domain.Entities.Ghg.GhgParameter> GhgParameters { get; set; }
        public DbSet<Domain.Entities.Ghg.Fuel> Fuels { get; set; }
        public DbSet<Domain.Entities.Ghg.EmissionFactor> EmissionFactors { get; set; }
        public DbSet<Domain.Entities.Ghg.CalculationFormula> CalculationFormulas { get; set; }
        public DbSet<Domain.Entities.Ghg.GhgPeriod> GhgPeriods { get; set; }
        public DbSet<Domain.Entities.Ghg.ActivityDataEntry> ActivityDataEntries { get; set; }
        public DbSet<Domain.Entities.Ghg.EmissionResult> EmissionResults { get; set; }
        public DbSet<Domain.Entities.Ghg.ProductFootprint> ProductFootprints { get; set; }
        #endregion
        #endregion



        #region BaseClass
        public void Migrate()
        {
            Database.Migrate();
        }

        public T GetShadowPropertyValue<T>(object entity, string propertyName) where T : IConvertible
        {
            var value = this.Entry(entity).Property(propertyName).CurrentValue;
            return value != null
                ? (T)Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture)
                : default!;
        }

        public object GetShadowPropertyValue(object entity, string propertyName)
        {
            return this.Entry(entity).Property(propertyName).CurrentValue;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges(); //NOTE: changeTracker.Entries<T>() will call it automatically.

            beforeSaveTriggers();
            var auditEntries = setAuditEntries();
            setHash();
            var events = domainEventEntities();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = base.SaveChanges();
            ChangeTracker.AutoDetectChangesEnabled = true;

            saveAuditEntries(auditEntries);
            _ = DispatchEvents(events);

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();
            var auditEntries = setAuditEntries();
            setHash();
            var events = domainEventEntities();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = await base.SaveChangesAsync(cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;

            saveAuditEntries(auditEntries);
            _ = DispatchEvents(events);

            return result;
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
        {
            ChangeTracker.DetectChanges();

            beforeSaveTriggers();

            ChangeTracker.AutoDetectChangesEnabled = false; // for performance reasons, to avoid calling DetectChanges() again.
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            ChangeTracker.AutoDetectChangesEnabled = true;

            return result;
        }

        private void beforeSaveTriggers()
        {
            //var res = SoftDeleteNavigationsCheck();
            //if (!string.IsNullOrWhiteSpace(res))
            //{
            //    throw new Exception(res);
            //}

            validateEntities();
            setShadowProperties();
        }

        private void setShadowProperties()
        {
            // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
            var props = this.GetService<IHttpContextAccessor>()?.GetShadowProperties();
            ChangeTracker.SetCreationTrackingEntityPropertyValues(props);
            ChangeTracker.SetModificationTrackingEntityPropertyValues(props);
            ChangeTracker.SetSoftDeleteEntityPropertyValues(props);
        }

        private void validateEntities()
        {
            var errors = this.GetValidationErrors();
            if (!string.IsNullOrWhiteSpace(errors))
            {
                // we can't use constructor injection anymore, because we are using the `AddDbContextPool<>`
                var loggerFactory = this.GetService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<ApplicationDbContext>();
                logger.LogError(errors);
                throw new InvalidOperationException(errors);
            }
        }

        private DomainEvent[] domainEventEntities()
        {
            return ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();
        }

        private void afterSaveTriggers()
        {
            dispatchEvents();
        }

        private async void dispatchEvents()
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .Select(x => x.Entity.DomainEvents)
                    .SelectMany(x => x)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);
                if (domainEventEntity == null) break;

                var domainEventService = this.GetService<IDomainEventService>();
                domainEventEntity.IsPublished = true;
                await domainEventService.Publish(domainEventEntity);
            }
        }

        private IList<AuditEntry> setAuditEntries()
        {
            var auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries<IHasRowIntegrity>())
            {
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                var auditEntry = new AuditEntry(entry);
                auditEntries.Add(auditEntry);


                foreach (var property in entry.Properties)
                {
                    var propertyName = property.Metadata.Name;
                    if (propertyName == nameof(IHasRowIntegrity.Hash))
                    {
                        continue;
                    }
                    if (propertyName == nameof(IHasRowVersion.RowVersion))
                    {
                        continue;
                    }

                    if (property.IsTemporary)
                    {
                        // It's an auto-generated value and should be retrieved from the DB after calling the base.SaveChanges().
                        auditEntry.AuditProperties.Add(new AuditProperty(propertyName, null, true, property));
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            //entry.Property(AuditableShadowProperties.CreatedDateTime).CurrentValue = now;//Shadow property
                            auditEntry.AuditProperties.Add(new AuditProperty(propertyName, property.CurrentValue, false, property));
                            break;
                        case EntityState.Modified:
                            auditEntry.AuditProperties.Add(new AuditProperty(propertyName, property.CurrentValue, false, property));
                            //entry.Property(AuditableShadowProperties.ModifiedDateTime).CurrentValue = now;//Shadow property
                            break;
                    }
                }
            }

            return auditEntries;
        }

        private void setHash()
        {
            foreach (var entry in ChangeTracker.Entries<IHasRowIntegrity>())
            {
                var entityHash = entry.GenerateEntityEntryHash(propertyToIgnore: nameof(IHasRowIntegrity.Hash));
                entry.Property(nameof(IHasRowIntegrity.Hash)).CurrentValue = entityHash;
            }
        }

        private void saveAuditEntries(IList<AuditEntry> auditEntries)
        {
            if (auditEntries.Count > 0)
            {
                foreach (var auditEntry in auditEntries)
                {
                    foreach (var auditProperty in auditEntry.AuditProperties.Where(x => x.IsTemporary))
                    {
                        // Now we have the auto-generated value from the DB.
                        auditProperty.Value = auditProperty.PropertyEntry.CurrentValue;
                        auditProperty.IsTemporary = false;
                    }
                    auditEntry.EntityEntry.Property(nameof(IHasRowIntegrity.Hash)).CurrentValue =
                        auditEntry.AuditProperties.ToDictionary(x => x.Name, x => x.Value).GenerateObjectHash();
                }

                base.SaveChanges();
            }
        }

        private async Task DispatchEvents(DomainEvent[] events)
        {
            var domainEventService = this.GetService<IDomainEventService>();
            foreach (var @event in events)
            {
                @event.IsPublished = true;
                await domainEventService.Publish(@event);
            }
        }

        #endregion



        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Adds all of the ASP.NET Core Identity related mappings at once.
            builder.AddCustomIdentityMappings();

            // Custom application mappings
            builder.AddCustomEntityMappings();

            builder.AddDateTimeUtcKindConverter();

            // This should be placed here, at the end.
            builder.AddCreationTrackingShadowProperties();
            builder.AddModificationTrackingShadowProperties();
            builder.AddSoftDeleteShadowProperties();
            builder.ApplySoftDeleteQueryFilters();
            builder.AddRowVersionField();
        }



    }
}

