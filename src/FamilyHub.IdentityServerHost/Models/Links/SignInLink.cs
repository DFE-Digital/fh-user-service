using System.Security.Claims;

namespace FamilyHub.IdentityServerHost.Models.Links;

public class SignInLink : Link
{
    public SignInLink(string href, string @class = "") : base(href, @class: @class)
    {
    }

    public override string Render()
    {
        return $"<a href = \"{Href}\" id=\"sign-in-link\" class=\"{Class}\">Sign In</a>";
    }
}
