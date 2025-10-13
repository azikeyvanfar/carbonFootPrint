using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace ContractorBackend.Application.Common.Services
{
    [Obsolete("use Enum Extensions")]
    public static class EnumHelper<T>
    where T : struct, Enum // This constraint requires C# 7.3 or later.
    {
        [Obsolete("use Enum Extensions")]
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (FieldInfo fi in value.GetType()
                .GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T)Enum.Parse(value.GetType(), fi.Name, false));
            }
            return enumValues;
        }

        [Obsolete("use Enum Extensions")]
        public static T Parse(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        [Obsolete("use Enum Extensions")]
        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static
                | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        [Obsolete("use Enum Extensions")]
        public static IList<string> GetDisplayValues(Enum value)
        {
            return GetNames(value).Select(obj => GetDisplayValue(Parse(obj))).ToList();
        }

        [Obsolete("use Enum Extensions")]
        private static string lookupResource(Type resourceManagerProvider, string resourceKey)
        {
            var resourceKeyProperty = resourceManagerProvider.GetProperty(resourceKey,
                BindingFlags.Static | BindingFlags.Public, null, typeof(string),
                new Type[0], null);
            if (resourceKeyProperty != null)
            {
                return (string)resourceKeyProperty.GetMethod.Invoke(null, null);
            }

            return resourceKey; // Fallback with the key name
        }

        [Obsolete("use Enum Extensions")]
        public static string GetDisplayValue(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo == null)
            {
                return "";
            }

            var descriptionAttributes = fieldInfo.GetCustomAttributes(
                typeof(DisplayAttribute), false) as DisplayAttribute[];

            if (descriptionAttributes == null) return string.Empty;

            if (descriptionAttributes.Count() > 0 && descriptionAttributes[0].ResourceType != null)
                return lookupResource(descriptionAttributes[0].ResourceType,
                    descriptionAttributes[0].Name);


            return (descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }

}
