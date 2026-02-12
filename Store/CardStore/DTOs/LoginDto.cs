using System.ComponentModel.DataAnnotations;

namespace CardStore.DTOs;

public class LoginDto
{
    [Required]
    [MaxLength(100)]
    public string UsernameOrEmail { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}