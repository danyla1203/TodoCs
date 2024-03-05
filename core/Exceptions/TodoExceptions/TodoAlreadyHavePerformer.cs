namespace todo.Exceptions.TodoExceptions;

public class TodoAlreadyHavePerformer : BaseClientException
{
  public TodoAlreadyHavePerformer()
  {
    ErrorMessage = "Todo Item already have performer";
    StatusCode = 409;
  }
}