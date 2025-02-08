using EnvironmentManager.Configuration;

namespace EnvironmentManager.Extensions;

public static class ClemYamlConfigurationExtensions
{
    public static ConsulEnvironmentYamlConfiguration? GetCurrentEnvironment(this ClemYamlConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        
        return configuration.KnownEnvironments.Length > configuration.SelectedEnvironmentIndex
            ? configuration.KnownEnvironments[configuration.SelectedEnvironmentIndex]
            : null;
    }
}