using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Common.Models;
using ContractorBackend.Application.Common.Services;
using ContractorBackend.Domain.Entities.Core;
using ContractorBackend.Domain.Enums.Core;
using MediatR;

namespace ContractorBackend.Application.MenuItems.Queries.GetMenuTypes
{
    public class GetMenuTypesQuery : IRequest<List<SelectModel>>
    {
    }

    public class GetMenuTypesQueryHandler : IRequestHandler<GetMenuTypesQuery, List<SelectModel>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<MenuItem> _repository;

        public GetMenuTypesQueryHandler(IMapper mapper, IRepository<MenuItem> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<List<SelectModel>> Handle(GetMenuTypesQuery request, CancellationToken cancellationToken)
        {

            var result = new List<SelectModel>();

            foreach (int i in Enum.GetValues(typeof(MenuType)))
            {
                string name = Enum.GetName(typeof(MenuType), i);
                var obj = new SelectModel()
                {
                    Text = EnumHelper<MenuType>.GetDisplayValue((MenuType)i),
                    Value = i.ToString()
                };
                result.Add(obj);
            }

            return result;
        }
    }
}
