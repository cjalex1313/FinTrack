using FinTrack.DataAccess;
using FinTrack.Shared.DTO.Setup;

namespace FinTrack.BusinessLogic.Services;

public interface ISetupService
{
    Task SetupHousehold(SetupDTO dto, Guid userId);
}

class SetupService : ISetupService
{
    private readonly FinDbContext _context;
    private readonly IHouseholdService _householdService;
    private readonly IIncomeService _incomeService;
    private readonly IExpenseBucketService _expenseBucketService;

    public SetupService(FinDbContext context, IHouseholdService householdService, IIncomeService incomeService, IExpenseBucketService expenseBucketService)
    {
        _context = context;
        _householdService = householdService;
        _incomeService = incomeService;
        _expenseBucketService = expenseBucketService;
    }
    
    public async Task SetupHousehold(SetupDTO dto, Guid userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var household = await _householdService.CreateHousehold(dto.Household, userId);
            foreach (var income in dto.RecurringIncomes)
            {
                income.HouseholdId = household.Id;
            }

            foreach (var expenseBucket in dto.ExpenseBuckets)
            {
                expenseBucket.HouseholdId = household.Id;
            }
            await _incomeService.AddHouseholdRecurringIncomes(dto.RecurringIncomes);
            await _expenseBucketService.AddExpenseBuckets(dto.ExpenseBuckets);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}