using System;

namespace ContractorBackend.Application.Common.Exceptions
{
    public class NotAllowedException : Exception
    {
        public string? CustomMessage { get; }

        public NotAllowedException()
            : base()
        {
            CustomMessage = null;
        }

        public NotAllowedException(string message)
            : base(message)
        {
            CustomMessage = message;
        }
    }
}
