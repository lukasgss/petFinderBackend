using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Pets;

public interface IPetRepository : IGenericRepository<Pet>
{
	Task<Pet?> GetPetByIdAsync(Guid petId);
}