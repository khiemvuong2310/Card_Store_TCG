using Microsoft.AspNetCore.Mvc;
using CardStore.Services;
using CardStore.DTOs;

namespace CardStore.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="registerDto">User registration data</param>
    /// <returns>Newly created user information</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data", errors = ModelState });
            }

            var user = await _authService.RegisterAsync(registerDto);
            
            _logger.LogInformation("User {Username} registered successfully", registerDto.Username);
            
            return Ok(new { 
                message = "User registered successfully", 
                user = new { 
                    user.Id, 
                    user.Username, 
                    user.Email 
                } 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {Username}", registerDto.Username);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Login user with username/email and password
    /// </summary>
    /// <param name="loginDto">Login credentials</param>
    /// <returns>JWT token and user information</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data", errors = ModelState });
            }

            var result = await _authService.LoginAsync(loginDto);
            
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            _logger.LogInformation("User {UsernameOrEmail} logged in successfully", loginDto.UsernameOrEmail);

            return Ok(new {
                message = "Login successful",
                token = result.Token,
                user = result.User,
                expiresAt = result.ExpiresAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user {UsernameOrEmail}", loginDto.UsernameOrEmail);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    /// <param name="token">JWT token to validate</param>
    /// <returns>Token validation result</returns>
    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] string token)
    {
        try
        {
            var isValid = await _authService.ValidateTokenAsync(token);
            
            if (isValid)
            {
                return Ok(new { message = "Token is valid", valid = true });
            }
            
            return BadRequest(new { message = "Token is invalid", valid = false });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}