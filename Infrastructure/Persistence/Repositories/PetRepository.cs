using Application.Common.Interfaces.Entities.Pets;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class PetRepository : GenericRepository<Pet>, IPetRepository
{
	private readonly AppDbContext _dbContext;

	public PetRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
	}

	public async Task<Pet?> GetPetByIdAsync(Guid petId)
	{
		return await _dbContext.Pets
			.Include(pet => pet.Breed)
			.Include(pet => pet.Colors)
			.Include(pet => pet.Owner)
			.Include(pet => pet.Vaccines)
			.Include(pet => pet.Species)
			.FirstOrDefaultAsync(pet => pet.Id == petId);
	}
}