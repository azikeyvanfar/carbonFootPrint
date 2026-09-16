using ContractorBackend.Domain.Entities.Log;
using System.Collections.Concurrent;

namespace ContractorBackend.WebApiClient.Filters
{
    public interface IErrorQueue
    {
        void Enqueue(ErrorHistory error);
        bool TryDequeue(out ErrorHistory error);
    }

    public class ErrorQueue : IErrorQueue
    {
        private readonly ConcurrentQueue<ErrorHistory> _queue = new();

        public void Enqueue(ErrorHistory error) => _queue.Enqueue(error);

        public bool TryDequeue(out ErrorHistory error) => _queue.TryDequeue(out error);
    }

}
