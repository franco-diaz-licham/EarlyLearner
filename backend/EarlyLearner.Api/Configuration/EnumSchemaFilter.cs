using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EarlyLearner.Api.Configuration;

/// <summary>
/// Updates generated OpenAPI enum schemas so Swagger documents enum values as camel-case strings.
/// </summary>
/// <remarks>
/// This affects API documentation only. Runtime enum parsing and serialization are configured separately through
/// <see cref="System.Text.Json.Serialization.JsonStringEnumConverter"/>.
/// </remarks>
public sealed class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// Rewrites enum schema metadata after Swashbuckle has generated the default schema.
    /// </summary>
    /// <param name="schema">Generated OpenAPI schema to update.</param>
    /// <param name="context">Schema generation context containing the reflected CLR type.</param>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsEnum) return;
        if (schema is not OpenApiSchema openApiSchema) return;

        openApiSchema.Type = JsonSchemaType.String;
        openApiSchema.Format = null;
        openApiSchema.Enum ??= [];
        openApiSchema.Enum.Clear();

        foreach (var enumValue in Enum.GetValues(context.Type)) {
            var enumName = enumValue.ToString();
            if (enumName is null) continue;
            openApiSchema.Enum.Add(JsonValue.Create(JsonNamingPolicy.CamelCase.ConvertName(enumName)));
        }
    }
}
