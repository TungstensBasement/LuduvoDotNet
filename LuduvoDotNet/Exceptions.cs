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