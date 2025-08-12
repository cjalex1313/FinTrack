using FinTrack.BusinessLogic.BackgroundJobs;
using Hangfire;

namespace FinTrack.Api.Background;

public static class BackgroundJobsManager
{
    public static void AddJobs(this IServiceCollection services)
    {
        services.AddTransient<ProcessRecurringExpensesJob>();
        
    }

    public static void RegisterJobs()
    {
        RecurringJob.AddOrUpdate<ProcessRecurringExpensesJob>("recurring-expenses-process-job", job => job.Execute(), Cron.Daily());
    }
}