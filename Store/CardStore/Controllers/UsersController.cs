using Microsoft.AspNetCore.Mvc;
using CardStore.Services;
using CardStore.DTOs;
using CardStore.Attributes;

namespace CardStore.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user profile
    /// </summary>
    /// <returns>Current user's profile information</returns>
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var user = await _userService.GetUserProfileAsync(userId.Value);
            
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="updateUserDto">Updated user information</param>
    /// <returns>Updated user profile</returns>
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
            }

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var updatedUser = await _userService.UpdateUserAsync(userId.Value, updateUserDto);
            
            if (updatedUser == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            _logger.LogInformation("User {UserId} profile updated successfully", userId);
            return Ok(new { success = true, data = updatedUser });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User information</returns>
    [HttpGet("{id}")]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> GetUser(int id)
    {
        try
        {
            // Check if requesting own profile or admin
            var currentUserId = GetCurrentUserId();
            if (currentUserId != id) // In real app, also check if user is admin
            {
                return Forbid("Access denied");
            }

            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            return Ok(new { success = true, data = user });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId}", id);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Get all active users (Admin only)
    /// </summary>
    /// <returns>List of all active users</returns>
    [HttpGet]
    // Note: In a real application, you'd check for admin role here
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            // Note: In a real application, you'd verify admin role here
            var users = await _userService.GetActiveUsersAsync();
            return Ok(new { success = true, data = users, count = users.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Delete user account (soft delete)
    /// </summary>
    /// <returns>Deletion result</returns>
    [HttpDelete("profile")]
    public async Task<IActionResult> DeleteAccount()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated" });
            }

            var success = await _userService.DeleteUserAsync(userId.Value);
            
            if (!success)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            _logger.LogInformation("User {UserId} deleted their account", userId);
            return Ok(new { success = true, message = "Account deleted successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user account");
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if username is available
    /// </summary>
    /// <param name="username">Username to check</param>
    /// <returns>Availability status</returns>
    [HttpGet("check-username/{username}")]
    public async Task<IActionResult> CheckUsername(string username)
    {
        try
        {
            var exists = await _userService.UsernameExistsAsync(username);
            return Ok(new { success = true, available = !exists, username = username });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking username {Username}", username);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    /// <summary>
    /// Check if email is available
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <returns>Availability status</returns>
    [HttpGet("check-email/{email}")]
    public async Task<IActionResult> CheckEmail(string email)
    {
        try
        {
            var exists = await _userService.EmailExistsAsync(email);
            return Ok(new { success = true, available = !exists, email = email });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email {Email}", email);
            return StatusCode(500, new { success = false, message = "Internal server error" });
        }
    }

    private int? GetCurrentUserId()
    {
        var userIdClaim = HttpContext.Items["UserId"];
        if (userIdClaim != null && int.TryParse(userIdClaim.ToString(), out int userId))
        {
            return userId;
        }
        return null;
    }
}