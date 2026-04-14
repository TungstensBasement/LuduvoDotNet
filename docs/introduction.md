# Introduction

LuduvoDotNet is a .NET client for the Luduvo API. It provides typed models, async methods, and domain-specific exceptions for common API errors.

## Main features

- User profile lookup with `GetUserByIdAsync`.
- User search with `SearchUsersAsync` and lightweight `PartialUser` results.
- Place lookup with `GetPlaceByIdAsync`.
- Place search with `SearchPlacesAsync` (supports pagination via `limit` and `offset`).

## Error handling

The client maps selected HTTP statuses to custom exceptions:

- `404` on place endpoints -> `PlaceNotFoundException`
- `404` on user profile endpoint -> `UserNotFoundException`
- `429` -> `TooManyRequestsException`

Other non-success responses raise `HttpRequestException` via `EnsureSuccessStatusCode()`.

## Learn by examples

- [Tutorial: Get User By ID](tutorial-get-user-by-id.md)
- [Tutorial: Search Users](tutorial-search-users.md)
- [Tutorial: Get Place By ID](tutorial-get-place-by-id.md)
- [Tutorial: Search Places](tutorial-search-places.md)
