using Application.Common.Interfaces.Providers;

namespace Application.Common.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid NewGuid()
    {
        return Guid.NewGuid();
    }
}