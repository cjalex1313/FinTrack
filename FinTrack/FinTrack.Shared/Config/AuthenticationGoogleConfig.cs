namespace FinTrack.Shared.Config
{
    public class AuthenticationGoogleConfig
    {
        public required bool Enabled { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
    }
}
