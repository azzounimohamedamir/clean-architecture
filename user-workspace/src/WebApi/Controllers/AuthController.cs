using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IIdentityService _identityService;

    public AuthController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<string>> Register([FromBody] RegisterRequest request)
    {
        var (success, userId) = await _identityService.CreateUserAsync(request.Username, request.Password);

        if (!success)
        {
            return BadRequest(new { message = "Username already exists" });
        }

        return Ok(new { userId });
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<string>> Login([FromBody] LoginRequest request)
    {
        var isValid = await _identityService.ValidateUserAsync(request.Username, request.Password);

        if (!isValid)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var token = await _identityService.GenerateJwtTokenAsync(request.Username);
        return Ok(new { token });
    }
}

public record RegisterRequest(string Username, string Password);
public record LoginRequest(string Username, string Password);
