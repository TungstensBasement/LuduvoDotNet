using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LuduvoDotNet.Records;

namespace LuduvoDotNet;

public partial class Luduvo
{
    /// <summary>
    /// Fetches one store item by ID from <c>/store/{id}</c>.
    /// </summary>
    /// <param name="id">Store item identifier.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Deserialized <see cref="StoreItem"/>.</returns>
    /// <exception cref="StoreItemNotFoundException">Thrown when the API returns 404.</exception>
    /// <exception cref="TooManyRequestsException">Thrown when the API rate limits the request.</exception>
    public async Task<StoreItem> GetStoreItemByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/store/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new StoreItemNotFoundException();
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();

        response.EnsureSuccessStatusCode();
        var item = await response.Content.ReadFromJsonAsync<StoreItem>(_jsonOptions, cancellationToken);
        if (item is null)
            throw new HttpRequestException("Store API returned an empty response body.");

        return item;
    }

    /// <summary>
    /// Searches store items using <c>/store?q={query}</c>.
    /// </summary>
    public Task<StoreItem[]> SearchStoreAsync(string? query, CancellationToken cancellationToken = default)
        => SearchStoreAsync(query, limit: null, offset: null, cancellationToken: cancellationToken);

    /// <summary>
    /// Searches store items using <c>/store?q={query}</c> with optional pagination.
    /// Supports both direct array responses and paged objects with an <c>items</c> array.
    /// </summary>
    /// <param name="query">Search term. Null is treated as an empty query.</param>
    /// <param name="limit">Optional page size from 1 to 100.</param>
    /// <param name="offset">Optional result offset. Must be 0 or greater.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>Array of matching store items, or empty array when no matches exist.</returns>
    public async Task<StoreItem[]> SearchStoreAsync(string? query, int? limit, int? offset, CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be greater than 0 and less or equal 100.");
        if (offset is < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "offset must be 0 or greater.");

        query ??= string.Empty;
        var path = $"/store?q={WebUtility.UrlEncode(query)}";
        if (limit.HasValue) path += $"&limit={limit.Value}";
        if (offset.HasValue) path += $"&offset={offset.Value}";

        var response = await _httpClient.GetAsync(path, cancellationToken);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();

        response.EnsureSuccessStatusCode();

        using var document = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);
        var root = document.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
            return root.Deserialize<StoreItem[]>(_jsonOptions) ?? Array.Empty<StoreItem>();

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("items", out var itemsElement) &&
            itemsElement.ValueKind == JsonValueKind.Array)
        {
            return itemsElement.Deserialize<StoreItem[]>(_jsonOptions) ?? Array.Empty<StoreItem>();
        }

        return Array.Empty<StoreItem>();
    }
}