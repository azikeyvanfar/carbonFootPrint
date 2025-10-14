using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Context
{
    public class LogDbContext : DbContext, ILogDbContext
    {

        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }
        #region TablesBase
        public DbSet<ErrorHistory> ErrorHistories { get; set; } = null!;
        public DbSet<AppLogEvent> AppLogEvents { get; set; } = null!;
        public DbSet<UserSignIn> UserSignIns { get; set; }
        #endregion
        public DbSet<BackgroundTaskHistory> BackgroundTaskHistories { get; set; } = null!;
        public DbSet<UserSyncLog> UserSyncLogs { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //SeedAllData allData = new SeedAllData();
            //allData.callAddSeed(builder);


            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            //builder.AddDateTimeUtcKindConverter();

            //// This should be placed here, at the end.
            //builder.AddCreationTrackingShadowProperties();
            //builder.AddModificationTrackingShadowProperties();
            //builder.AddSoftDeleteShadowProperties();
            //builder.ApplySoftDeleteQueryFilters();
            //builder.AddRowVersionField();
        }
    }
}