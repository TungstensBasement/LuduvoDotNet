# Getting Started

## Install package

```bash
dotnet add package LuduvoDotNet
```

## Create client

```csharp
using LuduvoDotNet;

var client = new Luduvo();
```

## Quick examples

```csharp
// User profile
var user = await client.GetUserByIdAsync(2);

// User search
var users = await client.SearchUsersAsync("igor", limit: 20, offset: 0);

// Place by id
var place = await client.GetPlaceByIdAsync(1);

// Place search
var places = await client.SearchPlacesAsync("test", limit: 20, offset: 0);
```

## Next steps

- Read [Tutorial: Get User By ID](tutorial-get-user-by-id.md)
- Read [Tutorial: Search Users](tutorial-search-users.md)
- Read [Tutorial: Get Place By ID](tutorial-get-place-by-id.md)
- Read [Tutorial: Search Places](tutorial-search-places.md)
