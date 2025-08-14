using System.ComponentModel.DataAnnotations;

namespace FinTrack.Shared.DTO.Auth;

public class ExternalLoginCallbackRequest
{
    [Required]
    public required string Credential { get; set; }

    //[Required]
    //public required string Email { get; set; }
    //[Required]
    //public required string ProviderKey { get; set; }
}