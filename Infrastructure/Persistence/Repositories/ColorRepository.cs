using Application.Common.Interfaces.Entities.Colors;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class ColorRepository : GenericRepository<Color>, IColorRepository
{
    private readonly AppDbContext _dbContext;
    public ColorRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Color>> GetMultipleColorsByIdsAsync(IEnumerable<int> colorIds)
    {
        return await _dbContext.Colors
            .Where(color => colorIds.Contains(color.Id))
            .ToListAsync();
    }
}