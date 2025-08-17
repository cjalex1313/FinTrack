using FinTrack.Shared.Common;
using FinTrack.Shared.Entities.Auth;

namespace FinTrack.Shared.Entities;

public class HouseholdMember
{
    public Guid HouseholdId { get; set; }
    public Guid UserId { get; set; }
    public HouseholdMemberRole Role { get; set; }
    public HouseholdMemberStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual Household Household { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;   
}