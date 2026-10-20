using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Business.DTOs.Users;
using VirtualWallet.Business.Exceptions;
using VirtualWallet.Business.Services.Contracts;

namespace VirtualWallet.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto registerDto)
    {
        try
        {
            var user = await _userService.RegisterAsync(registerDto);
            return CreatedAtAction(nameof(Register), new  { id = user.Id }, user);
        }
        catch (DuplicateEntityException ex)
        {
            //returns http 409 conflict if username/email/phone already exists
            return Conflict(new { message = ex.Message });
        }
        catch (InvalidUserInputException ex)
        {
            //returns http 400 bad req if password doesn't meet requirements
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        try
        {
            var user = await _userService.AuthenticateAsync(loginDto);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = false,
                    AllowRefresh = true
                });

            return Ok(new
            {
                message = "Login Successful",
                user
            });
        }
        catch (InvalidUserInputException ex)
        {
            // returns http 401 unauthorized for incorrect login info
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return NoContent();
    }
}
