namespace LuduvoDotNet;

public class UserNotFoundException:Exception
{
    public UserNotFoundException() { }
}
public class TooManyRequestsException:Exception
{
    public TooManyRequestsException() { }
}