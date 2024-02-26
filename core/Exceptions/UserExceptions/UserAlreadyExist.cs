namespace todo.Exceptions;

public class UserAlreadyExist : BaseClientException
{
  public UserAlreadyExist()
  {
    ErrorMessage = "User already exist";
    StatusCode = 409;
  }
}