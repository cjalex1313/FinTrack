namespace FinTrack.Shared.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required decimal Amount { get; set; }
    public required DateOnly Date { get; set; }
    public string? Description { get; set; }
    public Guid? ExpenseBucketId { get; set; }
    
    public virtual Household? Household { get; set; }
    public virtual ExpenseBucket? ExpenseBucket { get; set; }
}