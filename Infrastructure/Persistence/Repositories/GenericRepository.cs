using Application.Common.Interfaces.GenericRepository;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _entity;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _entity = dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _entity.FindAsync(id);
    }

    public void Add(T entity)
    {
        _entity.Add(entity);
    }

    public void Update(T entity)
    {
        _entity.Update(entity);
    }

    public void Delete(T entity)
    {
        _entity.Remove(entity);
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}