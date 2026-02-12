using CardStore.Models;

namespace CardStore.Data.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameWithPasswordAsync(string username);
    Task<User?> GetByEmailWithPasswordAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetActiveUsersAsync();
    Task<User?> GetUserWithOrdersAsync(int userId);
    Task<User?> GetUserWithCollectionsAsync(int userId);
    Task UpdateLastLoginAsync(int userId);
}