namespace FinTrack.Shared.Exceptions.Auth;

public class LoginUserNotFoundException : BaseException
{
    public LoginUserNotFoundException(string email)
    {
        this.StatusCode = 404;
        this.ErrorMessage = $"The email {email} was not found";
    }
}