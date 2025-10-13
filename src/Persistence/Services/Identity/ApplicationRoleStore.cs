using System;
using System.Security.Claims;
using ContractorBackend.Application.Common.Identity;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using ContractorBackend.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContractorBackend.Persistence.Services.Identity
{
    public class ApplicationRoleStore :
        RoleStore<Role, ApplicationDbContext, long, UserRole, RoleClaim>,
        IApplicationRoleStore
    {
        private readonly IApplicationDbContext _uow;
        private readonly IdentityErrorDescriber _describer;

        public ApplicationRoleStore(
            IApplicationDbContext uow,
            IdentityErrorDescriber describer)
            : base((ApplicationDbContext)uow, describer)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _describer = describer ?? throw new ArgumentNullException(nameof(describer));
        }

        #region BaseClass

        protected override RoleClaim CreateRoleClaim(Role role, Claim claim)
        {
            return new RoleClaim
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            };
        }

        #endregion

        #region CustomMethods

        #endregion
    }
}