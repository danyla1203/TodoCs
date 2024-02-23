namespace todo.Exceptions;

public abstract class BaseClientException : ApplicationException
{
  public int StatusCode { get; set; }
  public string? ErrorMessage { get; set; }
}
