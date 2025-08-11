using FinTrack.Shared.Common;

namespace FinTrack.Shared.DTO;

public class HouseholdMemberDTO
{
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }
    public HouseholdMemberRole Role { get; set; }
    public HouseholdMemberStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}