using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace ContractorBackend.Application.Common.Implementation
{
    public interface IAbstractQuery<T> : IRequest<T>
    {
        //public bool IsRefreshButton { get; set; }
    }

    public interface IAbstractQueryHandler<IAbstractQuery, TQueryResult, TEntity> :
        IRequestHandler<IAbstractQuery, TQueryResult>
        where IAbstractQuery : IRequest<TQueryResult>
        where TQueryResult : class
        where TEntity : class

    {
        Task<TQueryResult> Handle(IAbstractQuery request, CancellationToken cancellationToken);
        Task<Unit> SaveToDbOnRefreshButton(IAbstractQuery request, List<TEntity> list, string isIdProperty, CancellationToken cancellationToken);
    }


}
