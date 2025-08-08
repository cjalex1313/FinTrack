namespace FinTrack.Shared.DTO;

public class ExpenseBucketDTO
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required string Name { get; set; }
    public decimal MonthlyAmount { get; set; }
    public string? Description { get; set; }
}