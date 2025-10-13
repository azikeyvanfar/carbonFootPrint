using Microsoft.AspNetCore.Http;

namespace ContractorBackend.WebApiAdmin.Helpers
{
    public class ApiResult<T>
    {
        public ApiResult(bool isSuccess, int statusCode)
        {
            IsSuccess = isSuccess;
            Status = statusCode;
        }

        public ApiResult(bool isSuccess, int statusCode, string[] message)
            : this(isSuccess, statusCode)
        {
            Message = message;
        }

        public ApiResult(bool isSuccess, int statusCode, T data)
            : this(isSuccess, statusCode)
        {
            Data = data;
        }

        public ApiResult(bool isSuccess, int statusCode, (T, string) data)
            : this(isSuccess, statusCode)
        {
            Data = data.Item1;
            MethodCode = data.Item2;

        }
        public ApiResult(bool isSuccess, int statusCode, string[] message, T data)
            : this(isSuccess, statusCode, message)
        {
            Data = data;
        }

        public bool IsSuccess { get; private set; }

        public int Status { get; private set; }

        public string MethodCode { get; set; }

        public string[] Message { get; private set; }

        public T Data { get; private set; }
    }

    public class OkApiResult<T> : ApiResult<T>
    {
        private const int DefaultStatusCode = StatusCodes.Status200OK;


        public OkApiResult(string[] message)
            : base(true, DefaultStatusCode, message)
        {

        }

        public OkApiResult((T, string) data)
            : base(true, DefaultStatusCode, data)
        {

        }

        public OkApiResult(T data)
            : base(true, DefaultStatusCode, data)
        {

        }



        public OkApiResult(string[] message, T data)
            : base(true, DefaultStatusCode, message, data)
        {

        }
    }


    public class ICanApiResult<T>
    {
        public ICanApiResult(bool isSuccess, int statusCode)
        {
            Success = isSuccess;
            Status = statusCode;
        }

        public ICanApiResult(bool isSuccess, int statusCode, string[] message)
            : this(isSuccess, statusCode)
        {
            Message = message;
        }

        public ICanApiResult(bool isSuccess, int statusCode, T data)
            : this(isSuccess, statusCode)
        {
            Data = data;
        }

        public ICanApiResult(bool isSuccess, int statusCode, string[] message, T data)
            : this(isSuccess, statusCode, message)
        {
            Data = data;
        }

        public bool Success { get; private set; }

        public int Status { get; private set; }

        public string[] Message { get; private set; }

        public T Data { get; private set; }
    }

    public class OkICanApiResult<T> : ICanApiResult<T>
    {
        private const int DefaultStatusCode = StatusCodes.Status200OK;

        public OkICanApiResult(string[] message)
            : base(true, DefaultStatusCode, message)
        {

        }

        public OkICanApiResult(T data)
            : base(true, DefaultStatusCode, data)
        {

        }

        public OkICanApiResult(string[] message, T data)
            : base(true, DefaultStatusCode, message, data)
        {

        }
    }


}
