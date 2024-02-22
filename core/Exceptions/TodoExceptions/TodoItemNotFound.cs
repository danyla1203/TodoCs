namespace todo.Exceptions.TodoExceptions;

public class TodoItemNotFound : BaseClientException
{
  public TodoItemNotFound()
  {
    ErrorMessage = "Todo Item Not Found";
    StatusCode = 404;
  }
}