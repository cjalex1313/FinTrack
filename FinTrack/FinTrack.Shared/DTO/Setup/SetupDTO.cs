namespace FinTrack.Shared.DTO.Setup;

public class SetupDTO
{
    public required HouseholdDTO Household { get; set; }
    public required IEnumerable<RecurringIncomeDTO> RecurringIncomes { get; set; }
    public required IEnumerable<ExpenseBucketDTO> ExpenseBuckets { get; set; }
}