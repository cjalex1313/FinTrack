using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IIncomeService
{
    Task AddHouseholdRecurringIncomes(IEnumerable<RecurringIncomeDTO> incomesDto);
    Task<IEnumerable<OneTimeIncome>> GetOneTimeIncomesForMonth(Guid householdId, DateOnly dateInMonth);
    Task<IEnumerable<RecurringIncome>> GetRecurringIncomesForMonth(Guid householdId, DateOnly dateInMonth);
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

    public async Task<IEnumerable<OneTimeIncome>> GetOneTimeIncomesForMonth(Guid householdId, DateOnly dateInMonth)
    {
        var startOfMonth = new DateOnly(dateInMonth.Year, dateInMonth.Month, 1);
        var endOfMonth = new DateOnly(
            dateInMonth.Year,
            dateInMonth.Month,
            DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month)
        );
        var incomes = await _context.OneTimeIncomes
            .Where(oti => oti.HouseholdId == householdId && oti.Date >= startOfMonth && oti.Date <= endOfMonth)
            .ToListAsync();
        return incomes;
    }

    public async Task<IEnumerable<RecurringIncome>> GetRecurringIncomesForMonth(Guid householdId, DateOnly dateInMonth)
    {
        var startOfMonth = new DateOnly(dateInMonth.Year, dateInMonth.Month, 1);
        var endOfMonth = new DateOnly(
            dateInMonth.Year,
            dateInMonth.Month,
            DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month)
        );
        var incomes = await _context.RecurringIncomes
            .Where(r => r.HouseholdId == householdId
                        && r.StartDate <= endOfMonth
                        && (r.EndDate == null || r.EndDate >= startOfMonth))
            .ToListAsync();
        return incomes;       
    }
}