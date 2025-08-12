using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace FinTrack.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IdentityOptions _identityOptions;

    public AuthController(
        IAuthService authService,
        IOptions<IdentityOptions> identityOptions
    )
    {
        _authService = authService;
        _identityOptions = identityOptions.Value;
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
        await _authService.RegisterUser(registerRequest, true);
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

    [HttpPatch("profile-names")]
    [Authorize]
    public async Task<IActionResult> UpdateProfileNames([FromBody] UpdateProfileNamesRequest request)
    {
        var userId = GetUserId();
        await _authService.UpdateProfileNames(userId, request.FirstName, request.LastName);
        ProfileDTO result = await _authService.GetProfile(userId);
        return Ok(result);
    }

    [HttpPatch("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userId = GetUserId();
        await _authService.ChangePassword(userId, request.OldPassword, request.NewPassword);
        return Ok();
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.ForgotPassword(request.Email);
        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPassword(request.UserId, request.Token, request.Password);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost(nameof(ExternalLoginCallback))]
    public async Task<IActionResult> ExternalLoginCallback([FromBody] ExternalLoginCallbackRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (!await _authService.UserExists(request.Email))
        {
            var user = await _authService.RegisterUser(new RegisterRequest { Email = request.Email, Password = PasswordGenerator.GeneratePassword(_identityOptions) }, false);
            var loginInfo = new UserLoginInfo("Google", request.ProviderKey, "Google");
            var addLoginResult = await _authService.AddLoginAsync(user, loginInfo);
            if (!addLoginResult.Succeeded)
            {
                return BadRequest(addLoginResult.Errors);
            }
        }
        var token = await _authService.LoginByProviderKey(request.Email, request.ProviderKey);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var response = new LoginResult()
        {
            AccessToken = tokenString,
        };
        return Ok(response);
    }

}