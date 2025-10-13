using System;
using System.Collections.Generic;

namespace ContractorBackend.Application.Dtos.Core
{
    // todo: DELETE ME

    [Obsolete]
    public class UserModel
    {
        public UserModel()
        {
            //AccessibleMenuItems = new();
            //RoleClaims = new();
            AccessiblePageRouteClaims = new();
        }
        public long Id { get; set; }
        public string PersonnelCode { get; set; }
        public string OTP { get; set; }
        public DateTime? OtpCreationDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EnFirstName { get; set; }
        public string EnLastName { get; set; }
        public string FatherName { get; set; }
        public string NationalCode { get; set; }
        /// <summary>
        /// شماره شناسنامه
        /// </summary>
        public string IDCard { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool Gender { get; set; }
        public long? GroupId { get; set; }
        public string GroupName { get; set; }
        public long? OrganizationLevelId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public List<string> RoleNames { get; set; }
        public List<long> RoleIds { get; set; }
        public int UnReadTickets { get; set; }
        public List<AccessiblePageRouteClaim> AccessiblePageRouteClaims { get; set; }
        //public List<Claim> RoleClaims { get; set; }
    }

    public class AccessiblePageRouteClaim
    {
        public AccessiblePageRouteClaim()
        {
            AccessibleClaimIds = new();
        }
        public Guid PageRouteId { get; set; }

        public List<Guid> AccessibleClaimIds { get; set; }
    }
}
