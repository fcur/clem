namespace EnvironmentManager.Commands;

public record ListEnvironmentCommand(string[]? Args) : BaseApplicationCommand(ApplicationCommandCode.List, Args);