using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;

namespace ContractorBackend.Application.Shared.NewsCategories.Commands.UpdateNewsCategory
{
    public class UpdateNewsCategoryCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        /// <summary>
        /// فلگ اطلاعیه
        /// </summary>
        public bool IsNotifications { get; set; }
        public string Description { get; set; }
        public List<Guid>? BusinessUnitIds { get; set; }

    }

    public class UpdateNewsCategoryCommandhandler : IRequestHandler<UpdateNewsCategoryCommand>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbContext;
        public UpdateNewsCategoryCommandhandler(IMapper mapper, IApplicationDbContext dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public Task<Unit> Handle(UpdateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _dbContext.NewsCategories.FirstOrDefault(c => c.Id == request.Id);

            if (entity is null)
            {
                throw new NullReferenceException();
            }

            _mapper.Map(request, entity);

            _dbContext.NewsCategories.Update(entity);
            _dbContext.SaveChanges();

            var dbNewsCategoryOrgUnits = _dbContext.NewsCategoryOrgUnits.Where(x => x.NewsCategoryId == request.Id);

            _dbContext.NewsCategoryOrgUnits.RemoveRange(dbNewsCategoryOrgUnits);
            _dbContext.SaveChanges();

            if (request.BusinessUnitIds is not null && request.BusinessUnitIds.Any())
            {
                var insertList = request.BusinessUnitIds.Select(x => new NewsCategoryOrgUnit
                {
                    NewsCategoryId = request.Id,
                    //OrgUnitId =
                    //    _dbContext.BusinessUnits.FirstOrDefault(l =>l.Id == x) is not null ?
                    //    _dbContext.BusinessUnits.FirstOrDefault(l =>l.Id == x).Id : Guid.Empty
                });
                _dbContext.NewsCategoryOrgUnits.AddRange(insertList);
                _dbContext.SaveChanges();
            }

            return Task.FromResult(Unit.Value);
        }

        Task IRequestHandler<UpdateNewsCategoryCommand>.Handle(UpdateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
