using Application.Common.Interfaces.GenericRepository;
using Domain.Entities;

namespace Application.Common.Interfaces.Entities.Colors;

public interface IColorRepository : IGenericRepository<Color>
{
    Task<List<Color>> GetMultipleColorsByIdsAsync(IEnumerable<int> colorIds);
}