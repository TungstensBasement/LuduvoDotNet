# Introduction

LuduvoDotNet is a .NET client for the Luduvo API. It provides typed models, async methods, and domain-specific exceptions for common API errors.

## Main features

- User profile lookup with `GetUserByIdAsync`.
- User search with `SearchUsersAsync` and lightweight `PartialUser` results.
- User inventory lookup with `GetUserInventotoryAsync` (experimental).
- Place lookup with `GetPlaceByIdAsync`.
- Place search with `SearchPlacesAsync` (supports pagination via `limit` and `offset`).
- Store search with `SearchStoreAsync` (supports pagination via `limit` and `offset`).
- Store item lookup with `GetStoreItemByIdAsync`.

## Error handling

The client maps selected HTTP statuses to custom exceptions:

- `404` on place endpoints -> `PlaceNotFoundException`
- `404` on user profile endpoint -> `UserNotFoundException`
- `404` on store item endpoint -> `StoreItemNotFoundException`
- `429` -> `TooManyRequestsException`

Other non-success responses raise `HttpRequestException` via `EnsureSuccessStatusCode()`.

## Learn by examples

- [Tutorial: Get User By ID](tutorial-get-user-by-id.md)
- [Tutorial: Search Users](tutorial-search-users.md)
- [Tutorial: Get User Inventory](tutorial-get-user-inventory.md)
- [Tutorial: Get Place By ID](tutorial-get-place-by-id.md)
- [Tutorial: Search Places](tutorial-search-places.md)
- [Tutorial: Search Store](tutorial-search-store.md)
- [Tutorial: Get Store Item By ID](tutorial-get-store-item-by-id.md)
