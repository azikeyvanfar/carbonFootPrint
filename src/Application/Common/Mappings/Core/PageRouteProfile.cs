using System;
using AutoMapper;
using ContractorBackend.Application.Core.PageRoute.Commands.CreatePageRoute;
using ContractorBackend.Application.Core.PageRoute.Commands.UpdatePageRoute;
using ContractorBackend.Application.Dtos.Core;
using ContractorBackend.Domain.Entities.Core;

namespace ContractorBackend.Application.Mapping.Core
{
    public class PageRouteProfile : Profile
    {
        public PageRouteProfile()
        {
            CreateMap<PageRoute, PageRouteDto>().ForMember(c => c.RoleTypeId, x => x.MapFrom(d => ((int)d.RoleType).ToString()));

            CreateMap<CreatePageRouteCommand, PageRoute>().ForMember(c => c.Id, m => m.MapFrom(_ => Guid.NewGuid()));

            CreateMap<UpdatePageRouteCommand, PageRoute>();
        }
    }
}
