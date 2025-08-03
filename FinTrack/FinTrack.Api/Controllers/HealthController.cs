using AcademyOS.Api.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HealthController : BaseController
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok();
    }
}