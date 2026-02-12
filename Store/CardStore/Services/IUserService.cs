using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto?> GetUserByUsernameAsync(string username);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameWithPasswordAsync(string username);
    Task<User?> GetUserByEmailWithPasswordAsync(string email);
    Task<IEnumerable<UserDto>> GetActiveUsersAsync();
    Task<UserDto> CreateUserAsync(RegisterDto registerDto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<UserDto?> GetUserProfileAsync(int userId);
    Task UpdateLastLoginAsync(int userId);
}

// Additional DTO for updating user profile
public class UpdateUserDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}