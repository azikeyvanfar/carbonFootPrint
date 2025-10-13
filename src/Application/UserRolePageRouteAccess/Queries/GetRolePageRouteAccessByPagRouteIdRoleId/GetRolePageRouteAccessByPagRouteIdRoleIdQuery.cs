using System;
using System.Collections.Generic;
using ContractorBackend.Application.Dtos;
using MediatR;

namespace ContractorBackend.Application.UserRolePageRouteAccesses.Queries.GetRolePageRouteAccessByPagRouteIdRoleId
{
    public class GetRolePageRouteAccessByPagRouteIdRoleIdQuery : IRequest<List<RolePageRouteAccessDto>>
    {
        public Guid Id { get; set; }
        public long RoleId { get; set; }
        public Guid PageRouteId { get; set; }

        //public class GetRoleAccessItemByIdRoleIdMenuItemIdQueryHandler : IRequestHandler<GetRolePageRouteAccessByPagRouteIdRoleIdQuery,List< RolePageRouteAccessDto>>
        //{
        //    private readonly IMapper _mapper;

        //    private readonly IApplicationDbContext _context;
        //    public GetRoleAccessItemByIdRoleIdMenuItemIdQueryHandler(IMapper mapper ,IApplicationDbContext context)
        //    {
        //        _mapper = mapper;

        //        _context = context;
        //    }

        //    //public Task<List<RolePageRouteAccessDto>> Handle(GetRolePageRouteAccessByPagRouteIdRoleIdQuery request, CancellationToken cancellationToken)
        //    //{
        //    //    if ((request.Id == Guid.Empty|| request.MenuItemId == null) && (request.RoleId == 0|| request.RoleId == null) && (request.Id == Guid.Empty|| request.Id == null))
        //    //            {
        //    //        throw new BadRequestException();
        //    //    }

        //    //    var query = new List<RolePageRouteAccess>();
        //    //        query = _context.RoleAccessItems.Where(c => c.Id == request.Id || c.PageRouteId == request.PageRouteId || c.RoleId == request.RoleId).ToList();

        //    //    if (query is null)
        //    //        throw new NullReferenceException();

        //    //    var roleAccessItemDto =
        //    //         _mapper.Map<List<RolePageRouteAccess>, List<RoleAccessItemDto>>(query);
        //    //    return  Task.FromResult(roleAccessItemDto);
        //    //}
        //}

    }
}
