using Application.Common.Interfaces.Entities.Breeds;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class BreedRepository : GenericRepository<Breed>, IBreedRepository
{
    private readonly AppDbContext _dbContext;
    public BreedRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Breed?> GetBreedByIdAsync(int breedId)
    {
        return await _dbContext.Breeds
            .SingleOrDefaultAsync(breed => breed.Id == breedId);
    }

    public async Task<IEnumerable<Breed>> GetBreedsBySpeciesIdAsync(int speciesId)
    {
        return await _dbContext.Breeds
            .Include(breed => breed.Species)
            .AsNoTracking()
            .Where(breed => breed.SpeciesId == speciesId)
            .ToListAsync();
    }
}