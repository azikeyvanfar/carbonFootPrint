using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Application.Dtos.Share;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ContractorBackend.Application.Shared.NewsCategories.Queries.GetNewsCategoryById
{
    public class GetNewsCategoryByIdQuery : IRequest<NewsCategoryDto>
    {
        public Guid Id { get; set; }
        public GetNewsCategoryByIdQuery(Guid id)
        {
            Id = id;
        }

        public class GetNewsCategoryByIdQueryHandler : IRequestHandler<GetNewsCategoryByIdQuery, NewsCategoryDto>
        {
            private readonly IMapper _mapper;
            private readonly IApplicationDbContext _dbContext;

            public GetNewsCategoryByIdQueryHandler(
                IMapper mapper,
                IApplicationDbContext dbContext
                )
            {
                _mapper = mapper;
                _dbContext = dbContext;
            }
            public async Task<NewsCategoryDto> Handle(GetNewsCategoryByIdQuery request, CancellationToken cancellationToken)
            {
                var dto = await _dbContext.NewsCategories
                    .Where(x => x.Id == request.Id)
                    .ProjectTo<NewsCategoryDto>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (dto is null)
                {
                    throw new NullReferenceException();
                }

                return dto;
            }
        }
    }
}
