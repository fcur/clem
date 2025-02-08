using EnvironmentManager.Services;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace EnvironmentManager.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IApplicationRoot, ApplicationRoot>();
        return serviceCollection;
    }        

    
 
}