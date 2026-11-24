using System.Text.Json.Serialization;

namespace Iniciosesion.Models;

public sealed class LoginResponse
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }
}
