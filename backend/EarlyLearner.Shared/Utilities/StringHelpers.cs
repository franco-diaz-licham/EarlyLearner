using System.Globalization;
using System.Text;

namespace EarlyLearner.Shared.Utilities;

public static class StringHelpers
{
    public static string Pluralise(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name;

        name = ToSnakeCase(name);

        // ends with "y" and not a vowel before it, replace "y" with "ies"
        var endsWithY = name.EndsWith("y", true, CultureInfo.InvariantCulture);
        var endsWithConsonant = name.Length > 1 && !"aeiou".Contains(char.ToLower(name[name.Length - 2]));
        if (endsWithY && endsWithConsonant && name.Length > 1) return name.Substring(0, name.Length - 1) + "ies";

        // ends with "s", "x", "z", "ch", or "sh", add "es"
        if (name.EndsWith("s", true, CultureInfo.InvariantCulture) ||
            name.EndsWith("x", true, CultureInfo.InvariantCulture) ||
            name.EndsWith("z", true, CultureInfo.InvariantCulture) ||
            name.EndsWith("ch", true, CultureInfo.InvariantCulture) ||
            name.EndsWith("sh", true, CultureInfo.InvariantCulture)) {
            return name + "es";
        }

        // Default: just add "s"
        return name + "s";
    }

    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return name;
        var builder = new StringBuilder(name.Length + 4);
        for (var index = 0; index < name.Length; index++) {
            var character = name[index];

            if (character == '-') {
                builder.Append('_');
                continue;
            }

            if (char.IsUpper(character)) {
                if (index > 0) builder.Append('_');
                builder.Append(char.ToLowerInvariant(character));
                continue;
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}
