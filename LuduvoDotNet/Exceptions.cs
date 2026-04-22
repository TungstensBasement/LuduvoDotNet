namespace LuduvoDotNet;
/// <summary>
/// Exception thrown when a user is not found by the API. This can occur when requesting a user by an invalid ID or username
/// </summary>
public class UserNotFoundException:Exception
{
    /// <summary>
    /// Exception thrown when a user is not found by the API. This can occur when requesting a user by an invalid ID or username
    /// </summary>
    public UserNotFoundException() { }
}
/// <summary>
/// Exception thrown when api returns http status 429 (Too many requests)
/// </summary>
public class TooManyRequestsException:Exception
{
    /// <summary>
    /// Exception thrown when api returns http status 429 (Too many requests)
    /// </summary>
    public TooManyRequestsException() { }
}
/// <summary>
/// Exception thrown when a place is not found by the API. This can occur when requesting a place by an invalid ID
/// </summary>
public class PlaceNotFoundException:Exception
{
    /// <summary>
    /// Exception thrown when a place is not found by the API. This can occur when requesting a place by an invalid ID
    /// </summary>
    public PlaceNotFoundException() { }
}

/// <summary>
/// Exception thrown when a store item is not found by the API.
/// </summary>
public class StoreItemNotFoundException:Exception
{
    /// <summary>
    /// Exception thrown when a store item is not found by the API.
    /// </summary>
    public StoreItemNotFoundException() { }
}
