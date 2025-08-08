using System.Security.Claims;
using FinTrack.Shared.Exceptions.Auth;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.Api.Controllers;

public abstract class BaseController : ControllerBase
{
    protected Guid GetUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
        {
            throw new UserIdIncorrectException("");
        }
        return userId;
    }
}