namespace EnvironmentManager.Commands;

public record ListEnvironmentVersionsCommand(string[] Args) : BaseApplicationCommand(ApplicationCommandCode.List, Args)
{
    public readonly string Alias = Args[0];
}