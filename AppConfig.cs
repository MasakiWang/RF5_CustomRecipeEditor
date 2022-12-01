using Newtonsoft.Json;

[JsonObject]
public sealed class AppConfig
{
    public static AppConfig Instance { get; set; }

    [JsonProperty]
    public string Workspace { get; set; }
}