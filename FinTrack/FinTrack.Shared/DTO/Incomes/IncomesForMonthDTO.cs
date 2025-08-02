namespace FinTrack.Shared.DTO;

public class IncomesForMonthDTO
{
    public IEnumerable<OneTimeIncomeDTO> OneTimeIncomes { get; set; }
    public IEnumerable<RecurringIncomeDTO> RecurringIncomes { get; set; }   
}