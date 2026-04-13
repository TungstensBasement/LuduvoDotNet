using System.Net;
using System.Net.Http.Json;
using LuduvoDotNet.Records;

namespace LuduvoDotNet;

public partial class Luduvo
{
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
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<User>(_jsonOptions, cancellationToken);
        if (user is null) throw new UserNotFoundException();
        return user;
    }

    /// <summary>
    /// Searches users by username using <c>/users?q={username}</c> and returns lightweight profile entries.
    /// </summary>
    /// <param name="username">Username query text. The value is URL-encoded before sending.</param>
    /// <returns>An array of matching <see cref="PartialUser"/> entries, or an empty array when no users match.</returns>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 429.</exception>
    // TODO: handle when more than 20 users are available
    public async Task<PartialUser[]> SearchUsersAsync(string? username)
    {
        var response = await _httpClient.GetAsync($"/users?q={WebUtility.UrlEncode(username)}");
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();
        var users = await response.Content.ReadFromJsonAsync<PartialUser[]>(_jsonOptions);
        return users ?? Array.Empty<PartialUser>();
    }
}