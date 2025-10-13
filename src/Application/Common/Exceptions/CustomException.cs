using System;

namespace ContractorBackend.Application.Common.Exceptions
{
    public class CustomException : Exception
    {
        public string CustomMessage { get; }
        public object Errors { get; set; }
        public CustomException() : base()
        {

        }
        public CustomException(string customMessage) : base(customMessage)
        {
            CustomMessage = customMessage;
        }
        public CustomException(string customMessage, object errors) : base(customMessage)
        {
            CustomMessage = customMessage;
            Errors = errors;
        }

        public CustomException(string message, Exception inner) : base(message, inner) { }


        public static void ThrowIfNull(string? paramName = null)
        {
            throw new CustomException(string.Format("{0} یافت نشد.", paramName));
        }

    }


    public class HPRException : Exception
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }

        public HPRException(string Type, string Title, int Status, string Detail)
        {
            this.Type = Type;
            this.Title = Title;
            this.Status = Status;
            this.Detail = Detail;
        }

    }
}
