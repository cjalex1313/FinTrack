using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinTrack.Email;

public static class EmailModule
{
    public static void AddEmailModule(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfig = configuration.GetSection("EmailConfig").Get<EmailConfig>();
        if (emailConfig == null)
        {
            throw new Exception("Error - incorrect email config - unable to map email config");
        }
        services.AddSingleton<EmailConfig>(emailConfig);
        services.AddScoped<IEmailService, EmailService>();
    }
}