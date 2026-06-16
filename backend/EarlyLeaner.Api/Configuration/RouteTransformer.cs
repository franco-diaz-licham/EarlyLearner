using System.Text.RegularExpressions;

namespace EarlyLeaner.Api.Configuration;

public sealed class RouteTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        if (value is null) return null;
        var str = value.ToString()!;

        // Convert "PascalCase" or "PascalHTTPCase" to "pascal-case" or "pascal-http-case"
        // 1) insert dash between a lowercase/number and an uppercase letter
        // 2) insert dash between an uppercase followed by uppercase+lowercase boundary (HTMLParser -> html-parser)
        var result = Regex.Replace(input: str, pattern: "([a-z0-9])([A-Z])", replacement: "$1-$2");
        result = Regex.Replace(input: result, pattern: "([A-Z])([A-Z][a-z])", replacement: "$1-$2");

        return result.ToLowerInvariant();
    }

    public static string SplitToWords(string? value)
    {
        if (string.IsNullOrEmpty(value)) return value ?? string.Empty;

        // Insert space between lower/number and upper, and between upper followed by upper+lower
        var result = Regex.Replace(input: value!, pattern: "([a-z0-9])([A-Z])", replacement: "$1 $2");
        result = Regex.Replace(input: result, pattern: "([A-Z])([A-Z][a-z])", replacement: "$1 $2");
        return result;
    }
}

