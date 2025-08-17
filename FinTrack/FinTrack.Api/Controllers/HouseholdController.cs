using System.Net;
using FinTrack.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.BusinessLogic.Services.Auth;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class HouseholdController : BaseController
{
    private readonly IHouseholdService _householdService;
    private readonly ISetupService _setupService;
    private readonly IAuthService _authService;

    public HouseholdController(IHouseholdService householdService, ISetupService setupService, IAuthService authService)
    {
        _householdService = householdService;
        _setupService = setupService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserHouseholds()
    {
        var userId = GetUserId();
        List<HouseholdMember> households = await _householdService.GetUserHouseholds(userId);
        var result = households.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }

    [HttpGet("pending-invites")]
    public async Task<IActionResult> GetInvitedHouseholds()
    {
        var userId = GetUserId();
        List<HouseholdMember> householdMembers = await _householdService.GetUserPendingHouseholdInvitations(userId);
        var result = householdMembers.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }

    [HttpPatch("invites/accept/{householdId:guid}")]
    public async Task<IActionResult> AcceptHouseholdInvite([FromRoute] Guid householdId)
    {
        var userId = GetUserId();
        await _householdService.AcceptInvite(userId, householdId);
        return Ok();
    }

    [HttpGet("{householdId:guid}/members/all")]
    public async Task<IActionResult> GetHouseholdMembers([FromRoute] Guid householdId)
    {
        var userId = GetUserId();
        bool userIsOwner = await _householdService.IsUserHouseholdOwner(userId, householdId);
        if (!userIsOwner)
        {
            throw new BaseException("User is not authorized to see household members", (int)HttpStatusCode.Unauthorized);
        }
        List<HouseholdMember> members = await _householdService.GetHouseholdMembers(householdId);
        var result = members.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateHousehold([FromBody] HouseholdDTO dto)
    {
        var userId = GetUserId();
        Household household = await _householdService.CreateHousehold(dto, userId);
        var result = household.MapToDTO();
        return Ok(result);
    }

    [HttpPost("setup")]
    public async Task<IActionResult> SetupHousehold([FromBody] SetupDTO dto)
    {
        var userId = GetUserId();
        await _setupService.SetupHousehold(dto, userId);
        return Ok();
    }

    [HttpPatch("{householdId:guid}/invite/reject")]
    public async Task<IActionResult> RejectHouseholdInvite([FromRoute] Guid householdId)
    {
        var userId = GetUserId();
        await _householdService.RejectUserInvite(userId, householdId);
        return Ok();
    }

    [HttpPost("invite")]
    public async Task<IActionResult> InviteUserToHousehold([FromBody] HouseholdInviteDTO dto)
    {
        var userId = GetUserId();
        Household household = await _householdService.GetHousehold(dto.HouseholdId);
        bool userIsOwner =  household.OwnerId == userId;
        if (!userIsOwner)
        {
            throw new BaseException("User is not authorized to see household members", (int)HttpStatusCode.Unauthorized);
        }

        await _setupService.InviteUser(dto.Email, household);
        return Ok();
    }
    
    [HttpDelete("member")]
    public async Task<IActionResult> RemoveUserFromHousehold([FromBody] HouseholdMemberRemoveDTO dto)
    {
        var userId = GetUserId();
        Household household = await _householdService.GetHousehold(dto.HouseholdId);
        bool userIsOwner =  household.OwnerId == userId;
        if (!userIsOwner)
        {
            throw new BaseException("User is not authorized to see household members", (int)HttpStatusCode.Unauthorized);
        }

        var memberToRemove = await _authService.GetUserByEmail(dto.Email);
        if (memberToRemove == null)
        {
            throw new BaseException("User to remove not found", (int)HttpStatusCode.NotFound);
        }
        await _householdService.DeleteHouseholdMember(household.Id, memberToRemove.Id);
        return Ok();
    }
}