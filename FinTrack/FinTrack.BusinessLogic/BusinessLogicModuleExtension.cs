using FinTrack.BusinessLogic.Services;
using FinTrack.DataAccess;
using FinTrack.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.BusinessLogic;

public static class BusinessLogicModuleExtension
{
    public static void AddBusinessLogic(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDataAccess(configuration);
        services.AddEmailModule(configuration);
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHouseholdService, HouseholdService>();
        services.AddScoped<IIncomeService, IncomeService>();
        services.AddScoped<ISetupService, SetupService>();
        services.AddScoped<IExpenseService, ExpenseService>();
    }
}