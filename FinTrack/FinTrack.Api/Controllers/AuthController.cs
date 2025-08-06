using System.IdentityModel.Tokens.Jwt;
using FinTrack.BusinessLogic.Services;
using FinTrack.Shared.DTO.Auth;
using FinTrack.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyOS.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        this._authService = authService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResult>> Login([FromBody] LoginRequest loginRequest)
    {
        var token = await _authService.Login(loginRequest.Email, loginRequest.Password);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        LoginResult response = new LoginResult()
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
        var token = await this._authService.ConfirmEmail(emailValidationRequest.UserId, emailValidationRequest.Token);
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        LoginResult response = new LoginResult()
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
        ProfileDTO result = await _authService.GetProfile(userId);
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
}