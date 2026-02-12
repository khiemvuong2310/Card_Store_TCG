using CardStore.Models;
using Microsoft.EntityFrameworkCore;

namespace CardStore.Data.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByUsernameWithPasswordAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailWithPasswordAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<User>> GetActiveUsersAsync()
    {
        return await _dbSet
            .Where(u => u.IsActive)
            .OrderBy(u => u.Username)
            .ToListAsync();
    }

    public async Task<User?> GetUserWithOrdersAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Orders)
            .ThenInclude(o => o.OrderItems)
            .ThenInclude(oi => oi.Card)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<User?> GetUserWithCollectionsAsync(int userId)
    {
        return await _dbSet
            .Include(u => u.Collections)
            .ThenInclude(c => c.Card)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }
}