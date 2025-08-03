using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;

namespace FinTrack.Shared.Mappers;

public static class IncomesMapper
{
    public static OneTimeIncomeDTO MapToDTO(this OneTimeIncome income)
    {
        return new OneTimeIncomeDTO()
        {
            Id = income.Id,
            Amount = income.Amount,
            Date = income.Date,
            Description = income.Description,
            HouseholdId = income.HouseholdId
        };
    }

    public static RecurringIncomeDTO MapToDTO(this RecurringIncome income)
    {
        return new RecurringIncomeDTO()
        {
            Id = income.Id,
            HouseholdId = income.HouseholdId,
            Amount = income.Amount,
            StartDate = income.StartDate,
            EndDate = income.EndDate,
            Recurrence = income.Recurrence,
            Description = income.Description       
        };
    }
}