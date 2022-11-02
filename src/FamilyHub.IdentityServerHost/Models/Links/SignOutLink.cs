namespace FamilyHub.IdentityServerHost.Models.Links;

public class SignOutLink : Link
{
    public SignOutLink(string href, string @class = "") : base(href, @class: @class)
    {
    }

    public override string Render()
    {
        return $"<a href = \"{Href}\" id=\"sign-out-link\" class=\"{Class}\">Sign Out</a>";
    }
}


