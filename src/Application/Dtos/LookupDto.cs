using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    /// <summary>
    /// لیست نمایشی
    /// </summary>
    public class LookupDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// نام لیست
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// نام لیست والد
        /// </summary>
        public string? ParentName { get; set; } = null;
        /// <summary>
        /// شناسه لیست والد
        /// </summary>
        public Guid? ParentId { get; set; } = null;

        /// <summary>
        /// ایدی گروه
        /// </summary>
        public Guid? GroupId { get; set; }
        /// <summary>
        /// الو?ت نما?ش?
        /// </summary>
        public int? Priority { get; set; }


    }

    /// <summary>
    /// آیتم نمایشی
    /// </summary>
    public class LookupItemDto
    {
        /// <summary>
        /// شناسه آیتم
        /// </summary>
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// نام لیست
        /// </summary>
        public string ListName { get; set; }
        /// <summary>
        /// نام فارسی آیتم
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// نام انگلیسی آیتم
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// شناسه آیتم والد
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// نام آیتم والد
        /// </summary>
        public string? ParentName { get; set; }
        /// <summary>
        /// نام لیست والد
        /// </summary>
        public string ParentListName { get; set; }
        /// <summary>
        /// اولویت نمایشی آیتم
        /// </summary>
        public int Priority { get; set; } = 1;
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// دسته بندی (ایدی پدر)
        /// </summary>
        public Guid? CategoryId { get; set; }


        /// <summary>
        /// کد مربوط به Enumorable
        /// </summary>
        public string? EnumCode { get; set; }


    }

    /// <summary>
    /// افزودن لیست
    /// </summary>
    public class AddLookupDto
    {
        public bool IsActive { get; set; }
        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// شناسه والد
        /// </summary>
        public Guid? ParentId { get; set; } = null;
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }


    }

    /// <summary>
    /// بروز رسانی اطلاعات لیست
    /// </summary>
    public class UpdateLookupDto
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }


        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// شناسه والد
        /// </summary>
        public Guid? ParentId { get; set; } = null;
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }

    }

    /// <summary>
    /// افزودن ایتم به لیست
    /// </summary>
    public class AddLookupItemDto
    {

        public bool IsActive { get; set; }
        /// <summary>
        /// دسته بندی (ایدی پدر)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// شناسه ایدی والد ایتم نمایش داده شود
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// اولویت نمایشی
        /// </summary>
        public int Priority { get; set; } = 1;
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// کد مربوط به Enumorable
        /// </summary>
        public string? EnumCode { get; set; }
    }

    /// <summary>
    /// بروز رسانی اطلاعات ایتم لیست
    /// </summary>
    public class UpdateLookupItemDto
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        /// <summary>
        /// دسته بندی (ایدی پدر)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// شناسه گروه
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// نام فارسی
        /// </summary>
        public string FaName { get; set; }
        /// <summary>
        /// نام انگلیسی
        /// </summary>
        public string EnName { get; set; }
        /// <summary>
        /// شناسه ایدی والد ایتم نمایش داده شود
        /// </summary>
        public Guid? ParentId { get; set; }
        /// <summary>
        /// اولویت نمایشی
        /// </summary>
        public int Priority { get; set; } = 1;
        /// <summary>
        /// توضیحات
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// کد مربوط به Enumorable
        /// </summary>
        public string? EnumCode { get; set; }
    }

    public class LookupWithItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? ParentName { get; set; } = null;
        public Guid? ParentId { get; set; } = null;
        public List<LookupDto>? ChildList { get; set; } = null;
        public List<LookupItemDto>? Items { get; set; } = null;

    }



}
