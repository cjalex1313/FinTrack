using FinTrack.DataAccess;
using FinTrack.Shared.Common;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions.Household;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IHouseholdService
{
    Task<Household> CreateHousehold(HouseholdDTO dto, Guid userId);
    Task<List<Household>> GetUserHouseholds(Guid userId);
    Task AddHouseholdInvitation(Guid householdId, Guid userId);
}

class HouseholdService : IHouseholdService
{
    private readonly FinDbContext _context;

    public HouseholdService(FinDbContext context)
    {
        _context = context;
    }

    public async Task<Household> CreateHousehold(HouseholdDTO dto, Guid userId)
    {
        var householdExists = _context.Households.Any(h => h.OwnerId == userId);
        if (householdExists)
        {
            throw new UserHasHouseholdException(userId);
        }
        var household = new Household()
        {
            Name = dto.Name,
            OwnerId = userId
        };
        _context.Households.Add(household);
        await _context.SaveChangesAsync();
        return household;
    }

    public async Task<List<Household>> GetUserHouseholds(Guid userId)
    {
        var households = await _context.Households.Where(h => h.OwnerId == userId).ToListAsync();
        return households;
    }

    public async Task AddHouseholdInvitation(Guid householdId, Guid userId)
    {
        var householdMember = new HouseholdMember()
        {
            HouseholdId = householdId,
            UserId = userId,
            Role = HouseholdMemberRole.Member,
            Status = HouseholdMemberStatus.PendingResponse
        };
        _context.HouseholdMembers.Add(householdMember);
        await _context.SaveChangesAsync();       
    }
}