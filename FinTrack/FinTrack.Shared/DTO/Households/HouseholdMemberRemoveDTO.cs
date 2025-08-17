namespace FinTrack.Shared.DTO;

public class HouseholdMemberRemoveDTO
{
    public Guid HouseholdId { get; set; }
    public required string Email { get; set; }
}