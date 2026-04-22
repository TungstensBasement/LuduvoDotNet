# Tutorial: Get user inventory

This tutorial shows how to call `GetUserInventotoryAsync` and iterate inventory items.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var items = await client.GetUserInventotoryAsync(2, limit: 20, offset: 0);

    Console.WriteLine($"Inventory items: {items.Length}");
    foreach (var item in items)
    {
        Console.WriteLine($"- ItemId: {item.ItemId}, Category: {item.CategoryName}, AcquiredAt: {item.AcquiredAt:O}");
    }
}
catch (UserNotFoundException)
{
    Console.WriteLine("User was not found.");
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

- `GetUserInventotoryAsync` calls `/users/{id}/inventory`.
- `limit` is optional and must be in range `1..100`.
- `offset` is optional and must be `0` or greater.
- `UserNotFoundException` is thrown for `404` responses.

