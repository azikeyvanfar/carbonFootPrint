using System;
using System.Collections.Generic;
using ContractorBackend.Domain.Common;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Domain.Enums.Core;

namespace ContractorBackend.Domain.Entities.Core
{
    /// <summary>
    /// LoV data
    /// </summary>
    public class Lookup : IEntity, ICreationTrackingEntity, IModificationTrackingEntity, ISoftDeleteEntity
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// کد انگلیسی-عددی
        /// </summary>
        public string EnName { get; set; }
        /// <summary>   
        /// نام
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// ایدی والد
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// ایدی گروه
        /// </summary>
        public Guid? GroupId { get; set; }
        /// <summary>
        /// ل?ست ?ا آ?تم
        /// </summary>
        public LookupType Type { get; set; }
        /// <summary>
        /// الو?ت نما?ش?
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// دسته بندی (ایدی پدر)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// کد مربوط به Enumorable
        /// </summary>
        public string EnumCode { get; set; }

        #region navigation properties 
        public virtual Lookup Parent { get; set; }
        public virtual Lookup Category { get; set; }
        public virtual ICollection<Lookup> Children { get; set; }
        public virtual ICollection<Lookup> CategoryChildren { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        #endregion


    }


}
