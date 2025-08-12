namespace FinTrack.Shared.Entities;

public class ExpenseBucket
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required string Name { get; set; }
    public decimal MonthlyAmount { get; set; }
    public string? Description { get; set; }
    
    public virtual Household? Household { get; set; }   
}