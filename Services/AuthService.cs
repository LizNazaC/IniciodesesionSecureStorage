using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Iniciosesion.Models;

namespace Iniciosesion.Services;

public sealed class AuthService
{
    private const string ApiBaseUrl = "https://dummyjson.com";
    private const string TokenKey = "access_token";
    private const string UsernameKey = "username";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var loginRequest = new LoginRequest
        {
            Username = username,
            Password = password
        };

        var json = JsonSerializer.Serialize(loginRequest, JsonOptions);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync($"{ApiBaseUrl}/auth/login", content);

        if (response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.Unauthorized)
        {
            return false;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                "No se pudo completar el inicio de sesión.",
                null,
                response.StatusCode);
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson, JsonOptions);
        if (string.IsNullOrWhiteSpace(loginResponse?.AccessToken))
        {
            throw new InvalidOperationException("La respuesta de la API no contiene un token de acceso.");
        }

        await SaveSessionAsync(loginResponse.AccessToken, username);
        return true;
    }

    public async Task<UserProfile?> GetCurrentUserAsync()
    {
        string? token;
        try
        {
            token = await SecureStorage.Default.GetAsync(TokenKey);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("No se pudo leer la sesión almacenada de forma segura.", exception);
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/auth/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var response = await _httpClient.SendAsync(request);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            Logout();
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                "No se pudo consultar el perfil del usuario.",
                null,
                response.StatusCode);
        }

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<UserProfile>(responseJson, JsonOptions)
            ?? throw new InvalidOperationException("La API devolvió un perfil vacío.");
    }

    public void Logout()
    {
        Preferences.Default.Remove(UsernameKey);

        try
        {
            SecureStorage.Default.Remove(TokenKey);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException("No se pudo eliminar el token almacenado de forma segura.", exception);
        }
    }

    private static async Task SaveSessionAsync(string token, string username)
    {
        try
        {
            await SecureStorage.Default.SetAsync(TokenKey, token);
            Preferences.Default.Set(UsernameKey, username);
        }
        catch (Exception exception)
        {
            Preferences.Default.Remove(UsernameKey);
            try
            {
                SecureStorage.Default.Remove(TokenKey);
            }
            catch
            {
            }

            throw new InvalidOperationException("No se pudo guardar la sesión de forma segura.", exception);
        }
    }
}
