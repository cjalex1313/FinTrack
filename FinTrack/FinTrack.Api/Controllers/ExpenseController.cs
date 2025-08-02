using AcademyOS.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Entities;
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

    public ExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] ExpenseDTO dto)
    {
        var expense = await _expenseService.AddExpense(dto);
        return Ok(expense);
    }

    [HttpGet("buckets")]
    public async Task<IActionResult> GetHouseholdBuckets([FromQuery] Guid householdId)
    {
        IEnumerable<ExpenseBucket> buckets = await _expenseService.GetHouseholdBuckets(householdId);
        var response = buckets.Select(b => b.MapToDTO()).ToList();
        return Ok(response);
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