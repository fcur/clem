using EnvironmentManager.Extensions;

namespace EnvironmentManager.Configuration;

public static class AppConfigurationManager
{
    private const string ConfigFileName = "clem.yaml";
    
    public static ClemYamlConfiguration LoadOrCreateDefault(
        Func<string, ClemYamlConfiguration> deserializer,
        Func<ClemYamlConfiguration, string> serializer)
    {
        var configFilePath = GetConfigFilePath();

        if (File.Exists(configFilePath))
        {
            var existingFileContents = File.ReadAllText(configFilePath);
            
            var result = deserializer(existingFileContents);
            
            return result;
        }

        var config = new ClemYamlConfiguration();
        
        var fileContents = serializer(config);
        File.WriteAllText(configFilePath, fileContents);
        
        return config;
    }

    public static void UpdateConfiguration(ClemYamlConfiguration configuration, Func<ClemYamlConfiguration, string> serializer)
    {
        var configFilePath = GetConfigFilePath();
        var fileContents = serializer(configuration);
        File.WriteAllText(configFilePath, fileContents);
    }

    /// <returns>%HOMEDRIVE%%HOMEPATH%</returns>
    public static string GetConfigFilePath()
    {
        var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var configFilePath = Path.Combine(userProfilePath, ConfigFileName);

        return configFilePath;
    }

   
}

