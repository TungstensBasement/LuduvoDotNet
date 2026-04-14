using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LuduvoDotNet.Records;
namespace LuduvoDotNet;

public partial class Luduvo
{
    public async Task<Place> GetPlaceByIdAsync(uint id)
    {
        var response = await _httpClient.GetAsync($"places/{id}");
        if(response.StatusCode == HttpStatusCode.NotFound)
            throw new PlaceNotFoundException();
        if(response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new TooManyRequestsException();
        response.EnsureSuccessStatusCode();
        var place = await response.Content.ReadFromJsonAsync<Place>(_jsonOptions);
        return place;
    }
    public Task<Place[]> SearchPlacesAsync(string? query)
        => SearchPlacesAsync(query, limit: null, offset: null, cancellationToken: default);
    public async Task<Place[]> SearchPlacesAsync(string? query, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)
    {
        if (limit is <= 0 or >= 100)
            throw new ArgumentOutOfRangeException(nameof(limit), "limit must be greater than 0 and less than 100.");
        if (offset is < 0)
            throw new ArgumentOutOfRangeException(nameof(offset), "offset must be 0 or greater.");

        query ??= string.Empty;
        var path = $"/places?q={WebUtility.UrlEncode(query)}";
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
            return root.Deserialize<Place[]>(_jsonOptions) ?? Array.Empty<Place>();
        }

        if (root.ValueKind == JsonValueKind.Object &&
            root.TryGetProperty("places", out var placesElement) &&
            placesElement.ValueKind == JsonValueKind.Array)
        {
            return placesElement.Deserialize<Place[]>(_jsonOptions) ?? Array.Empty<Place>();
        }

        return Array.Empty<Place>();
    }
}