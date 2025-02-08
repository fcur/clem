namespace EnvironmentManager.Commands;

public record UnknownEnvironmentCommand(ApplicationCommandCode Code, string[]? Args) : BaseApplicationCommand(Code, Args);