using System.ComponentModel.DataAnnotations;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Log
{
    public class UserSyncLog : Entity<long>, ICreationTrackingEntity
    {
        /// <summary>
        /// نوع رویداد : 10 اکشن بروز رسانی 20 اکشن اضافه شدن
        /// </summary>
        public int EventType { get; set; }

        /// <summary>
        /// وضعیت اجرای رویداد : موفق 200 خطا 500
        /// </summary>
        public int EventStatus { get; set; }

        /// <summary>
        /// پیغام
        /// </summary>
        public string Message { get; set; }

    }

    public enum EventSyncType
    {

        [Display(Name = "2")]
        Modified_Action = 10,
        [Display(Name = "3")]
        Added_Function = 20
    }

    public enum EventStatus
    {
        [Display(Name = "200")]
        Success = 200,
        [Display(Name = "500")]
        Feild = 500
    }
}
