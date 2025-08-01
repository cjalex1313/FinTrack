using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;

namespace FinTrack.BusinessLogic.Services;

public interface IIncomeService
{
    Task AddHouseholdRecurringIncomes(IEnumerable<RecurringIncomeDTO> incomesDto);
}

class IncomeService : IIncomeService
{
    private readonly FinDbContext _context;

    public IncomeService(FinDbContext context)
    {
        _context = context;
    }

    public async Task AddHouseholdRecurringIncomes(IEnumerable<RecurringIncomeDTO> incomesDto)
    {
        foreach (var incomeDto in incomesDto)
        {
            var income = new RecurringIncome()
            {
                HouseholdId = incomeDto.HouseholdId,
                Amount = incomeDto.Amount,
                Description = incomeDto.Description,
                StartDate = incomeDto.StartDate,
                EndDate = incomeDto.EndDate,
                Recurrence = incomeDto.Recurrence
            };
            _context.RecurringIncomes.Add(income);
        }
        await _context.SaveChangesAsync();
    }
}