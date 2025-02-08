using EnvironmentManager.Configuration;
using EnvironmentManager.Services.Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EnvironmentManager.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static HostApplicationBuilder AddServices(this HostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder;
    }

    public static HostApplicationBuilder AddYamlSerializer(this HostApplicationBuilder builder, ISerializer serializer)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(serializer);

        builder.Services.AddSingleton(serializer);
        return builder;
    }

    public static HostApplicationBuilder AddYamlDeserializer(this HostApplicationBuilder builder,
        IDeserializer deserializer)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(deserializer);

        builder.Services.AddSingleton(deserializer);
        return builder;
    }

    public static HostApplicationBuilder AddYamlConfiguration(this HostApplicationBuilder builder,
        ClemYamlConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configuration);

        builder.Services.AddSingleton(configuration);
        return builder;
    }

    public static HostApplicationBuilder SetupConsulClient(this HostApplicationBuilder builder, ClemYamlConfiguration configuration)
    {
        builder.Services.AddRefitClient<IConsulApi>(null, configuration.LocalEnvironment.Alias)
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(configuration.LocalEnvironment.Uri);
                _ = client.DefaultRequestHeaders.TryAddWithoutValidation("X-Consul-Token", configuration.LocalEnvironment.Token);
            });

        return builder;
    }
}

public static class App
{
    public static HostApplicationBuilder CreateApplicationBuilder(string[] args)
    {
        var yamlSerializer = CreateSerializer();
        var yamlDeserializer = CreateDeserializer();

        var configuration = AppConfigurationManager.LoadOrCreateDefault(
            yamlDeserializer.Deserialize<ClemYamlConfiguration>,
            yamlSerializer.Serialize);

        var settings = new HostApplicationBuilderSettings
        {
            Args = args,
            Configuration = new ConfigurationManager(),
            ApplicationName = "clem",
            DisableDefaults = true
        };

        var builder = new HostApplicationBuilder(settings);
        builder.AddYamlSerializer(yamlSerializer);
        builder.AddYamlDeserializer(yamlDeserializer);
        builder.AddYamlConfiguration(configuration);
        builder.SetupConsulClient(configuration);
        builder.Services.AddServices();

        return builder;
    }

    private static ISerializer CreateSerializer()
    {
        var serializerBuilder = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(HyphenatedNamingConvention.Instance);
        var serializer = serializerBuilder.Build();

        return serializer;
    }

    private static IDeserializer CreateDeserializer()
    {
        var deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(HyphenatedNamingConvention.Instance);
        var deserializer = deserializerBuilder.Build();

        return deserializer;
    }
}