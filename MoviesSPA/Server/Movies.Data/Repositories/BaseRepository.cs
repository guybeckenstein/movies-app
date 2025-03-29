namespace Movies.Repository.Repositories;

public class BaseRepository(MyDbContext dbContext)
{
    protected readonly MyDbContext _dbContext = dbContext;
}
