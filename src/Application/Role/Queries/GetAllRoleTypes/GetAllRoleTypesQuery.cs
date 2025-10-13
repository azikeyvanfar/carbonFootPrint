using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.Role.Queries.GetAllRoleTypes
{
    public class GetAllRoleTypesQuery : IRequest<List<SelectModel>>
    {
    }

    public class GetAllRoleTypesQueryHandler : IRequestHandler<GetAllRoleTypesQuery, List<SelectModel>>
    {
        public GetAllRoleTypesQueryHandler()
        {

        }
        public async Task<List<SelectModel>> Handle(GetAllRoleTypesQuery request, CancellationToken cancellationToken)
        {
            var result = new List<SelectModel>();

            foreach (int i in Enum.GetValues(typeof(RoleType)))
            {
                string name = Enum.GetName(typeof(RoleType), i);
                var obj = new SelectModel()
                {
                    Text = name,
                    Value = i.ToString()
                };
                result.Add(obj);
            }

            return result;
        }
    }
}
