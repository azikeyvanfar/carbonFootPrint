using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;



namespace ContractorBackend.Application.Common.Implementation
{
    public abstract class AbstractQueryHandler<IAbstractQuery, TQueryResult, TEntity /* ,IsSuitDto, ProjectDto*/>
        :
        IAbstractQueryHandler<IAbstractQuery, TQueryResult, TEntity>
        where IAbstractQuery : IRequest<TQueryResult>
        where TQueryResult : class
        where TEntity : class
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly IApplicationDbContext _dbContext;
        protected readonly IRepository<TEntity> _repository;
        private readonly IMapper _mapper;


        public AbstractQueryHandler(
            IHttpContextAccessor httpContextAccessor,
            IApplicationDbContext dbContext,
            IRepository<TEntity> repository,
            IMapper mapper
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
            _repository = repository;
            _mapper = mapper;
        }

        public IApplicationDbContext DbContext { get; }
        public UserManager<User> UserManager { get; }

        public abstract Task<TQueryResult> Handle(IAbstractQuery request, CancellationToken cancellationToken);

        public async Task<Unit> SaveToDbOnRefreshButton(IAbstractQuery request, List<TEntity> list, string isIdProperty, CancellationToken cancellationToken)
        {
            var watch = new Stopwatch();
            watch.Start();

            var fromApi = list.ToHashSet();

            var fromDb = _repository.GetAllAsNoTracking().ToHashSet();
            var existIdsInDBIds = fromDb.Select(x => x.GetType().GetProperty(isIdProperty).GetValue(x, null) ?? null).ToHashSet();


            #region Insert
            var toInsert = fromApi.Where(x => existIdsInDBIds.All(l => l.ToString() != x.GetType().GetProperty(isIdProperty).GetValue(x, null).ToString())).ToHashSet();
            foreach (var item in toInsert)
            {
                item.GetType().GetProperty("Id").SetValue(item, null);
            }
            _repository.InsertRange(toInsert);
            #endregion


            #region Update
            var toUpdate = fromApi.Where(x => existIdsInDBIds.Any(l => l.ToString() != x.GetType().GetProperty(isIdProperty).GetValue(x, null).ToString())).ToHashSet();
            foreach (var item in toUpdate)
            {
                var entity = fromDb.FirstOrDefault(x =>
                                    x.GetType()?
                                    .GetProperty(isIdProperty)?
                                    .GetValue(x).ToString()
                                    ==
                                    item.GetType()?
                                    .GetProperty(isIdProperty)?
                                    .GetValue(item).ToString()
                                    );

                if (entity != null)
                {
                    _mapper.Map(item, entity);
                    _repository.Update(entity as TEntity);
                }
                else
                {
                    // record is in api but not in db
                    // this record is handled in insert Region
                }
            }
            #endregion


            #region Delete
            var toDelete = fromDb.Where(x =>
                                    fromApi.All(l =>
                                            l.GetType().GetProperty(isIdProperty).GetValue(l, null).ToString() !=
                                            x.GetType().GetProperty(isIdProperty).GetValue(x, null).ToString()
                                            )
                                    )
                                    .ToHashSet();
            if (toDelete.Any())
            {
                _repository.DeleteRange(toDelete);
            }
            #endregion


            watch.Stop();
            var aaaa = watch.Elapsed;


            return Unit.Value;

        }

    }

}
