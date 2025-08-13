namespace FinTrack.BusinessLogic.Services.Auth
{
    public class ValidateGoogleTokenResult
    {
        public bool IsValid { get; internal set; }
        public string? Subject { get; internal set; }
        public string? Email { get; internal set; }
    }
}