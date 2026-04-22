# Tutorial: Get store item by ID

This tutorial shows how to load one store item with `GetStoreItemByIdAsync`.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var item = await client.GetStoreItemByIdAsync(14);

    Console.WriteLine($"ID: {item.Id}");
    Console.WriteLine($"Name: {item.Name}");
    Console.WriteLine($"Creator: {item.CreatorUsername}");
    Console.WriteLine($"Status: {item.Status}");
}
catch (StoreItemNotFoundException)
{
    Console.WriteLine("Store item was not found.");
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

- `GetStoreItemByIdAsync` calls `/store/{id}`.
- `StoreItemNotFoundException` is thrown for `404` responses.
- `TooManyRequestsException` is thrown for `429` responses.

