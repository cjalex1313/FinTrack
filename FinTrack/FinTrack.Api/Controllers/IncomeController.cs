using System.Net;
using FinTrack.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

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
    public async Task<IActionResult> GetIncomeForMonth([FromQuery] DateOnly dateInMonth, [FromQuery] Guid householdId)
    {
        var oneTimeIncomesForMonth = await _incomeService.GetOneTimeIncomesForMonth(householdId, dateInMonth);
        var recurringIncomesForMonth = await _incomeService.GetRecurringIncomesForMonth(householdId, dateInMonth);

        var result = new IncomesForMonthDTO
        {
            OneTimeIncomes = oneTimeIncomesForMonth.Select(oti => oti.MapToDTO()).ToList(),
            RecurringIncomes = recurringIncomesForMonth.Select(ri => ri.MapToDTO()).ToList()
        };
        return Ok(result);
    }

    [HttpPost("one-time")]
    public async Task<IActionResult> AddOneTimeIncome([FromBody] OneTimeIncomeDTO dto)
    {
        OneTimeIncome income = await _incomeService.AddOneTimeIncome(dto);
        var result = income.MapToDTO();
        return Ok(result);
    }

    [HttpPost("recurring")]
    public async Task<IActionResult> AddRecurringIncome([FromBody] RecurringIncomeDTO dto)
    {
        RecurringIncome income = await _incomeService.AddRecurringIncome(dto);
        var result = income.MapToDTO();
        return Ok(result);
    }

    [HttpPut("one-time")]
    public async Task<IActionResult> UpdateOneTimeIncome([FromBody] OneTimeIncomeDTO dto)
    {
        var updatedOneTimeIncome = await _incomeService.UpdateOneTimeIncome(dto);
        var result = updatedOneTimeIncome.MapToDTO();
        return Ok(result);
    }

    [HttpPut("recurring")]
    public async Task<IActionResult> UpdateRecurringIncome([FromBody] RecurringIncomeDTO dto)
    {
        RecurringIncome updatedIncome = await _incomeService.UpdateRecurringIncome(dto);
        var result = updatedIncome.MapToDTO();
        return Ok(result);
    }

    [HttpDelete("one-time/{id:guid}")]
    public async Task<IActionResult> DeleteOneTimeIncome([FromRoute] Guid id)
    {
        await _incomeService.DeleteOneTimeIncome(id);
        return Ok();
    }

    [HttpDelete("recurring/{id:guid}")]
    public async Task<IActionResult> DeleteRecurringIncome([FromRoute] Guid id)
    {
        await _incomeService.DeleteRecurringIncome(id);
        return Ok();
    }

    // private async Task<Guid> GetDefaultHouseholdId()
    // {
    //     var userId = GetUserId();
    //     var userHouseholds = await _householdService.GetUserHouseholds(userId);
    //     if (!userHouseholds.Any())
    //     {
    //         throw new BaseException("No user households found. Please create a household first.",
    //             (int)HttpStatusCode.NotFound);
    //     }
    //     var household = userHouseholds.First();
    //     return household.Id;
    // }
}