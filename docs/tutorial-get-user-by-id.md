# Tutorial: Get a user profile by ID

This tutorial shows how to load one user profile with `GetUserByIdAsync` and handle common API errors.

## Example

```csharp
using LuduvoDotNet;

var client = new Luduvo();

try
{
    var user = await client.GetUserByIdAsync(2);

    Console.WriteLine($"ID: {user.UserId}");
    Console.WriteLine($"Username: {user.Username}");
    Console.WriteLine($"Display name: {user.DisplayName}");
    Console.WriteLine($"Friends: {user.FriendCount}");
}
catch (UserNotFoundException)
{
    Console.WriteLine("User was not found.");
}
catch (TooManyRequestsException)
{
    Console.WriteLine("Rate limit reached. Try again later.");
}
```

## Notes

- `GetUserByIdAsync` calls `/users/{userId}/profile`.
- `UserNotFoundException` is thrown for `404` responses.
- `TooManyRequestsException` is thrown for `429` responses.

