using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;

namespace FinTrack.Shared.Mappers;

public static class ExpensesMapper
{
    public static ExpenseBucketDTO MapToDTO(this ExpenseBucket bucket)
    {
        return new ExpenseBucketDTO()
        {
            Id = bucket.Id,
            Name = bucket.Name,
            Description = bucket.Description,
            MonthlyAmount = bucket.MonthlyAmount,
            HouseholdId = bucket.HouseholdId
        };
    }

    public static ExpenseDTO MapToDTO(this Expense expense)
    {
        return new ExpenseDTO()
        {
            Id = expense.Id,
            Amount = expense.Amount,
            Date = expense.Date,
            Description = expense.Description,
            ExpenseBucketId = expense.ExpenseBucketId
        };       
    }

    public static RecurringExpenseDTO MapToDTO(this RecurringExpense expense)
    {
        return new RecurringExpenseDTO()
        {
            Id = expense.Id,
            ExpenseBucketId = expense.ExpenseBucketId,
            Amount = expense.Amount,
            Recurrence = expense.Recurrence,
            Description = expense.Description       
        };       
    }
}