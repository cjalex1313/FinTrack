using FinTrack.DataAccess;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;

namespace FinTrack.BusinessLogic.Services;

public interface IExpenseBucketService
{
    Task AddExpenseBuckets(IEnumerable<ExpenseBucketDTO> expenseBuckets);
}

class ExpenseBucketService : IExpenseBucketService
{
    private readonly FinDbContext _dbContext;

    public ExpenseBucketService(FinDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddExpenseBuckets(IEnumerable<ExpenseBucketDTO> expenseBuckets)
    {
        foreach (var expenseBucketDTO in expenseBuckets)
        {
            var expenseBucket = new ExpenseBucket()
            {
                Id = expenseBucketDTO.Id,
                Name = expenseBucketDTO.Name,
                Description = expenseBucketDTO.Description,
                MonthlyAmount = expenseBucketDTO.MonthlyAmount,
                HouseholdId = expenseBucketDTO.HouseholdId
            };
            _dbContext.ExpenseBuckets.Add(expenseBucket);
        }
        await _dbContext.SaveChangesAsync();
    }
}