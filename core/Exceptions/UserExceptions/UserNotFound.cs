namespace todo.Exceptions;

public class UserNotFound : BaseClientException
{
  public UserNotFound()
  {
    ErrorMessage = "User don't found";
    StatusCode = 404;
  }
}