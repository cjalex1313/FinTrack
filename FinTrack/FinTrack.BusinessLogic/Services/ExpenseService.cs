using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IExpenseService
{
    Task AddExpenseBuckets(IEnumerable<ExpenseBucketDTO> expenseBuckets);
    Task<Expense> AddExpense(ExpenseDTO dto);
    Task<IEnumerable<ExpenseBucket>> GetHouseholdBuckets(Guid householdId);
    Task<IEnumerable<Expense>> GetExpensesForMonth(Guid householdId, DateOnly startOfMonth, DateOnly endOfMonth);
}

class ExpenseService : IExpenseService
{
    private readonly FinDbContext _dbContext;

    public ExpenseService(FinDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddExpenseBuckets(IEnumerable<ExpenseBucketDTO> expenseBuckets)
    {
        foreach (var expenseBucketDTO in expenseBuckets)
        {
            var expenseBucket = new ExpenseBucket()
            {
                Id = expenseBucketDTO.Id,
                Name = expenseBucketDTO.Name,
                Description = expenseBucketDTO.Description,
                MonthlyAmount = expenseBucketDTO.MonthlyAmount,
                HouseholdId = expenseBucketDTO.HouseholdId
            };
            _dbContext.ExpenseBuckets.Add(expenseBucket);
        }
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Expense> AddExpense(ExpenseDTO dto)
    {
        var expnse = new Expense()
        {
            HouseholdId = dto.HouseholdId,
            Amount = dto.Amount,
            Date = dto.Date,
            Description = dto.Description,
            ExpenseBucketId = dto.ExpenseBucketId
        };
        _dbContext.Expenses.Add(expnse);
        await _dbContext.SaveChangesAsync();
        return expnse;
    }

    public async Task<IEnumerable<ExpenseBucket>> GetHouseholdBuckets(Guid householdId)
    {
        var buckets = await _dbContext.ExpenseBuckets.Where(b => b.HouseholdId == householdId).ToListAsync();
        return buckets;       
    }

    public async Task<IEnumerable<Expense>> GetExpensesForMonth(Guid householdId, DateOnly startOfMonth, DateOnly endOfMonth)
    {
        var expenses = await _dbContext.Expenses.Where(e => e.HouseholdId == householdId && e.Date >= startOfMonth && e.Date <= endOfMonth).ToListAsync();
        return expenses;       
    }
}