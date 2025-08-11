using FinTrack.BusinessLogic.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FinTrack.BusinessLogic.BackgroundJobs;

public class ProcessRecurringExpensesJob
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ProcessRecurringExpensesJob> _logger;
    private const int batchSize = 100;

    public ProcessRecurringExpensesJob(IExpenseService expenseService, ILogger<ProcessRecurringExpensesJob> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }
    
    public async Task Execute()
    {
        int skip = 0;
        var recurringExpesnsesToProcess = await _expenseService.GetRecurringExpensesToProcess(skip, batchSize);
        while (recurringExpesnsesToProcess.Any())
        {
            skip += batchSize;

            foreach (var expense in recurringExpesnsesToProcess)
            {
                try
                {
                    await _expenseService.AddExpenseFromRecurring(expense);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error processing expense in ProcessRecurringExpensesJob, recurringExpenseId: {expense.Id}, error: {JsonConvert.SerializeObject(e)}");
                }
            }
            
            recurringExpesnsesToProcess = await _expenseService.GetRecurringExpensesToProcess(skip, batchSize);
        }
    }
}