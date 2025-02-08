using Refit;

namespace EnvironmentManager.Services.Consul;

public interface IConsulApi
{
    // HTTP GET /v1/kv/?keys
    [Get("/v1/kv/")]
    Task<IReadOnlyCollection<string>> GetAllKeys(string keys = "");
    [Get("/v1/kv/")]
    Task<IReadOnlyCollection<string>> GetAllKeys([Header("X-Consul-Token")] string token, string keys = "");
    
    // HTTP GET /v1/kv/?recurse
    [Get("/v1/kv/")]
    Task<IReadOnlyCollection<ConsulConfigFullDto>> GetAllValues(string recurse = "");
    [Get("/v1/kv/")]
    Task<IReadOnlyCollection<ConsulConfigFullDto>> GetAllValues([Header("X-Consul-Token")] string token, string recurse = "");
    
    // HTTP DELETE /v1/kv/test2?dc=docker
    [Delete("/v1/kv/{key}")]
    Task<bool> Clear([AliasAs("key")] string key);
    [Delete("/v1/kv/{key}")]
    Task<bool> Clear([Header("X-Consul-Token")] string token, [AliasAs("key")] string key);
    
    // HTTP GET /v1/kv/test2?dc=docker
    [Get("/v1/kv/{key}")]
    Task<string> Load([AliasAs("key")] string key);
    [Get("/v1/kv/{key}")]
    Task<string> Load([Header("X-Consul-Token")] string token, [AliasAs("key")] string key);
    
    // HTTP PUT /v1/kv/test?dc=docker&flags=0
    [Put("/v1/kv/{key}")]
    Task<bool> Update([AliasAs("key")] string key, [Body] string body); 
    [Put("/v1/kv/{key}")]
    Task<bool> Update([Header("X-Consul-Token")] string token, [AliasAs("key")] string key, [Body] string body);
}