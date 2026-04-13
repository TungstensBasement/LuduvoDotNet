using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
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
    public Task<PartialUser[]> SearchUsersAsync(string? username)
        => SearchUsersAsync(username, limit: null, offset: null, cancellationToken: default);

    /// <summary>
    /// Searches users by username using <c>/users?q={username}</c> with optional pagination.
    /// Supports both API response formats: direct array and paged object with <c>users</c> array.
    /// </summary>
    /// <param name="username">Username query text. The value is URL-encoded before sending.</param>
    /// <param name="limit">Optional page size. Must be greater than 0 when provided.</param>
    /// <param name="offset">Optional result offset. Must be 0 or greater when provided.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>An array of matching <see cref="PartialUser"/> entries, or an empty array when no users match.</returns>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 429.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> are outside valid range.</exception>
    public async Task<PartialUser[]> SearchUsersAsync(string? username, int? limit, int? offset, CancellationToken cancellationToken = default)
    {
        if (limit is <= 0)
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be greater than 0.");
        if (offset is < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "offset must be 0 or greater.");

        username ??= string.Empty;
        var path = $"/users?q={WebUtility.UrlEncode(username)}";
        if (limit.HasValue) path += $"&limit={limit.Value}";
        if (offset.HasValue) path += $"&offset={offset.Value}";

        var response = await _httpClient.GetAsync(path, cancellationToken);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.Deserialize<PartialUser[]>(_jsonOptions) ?? Array.Empty<PartialUser>();
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("users", out var usersElement) &&
            usersElement.ValueKind == JsonValueKind.Array)
        {
            return usersElement.Deserialize<PartialUser[]>(_jsonOptions) ?? Array.Empty<PartialUser>();
        }

        return Array.Empty<PartialUser>();
    }
}