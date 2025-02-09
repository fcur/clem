namespace EnvironmentManager.Commands;

public record BackupLocalEnvironmentCommand(string[]? Args) : BaseApplicationCommand(ApplicationCommandCode.Backup, Args);