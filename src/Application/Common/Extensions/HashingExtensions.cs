using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using ContractorBackend.Application.Common.Interfaces;
using ContractorBackend.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ContractorBackend.Application.Common.Extensions
{
    public static class HashingExtensions
    {
        public static string GenerateObjectHash(this object? @object)
        {
            if (@object == null)
            {
                return string.Empty;
            }

            var jsonData = JsonConvert.SerializeObject(@object, Formatting.Indented);
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(jsonData);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }

        public static string GenerateEntityEntryHash(this EntityEntry entry, string propertyToIgnore)
        {
            var auditEntry = new Dictionary<string, object>();
            foreach (var property in entry.Properties)
            {
                var propertyName = property.Metadata.Name;
                if (propertyName == propertyToIgnore)
                {
                    continue;
                }
                if (propertyName == nameof(IHasRowVersion.RowVersion))
                {
                    continue;
                }
                auditEntry[propertyName] = property.CurrentValue;
            }
            return auditEntry.GenerateObjectHash();
        }

        public static string GenerateEntityHash<TEntity>(this DbContext context, TEntity entity, string propertyToIgnore)
        {
            return context.Entry(entity).GenerateEntityEntryHash(propertyToIgnore);
        }

        public static string GenerateEntityHash<TEntity>(this IApplicationDbContext context, TEntity entity, string propertyToIgnore) where TEntity : class
        {
            return context.Entry(entity).GenerateEntityEntryHash(propertyToIgnore);
        }
    }
}
