using System;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Domain.Entities.Ghg
{
    /// <summary>
    /// انواع سوخت و ارزش حرارتی پایین (LHV) - مطابق جدول LHV of fuels در Settings!B29
    /// </summary>
    public class Fuel : BaseEntity, IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// نام سوخت (Natural Gas, Gas oil, Methane, ...)
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// نام فارسی سوخت
        /// </summary>
        public string? FaName { get; set; }

        /// <summary>
        /// ارزش حرارتی پایین
        /// </summary>
        public double Lhv { get; set; }

        /// <summary>
        /// واحد LHV (GJ/kNm3, GJ/Lit, GJ/ton, ...)
        /// </summary>
        public string LhvUnit { get; set; } = null!;

        /// <summary>
        /// واحد مصرف (kNm3, Lit, ton)
        /// </summary>
        public string ConsumptionUnit { get; set; } = null!;

        /// <summary>
        /// منبع LHV
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// سال اعتبار
        /// </summary>
        public int ValidFromYear { get; set; }
    }
}
