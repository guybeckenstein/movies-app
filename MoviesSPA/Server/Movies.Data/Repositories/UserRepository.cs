#nullable enable
using Microsoft.EntityFrameworkCore;
using Movies.Data.Entities;
using Movies.Data.Models;
using Movies.Data.Repositories.Interfaces;
using Movies.Repository;
using Movies.Repository.Repositories;
using System.Threading.Tasks;

namespace Movies.Data.Repositories;

public class UserRepository(MyDbContext dbContext) : BaseRepository(dbContext), IUserRepository
{
    public async Task<User?> Find(BaseUser user)
    {
        var res = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == user.Username);

        return res;
    }
    public async Task<bool> Add(User model)
    {
        var res = await _dbContext.Users.AddAsync(model);

        if (res is null)
        {
            return false;
        }
        var entityInserted = await _dbContext.SaveChangesAsync();

        return entityInserted.Equals(1);
    }
    public async Task<bool> Update(User model)
    {
        var res = _dbContext.Users.Update(model);

        if (res is null)
        {
            return false;
        }
        var entityInserted = await _dbContext.SaveChangesAsync();

        return entityInserted.Equals(1);
    }
}
