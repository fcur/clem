namespace EnvironmentManager.Commands;

public record struct ApplicationCommandCode(string Value)
{
    public const string HelpCode = "help";
    public const string ListCode = "list";
    public const string SwitchCode = "switch";
    public const string CloneCode = "clone";
    public const string AddCode = "add";
    public const string BackupCode = "backup";

    public static readonly ApplicationCommandCode Unknown = new ApplicationCommandCode(string.Empty);
    public static readonly ApplicationCommandCode Help = new ApplicationCommandCode(HelpCode);
    public static readonly ApplicationCommandCode List = new ApplicationCommandCode(ListCode);
    public static readonly ApplicationCommandCode Switch = new ApplicationCommandCode(SwitchCode);
    public static readonly ApplicationCommandCode CloneRemote = new ApplicationCommandCode(CloneCode);
    public static readonly ApplicationCommandCode AddRemote = new ApplicationCommandCode(AddCode);
    public static readonly ApplicationCommandCode Backup = new ApplicationCommandCode(BackupCode);

    public static IReadOnlyCollection<(string Code, string Description)> GetHelpInformation()
    {
        return
        [
            (ListCode, "Print available environment list."),
            ($"{ListCode} [alias]", "Print specific environment versions."),
            (SwitchCode, "Switch to the latest environment version."),
            ($"{SwitchCode} [alias]", "Switch to a specific environment version."),
            ($"{CloneCode} [alias]", "Clone the defined remote environment."),
            ($"{AddCode} [alias] [endpoint]", "Save remote environment without authorization in configuration."),
            ($"{AddCode} [alias] [endpoint] [token]", "Save remote environment with authorization via token in configuration."),
            (BackupCode, "Backup local environment."),
        ];
    }
}