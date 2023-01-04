using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyHub.IdentityServerHost.UI.UnitTests;

public class MySignInResult : Microsoft.AspNetCore.Identity.SignInResult
{
    public void SetSucceeded(bool succeeded)
    {
        this.Succeeded = succeeded;
    }

    public void SetLockedOut(bool lockedout)
    {
        this.IsLockedOut = lockedout;
    }
}
