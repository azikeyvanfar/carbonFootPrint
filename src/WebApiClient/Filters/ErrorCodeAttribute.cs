using System;
using ContractorBackend.Common.ErrorCodeHelper;

namespace ContractorBackend.WebApiClient.Filters
{
    public class ErrorCodeAttribute : Attribute
    {
        public string ErrorCode { get; set; }


        public ErrorCodeAttribute(string code)
        {
            ErrorCode = code;
        }

        [Obsolete]
        public ErrorCodeAttribute(ErrorCodeEnum errEnum)
        {
            ErrorCode = ErrorCodeClass.ErrorCodesDict[errEnum];
        }
    }
}
