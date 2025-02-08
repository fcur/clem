namespace EnvironmentManager.Services;

public interface IApplicationRoot
{
    Task Start(string[]? args, CancellationToken token = default);
}