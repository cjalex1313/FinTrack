using FinTrack.BusinessLogic.Services;
using FinTrack.BusinessLogic.Services.Auth;
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
        if(configuration.GetValue<bool>("ApiConfig:Authentication:Google:Enabled"))
        {
            var clientId = configuration.GetValue<string>("ApiConfig:Authentication:Google:ClientId");
            if (clientId == null)
            {
                throw new Exception("Error - incorrect google config - unable to map google config");
            }
            services.AddScoped(sp=> new GoogleJwtValidator(clientId));
        }
    }
}