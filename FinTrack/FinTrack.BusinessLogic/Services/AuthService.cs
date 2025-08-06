using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using FinTrack.Email;
using FinTrack.Shared.Common;
using FinTrack.Shared.Config;
using FinTrack.Shared.DTO.Auth;
using FinTrack.Shared.Entities.Auth;
using FinTrack.Shared.Exceptions;
using FinTrack.Shared.Exceptions.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FinTrack.BusinessLogic.Services;

public interface IAuthService
{
    Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info);
    Task<JwtSecurityToken> ConfirmEmail(Guid userId, string token);
    Task EnsureAdminExists();
    Task EnsureRolesExistInDb();
    Task<ProfileDTO> GetProfile(Guid userId);
    Task<JwtSecurityToken> Login(string email, string password);
    Task<ApplicationUser> RegisterUser(RegisterRequest registerRequest);
}

public class AuthService : IAuthService
{
    private readonly ApiConfig _apiConfig;
    private readonly IEmailService _emailService;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityOptions _identityOptions;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IEmailService emailService, ApiConfig apiConfig)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _emailService = emailService;
        _apiConfig = apiConfig;
    }

    public async Task<IdentityResult> AddLoginAsync(ApplicationUser user, UserLoginInfo info)
    {
        var identityResult = await _userManager.AddLoginAsync(user, info);
        return identityResult;
    }

    public async Task<JwtSecurityToken> ConfirmEmail(Guid userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new UserIdNotFoundException(userId);
        }
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (result == null || !result.Succeeded)
        {
            throw new BaseException()
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessage = result != null ? String.Join(". ", result.Errors.Select(e => e.Description).ToList()) : "Error while confirming email via confirmation token"
            };
        }

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _apiConfig.JWTConfig.ValidIssuer)
        };
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        if (user.Email != null)
        {
            authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
        }
        var jwtToken = GetToken(authClaims);
        return jwtToken;
    }

    public async Task EnsureAdminExists()
    {
        var adminEmail = _apiConfig.AdminConfig.Email;
        var adminPassowrd = _apiConfig.AdminConfig.Password;
        var admin = await _userManager.FindByNameAsync("admin");
        if (admin == null)
        {
            var identityAdmin = await AddUser("admin", adminEmail, adminPassowrd);
            await AddRoleToUser(identityAdmin, Roles.Admin);
        }
        else
        {
            admin.Email = adminEmail;
            admin.EmailConfirmed = true;
            await _userManager.UpdateAsync(admin);
            var token = await _userManager.GeneratePasswordResetTokenAsync(admin);
            var result = await _userManager.ResetPasswordAsync(admin, token, adminPassowrd);
            if (!result.Succeeded)
            {
                throw new BaseException("Error while setting admin password");
            }

            var roles = await _userManager.GetRolesAsync(admin);
            if (!roles.Contains(Roles.Admin))
            {
                await _userManager.AddToRoleAsync(admin, Roles.Admin);
            }
        }
    }

    public async Task EnsureRolesExistInDb()
    {
        if (!await _roleManager.RoleExistsAsync(Roles.Admin))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
        }
        if (!await _roleManager.RoleExistsAsync(Roles.User))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid>(Roles.User));
        }
    }

    public async Task<ProfileDTO> GetProfile(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new BaseException("Error while getting user profile");
        }
        var userRoles = await _userManager.GetRolesAsync(user);
        var result = new ProfileDTO()
        {
            Id = user.Id,
            Email = user.Email!,
            Roles = userRoles.ToList()
        };
        return result;
    }

    public async Task<JwtSecurityToken> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new LoginUserNotFoundException(email);
        }
        var loginAttempt = await _userManager.CheckPasswordAsync(user, password);
        if (!loginAttempt)
        {
            throw new PasswordIncorrectException();
        }

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Iss, _apiConfig.JWTConfig.ValidIssuer)
        };
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }
        if (user.Email != null)
        {
            authClaims.Add(new Claim(ClaimTypes.Email, user.Email));
        }
        var token = GetToken(authClaims);
        return token;
    }
    public async Task<ApplicationUser> RegisterUser(RegisterRequest registerRequest)
    {
        var emailExists = (await _userManager.FindByEmailAsync(registerRequest.Email)) != null;
        if (emailExists)
        {
            throw new EmailAlreadyExistsException(registerRequest.Email);
        }
        var identityUser = await AddUser(registerRequest.Email, registerRequest.Email, registerRequest.Password);
        await AddRoleToUser(identityUser, Roles.User);
        return identityUser;
    }
    private async Task AddRoleToUser(ApplicationUser identityUser, string role)
    {
        var roleResult = await _userManager.AddToRoleAsync(identityUser, role);
        if (!roleResult.Succeeded)
        {
            throw new UserCreationException();
        }
    }

    private async Task<ApplicationUser> AddUser(string username, string email, string password)
    {
        var identityUser = new ApplicationUser()
        {
            UserName = username,
            Email = email,
            SecurityStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = username == "admin"
        };
        var result = await _userManager.CreateAsync(identityUser, password);
        if (!result.Succeeded)
        {
            throw new UserCreationException();
        }

        if (username == "admin") return identityUser;

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
        var encodedToken = Uri.EscapeDataString(confirmationToken);
        _emailService.SendEmail(new Email.Models.MailData
        {
            Email = email,
            Name = username,
            Subject = "Email confirmation",
            Body =
                $"Welcome to FinTrack. Click <a href=\"{_apiConfig.EmailConfirmationUrl + "?userId=" + identityUser.Id + "&token=" + encodedToken}\">here</a> to confirm your email"
        }, MimeKit.Text.TextFormat.Html);
        return identityUser;
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims)
    {
        var jwtSecret = _apiConfig.JWTConfig.Secret;
        if (jwtSecret == null)
        {
            throw new BaseException("JWT configuration invalid");
        }
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddMonths(12),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return token;
    }
}