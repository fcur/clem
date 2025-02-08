#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace EnvironmentManager.Configuration;

public sealed class ConsulEnvironmentYamlConfiguration
{
    public string Alias { get; set; }
    public string Uri { get; set; }
    public string? Token { get; set; }

    public static readonly ConsulEnvironmentYamlConfiguration Default = new ConsulEnvironmentYamlConfiguration
    {
        Alias = "default",
        Uri = "http://localhost:8500",
        Token = string.Empty,
    };
}