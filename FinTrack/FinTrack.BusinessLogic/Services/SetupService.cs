using FinTrack.BusinessLogic.Services.Auth;
using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;

namespace FinTrack.BusinessLogic.Services;

public interface ISetupService
{
    Task SetupHousehold(SetupDTO dto, Guid userId);
    Task InviteUser(string email, Guid householdId);
}

class SetupService : ISetupService
{
    private readonly FinDbContext _context;
    private readonly IHouseholdService _householdService;
    private readonly IIncomeService _incomeService;
    private readonly IExpenseService _expenseService;
    private readonly IAuthService _authService;

    public SetupService(FinDbContext context, IHouseholdService householdService, IIncomeService incomeService, IExpenseService expenseService, IAuthService authService)
    {
        _context = context;
        _householdService = householdService;
        _incomeService = incomeService;
        _expenseService = expenseService;
        _authService = authService;
    }

    public async Task SetupHousehold(SetupDTO dto, Guid userId)
    {
        using Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            Household household = await _householdService.CreateHousehold(dto.Household, userId);
            foreach (RecurringIncomeDTO income in dto.RecurringIncomes)
            {
                income.HouseholdId = household.Id;
            }
            foreach (ExpenseBucketDTO expenseBucket in dto.ExpenseBuckets)
            {
                expenseBucket.HouseholdId = household.Id;
            }
            await _incomeService.AddHouseholdRecurringIncomes(dto.RecurringIncomes);
            await _expenseService.AddExpenseBuckets(dto.ExpenseBuckets);

            foreach (HouseholdInviteDTO invitation in dto.Invites)
            {
                await InviteUser(invitation.Email, household.Id);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task InviteUser(string email, Guid householdId)
    {
        Shared.Entities.Auth.ApplicationUser? user = await _authService.GetUserByEmail(email);
        if (user == null)
        {
            Household household = await _householdService.GetHousehold(householdId);
            user = await _authService.InviteEmailToHousehold(email, household.Name);
        }
        await _householdService.AddHouseholdInvitation(householdId, user.Id);
    }
}
