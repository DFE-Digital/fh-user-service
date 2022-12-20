using FamilyHub.IdentityServerHost.Models;

namespace FamilyHub.IdentityServerHost.Helpers;

public static class RoleHelper
{
    public static string GetRoleFullName(string role)
    {
        switch (role)
        {
            case "DfEAdmin":
                return "Department for Education administrator";
                
            case "LAAdmin":
                return "Local authority administrator";
          
            case "VCSAdmin":
                return "Voluntary community organisation administrator";
                
            case "Professional":
                return "Department for Education administrator";
              
        }

        return string.Empty;
    }
}
