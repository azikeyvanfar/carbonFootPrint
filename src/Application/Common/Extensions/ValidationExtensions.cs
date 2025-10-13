using System.Collections.Generic;
using DNTPersianUtils.Core;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static bool HasValidMobileNumber(string value)
        {
            if (!value.IsValidIranianMobileNumber())
            {
                return false;
            }

            return true;
        }

        public static bool HasValidMobileNumber(IEnumerable<string>? values)
        {
            if (values is null)
            {
                return true;
            }

            foreach (var value in values)
            {
                if (!HasValidMobileNumber(value))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
