using System.Net;
using FinTrack.DataAccess;
using FinTrack.Shared.Common;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Exceptions.Household;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.BusinessLogic.Services;

public interface IHouseholdService
{
    Task<Household> CreateHousehold(HouseholdDTO dto, Guid userId);
    Task<List<HouseholdMember>> GetUserHouseholds(Guid userId);
    Task AddHouseholdInvitation(Guid householdId, Guid userId);
    Task<List<HouseholdMember>> GetUserPendingHouseholdInvitations(Guid userId);
    Task AcceptInvite(Guid userId, Guid householdId);
    Task<bool> IsUserHouseholdOwner(Guid userId, Guid householdId);
    Task<List<HouseholdMember>> GetHouseholdMembers(Guid householdId);
    Task<Household> GetHousehold(Guid id);
    Task DeleteHouseholdMember(Guid householdId, Guid memberId);
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
        var houseHoldMember = new HouseholdMember()
        {
            Household = household,
            UserId = userId,
            Role = HouseholdMemberRole.Owner,
            Status = HouseholdMemberStatus.Active
        };
        _context.HouseholdMembers.Add(houseHoldMember);
        await _context.SaveChangesAsync();       
        return household;
    }

    public async Task<List<HouseholdMember>> GetUserHouseholds(Guid userId)
    {
        var households = await _context.HouseholdMembers.Where(h => h.UserId == userId && h.Status == HouseholdMemberStatus.Active).Include(hm => hm.Household).Include(hm => hm.User).ToListAsync();
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

    public async Task<List<HouseholdMember>> GetUserPendingHouseholdInvitations(Guid userId)
    {
        var householdMembers = await _context.HouseholdMembers.Where(h => h.UserId == userId && h.Status == HouseholdMemberStatus.PendingResponse).Include(hm => hm.Household).Include(hm => hm.User).ToListAsync();
        return householdMembers;       
    }

    public async Task AcceptInvite(Guid userId, Guid householdId)
    {
        var householdMember =
            await _context.HouseholdMembers.FirstOrDefaultAsync(hm =>
                hm.HouseholdId == householdId && hm.UserId == userId);
        if (householdMember is not { Status: HouseholdMemberStatus.PendingResponse })
        {
            throw new BaseException("Invite not found", (int)HttpStatusCode.NotFound);
        }

        householdMember.Status = HouseholdMemberStatus.Active;
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsUserHouseholdOwner(Guid userId, Guid householdId)
    {
        var household = await _context.Households.FindAsync(householdId);
        if (household == null)
        {
            throw new BaseException("Household not found", (int)HttpStatusCode.NotFound);
        }
        return household.OwnerId == userId;       
    }

    public async Task<List<HouseholdMember>> GetHouseholdMembers(Guid householdId)
    {
        var householdMembers = await _context.HouseholdMembers.Where(hm => hm.HouseholdId == householdId).Include(hm => hm.Household).Include(hm => hm.User).ToListAsync();
        return householdMembers;       
    }

    public async Task<Household> GetHousehold(Guid id)
    {
        var household = await _context.Households.FindAsync(id);
        if (household == null)
        {
            throw new BaseException("Household not found", (int)HttpStatusCode.NotFound);
        }
        return household;       
    }

    public async Task DeleteHouseholdMember(Guid householdId, Guid memberId)
    {
        var householdMember = await _context.HouseholdMembers.FirstOrDefaultAsync(hm => hm.HouseholdId == householdId && hm.UserId == memberId);
        if (householdMember == null)
        {
            throw new BaseException("Household member not found", (int)HttpStatusCode.NotFound);
        }
        _context.HouseholdMembers.Remove(householdMember);
        await _context.SaveChangesAsync();       
    }
}