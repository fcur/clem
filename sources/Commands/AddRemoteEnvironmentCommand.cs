namespace EnvironmentManager.Commands;

public record AddRemoteEnvironmentCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.AddRemote, Args)
{
    public readonly string Alias = Args[0];
    public readonly string Uri = Args[1];
    public readonly string Token = Args[2];
}