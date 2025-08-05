using System.Net;
using AcademyOS.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ExpenseController : BaseController
{
    private readonly IExpenseService _expenseService;
    private readonly IHouseholdService _householdService;

    public ExpenseController(IExpenseService expenseService, IHouseholdService householdService)
    {
        _expenseService = expenseService;
        _householdService = householdService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] ExpenseDTO dto)
    {
        var expense = await _expenseService.AddExpense(dto);
        return Ok(expense.MapToDTO());
    }

    [HttpPut]
    public async Task<IActionResult> UpdateExpense([FromBody] ExpenseDTO dto)
    {
        Expense expense = await _expenseService.UpdateExpense(dto);
        var response = expense.MapToDTO();
        return Ok(response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteExpense([FromRoute] Guid id)
    {
        await _expenseService.DeleteExpense(id);
        return Ok();
    }

    [HttpGet("buckets")]
    public async Task<IActionResult> GetHouseholdBuckets([FromQuery] Guid householdId)
    {
        IEnumerable<ExpenseBucket> buckets = await _expenseService.GetHouseholdBuckets(householdId);
        var response = buckets.Select(b => b.MapToDTO()).ToList();
        return Ok(response);
    }

    [HttpPost("bucket")]
    public async Task<IActionResult> CreateBucket([FromBody] ExpenseBucketDTO dto)
    {
        ExpenseBucket newBucket = await _expenseService.AddExpenseBucket(dto);
        var result = newBucket.MapToDTO();
        return Ok(result);
    }

    [HttpPut("bucket")]
    public async Task<IActionResult> UpdateBucket([FromBody] ExpenseBucketDTO dto)
    {
        ExpenseBucket updatedBucket = await _expenseService.UpdateExpenseBucket(dto);
        var result = updatedBucket.MapToDTO();
        return Ok(result);
    }

    [HttpDelete("bucket/{id:guid}")]
    public async Task<IActionResult> DeleteBucket([FromRoute] Guid id)
    {
        await _expenseService.DeleteExpenseBucket(id);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetExpensesForMonth([FromQuery] Guid householdId, [FromQuery] DateOnly dateInMonth)
    {
        var startOfMonth = new DateOnly(dateInMonth.Year, dateInMonth.Month, 1);
        var endOfMonth = new DateOnly(
            dateInMonth.Year,
            dateInMonth.Month,
            DateTime.DaysInMonth(dateInMonth.Year, dateInMonth.Month)
        );
        IEnumerable<Expense> expenses =
            await _expenseService.GetExpensesForMonth(householdId, startOfMonth, endOfMonth);
        var result = expenses.Select(e => e.MapToDTO()).ToList();
        return Ok(result);       
    }
}