using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// نواحی مجتمع فولاد مبارکه - مطابق Settings!B341 در ورک‌بوک MSC-GHG Atlas
    /// </summary>
    public class GhgArea : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// کد ناحیه (1 تا 9)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// نام انگلیسی ناحیه
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی ناحیه
        /// </summary>
        public string FaName { get; set; } = null!;

        /// <summary>
        /// کد کمیته (مثل IRM, SMC, ...)
        /// </summary>
        public string? CommitteeCode { get; set; }

        /// <summary>
        /// تعداد افراد استخدامی
        /// </summary>
        public int? EmployeeCount { get; set; }

        /// <summary>
        /// تعداد افراد پیمانکاری
        /// </summary>
        public int? ContractorCount { get; set; }

        public virtual ICollection<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
    }
}
