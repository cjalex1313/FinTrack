using FinTrack.Api.Security;
using FinTrack.BusinessLogic.Services;
using FinTrack.BusinessLogic.Services.Auth;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        Guid userId = GetUserId();
        List<HouseholdMember> households = await _householdService.GetUserHouseholds(userId);
        List<HouseholdMemberDTO> result = households.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }

    [HttpGet("pending-invites")]
    public async Task<IActionResult> GetInvitedHouseholds()
    {
        Guid userId = GetUserId();
        List<HouseholdMember> householdMembers = await _householdService.GetUserPendingHouseholdInvitations(userId);
        List<HouseholdMemberDTO> result = householdMembers.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }

    [HttpPatch("invites/accept/{householdId:guid}")]
    public async Task<IActionResult> AcceptHouseholdInvite([FromRoute] Guid householdId)
    {
        Guid userId = GetUserId();
        await _householdService.AcceptInvite(userId, householdId);
        return Ok();
    }

    [HttpGet("{householdId:guid}/members/all")]
    [HouseholdOwnerAuthorize]
    public async Task<IActionResult> GetHouseholdMembers([FromRoute] Guid householdId)
    {
        List<HouseholdMember> members = await _householdService.GetHouseholdMembers(householdId);
        List<HouseholdMemberDTO> result = members.Select(h => h.MapToDTO()).ToList();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateHousehold([FromBody] HouseholdDTO dto)
    {
        Guid userId = GetUserId();
        Household household = await _householdService.CreateHousehold(dto, userId);
        HouseholdDTO result = household.MapToDTO();
        return Ok(result);
    }

    [HttpPost("setup")]
    public async Task<IActionResult> SetupHousehold([FromBody] SetupDTO dto)
    {
        Guid userId = GetUserId();
        await _setupService.SetupHousehold(dto, userId);
        return Ok();
    }

    [HttpPatch("{householdId:guid}/invite/reject")]
    public async Task<IActionResult> RejectHouseholdInvite([FromRoute] Guid householdId)
    {
        Guid userId = GetUserId();
        await _householdService.RejectUserInvite(userId, householdId);
        return Ok();
    }

    [HttpPost("{householdId:guid}/invite")]
    [HouseholdOwnerAuthorize]
    public async Task<IActionResult> InviteUserToHousehold([FromRoute] Guid householdId, [FromBody] HouseholdInviteDTO dto)
    {
        await _setupService.InviteUser(dto.Email, householdId);
        return Ok();
    }

    [HttpDelete("{householdId:guid}/member")]
    [HouseholdOwnerAuthorize]
    public async Task<IActionResult> RemoveUserFromHousehold([FromRoute] Guid householdId, [FromBody] HouseholdMemberRemoveDTO dto)
    {
        Shared.Entities.Auth.ApplicationUser? memberToRemove = await _authService.GetUserByEmail(dto.Email);
        if (memberToRemove == null)
        {
            throw new BaseException("User to remove not found", (int)HttpStatusCode.NotFound);
        }
        await _householdService.DeleteHouseholdMember(householdId, memberToRemove.Id);
        return Ok();
    }
}
