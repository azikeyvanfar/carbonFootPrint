using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// گازهای گلخانه‌ای - مطابق ردیف GWPs در شیت Settings ورک‌بوک MSC-GHG
    /// </summary>
    public class GhgGas : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// نام گاز (CO2, CH4 (C,B), N2O, SF6, ...)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی
        /// </summary>
        public string? FaName { get; set; }

        /// <summary>
        /// فرمول شیمیایی
        /// </summary>
        public string? ChemicalFormula { get; set; }

        public virtual ICollection<GlobalWarmingPotential> GlobalWarmingPotentials { get; set; } = new List<GlobalWarmingPotential>();
    }
}
