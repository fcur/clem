namespace EnvironmentManager.Commands;

public record struct ApplicationCommandCode(string Value)
{
    public const string HelpCode = "help";
    public const string ListCode = "list";
    public const string SwitchCode = "switch";
    public const string CloneCode = "clone";
    public const string AddCode = "add";

    public static readonly ApplicationCommandCode Unknown = new ApplicationCommandCode(string.Empty);
    public static readonly ApplicationCommandCode Help = new ApplicationCommandCode(HelpCode);
    public static readonly ApplicationCommandCode List = new ApplicationCommandCode(ListCode);
    public static readonly ApplicationCommandCode Switch = new ApplicationCommandCode(SwitchCode);
    public static readonly ApplicationCommandCode CloneRemote = new ApplicationCommandCode(CloneCode);
    public static readonly ApplicationCommandCode AddRemote = new ApplicationCommandCode(AddCode);
}