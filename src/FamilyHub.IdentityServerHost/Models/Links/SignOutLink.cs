namespace FamilyHub.IdentityServerHost.Models.Links;

public class SignOutLink : Link
{
    private readonly string _userName;
    private readonly bool _useOriginalCode;
    public SignOutLink(bool useOriginalCode, string userName, string href, string @class = "") : base(href, @class: @class)
    {
        _useOriginalCode = useOriginalCode;
        _userName = userName;
    }

    public override string Render()
    {
        if (_useOriginalCode)
        {
            return $"<a href = \"{Href}\" id=\"sign-out-link\" class=\"{Class}\">Hello {_userName} Sign Out</a>";
        }
        else
        {
            return $"<a href = \"{Href}\" id=\"sign-out-link\" class=\"{Class}\">Sign Out</a>";
        }
        
    }
}


