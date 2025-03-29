#nullable enable
using Movies.Data.Entities;
using Movies.Data.Models;
using System.Threading.Tasks;

namespace Movies.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> Find(BaseUser user);
    Task<bool> Add(User model);
    Task<bool> Update(User model);
}
