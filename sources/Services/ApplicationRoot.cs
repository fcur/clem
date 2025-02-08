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
        if (args is null || args.Length == 0)
        {
            return Task.CompletedTask;
        }

        if (string.IsNullOrEmpty(_configuration.WorkingDirectory))
        {
            return RequestWorkingDirectoryLocation();
        }

        if (!BaseApplicationCommand.TryCreateCommand(args, out var command))
        {
            return PrintHelpInformation(token);
        }

        return command switch
        {
            // TODO: add backup for local data before overriding by remote data
            HelpEnvironmentCommand helpCommand => HandleCommand(helpCommand, token),
            SwitchEnvironmentCommand switchEnvironment => HandleCommand(switchEnvironment, token),
            ListEnvironmentCommand listCommand => HandleCommand(listCommand, token),
            CloneRemoteEnvironmentCommand cloneCommand => HandleCommand(cloneCommand, token),
            AddRemoteEnvironmentCommand addCommand => HandleCommand(addCommand, token),
            _ => Task.CompletedTask
        };
    }

    private Task RequestWorkingDirectoryLocation()
    {
        var unknownLocation = true;

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

            unknownLocation = false;
        } while (unknownLocation);

        return Task.CompletedTask;
    }

    private Task HandleCommand(HelpEnvironmentCommand command, CancellationToken token)
    {
        return PrintHelpInformation(token);
    }

    private Task HandleCommand(SwitchEnvironmentCommand command, CancellationToken token)
    {
        // cmd: switch remote1
        throw new NotImplementedException();
    }

    private Task HandleCommand(ListEnvironmentCommand command, CancellationToken token)
    {
        // cmd: list
        throw new NotImplementedException();
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

        var remoteData = await RemoteDataManager.RequestData(environmentConfig, token);

        if (remoteData.Count == 0)
        {
            return;
        }

        var unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await RemoteDataManager.SaveRemoteData(remoteData, _configuration.WorkingDirectory!, environmentConfig.Alias, unixTime, token);
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

    private Task PrintHelpInformation(CancellationToken token)
    {
        Console.WriteLine("Available commands:");

        Console.WriteLine(ApplicationCommandCode.HelpCode);
        Console.WriteLine(ApplicationCommandCode.ListCode);
        Console.WriteLine(ApplicationCommandCode.SwitchCode);
        Console.WriteLine(ApplicationCommandCode.CloneCode);
        Console.WriteLine(ApplicationCommandCode.AddCode);

        return Task.CompletedTask;
    }
}