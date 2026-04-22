# Tutorial: Get place details by ID

This tutorial shows how to load one place with `GetPlaceByIdAsync` and handle common API errors.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var place = await client.GetPlaceByIdAsync(1);

    Console.WriteLine($"ID: {place.Id}");
    Console.WriteLine($"Title: {place.Title}");
    Console.WriteLine($"Owner: {place.OwnerUsername}");
    Console.WriteLine($"Visits: {place.VisitCount}");
}
catch (PlaceNotFoundException)
{
    Console.WriteLine("Place was not found.");
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

- `GetPlaceByIdAsync` calls `/places/{id}`.
- `PlaceNotFoundException` is thrown for `404` responses.
- `TooManyRequestsException` is thrown for `429` responses.
- Other non-success responses throw `HttpRequestException`.

