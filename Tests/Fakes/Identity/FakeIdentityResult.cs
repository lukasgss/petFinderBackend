using Microsoft.AspNetCore.Identity;

namespace Tests.Fakes.Identity;

public class FakeIdentityResult : IdentityResult
{
    public FakeIdentityResult(bool succeeded)
    {
        Succeeded = succeeded;
    }
}