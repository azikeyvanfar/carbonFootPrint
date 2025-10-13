using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Entities.Identity;
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
        int SaveChanges(bool acceptAllChangesOnSuccess);

        int SaveChanges();


        DatabaseFacade Database { get; }
    }
}
