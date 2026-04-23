using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LuduvoDotNet;

/// <summary>
/// Client for the Luduvo API.
/// </summary>
public partial class Luduvo
{
    private sealed class LoginResponse
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public JsonElement User { get; set; }
    }

    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new HexColorJsonConverter(), new LongDateTimeJsonConverter()}
    };

    private static readonly HttpClient _sharedHttpClient = new()
    {
        BaseAddress = new Uri("https://api.luduvo.com")
    };

    private readonly HttpClient _httpClient;

    /// <summary>
    /// Used to create a new instance of the <see cref="Luduvo"/> client. The client is thread-safe and can be reused for multiple requests.
    /// </summary>
    public Luduvo() : this(_sharedHttpClient)
    {
    }
    
    public Luduvo(string token):this(_sharedHttpClient)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public static async Task<Luduvo> LoginAsync(string identifier, string password)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be empty.", nameof(identifier));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        _sharedHttpClient.DefaultRequestHeaders.Remove("Authorization");

        var response = await _sharedHttpClient.PostAsJsonAsync(
            "/auth/login",
            new { identifier, password },
            _jsonOptions);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();

        response.EnsureSuccessStatusCode();

        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(_jsonOptions);
        if (loginResponse?.Token is null or "")
            throw new JsonException("Login response did not contain a token.");

        return new Luduvo(loginResponse.Token);
    }

    /// <summary>
    /// Creates a client that uses a caller-provided <see cref="HttpClient"/> instance.
    /// </summary>
    /// <param name="httpClient">Configured HTTP client (for example with custom handlers or base address).</param>
    public Luduvo(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }
}
