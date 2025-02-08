using EnvironmentManager.Configuration;

namespace EnvironmentManager.Extensions;

public sealed class ClemYamlConfiguration
{
    public ConsulEnvironmentYamlConfiguration LocalEnvironment { get; set; } = ConsulEnvironmentYamlConfiguration.Default;
    public ushort SelectedEnvironmentIndex { get; set; } = 0;
    public ConsulEnvironmentYamlConfiguration[] KnownEnvironments { get; set; } = [];
    public string? WorkingDirectory { get; set; }
}