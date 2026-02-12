using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CardStore.DTOs;
using CardStore.Models;
using Microsoft.IdentityModel.Tokens;

namespace CardStore.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public AuthService(IUserService userService, IConfiguration configuration, IMapper mapper)
    {
        _userService = userService;
        _configuration = configuration;
        _mapper = mapper;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
    {
        // Try to find user by username or email (with password hash)
        var userEntity = await _userService.GetUserByUsernameWithPasswordAsync(loginDto.UsernameOrEmail) ??
                        await _userService.GetUserByEmailWithPasswordAsync(loginDto.UsernameOrEmail);

        if (userEntity == null || !userEntity.IsActive)
            return null;

        // Verify password
        if (!VerifyPassword(loginDto.Password, userEntity.PasswordHash))
            return null;

        // Convert to UserDto for response
        var userDto = _mapper.Map<UserDto>(userEntity);
        
        var token = GenerateJwtToken(userDto);
        var expiresAt = DateTime.UtcNow.AddHours(24); // Token expires in 24 hours

        // Update last login
        await _userService.UpdateLastLoginAsync(userEntity.Id);

        return new LoginResponseDto
        {
            Token = token,
            User = userDto,
            ExpiresAt = expiresAt
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if username or email already exists
        if (await _userService.UsernameExistsAsync(registerDto.Username))
            throw new InvalidOperationException("Username already exists");

        if (await _userService.EmailExistsAsync(registerDto.Email))
            throw new InvalidOperationException("Email already exists");

        // Create modified registerDto with hashed password for user service
        var modifiedRegisterDto = new RegisterDto
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            Password = HashPassword(registerDto.Password),
            ConfirmPassword = HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            PhoneNumber = registerDto.PhoneNumber,
            Address = registerDto.Address,
            City = registerDto.City,
            PostalCode = registerDto.PostalCode,
            Country = registerDto.Country
        };
        
        var createdUser = await _userService.CreateUserAsync(modifiedRegisterDto);
        return createdUser;
    }

    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetTokenValidationParameters();

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public string GenerateJwtToken(UserDto user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long!";
        var issuer = jwtSettings["Issuer"] ?? "CardStoreAPI";
        var audience = jwtSettings["Audience"] ?? "CardStoreClients";

        var key = Encoding.ASCII.GetBytes(secretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string HashPassword(string password)
    {
        // Using BCrypt for password hashing
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    private TokenValidationParameters GetTokenValidationParameters()
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "your-super-secret-key-that-is-at-least-32-characters-long!";
        var issuer = jwtSettings["Issuer"] ?? "CardStoreAPI";
        var audience = jwtSettings["Audience"] ?? "CardStoreClients";

        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    }
}