using FinTrack.Shared.Common;

namespace FinTrack.Shared.DTO;

public class RecurringExpenseDTO
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public required decimal Amount { get; set; }
    public DateOnly NextDate { get; set; }   
    public required RecurrenceType Recurrence { get; set; }
    public string? Description { get; set; }
    public Guid? ExpenseBucketId { get; set; }
}