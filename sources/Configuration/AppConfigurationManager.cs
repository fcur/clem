using System.Runtime.InteropServices.JavaScript;
using EnvironmentManager.Extensions;
using EnvironmentManager.Services;

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

    public static void UpdateConfiguration(ClemYamlConfiguration configuration,
        Func<ClemYamlConfiguration, string> serializer)
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

    public static IReadOnlyCollection<(string Alias, int Count)> CheckKnownEnvironments(string workingDirectory)
    {
        var directoriesInfo = Directory.GetDirectories(workingDirectory)
            .Select(v => (new DirectoryInfo(v).Name, Directory.GetDirectories(v).Length))
            .Where(v => !v.Name.Contains(ConsulDataManager.BackupDirectory))
            .ToArray();

        return directoriesInfo;
    }

    public static IReadOnlyCollection<(string Version, DateTimeOffset VersionDate)> CheckKnownEnvironmentVersions(string workingDirectory, string alias)
    {
        var environmentFolderPath = Path.Combine(workingDirectory, alias);

        var directoriesInfo = Directory.GetDirectories(environmentFolderPath)
            .Select(v =>
            {
                var directoryInfo = new DirectoryInfo(v);

                if (!long.TryParse(directoryInfo.Name, out var unixSeconds))
                {
                    return (string.Empty, DateTimeOffset.MinValue);
                }

                var time = DateTimeOffset.FromUnixTimeSeconds(unixSeconds);

                return (directoryInfo.Name, time);
            }).Where(v => !string.IsNullOrEmpty(v.Item1))
            .ToArray();

        return directoriesInfo;
    }
}