using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity.Data;
using ToDoListWebApp.Models.Authentication;
using ToDoListWebApp.Contexts;
using WebApiAuthentication.Models.Authentication;
using ToDoListWebApp.Entities;

namespace ToDoListWebApp.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly TodoContext _context;

    public AuthController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, TodoContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _context = context;
    }


    [Authorize]
    [HttpGet("userdata")]
    public async Task<IActionResult> GetUserData()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized();
        }

        var userData = new UserDataResponse
        {
            Username = user.UserName,
            Email = user.Email,
            LatestLoginTime = user.LatestLoginTime,
            CreatedTodoItemsCount = user.CreatedTodoItemsCount,
            DeletedTodoItemsCount = user.DeletedTodoItemsCount,
            PhoneNumber = user.PhoneNumber
        };

        return Ok(userData);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegistrationModel request)
    {


        var user = new User { UserName = request.Username, Email = request.Email, LatestLoginTime = DateTime.Now };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var loginModel = new LoginModel
            {
                Username = request.Username,
                Password = request.Password,
            };

            var loginResult = await Login(loginModel);
            
            return Ok(loginResult);

        }
 
        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel request)
    {
 
        var user = await _userManager.FindByNameAsync(request.Username);

        if (user == null)
        {
            return BadRequest("Invalid username or password.");
        }

        var result = await _signInManager.PasswordSignInAsync(request.Username, request.Password, true, false);

        if (result.Succeeded)
        {
            var refreshToken = GenerateRefreshToken();
            var token = GenerateJwtToken(user);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(30);

            await _userManager.UpdateAsync(user);

            var loginResponse = new LoginResponse
            {
                JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            };
 
            return Ok(loginResponse);
        }
 
        return BadRequest("Invalid username or password.");
    }


    [AllowAnonymous]
    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshModel request)
    {
 
        var principal = GetPrincipalFromExpiredToken(request.AccessToken);

        if (principal?.Identity?.Name is null)
            return Unauthorized();

        var user = await _userManager.FindByNameAsync(principal.Identity.Name);

        if (user is null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
            return Unauthorized();

        var token = GenerateJwtToken(user);

        return Ok(new LoginResponse
        {
            JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            RefreshToken = request.RefreshToken
        });
    }

    [Authorize]
    [HttpDelete("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var user = await _userManager.GetUserAsync(User);
        user.RefreshToken = null;
        await _userManager.UpdateAsync(user);
        return Ok();
    }

    private JwtSecurityToken GenerateJwtToken(User user)
    {
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSecurityToken:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSecurityToken:Issuer"],
            audience: _configuration["JwtSecurityToken:Audience"],
            claims: claims,
            expires: DateTime.Now.AddSeconds(30),
            signingCredentials: creds);

        return token;
    }

    private static string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var key = _configuration["JwtSecurityToken:Key"] ?? throw new InvalidOperationException("Key not configured");

        var validation = new TokenValidationParameters
        {
            ValidIssuer = _configuration["JwtSecurityToken:Issuer"],
            ValidAudience = _configuration["JwtSecurityToken:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = false
        };

        return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
    }
}

