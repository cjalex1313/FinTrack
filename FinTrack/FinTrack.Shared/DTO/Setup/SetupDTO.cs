namespace FinTrack.Shared.DTO.Setup;

public class SetupDTO
{
    public required HouseholdDTO Household { get; set; }
    public IEnumerable<RecurringIncomeDTO> RecurringIncomes { get; set; } = new List<RecurringIncomeDTO>();
    public IEnumerable<ExpenseBucketDTO> ExpenseBuckets { get; set; } = new List<ExpenseBucketDTO>();
    public IEnumerable<HouseholdInviteDTO> Invites { get; set; } = new List<HouseholdInviteDTO>(); 
}