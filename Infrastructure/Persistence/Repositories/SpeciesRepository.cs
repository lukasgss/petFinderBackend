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
        _dbContext = dbContext;
    }

    public async Task<Species?> GetSpeciesByIdAsync(int speciesId)
    {
        return await _dbContext.Species
            .SingleOrDefaultAsync(species => species.Id == speciesId);
    }
}