using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Core;
using MediatR;
using Newtonsoft.Json;

namespace ContractorBackend.Application.Core.PageRoute.Commands.InsertData
{
    public class InsertDataCommand : IRequest
    {
    }

    public class InsertDataCommandHandler : IRequestHandler<InsertDataCommand>
    {
        private readonly IApplicationDbContext _context;
        public InsertDataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Unit> Handle(InsertDataCommand request, CancellationToken cancellationToken)
        {
            var path = "C:\\routing.txt";
            var jsonString = System.IO.File.ReadAllText(path);

            var lst = JsonConvert.DeserializeObject<List<RoutDto>>(jsonString);
            foreach (RoutDto item in lst)
            {
                //var obj = new Domain.Entities.PageRoute()
                //{
                //    Id = Guid.NewGuid(),
                //    Icon = item.icon,
                //    IsActive = true,
                //    RoleType = item.access == "admin" ? RoleType.Manager : RoleType.Employee,
                //    RouteName = item.label,
                //    Route = item.value
                //};
                //_context.PageRoutes.Add(obj);
            }
            _context.SaveChanges();

            return await Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<InsertDataCommand>.Handle(InsertDataCommand request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
