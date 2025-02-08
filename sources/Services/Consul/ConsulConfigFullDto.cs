using System.Text.Json.Serialization;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace EnvironmentManager.Services.Consul;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class ConsulConfigFullDto
{
    [JsonPropertyName("LockIndex")]
    public int LockIndex { get; set; }
    
    [JsonPropertyName("Key")]
    public string Key { get; set; }
    
    [JsonPropertyName("Flags")]
    public int Flags { get; set; }
    
    [JsonPropertyName("Value")]
    public string Value { get; set; }
    
    [JsonPropertyName("CreateIndex")]
    public int CreateIndex { get; set; }
    
    [JsonPropertyName("ModifyIndex")]
    public int ModifyIndex { get; set; }
}