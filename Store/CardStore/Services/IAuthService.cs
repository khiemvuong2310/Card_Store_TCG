using CardStore.DTOs;

namespace CardStore.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    Task<bool> ValidateTokenAsync(string token);
    string GenerateJwtToken(UserDto user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}