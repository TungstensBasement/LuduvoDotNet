# Tutorial: Search users by username

This tutorial shows how to search users with `SearchUsersAsync`, inspect `PartialUser` results, and optionally load a full profile.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var results = await client.SearchUsersAsync("igor");

    Console.WriteLine($"Found {results.Length} users");

    foreach (var partial in results)
    {
        Console.WriteLine($"- {partial.username} ({partial.display_name}), role: {partial.role}");
    }

    if (results.Length > 0)
    {
        var fullUser = await results[0].GetUserAsync();
        Console.WriteLine($"First match full profile ID: {fullUser.UserId}");
    }
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

## Understanding `PartialUser`

`SearchUsersAsync` returns `PartialUser[]`. A `PartialUser` contains lightweight fields from the search endpoint:

- `id`
- `username`
- `display_name`
- `role`
- `created_at`
- `head_color`
- `torso_color`

Use `GetUserAsync()` on a `PartialUser` when you need the full `User` model.

