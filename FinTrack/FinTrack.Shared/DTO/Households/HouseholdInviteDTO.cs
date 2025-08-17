namespace FinTrack.Shared.DTO;

public class HouseholdInviteDTO
{
    public Guid HouseholdId { get; set; }
    public required string Email { get; set; }
}