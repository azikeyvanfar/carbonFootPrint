using System.Threading.Tasks;
using System.Threading;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Entities.Log;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;


namespace ContractorBackend.Application.Common.Interfaces
{
    public interface ILogDbContext
    {

        #region TablesBase
        DbSet<ErrorHistory> ErrorHistories { get; set; }
        DbSet<AppLogEvent> AppLogEvents { get; set; }
        DbSet<UserSignIn> UserSignIns { get; set; }

        #endregion
        DbSet<BackgroundTaskHistory> BackgroundTaskHistories { get; set; }
        DbSet<UserSyncLog> UserSyncLogs { get; set; }
        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new());

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());


        DatabaseFacade Database { get; }
    }
}
