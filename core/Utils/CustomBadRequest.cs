using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace todo.Utils;

public class ErrorItem
{
    public string Field { get; set; }
    public string Message { get; set; }
}

public class CustomBadRequest
{
    public int StatusCode { get; }
    public string Message { get; }
    public List<ErrorItem> Errors { get; }

    CustomBadRequest(List<ErrorItem> Errors)
    {
        this.Errors = Errors;
        StatusCode = 400;
        Message = "One or more validation errors occurred.";
    }

    public static CustomBadRequest FormatException(ModelStateDictionary input)
    {
        List<ErrorItem> errors = new List<ErrorItem>();
        foreach (var modelStateKey in input.Keys)
        {
            var modelStateVal = input[modelStateKey];
            foreach (ModelError error in modelStateVal.Errors)
            {
                errors.Add(new ErrorItem
                {
                    Field = modelStateKey,
                    Message = error.ErrorMessage
                });
            }
        }
        return new CustomBadRequest(errors);
    }
}