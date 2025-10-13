using System.Collections.Generic;

namespace ContractorBackend.Common.ErrorCodeHelper
{
    public static class ErrorCodeClass
    {
        public static Dictionary<ErrorCodeEnum, string> ErrorCodesDict = new(){
            { ErrorCodeEnum.AccLogin ,"100-1" }
        };
    }
}
