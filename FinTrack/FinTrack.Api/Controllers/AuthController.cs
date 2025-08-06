using AcademyOS.Api.Controllers;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO.Auth;
using FinTrack.Shared.Entities.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IdentityOptions _identityOptions;

    public AuthController(IAuthService authService, SignInManager<ApplicationUser> signInManager, IdentityOptions identityOptions)
    {
        _authService = authService;
        _signInManager = signInManager;
        _identityOptions = identityOptions;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest loginRequest)
    {
        var token = await _authService.Login(loginRequest.Email, loginRequest.Password);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new LoginResult()
        {
            AccessToken = tokenString,
        };
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        await _authService.RegisterUser(registerRequest);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPatch("confirm-email")]
    public async Task<ActionResult<LoginResult>> ConfirmEmail([FromBody] EmailValidationRequest emailValidationRequest)
    {
        var token = await _authService.ConfirmEmail(emailValidationRequest.UserId, emailValidationRequest.Token);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new LoginResult()
        {
            AccessToken = tokenString,
        };
        return Ok(response);
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<ActionResult<ProfileDTO>> GetProfile()
    {
        var userId = GetUserId();
        var result = await _authService.GetProfile(userId);
        return Ok(result);
    }


    [HttpGet]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
    {
        if (remoteError != null)
            return RedirectToAction("Login", new { ErrorMessage = $"Error from external provider: {remoteError}" });

        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
            return RedirectToAction("Login");

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
        if (result.Succeeded)
        {
            return LocalRedirect(returnUrl ?? "/");
        }

        // If the user does not have an account, create one
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email != null)
        {
            var user = await _authService.RegisterUser(new RegisterRequest { Email = email, Password = PasswordGenerator.GeneratePassword(_identityOptions) });
            await _authService.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl ?? "/");
        }

        return RedirectToAction("Login");
    }

}