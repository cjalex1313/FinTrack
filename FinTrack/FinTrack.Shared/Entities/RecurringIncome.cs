using FinTrack.Shared.Common;

namespace FinTrack.Shared.Entities;

public class RecurringIncome
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required decimal Amount { get; set; }
    public required DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public required RecurrenceType Recurrence { get; set; }
    public string? Description { get; set; }
    
    public virtual Household? Household { get; set; }
}