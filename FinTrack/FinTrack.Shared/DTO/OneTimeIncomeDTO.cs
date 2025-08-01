namespace FinTrack.Shared.DTO;

public class OneTimeIncomeDTO
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required decimal Amount { get; set; }
    public required DateOnly Date { get; set; }
    public string? Description { get; set; }
}