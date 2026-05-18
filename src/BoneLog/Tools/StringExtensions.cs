using System.Globalization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BoneLog.Tools;

public static class StringExtensions
{
    private static readonly IDeserializer FrontMatterDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public static DateTime? ToDateTime(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var dt) ? dt : null;
    }

    public static T DeserializeFrontMatter<T>(this string yamlContent) where T : class, new() => FrontMatterDeserializer.Deserialize<T>(yamlContent);
}
