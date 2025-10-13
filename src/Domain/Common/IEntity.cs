using System;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Common
{
    public interface IBaseEntity
    {

    }

    public abstract class BaseEntity
    {
        //public EntityStatus Status { get; set; }
        //public long? IsSuiteId { get; set; }

        ///// <summary>
        ///// تاریخ ارسال درخواست به Is-suit
        ///// </summary>
        //public DateTime? RequestDate { get; set; }

        ///// <summary>
        ///// تاریخ دریافت پاسخ Is-suit
        ///// </summary>
        //public DateTime? ResponseDate { get; set; }
    }
    public abstract class IsSuiteBaseEntity
    {
        public EntityStatus Status { get; set; }
        public long? IsSuiteId { get; set; }

        /// <summary>
        /// تاریخ ارسال درخواست به Is-suit
        /// </summary>
        public DateTime? RequestDate { get; set; }

        /// <summary>
        /// تاریخ دریافت پاسخ Is-suit
        /// </summary>
        public DateTime? ResponseDate { get; set; }
    }
    public interface IEntity<T> : IBaseEntity
    {
        public T Id { get; set; }

        public bool IsActive { get; set; }
    }

    public interface IEntity : IEntity<Guid>
    {
    }



    public class Entity<TKey>
    {
        public TKey Id { get; set; }
    }

    public class Entity : Entity<Guid>
    {

    }
}