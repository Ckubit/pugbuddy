using System;
using System.Globalization;
using System.Web.Security;
using BlogEngine.Core;

public partial class admin_admin : System.Web.UI.MasterPage
{
    private const string GRAVATAR_IMAGE = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" />";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
            Response.Redirect(Utils.RelativeWebRoot);
    }

    protected AuthorProfile AdminProfile()
    {
        try
        {
            return AuthorProfile.GetProfile(System.Threading.Thread.CurrentPrincipal.Identity.Name);
        }
        catch (Exception e)
        {
            Utils.Log(e.Message);
            return null;
        }
    }

    protected string AdminPhoto()
    {
        string src = Utils.AbsoluteWebRoot + "admin/images/no_avatar.png";
        string adminName = "";

        if (AdminProfile() != null)
        {
            adminName = AdminProfile().DisplayName;
            if (!string.IsNullOrEmpty(AdminProfile().PhotoURL))
            {
                src = AdminProfile().PhotoURL;
            }else
            {
                if(!string.IsNullOrEmpty(AdminProfile().EmailAddress) &&
                    BlogSettings.Instance.Avatar != "none")
                        src = Avatar(AdminProfile().EmailAddress);
            }
        }
        return string.Format(CultureInfo.InvariantCulture, GRAVATAR_IMAGE, src, adminName);
    }

    protected string Avatar(string email)
    {
        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5").ToLowerInvariant();
        string src = "http://www.gravatar.com/avatar/" + hash + ".jpg?s=28&amp;d=";

        switch (BlogSettings.Instance.Avatar)
        {
            case "identicon":
                src += "identicon";
                break;
            case "wavatar":
                src += "wavatar";
                break;
            default:
                src += "monsterid";
                break;
        }
        return src;
    }
}