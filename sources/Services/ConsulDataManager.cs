using System.Text.Json;
using EnvironmentManager.Configuration;
using EnvironmentManager.Services.Consul;
using Refit;

namespace EnvironmentManager.Services;

public static class ConsulDataManager
{
    private const string BackupDirectory = "backup";
    private const string SnapshotFileName = "snapshot.json";
    private const string ValuesDirectory = "values";
    private const string ValueFileExtension = ".txt";
    
    public static async Task<IReadOnlyCollection<ConsulConfigFullDto>> Request(ConsulEnvironmentYamlConfiguration consulEnvironment, CancellationToken token)
    {
        var consulApi = RestService.For<IConsulApi>(consulEnvironment.Uri);

        try
        {
            var result = await consulApi.GetAllValues(token: consulEnvironment.Token ?? string.Empty);

            return result;
        }
        catch (Exception ex)
        {
            // TBD
        }

        return Array.Empty<ConsulConfigFullDto>();
    }

    public static Task SaveBackup(IReadOnlyCollection<ConsulConfigFullDto> data, string workingDirectory, long unixTime, CancellationToken token) =>
        Save(data, workingDirectory, BackupDirectory, unixTime, token);
    
    public static async Task Save(IReadOnlyCollection<ConsulConfigFullDto> data, string workingDirectory, string alias, long unixTime, CancellationToken token)
    {
        var remoteEnvLocalFolder = Path.Combine(workingDirectory, alias);
        
        if (!Directory.Exists(remoteEnvLocalFolder))
        {
            Directory.CreateDirectory(remoteEnvLocalFolder);
        }

        var workingFolder = Path.Combine(remoteEnvLocalFolder, unixTime.ToString());
        Directory.CreateDirectory(workingFolder);

        await SaveSnapshot(data, workingFolder, token);
        await SaveValues(data, workingFolder, token);
    }

    public static async Task Upload(ConsulEnvironmentYamlConfiguration consulEnvironment, string workingDirectory, string alias, string version, CancellationToken token)
    {
        var valuesFolderPath = Path.Combine(workingDirectory, alias, version, ValuesDirectory);

        if (!Directory.Exists(valuesFolderPath))
        {
            return;
        }
        
        var configFiles = Directory.GetFiles(valuesFolderPath, "*", SearchOption.AllDirectories );

        if (configFiles.Length == 0)
        {
            return;
        }
        
        var consulApi = RestService.For<IConsulApi>(consulEnvironment.Uri);

        try
        {
            var tasks = configFiles.Select(v => UpdateConfiguration(consulEnvironment, consulApi, valuesFolderPath, v, token)).ToArray();
            await Task.WhenAll(tasks);
        } 
        catch (Exception ex)
        {
            // TBD
        }
    }

    public static async Task<bool> Drop(ConsulEnvironmentYamlConfiguration consulEnvironment, IReadOnlyCollection<string> keys, CancellationToken token)
    {
        var consulApi = RestService.For<IConsulApi>(consulEnvironment.Uri);

        try
        {
            var tasks = keys.Select(v => consulApi.Clear(consulEnvironment.Token ?? string.Empty, v)).ToArray();
            var dropResults = await Task.WhenAll(tasks);
            var isSuccess = dropResults.All(v=> v);

            return isSuccess;
        }
        catch (Exception ex)
        {
            // TBD
        }
        
        return false;
    }
    
    private static async Task UpdateConfiguration(ConsulEnvironmentYamlConfiguration consulEnvironment, IConsulApi consulApi, string valuesFolderPath, string configFilePath, CancellationToken token)
    {
        var consulKey = ConsulKey(valuesFolderPath, configFilePath);
        var consulValue = await File.ReadAllTextAsync(configFilePath, token);

        await consulApi.Update(consulEnvironment.Token ?? string.Empty, consulKey, consulValue);

        return;

        string ConsulKey(string valuesFolderRoot, string dataPath)
        {
            var relativePath = Path.GetRelativePath(valuesFolderRoot, dataPath);
            var extension = Path.GetExtension(dataPath);
            
            return relativePath.Replace('\\', '/').Replace(extension, string.Empty);
        }
    }

    private static async Task SaveSnapshot(IReadOnlyCollection<ConsulConfigFullDto> data, string workingFolder, CancellationToken token)
    {
        var snapshotFilePath = Path.Combine(workingFolder, SnapshotFileName);
        await using var snapFileStream = File.Create(snapshotFilePath);
        await JsonSerializer.SerializeAsync(snapFileStream, data, cancellationToken: token);
    }
    
    private static async Task SaveValues(IReadOnlyCollection<ConsulConfigFullDto> data, string workingFolder, CancellationToken token)
    {
        var valuesFolderPath = Path.Combine(workingFolder, ValuesDirectory);
        Directory.CreateDirectory(valuesFolderPath);

        var allKeys = data.Select(v => v.Key).ToArray();
        EnsureValuesDataFolders(valuesFolderPath, allKeys);

        foreach (var item in data)
        {
            var contentFilePath = ValueDataPath(valuesFolderPath, item.Key);
            var contentBytes = Convert.FromBase64String(item.Value);
            await using var valFileStream = File.Create(contentFilePath);
            await valFileStream.WriteAsync(contentBytes, token);
        }

        return;

        string ValueDataPath(string contentFolderPath, string key)
        {
            return Path.Combine(contentFolderPath, $"{key}{ValueFileExtension}");
        }
    }
    
    private static void EnsureValuesDataFolders(string workingFolder, IReadOnlyCollection<string> keys)
    {
        var directories = keys.Select(KeysSelector).Distinct().ToArray();
        
        Parallel.ForEach(directories, directoryPath => Directory.CreateDirectory(directoryPath));
        
        return;

        string KeysSelector(string key)
        {
            var parts = key.Split('/').ToArray();
            if (parts.Length == 1)
            {
                return workingFolder;
            }

            var wantedParts = parts.Prepend(workingFolder).SkipLast(1).ToArray();
            return Path.Combine(wantedParts);
        }
    }
}