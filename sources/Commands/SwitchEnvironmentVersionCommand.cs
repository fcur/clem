namespace EnvironmentManager.Commands;

public record SwitchEnvironmentVersionCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.Switch, Args)
{
    public readonly string Alias = Args[0];
    public readonly string Version = Args[1];
}