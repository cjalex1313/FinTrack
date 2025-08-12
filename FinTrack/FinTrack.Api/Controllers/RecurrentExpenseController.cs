using FinTrack.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO;
using FinTrack.Shared.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RecurrentExpenseController : BaseController
{
    private readonly IExpenseService _expenseService;

    public RecurrentExpenseController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet("household/{id:guid}")]
    public async Task<IActionResult> GetHouseholdRecurringExpenses([FromRoute] Guid id)
    {
        var expenses = await _expenseService.GetRecurringExpensesForHousehold(id);
        var result = expenses.Select(e =>
        {
            return e.MapToDTO();
        });
        return Ok(result);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetRecurrentExpense([FromRoute] Guid id)
    {
        var expense = await _expenseService.GetRecurringExpense(id);
        var result = expense.MapToDTO();
        return Ok(result);   
    }

    [HttpPost]
    public async Task<IActionResult> CreateRecurringExpense([FromBody] RecurringExpenseDTO dto)
    {
        var expense = await _expenseService.AddRecurringExpense(dto);
        var result = expense.MapToDTO();
        return Ok(result);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateRecurringExpense([FromBody] RecurringExpenseDTO dto)
    {
        var expense = await _expenseService.UpdateRecurringExpense(dto);
        var result = expense.MapToDTO();
        return Ok(result);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteRecurringExpense([FromRoute] Guid id)
    {
        await _expenseService.DeleteRecurringExpense(id);
        return Ok();   
    }
}