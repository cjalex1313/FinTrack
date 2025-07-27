namespace FinTrack.Shared.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; set; }
    public string ErrorMessage { get; set; }

    public BaseException() : base("Internal server error")
    {
        StatusCode = 500;
        ErrorMessage = "Internal server error";
    }

    public BaseException(string errorMessage) : base(errorMessage)
    {
        StatusCode = 500;
        ErrorMessage = errorMessage;
    }
    
    public BaseException(string errorMessage, int statusCode) : base(errorMessage)
    {
        StatusCode = statusCode;
        ErrorMessage = errorMessage;
    }
}