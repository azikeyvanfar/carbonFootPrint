using System.Threading.Tasks;
using ContractorBackend.Domain.Common;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
