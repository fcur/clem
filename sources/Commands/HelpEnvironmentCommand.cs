namespace EnvironmentManager.Commands;

public record HelpEnvironmentCommand(string[]? Args) : BaseApplicationCommand(ApplicationCommandCode.Help, Args);