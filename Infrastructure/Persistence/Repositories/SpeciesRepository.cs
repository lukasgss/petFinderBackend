using Application.Common.Interfaces.Entities.AnimalSpecies;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SpeciesRepository : GenericRepository<Species>, ISpeciesRepository
{
    private readonly AppDbContext _dbContext;
    
    public SpeciesRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Species>> GetAllSpecies()
    {
        return await _dbContext.Species
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Species?> GetSpeciesByIdAsync(int speciesId)
    {
        return await _dbContext.Species
            .SingleOrDefaultAsync(species => species.Id == speciesId);
    }
}