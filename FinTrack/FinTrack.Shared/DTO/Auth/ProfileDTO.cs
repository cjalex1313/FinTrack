using FinTrack.Shared.Common;

namespace FinTrack.Shared.DTO.Auth;

public class ProfileDTO
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}