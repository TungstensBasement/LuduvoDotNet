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
    private static readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://api.luduvo.com")
    };
    /// <summary>
    /// Used to create a new instance of the <see cref="Luduvo"/> client. The client is thread-safe and can be reused for multiple requests.
    /// </summary>
    public Luduvo()
    {
        
    }

    /// <summary>
    /// Fetches a user profile by ID from <c>/users/{userId}/profile</c> and deserializes it into <see cref="User"/>.
    /// </summary>
    /// <param name="userId">The numeric user identifier.</param>
    /// <returns>The deserialized user profile.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the API returns 404 or an empty payload.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    public async Task<User> GetUserByIdAsync(uint userId)
    {
        var response=await _httpClient.GetAsync($"/users/{userId}/profile");
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new UserNotFoundException();
        if(response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();
        var user=await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
        if (user is null) throw new UserNotFoundException();
        return user;
    }
}
