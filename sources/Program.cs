using EnvironmentManager.Extensions;
using EnvironmentManager.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = App.CreateApplicationBuilder(args);
builder.AddServices();
using var host = builder.Build();

var app = host.Services.GetRequiredService<IApplicationRoot>();

await host.StartAsync();
await app.Start(args);
await host.StopAsync();
