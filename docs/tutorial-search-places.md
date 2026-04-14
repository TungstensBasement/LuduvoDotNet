# Tutorial: Search places

This tutorial shows how to search places with `SearchPlacesAsync` and handle paged responses.

## Basic search

```csharp
using LuduvoDotNet;

var client = new Luduvo();
var places = await client.SearchPlacesAsync("test");

Console.WriteLine($"Found {places.Length} places");
foreach (var place in places)
{
    Console.WriteLine($"- {place.title} by {place.owner_username}");
}
```

## Search with pagination

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var places = await client.SearchPlacesAsync("test", limit: 20, offset: 0);

    foreach (var place in places)
    {
        Console.WriteLine($"{place.id}: {place.title} (players: {place.active_players}/{place.max_Players})");
    }
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Invalid paging arguments: {ex.Message}");
}
catch (TooManyRequestsException)
{
    Console.WriteLine("Rate limit reached. Try again later.");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API request failed: {ex.Message}");
}
```

## Notes

- `SearchPlacesAsync` calls `/places?q={query}`.
- The API may return either a direct array or an object with a `places` array.
- `limit` must be greater than `0` and less than `100`.
- `offset` must be `0` or greater.

