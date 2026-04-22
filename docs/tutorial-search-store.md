# Tutorial: Search store items

This tutorial shows how to search store items with `SearchStoreAsync`.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var items = await client.SearchStoreAsync("test", limit: 20, offset: 0);

    Console.WriteLine($"Found {items.Length} items");
    foreach (var item in items)
    {
        Console.WriteLine($"- {item.Id}: {item.Name} ({item.CategoryName}) price={item.Price}");
    }
}
catch (TooManyRequestsException)
{
    Console.WriteLine("Rate limit reached. Try again later.");
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Invalid paging arguments: {ex.Message}");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API request failed: {ex.Message}");
}
```

## Notes

- `SearchStoreAsync` calls `/store?q={query}`.
- The API can return either a JSON array or an object with an `items` array.
- `limit` is optional and must be in range `1..100`.
- `offset` is optional and must be `0` or greater.

