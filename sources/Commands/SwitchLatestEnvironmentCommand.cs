namespace EnvironmentManager.Commands;

public record SwitchLatestEnvironmentCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.Switch, Args)
{
    public readonly string Alias = Args[0];
}