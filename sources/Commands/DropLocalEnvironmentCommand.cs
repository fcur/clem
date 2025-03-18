namespace EnvironmentManager.Commands;

public record DropLocalEnvironmentCommand(string[]? Args) : BaseApplicationCommand(ApplicationCommandCode.Drop, Args);