namespace EnvironmentManager.Commands;

public record CloneRemoteEnvironmentCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.CloneRemote, Args)
{
    public readonly string Alias = Args[0];
}