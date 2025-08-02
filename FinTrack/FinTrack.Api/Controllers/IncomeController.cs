using AcademyOS.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class IncomeController : BaseController
{
    private readonly IIncomeService _incomeService;
    private readonly IHouseholdService _householdService;

    public IncomeController(IIncomeService incomeService, IHouseholdService householdService)
    {
        _incomeService = incomeService;
        _householdService = householdService;
    }

    [HttpGet("for-month")]
    public async Task<IActionResult> GetIncomeForMonth([FromQuery] DateOnly dateInMonth)
    {
        var userId = GetUserId();
        var userHouseholds = await _householdService.GetUserHouseholds(userId);
        if (!userHouseholds.Any())
        {
            return BadRequest();
        }
        var household = userHouseholds.First();
        var oneTimeIncomesForMonth = await _incomeService.GetOneTimeIncomesForMonth(household.Id, dateInMonth);
        var recurringIncomesForMonth = await _incomeService.GetRecurringIncomesForMonth(household.Id, dateInMonth);

        var result = new IncomesForMonthDTO
        {
            OneTimeIncomes = oneTimeIncomesForMonth.Select(oti => oti.MapToDTO()).ToList(),
            RecurringIncomes = recurringIncomesForMonth.Select(ri => ri.MapToDTO()).ToList()
        };
        return Ok(result);
    }
    
}