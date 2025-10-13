using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Shared;
using MediatR;

namespace ContractorBackend.Application.Shared.NewsCategories.Commands.CreateNewsCategory
{
    public class CreateNewsCategoryCommand : IRequest
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }
        /// <summary>
        /// فلگ اطلاعیه
        /// </summary>
        public bool IsNotifications { get; set; }

        public string Description { get; set; }

        public List<Guid>? BusinessUnitIds { get; set; }
    }

    public class CreateNewsCategoryCommandHandler : IRequestHandler<CreateNewsCategoryCommand>
    {
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _dbcontext;
        public CreateNewsCategoryCommandHandler(IMapper mapper, IApplicationDbContext dbcontext)
        {
            _mapper = mapper;
            _dbcontext = dbcontext;
        }

        public async Task<Unit> Handle(CreateNewsCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<NewsCategory>(request);

            var added = _dbcontext.NewsCategories.Add(entity).Entity;
            _dbcontext.SaveChanges();

            if (request.BusinessUnitIds.Any())
            {
                var NewsCategoryOrgUnits = request.BusinessUnitIds.Select(x => new NewsCategoryOrgUnit
                {
                    NewsCategoryId = added.Id,
                    OrgUnitId = x
                }).ToList();

                _dbcontext.NewsCategoryOrgUnits.AddRange(NewsCategoryOrgUnits);
                _dbcontext.SaveChanges();
            }

            return Unit.Value;
        }
    }
}
