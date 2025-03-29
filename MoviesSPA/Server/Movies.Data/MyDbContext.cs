using Microsoft.EntityFrameworkCore;
using Movies.Data.Entities;

namespace Movies.Repository;

public sealed class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
}