using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuduvoDotNet;

/// <summary>
/// Client for the Luduvo API.
/// </summary>
public class Luduvo
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        Converters = { new HexColorJsonConverter() }
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

    /// <summary>
    /// Creates a client that uses a caller-provided <see cref="HttpClient"/> instance.
    /// </summary>
    /// <param name="httpClient">Configured HTTP client (for example with custom handlers or base address).</param>
    public Luduvo(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Fetches a user profile by ID from <c>/users/{userId}/profile</c> and deserializes it into <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The numeric user identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>The deserialized user profile.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the API returns 404 or an empty payload.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    public async Task<User> GetUserByIdAsync(uint userId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/users/{userId}/profile", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new UserNotFoundException();
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new UserNotFoundException();
        }
        var user = await response.Content.ReadFromJsonAsync<User>(_jsonOptions, cancellationToken);
        if (user is null) throw new UserNotFoundException();
        return user;
    }

    public async Task<PartialUser[]> SearchUsersAsync(string username)
    {
        var response = await _httpClient.GetAsync($"/users?q={WebUtility.UrlEncode(username)}");
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<PartialUser[]>(_jsonOptions);
        return users ?? Array.Empty<PartialUser>();
    }
}
