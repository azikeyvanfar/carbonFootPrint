using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Log
{
    /// <summary>
    /// جدول تاریخچه اجرای background Task ها
    /// </summary>
    public class BackgroundTaskHistory : Entity<long>, ICreationTrackingEntity, IModificationTrackingEntity
    {
        /// <summary>
        /// نام background Task
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// زمان اجرای background Task
        /// </summary>
        public string StartProcess { get; set; }

        /// <summary>
        /// زمان پایان background Task
        /// </summary>
        public string? EndProcess { get; set; } = null;

        /// <summary>
        /// ایا background task موفقیت آمیز بوده است یا خیر؟
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// پیغام
        /// </summary>
        public string? Message { get; set; }
    }
}
