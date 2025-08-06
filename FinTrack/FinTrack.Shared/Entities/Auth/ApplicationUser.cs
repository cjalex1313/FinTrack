using Microsoft.AspNetCore.Identity;

namespace FinTrack.Shared.Entities.Auth;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}