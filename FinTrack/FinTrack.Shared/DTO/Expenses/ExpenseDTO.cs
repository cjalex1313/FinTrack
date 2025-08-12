namespace FinTrack.Shared.DTO;

public class ExpenseDTO
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid? ExpenseBucketId { get; set; }
    public required decimal Amount { get; set; }
    public required DateOnly Date { get; set; }
    public string? Description { get; set; }
}