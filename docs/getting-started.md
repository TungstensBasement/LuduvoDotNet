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

## Authenticate (optional)

```csharp
// Login with identifier (username or email) and password.
var authedClient = await Luduvo.LoginAsync("myUsername", "myPassword");

// Or create a client directly from an access token.
var tokenClient = new Luduvo("your-access-token");
```

## Quick examples

```csharp
// User profile
var user = await client.GetUserByIdAsync(2);

// User search
var users = await client.SearchUsersAsync("igor", limit: 20, offset: 0);

// User inventory (experimental)
var inventory = await client.GetUserInventotoryAsync(2, limit: 20, offset: 0);

// Place by id
var place = await client.GetPlaceByIdAsync(1);

// Place search
var places = await client.SearchPlacesAsync("test", limit: 20, offset: 0);

// Store search
var storeItems = await client.SearchStoreAsync("test", limit: 20, offset: 0);

// Store item by id
var storeItem = await client.GetStoreItemByIdAsync(14);
```

## Next steps

- Read [Tutorial: Get User By ID](tutorial-get-user-by-id.md)
- Read [Tutorial: Search Users](tutorial-search-users.md)
- Read [Tutorial: Get User Inventory](tutorial-get-user-inventory.md)
- Read [Tutorial: Get Place By ID](tutorial-get-place-by-id.md)
- Read [Tutorial: Search Places](tutorial-search-places.md)
- Read [Tutorial: Search Store](tutorial-search-store.md)
- Read [Tutorial: Get Store Item By ID](tutorial-get-store-item-by-id.md)
- Read [Luduvo API: Overview](luduvo-api/overview.md)
