using Google.Apis.Auth;


namespace FinTrack.BusinessLogic.Services.Auth
{

    public class GoogleJwtValidator(string clientId)
    {
        public async Task<ValidateGoogleTokenResult> ValidateGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = [clientId]
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