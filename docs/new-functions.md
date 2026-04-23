# New Functions

This page documents the most recently added methods and overloads in `Luduvo`.

## Authentication

### `LoginAsync(string identifier, string password)`

Logs in with credentials using `POST /auth/login` and returns an authenticated `Luduvo` client.

```csharp
var client = await Luduvo.LoginAsync("myUsername", "myPassword");
```

Behavior:

- Sends JSON payload:

```json
{
  "identifier": "myUsername",
  "password": "myPassword"
}
```

- Expects a response containing at least `token`.
- Applies `Authorization: Bearer <token>` for subsequent requests on the returned client.

Exceptions:

- `ArgumentException` when identifier or password is empty.
- `TooManyRequestsException` when API returns `429`.
- `HttpRequestException` for other non-success status codes.
- `JsonException` when response has no token.

### `Luduvo(string token)`

Creates a client with bearer authentication already configured.

```csharp
var client = new Luduvo("your-access-token");
```

## Search overloads with paging

The following methods support optional pagination through `limit` and `offset`:

- `SearchUsersAsync(string? username, int? limit, int? offset, CancellationToken cancellationToken = default)`
- `SearchPlacesAsync(string? query, int? limit = null, int? offset = null, CancellationToken cancellationToken = default)`
- `SearchStoreAsync(string? query, int? limit, int? offset, CancellationToken cancellationToken = default)`
- `GetUserInventotoryAsync(int id, int? limit, int? offset, CancellationToken cancellationToken = default)`

Example:

```csharp
var users = await client.SearchUsersAsync("igor", limit: 20, offset: 0);
var places = await client.SearchPlacesAsync("test", limit: 20, offset: 0);
var items = await client.SearchStoreAsync("hat", limit: 20, offset: 20);
var inventory = await client.GetUserInventotoryAsync(2, limit: 20, offset: 0);
```

Validation rules:

- `limit`: must be greater than `0` (and at most `100` where enforced by the method).
- `offset`: must be `0` or greater.

## Profile update

### `UpdateMyProfileAsync(UpdateMyProfileRequest request, CancellationToken cancellationToken = default)`

Updates the authenticated user's profile via `PUT /me/profile`.

```csharp
await client.UpdateMyProfileAsync(new UpdateMyProfileRequest
{
    Status = "Busy",
    Bio = "Building things",
    DisplayName = "Tungsten",
    AccentColor = Color.FromArgb(255, 255, 0, 153),
    AllowJoins = false
});
```

Notes:

- The endpoint uses bearer auth (`Authorization: Bearer <token>`).
- If a field is omitted in the payload, the API may reset it to `null` or a server default.
- Additional editable API fields can be sent through `UpdateMyProfileRequest.AdditionalProperties`.

