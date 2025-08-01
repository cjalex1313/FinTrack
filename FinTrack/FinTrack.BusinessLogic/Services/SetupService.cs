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

    public SetupService(FinDbContext context, IHouseholdService householdService, IIncomeService incomeService)
    {
        _context = context;
        _householdService = householdService;
        _incomeService = incomeService;
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
            await _incomeService.AddHouseholdRecurringIncomes(dto.RecurringIncomes);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}