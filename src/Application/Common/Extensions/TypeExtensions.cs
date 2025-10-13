using System;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSearchable(this Type type)
        {
            return type.IsEnum
               || type.Equals(typeof(string))
               || type.Equals(typeof(int))
               || type.Equals(typeof(long))
               || type.Equals(typeof(decimal))
               || type.Equals(typeof(float))
               || type.Equals(typeof(double))
               || type.Equals(typeof(int?))
               || type.Equals(typeof(long?))
               || type.Equals(typeof(decimal?))
               || type.Equals(typeof(float?))
               || type.Equals(typeof(double?))
               || type.Equals(typeof(DateTime))
               || type.Equals(typeof(DateTime?));
        }
    }
}
