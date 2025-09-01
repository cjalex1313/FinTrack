using FinTrack.Shared.Exceptions.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinTrack.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected internal Guid GetUserId()
    {
        string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
        {
            throw new UserIdIncorrectException("");
        }
        return userId;
    }
}
