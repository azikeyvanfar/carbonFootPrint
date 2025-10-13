using System;

namespace ContractorBackend.Application.Common.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public string? CustomMessage { get; }

        public ForbiddenAccessException() : base()
        {
            CustomMessage = null;
        }

        public ForbiddenAccessException(string message)
            : base(message)
        {
            CustomMessage = message;
        }

        public ForbiddenAccessException(string message, Exception exception)
            : base(message, exception)
        {
            CustomMessage = message;
        }
    }
}
