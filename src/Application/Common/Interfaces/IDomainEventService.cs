using ContractorBackend.Domain.Common;

namespace ContractorBackend.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        System.Threading.Tasks.Task Publish(DomainEvent domainEvent);
    }
}
