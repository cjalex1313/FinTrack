using FinTrack.Shared.Common;

namespace FinTrack.Shared.DTO;

public class HouseholdMemberDTO
{
    public Guid HouseholdId { get; set; }
    public required string HouseholdName { get; set; }
    public required string UserEmail { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }   
    public HouseholdMemberRole Role { get; set; }
    public HouseholdMemberStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}