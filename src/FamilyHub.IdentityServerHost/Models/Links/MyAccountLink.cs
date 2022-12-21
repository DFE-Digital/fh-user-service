namespace FamilyHub.IdentityServerHost.Models.Links;

public class MyAccountLink : Link
{
    public MyAccountLink(string href, string @class = "") : base(href, @class: @class)
    {
    }

    public override string Render()
    {
        return $"<a href = \"{Href}\" id=\"sign-in-link\" class=\"{Class}\">My Account</a>";
    }
}
