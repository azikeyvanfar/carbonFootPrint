using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    public class UserDto
    {
        public long Id { get; set; }
        public bool IsActive { get; set; }


        public Guid? EmployeeId { get; set; }


        public string FullName { get; set; }
        /// <summary>
        /// شماره پرسنلی فرد
        /// </summary>
        public string PersonnelCode { get; set; }
        /// <summary>
        /// نام
        /// </summary> 
        public string FirstName { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary> 
        public string LastName { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string NationalCode { get; set; }

        public List<RoleDto> Roles { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}
