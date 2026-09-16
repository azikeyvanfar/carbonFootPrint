using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// مراکز هزینه و فرآیندهای واحد - مطابق شیت Unit List در ورک‌بوک MSC-GHG Atlas
    /// </summary>
    public class CostCenter : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// کد مرکز هزینه (مثل 1310 برای احیای مستقیم 1)
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// نام مرکز هزینه
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فرآیند واحد (Unit Process)
        /// </summary>
        public string? UnitProcess { get; set; }

        /// <summary>
        /// ناحیه
        /// </summary>
        public Guid? AreaId { get; set; }
        public virtual GhgArea? Area { get; set; }
    }
}
