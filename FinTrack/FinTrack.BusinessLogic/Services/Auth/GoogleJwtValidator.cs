using Google.Apis.Auth;


namespace FinTrack.BusinessLogic.Services.Auth
{

    public class GoogleJwtValidator(string clientId)
    {
        private readonly string _clientId = clientId;

        public async Task<ValidateGoogleTokenResult> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = [_clientId]
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new ValidateGoogleTokenResult
                {
                    IsValid = true,
                    Subject = payload.Subject,
                    Email = payload.Email,
                };
            }
            catch (InvalidJwtException)
            {
                return new ValidateGoogleTokenResult
                {
                    IsValid = false
                };
            }
        }
    }
}