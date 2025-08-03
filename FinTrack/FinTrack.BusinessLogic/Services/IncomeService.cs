using System.Net;
using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IIncomeService
{
    Task AddHouseholdRecurringIncomes(IEnumerable<RecurringIncomeDTO> incomesDto);
    Task<IEnumerable<OneTimeIncome>> GetOneTimeIncomesForMonth(Guid householdId, DateOnly dateInMonth);
    Task<IEnumerable<RecurringIncome>> GetRecurringIncomesForMonth(Guid householdId, DateOnly dateInMonth);
    Task<OneTimeIncome> AddOneTimeIncome(OneTimeIncomeDTO dto);
    Task<RecurringIncome> AddRecurringIncome(RecurringIncomeDTO dto);
    Task<OneTimeIncome> UpdateOneTimeIncome(OneTimeIncomeDTO dto);
    Task<RecurringIncome> UpdateRecurringIncome(RecurringIncomeDTO dto);
    Task<OneTimeIncome> GetOneTimeIncome(Guid id);
    Task DeleteOneTimeIncome(Guid incomeId);
    Task<RecurringIncome> GetRecurringIncome(Guid id);
    Task DeleteRecurringIncome(Guid incomeId);
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

    public async Task<OneTimeIncome> AddOneTimeIncome(OneTimeIncomeDTO dto)
    {
        var income = new OneTimeIncome()
        {
            HouseholdId = dto.HouseholdId,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description
        };
        _context.OneTimeIncomes.Add(income);
        await _context.SaveChangesAsync();
        return income;       
    }

    public async Task<RecurringIncome> AddRecurringIncome(RecurringIncomeDTO dto)
    {
        var income = new RecurringIncome()
        {
            HouseholdId = dto.HouseholdId,
            Amount = dto.Amount,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Recurrence = dto.Recurrence,
            Description = dto.Description
        };
        _context.RecurringIncomes.Add(income);
        await _context.SaveChangesAsync();
        return income;       
    }

    public async Task<OneTimeIncome> UpdateOneTimeIncome(OneTimeIncomeDTO dto)
    {
        var oneTimeIncome = await _context.OneTimeIncomes.FindAsync(dto.Id);
        if (oneTimeIncome == null)
        {
            throw new BaseException("One time income not found", (int)HttpStatusCode.NotFound);
        }
        oneTimeIncome.Amount = dto.Amount;
        oneTimeIncome.Date = dto.Date;
        oneTimeIncome.Description = dto.Description;
        await _context.SaveChangesAsync();
        return oneTimeIncome;       
    }

    public async Task<RecurringIncome> UpdateRecurringIncome(RecurringIncomeDTO dto)
    {
        var recurringIncome = await _context.RecurringIncomes.FindAsync(dto.Id);
        if (recurringIncome == null)
        {
            throw new BaseException("Recurring income not found", (int)HttpStatusCode.NotFound);
        }
        recurringIncome.Amount = dto.Amount;
        recurringIncome.StartDate = dto.StartDate;
        recurringIncome.EndDate = dto.EndDate;
        recurringIncome.Recurrence = dto.Recurrence;
        recurringIncome.Description = dto.Description;
        await _context.SaveChangesAsync();
        return recurringIncome;       
    }

    public async Task<OneTimeIncome> GetOneTimeIncome(Guid id)
    {
        var income = await _context.OneTimeIncomes.FindAsync(id);
        if (income == null)
        {
            throw new BaseException("One time income not found", (int)HttpStatusCode.NotFound);
        }

        return income;
    }

    public async Task DeleteOneTimeIncome(Guid id)
    {
        var income = await _context.OneTimeIncomes.FindAsync(id);
        if (income == null)
        {
            throw new BaseException("One time income not found", (int)HttpStatusCode.NotFound);       
        }
        _context.OneTimeIncomes.Remove(income);
        await _context.SaveChangesAsync();
    }

    public async Task<RecurringIncome> GetRecurringIncome(Guid id)
    {
        var income = await _context.RecurringIncomes.FindAsync(id);
        if (income == null)
        {
            throw new BaseException("Recurring income not found", (int)HttpStatusCode.NotFound);
        }
        return income;       
    }

    public async Task DeleteRecurringIncome(Guid income)
    {
        var incomeToDelete = await _context.RecurringIncomes.FindAsync(income);
        if (incomeToDelete == null)
        {
            throw new BaseException("Recurring income not found", (int)HttpStatusCode.NotFound);       
        }
        _context.RecurringIncomes.Remove(incomeToDelete);
        await _context.SaveChangesAsync();       
    }
}