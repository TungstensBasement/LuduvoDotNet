using System.Text.Json;

namespace LuduvoDotNet;

/// <summary>
/// Client for the Luduvo API.
/// </summary>
public partial class Luduvo
{
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
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");  
    }

    public static Task<Luduvo> LoginAsync(string identifier, string password)
    {
        _sharedHttpClient.DefaultRequestHeaders.Remove("Authorization");
        //_sharedHttpClient.PostAsync("/auth/login",)
        throw new NotImplementedException();
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
