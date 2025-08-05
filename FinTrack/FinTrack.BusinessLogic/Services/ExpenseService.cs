using System.Net;
using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IExpenseService
{
    Task AddExpenseBuckets(IEnumerable<ExpenseBucketDTO> expenseBuckets);
    Task<Expense> AddExpense(ExpenseDTO dto);
    Task<IEnumerable<ExpenseBucket>> GetHouseholdBuckets(Guid householdId);
    Task<IEnumerable<Expense>> GetExpensesForMonth(Guid householdId, DateOnly startOfMonth, DateOnly endOfMonth);
    Task<ExpenseBucket> AddExpenseBucket(ExpenseBucketDTO newBucket);
    Task<ExpenseBucket> UpdateExpenseBucket(ExpenseBucketDTO dto);
    Task DeleteExpenseBucket(Guid id);
    Task<Expense> UpdateExpense(ExpenseDTO dto);
    Task DeleteExpense(Guid id);
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

    public async Task<ExpenseBucket> AddExpenseBucket(ExpenseBucketDTO dto)
    {
        var newBucket = new ExpenseBucket()
        {
            Name = dto.Name,
            Description = dto.Description,
            MonthlyAmount = dto.MonthlyAmount,
            HouseholdId = dto.HouseholdId
        };
        _dbContext.ExpenseBuckets.Add(newBucket);
        await _dbContext.SaveChangesAsync();
        return newBucket;       
    }

    public async Task<ExpenseBucket> UpdateExpenseBucket(ExpenseBucketDTO dto)
    {
        var expenseBucket = await _dbContext.ExpenseBuckets.FindAsync(dto.Id);
        if (expenseBucket == null)
        {
            throw new BaseException("Expense bucket not found", (int)HttpStatusCode.NotFound);
;        }
        expenseBucket.Name = dto.Name;
        expenseBucket.Description = dto.Description;
        expenseBucket.MonthlyAmount = dto.MonthlyAmount;
        await _dbContext.SaveChangesAsync();
        return expenseBucket;       
    }

    public async Task DeleteExpenseBucket(Guid id)
    {
        var expenseBucket = await _dbContext.ExpenseBuckets.FindAsync(id);
        if (expenseBucket == null)
        {
            throw new BaseException("Expense bucket not found", (int)HttpStatusCode.NotFound);
        }
        _dbContext.ExpenseBuckets.Remove(expenseBucket);
        await _dbContext.SaveChangesAsync();       
    }

    public async Task<Expense> UpdateExpense(ExpenseDTO dto)
    {
        var expense = await _dbContext.Expenses.FindAsync(dto.Id);
        if (expense == null)
        {
            throw new BaseException("Expense not found", (int)HttpStatusCode.NotFound);
        }
        expense.Amount = dto.Amount;
        expense.Date = dto.Date;
        expense.Description = dto.Description;
        expense.ExpenseBucketId = dto.ExpenseBucketId;
        await _dbContext.SaveChangesAsync();
        return expense;       
    }

    public async Task DeleteExpense(Guid id)
    {
        var expense = await _dbContext.Expenses.FindAsync(id);
        if (expense == null)
        {
            throw new BaseException("Expense not found", (int)HttpStatusCode.NotFound);
        }
        _dbContext.Expenses.Remove(expense);
        await _dbContext.SaveChangesAsync();       
    }
}