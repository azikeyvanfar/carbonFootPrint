using System;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ContractorBackend.WebApiAdmin.Filters.SwaggerFilters
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type.IsEnum)
            {
                schema.Enum.Clear();
                Enum.GetNames(context.Type)
                    .ToList()
                    .ForEach(name => schema.Enum.Add(new OpenApiString($"{name}")));
            }
        }
    }
}
