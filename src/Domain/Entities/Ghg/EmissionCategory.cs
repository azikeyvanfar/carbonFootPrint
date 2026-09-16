using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Enums.Ghg;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// دسته‌بندی منابع انتشار - مطابق جدول Codes of Emission Categories در Settings!B162
    /// کد انتشار = ناحیه-علامت-شماره (مثل 1-C-1)
    /// </summary>
    public class EmissionCategory : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// نام انگلیسی دسته (Combustion, Vent, ...)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی دسته (احتراقی، فرآیندی، ...)
        /// </summary>
        public string FaName { get; set; } = null!;

        /// <summary>
        /// علامت اختصاری (C, V, F, M, E, T, D, P, B, R, L, X, W, Z, Y, N, ...)
        /// </summary>
        public string Sign { get; set; } = null!;

        /// <summary>
        /// شماره دسته بر اساس ISO 14064-1:2018 (1 تا 6)
        /// </summary>
        public int CategoryNo { get; set; }

        /// <summary>
        /// اسکوپ GHG Protocol (1 تا 3)
        /// </summary>
        public int Scope { get; set; }

        /// <summary>
        /// شرح منبع انتشار
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// ترتیب نمایش
        /// </summary>
        public int Priority { get; set; }
    }
}
