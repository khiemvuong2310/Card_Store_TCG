using AutoMapper;
using CardStore.Data.Repositories;
using CardStore.DTOs;
using CardStore.Models;

namespace CardStore.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
    {
        var users = await _userRepository.GetActiveUsersAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> CreateUserAsync(RegisterDto registerDto)
    {
        var user = _mapper.Map<User>(registerDto);
        // The password from registerDto is already hashed by AuthService
        user.PasswordHash = registerDto.Password;
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        
        var createdUser = await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            return null;

        _mapper.Map(updateUserDto, existingUser);
        existingUser.UpdatedAt = DateTime.UtcNow;
        
        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        await _userRepository.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        // Soft delete - mark as inactive
        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.UsernameExistsAsync(username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.EmailExistsAsync(email);
    }

    public async Task<UserDto?> GetUserProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        await _userRepository.UpdateLastLoginAsync(userId);
        await _userRepository.SaveChangesAsync();
    }

    public async Task<User?> GetUserByUsernameWithPasswordAsync(string username)
    {
        return await _userRepository.GetByUsernameWithPasswordAsync(username);
    }

    public async Task<User?> GetUserByEmailWithPasswordAsync(string email)
    {
        return await _userRepository.GetByEmailWithPasswordAsync(email);
    }
}