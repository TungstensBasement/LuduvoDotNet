using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LuduvoDotNet.Records;

namespace LuduvoDotNet;

public partial class Luduvo
{
    /// <summary>
    /// Updates the authenticated user's profile via <c>PUT /me/profile</c>.
    /// Fields omitted from <paramref name="request"/> may be reset by the API to default values.
    /// </summary>
    /// <param name="request">Profile update payload.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 429.</exception>
    public async Task UpdateMyProfileAsync(UpdateMyProfileRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var response = await _httpClient.PutAsJsonAsync("/me/profile", request, _jsonOptions, cancellationToken);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();

        response.EnsureSuccessStatusCode();
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
        if (limit is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be greater than 0 and less or equal 100.");
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

    /// <summary>
    /// Gets a user's headshot image bytes from <c>/users/{userId}/avatar/headshot</c>.
    /// Handles redirect responses by following the <c>Location</c> header.
    /// </summary>
    /// <param name="userId">The numeric user identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>Raw image bytes returned by the API.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the API returns 404.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 404/429, redirect metadata is invalid, or redirect limit is exceeded.</exception>
    public async Task<byte[]> GetUserHeadshot(int userId, CancellationToken cancellationToken = default)
    {
        static bool IsRedirect(HttpStatusCode code) =>
            code is HttpStatusCode.MovedPermanently
                or HttpStatusCode.Redirect
                or HttpStatusCode.RedirectMethod
                or HttpStatusCode.TemporaryRedirect
                or HttpStatusCode.PermanentRedirect;

        const int maxRedirects = 5;
        Uri requestUri = new($"/users/{userId}/avatar/headshot", UriKind.Relative);

        for (var redirectCount = 0; redirectCount <= maxRedirects; redirectCount++)
        {
            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new UserNotFoundException();
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
                throw new TooManyRequestsException();

            if (IsRedirect(response.StatusCode))
            {
                var location = response.Headers.Location
                    ?? throw new HttpRequestException("Headshot redirect did not include a Location header.");

                if (!location.IsAbsoluteUri)
                {
                    if (_httpClient.BaseAddress is null)
                        throw new HttpRequestException("Cannot resolve relative headshot redirect URI because HttpClient.BaseAddress is null.");

                    location = new Uri(_httpClient.BaseAddress, location);
                }

                requestUri = location;
                continue;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }

        throw new HttpRequestException($"Too many redirects while fetching user headshot (max: {maxRedirects}).");
    }

    /// <summary>
    /// Gets a user's inventory from <c>/users/{id}/inventory</c>.
    /// </summary>
    /// <param name="id">The numeric user identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>An array of <see cref="InventoryItem"/> entries, or an empty array when no items are returned.</returns>
    /// <exception cref="UserNotFoundException">Thrown when the API returns 404.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 404/429.</exception>
    public Task<InventoryItem[]> GetUserInventotoryAsync(int id, CancellationToken cancellationToken = default)
        => GetUserInventotoryAsync(id, limit: null, offset: null, cancellationToken: cancellationToken);

    /// <summary>
    /// Gets a user's inventory from <c>/users/{id}/inventory</c> with optional pagination.
    /// Supports both API response formats: direct array and paged object with <c>items</c> array.
    /// </summary>
    /// <param name="id">The numeric user identifier.</param>
    /// <param name="limit">Optional page size. Must be greater than 0 and less or equal 100 when provided.</param>
    /// <param name="offset">Optional result offset. Must be 0 or greater when provided.</param>
    /// <param name="cancellationToken">Optional cancellation token to cancel the request.</param>
    /// <returns>An array of <see cref="InventoryItem"/> entries, or an empty array when no items are returned.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="limit"/> or <paramref name="offset"/> are outside valid range.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the API returns 404.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    /// <exception cref="HttpRequestException">Thrown when the API returns a non-success status code other than 404/429.</exception>
    public async Task<InventoryItem[]> GetUserInventotoryAsync(int id, int? limit, int? offset, CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be greater than 0 and less or equal 100.");
        if (offset is < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "offset must be 0 or greater.");

        var path = $"/users/{id}/inventory";
        var hasQuery = false;
        if (limit.HasValue)
        {
            path += $"?limit={limit.Value}";
            hasQuery = true;
        }

        if (offset.HasValue)
            path += hasQuery ? $"&offset={offset.Value}" : $"?offset={offset.Value}";

        var response = await _httpClient.GetAsync(path, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new UserNotFoundException();
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            return root.Deserialize<InventoryItem[]>(_jsonOptions) ?? Array.Empty<InventoryItem>();
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("items", out var usersElement) &&
            usersElement.ValueKind == JsonValueKind.Array)
        {
            return usersElement.Deserialize<InventoryItem[]>(_jsonOptions) ?? Array.Empty<InventoryItem>();
        }

        return Array.Empty<InventoryItem>();
    }
}