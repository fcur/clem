namespace EnvironmentManager.Commands;

public abstract record BaseApplicationCommand(ApplicationCommandCode Code, string[]? Args = null)
{
    public static bool TryCreateCommand(string[]? args, out BaseApplicationCommand result)
    {
        if (args is null || args.Length == 0)
        {
            result = new UnknownEnvironmentCommand(ApplicationCommandCode.Unknown, args);
            return false;
        }

        var commandCode = args[0];
        var commandArgs = args.Length > 1 ? args.Skip(1).ToArray() : null;

        var targetCode = commandCode.ToLowerInvariant();

        result = targetCode switch
        {
            ApplicationCommandCode.HelpCode => new HelpEnvironmentCommand(commandArgs),
            ApplicationCommandCode.ListCode => new ListEnvironmentCommand(commandArgs),
            ApplicationCommandCode.SwitchCode when commandArgs is { Length: 1 } => new SwitchEnvironmentCommand(commandArgs),
            ApplicationCommandCode.CloneCode when commandArgs is { Length: 1 } => new CloneRemoteEnvironmentCommand(commandArgs),
            ApplicationCommandCode.AddCode when commandArgs is { Length: 3 } => new AddRemoteEnvironmentCommand(commandArgs),
            _ => new UnknownEnvironmentCommand(new ApplicationCommandCode(targetCode), commandArgs)
        };
        return true;
    }
}