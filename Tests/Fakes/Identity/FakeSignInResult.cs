using Microsoft.AspNetCore.Identity;

namespace Tests.Fakes.Identity;

public class FakeSignInResult : SignInResult
{
    public FakeSignInResult(bool succeeded, bool isLockedOut)
    {
        Succeeded = succeeded;
        IsLockedOut = isLockedOut;
    }
}