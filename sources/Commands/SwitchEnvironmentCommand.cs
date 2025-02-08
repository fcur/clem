namespace EnvironmentManager.Commands;

public record SwitchEnvironmentCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.Switch, Args)
{
    public readonly string Alias = Args[0];
}