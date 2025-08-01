using AcademyOS.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.DTO.Setup;
using FinTrack.Shared.Entities;
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

    public HouseholdController(IHouseholdService householdService, ISetupService setupService)
    {
        _householdService = householdService;
        _setupService = setupService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserHouseholds()
    {
        var userId = GetUserId();
        List<Household> households = await _householdService.GetUserHouseholds(userId);
        var result = households.Select(h => h.MapToDTO()).ToList();
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
}