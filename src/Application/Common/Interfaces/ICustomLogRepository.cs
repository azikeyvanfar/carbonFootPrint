using System;
using System.Threading.Tasks;
using ContractorBackend.Common.Extensions;
using ContractorBackend.Domain.Entities.Log;

namespace ContractorBackend.Application.Common.Interfaces
{
    /// <summary>
    /// سرویس لاگ برای background job ها و ایونت های اجرایی در آنها
    /// </summary>
    public interface ICustomLogRepository
    {
        Task<BackgroundTaskHistory> UpdateLogTask(BackgroundTaskHistory log);
        Task<BackgroundTaskHistory> AddLogTask(BackgroundTaskHistory log);
        Task<UserSyncLog> AddLogEvent(EventSyncType Type, EventStatus Status, string Message);
    }

    public class CustomLogRepository : ICustomLogRepository
    {
        private readonly ILogDbContext _dbContext;

        public CustomLogRepository(ILogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// ثبت لاگ برای ایونت اجرایی درون یک جاب
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="Status"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public async Task<UserSyncLog> AddLogEvent(EventSyncType Type, EventStatus Status, string Message)
        {

            int TypeId = Convert.ToInt32(Type.GetDisplay());
            int StatusId = Convert.ToInt32(Status.GetDisplay());
            var log = new UserSyncLog()
            {
                EventStatus = StatusId,
                EventType = TypeId,
                Message = Message,
            };

            await _dbContext.Set<UserSyncLog>().AddAsync(log);
            await _dbContext.SaveChangesAsync();

            return log;
        }

        /// <summary>
        /// ثبت لاگ برای جاب
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<BackgroundTaskHistory> AddLogTask(BackgroundTaskHistory log)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            await _dbContext.Set<BackgroundTaskHistory>().AddAsync(log);
            await _dbContext.SaveChangesAsync();

            return log;
        }

        public async Task<BackgroundTaskHistory> UpdateLogTask(BackgroundTaskHistory log)
        {
            if (log is null) throw new ArgumentNullException(nameof(log));

            _dbContext.Set<BackgroundTaskHistory>().Update(log);
            await _dbContext.SaveChangesAsync();

            return log;
        }
    }
}
