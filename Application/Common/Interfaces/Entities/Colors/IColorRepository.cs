using Application.Common.Interfaces.GenericRepository;
using Color = Domain.Entities.Color;

namespace Application.Common.Interfaces.Entities.Colors;

public interface IColorRepository : IGenericRepository<Color>
{
    Task<List<Color>> GetMultipleColorsByIdsAsync(IEnumerable<int> colorIds);
}