using System.Diagnostics.CodeAnalysis;
using EnvironmentManager.Commands;
using EnvironmentManager.Configuration;
using EnvironmentManager.Extensions;
using YamlDotNet.Serialization;

namespace EnvironmentManager.Services;

public sealed class ApplicationRoot : IApplicationRoot
{
    private readonly ClemYamlConfiguration _configuration;
    private readonly ISerializer _serializer;

    public ApplicationRoot(ClemYamlConfiguration configuration, ISerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentNullException.ThrowIfNull(serializer);

        _configuration = configuration;
        _serializer = serializer;
    }

    public Task Start(string[]? args, CancellationToken token = default)
    {
        if (string.IsNullOrEmpty(_configuration.WorkingDirectory))
        {
            return RequestWorkingDirectoryLocation();
        }
        
        if (args is null || args.Length == 0|| !BaseApplicationCommand.TryCreateCommand(args, out var command))
        {
            return PrintHelpInformation(token);
        }

        return command switch
        {
            HelpEnvironmentCommand helpCommand => HandleCommand(helpCommand, token),
            SwitchLatestEnvironmentCommand switchLatestCommand => HandleCommand(switchLatestCommand, token),
            SwitchEnvironmentVersionCommand switchVersionEnvironment => HandleCommand(switchVersionEnvironment, token),
            ListEnvironmentCommand listCommand => HandleCommand(listCommand, token),
            ListEnvironmentVersionsCommand listVersionsCommand  => HandleCommand(listVersionsCommand, token),
            CloneRemoteEnvironmentCommand cloneCommand => HandleCommand(cloneCommand, token),
            AddRemoteEnvironmentCommand addCommand => HandleCommand(addCommand, token),
            BackupLocalEnvironmentCommand backupLocalCommand=> HandleCommand(backupLocalCommand, token),
            _ => Task.CompletedTask
        };
    }

    private Task RequestWorkingDirectoryLocation()
    {
        var locationIsUnset = true;

        do
        {
            Console.Clear();
            Console.WriteLine("Please enter working directory location:");
            var location = Console.ReadLine();

            if (!Directory.Exists(location))
            {
                continue;
            }

            _configuration.WorkingDirectory = location;

            AppConfigurationManager.UpdateConfiguration(_configuration, _serializer.Serialize);

            locationIsUnset = false;
        } while (locationIsUnset);

        return Task.CompletedTask;
    }

    private Task HandleCommand(HelpEnvironmentCommand command, CancellationToken token)
    {
        return PrintHelpInformation(token);
    }

    private Task HandleCommand(SwitchLatestEnvironmentCommand command, CancellationToken token)
    {
        // cmd: switch remote1

        // ReSharper disable once NullableWarningSuppressionIsUsed
        var knownVersions = AppConfigurationManager.CheckKnownEnvironmentVersions(_configuration.WorkingDirectory!, command.Alias);
        var latest = knownVersions.MaxBy(v => v.VersionDate);
        var version = latest.Version;
        
        var versionCommand = new SwitchEnvironmentVersionCommand([command.Alias,version]);
        return HandleCommand(versionCommand, token);
    }
    
    private async Task HandleCommand(SwitchEnvironmentVersionCommand versionCommand, CancellationToken token)
    {
        // cmd: switch remote1 1739055013

        var backupLocalEnvCommand = new BackupLocalEnvironmentCommand(versionCommand.Args);
        await HandleCommand(backupLocalEnvCommand, token);

        // ReSharper disable once NullableWarningSuppressionIsUsed
        await ConsulDataManager.Upload(_configuration.LocalEnvironment, _configuration.WorkingDirectory!, versionCommand.Alias, versionCommand.Version, token);
    }

    private Task HandleCommand(ListEnvironmentCommand command, CancellationToken token)
    {
        // cmd: list
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var availableEnvironments = AppConfigurationManager.CheckKnownEnvironments(_configuration.WorkingDirectory!);

        foreach (var info in availableEnvironments)
        {
            Console.WriteLine($"'{info.Alias}' contains ({info.Count}) versions");
        }
        
        return Task.CompletedTask;
    }
    
    private Task HandleCommand(ListEnvironmentVersionsCommand command, CancellationToken token)
    {
        // cmd: list remote1
        // ReSharper disable once NullableWarningSuppressionIsUsed
        var availableVersions = AppConfigurationManager.CheckKnownEnvironmentVersions(_configuration.WorkingDirectory!, command.Alias);

        foreach (var info in availableVersions)
        {
            Console.WriteLine($"'{info.Version}' version from '{info.VersionDate:yyyy/MM/dd hh:mm:ss}'");
        }
        
        return Task.CompletedTask;
    }

    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    private async Task HandleCommand(CloneRemoteEnvironmentCommand command, CancellationToken token)
    {
        // cmd: clone remote1
        var environmentConfig = _configuration.KnownEnvironments.FirstOrDefault(v => v.Alias == command.Alias);

        if (environmentConfig == null)
        {
            return;
        }

        var remoteData = await ConsulDataManager.Request(environmentConfig, token);

        if (remoteData.Count == 0)
        {
            return;
        }

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await ConsulDataManager.Save(remoteData, _configuration.WorkingDirectory!, environmentConfig.Alias, unixTime, token);
    }

    private Task HandleCommand(AddRemoteEnvironmentCommand command, CancellationToken token)
    {
        // cmd: add remote2 http://localhost:8501 c177e7c8-b2e7-46c1-bf49-acecb95d88e2
        var knownEnvironments = new List<ConsulEnvironmentYamlConfiguration>(_configuration.KnownEnvironments)
        {
            new ConsulEnvironmentYamlConfiguration()
            {
                Alias = command.Alias,
                Uri = command.Uri,
                Token = command.Token
            }
        };

        _configuration.KnownEnvironments = knownEnvironments.ToArray();
        AppConfigurationManager.UpdateConfiguration(_configuration, _serializer.Serialize);
        return Task.CompletedTask;
    }

    private async Task HandleCommand(BackupLocalEnvironmentCommand command, CancellationToken token)
    {
        // cmd: backup
        var environmentConfig = _configuration.LocalEnvironment;
        
        var localData = await ConsulDataManager.Request(environmentConfig, token);
        if (localData.Count == 0)
        {
            return;
        }
        
        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        // ReSharper disable once NullableWarningSuppressionIsUsed
        await ConsulDataManager.Save(localData, _configuration.WorkingDirectory!, ConsulDataManager.BackupDirectory, unixTime, token);
    }

    private Task PrintHelpInformation(CancellationToken token)
    {
        Console.WriteLine("Available commands:");
        var commands = ApplicationCommandCode.GetHelpInformation();

        foreach (var item in commands)
        {
            Console.WriteLine($"{item.Code}: {item.Description}");
        }
        
        return Task.CompletedTask;
    }
}