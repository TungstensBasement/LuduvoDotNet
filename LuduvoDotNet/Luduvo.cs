using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuduvoDotNet;

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
    public Luduvo()
    {
        
    }

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
