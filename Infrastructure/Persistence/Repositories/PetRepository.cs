using Application.Common.Interfaces.Entities.Pets;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Persistence.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
			.Include(pet => pet.Images)
			.FirstOrDefaultAsync(pet => pet.Id == petId);
	}

	public async Task<bool> CreatePetAndImages(Pet petToBeCreated, List<PetImage> images)
	{
		await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync();

		try
		{
			_dbContext.Pets.Add(petToBeCreated);
			await _dbContext.SaveChangesAsync();

			petToBeCreated.Images = images;
			await _dbContext.SaveChangesAsync();

			await transaction.CommitAsync();

			return true;
		}
		catch (Exception)
		{
			await transaction.RollbackAsync();
			return false;
		}
	}
}