using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace BoneLog.Tools;

public static class FrontMatterDeserializer
{
    private static readonly IDeserializer Deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    public static T Deserialize<T>(string yamlContent) where T : class, new() => Deserializer.Deserialize<T>(yamlContent);
}
