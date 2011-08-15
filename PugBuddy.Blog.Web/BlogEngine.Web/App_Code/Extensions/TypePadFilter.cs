using BlogEngine.Core;
using BlogEngine.Core.Web.Controls;
using Joel.Net;

[Extension("TypePad anti-spam comment filter (based on AkismetFilter)", "1.0", "<a href=\"http://lucsiferre.net\">By Chris Nicola</a>")]
public class TypePadFilter : ICustomFilter
{
    private static ExtensionSettings _settings;
    private static Akismet _api;
    private static string _site;
    private static string _key;
    private static bool _fallThrough = true;

    public TypePadFilter()
    {
        InitSettings();
    }

    #region ICustomFilter Members

    public bool Initialize()
    {
        if (!ExtensionManager.ExtensionEnabled("TypePadFilter"))
            return false;

        _site = _settings.GetSingleValue("SiteURL");
        _key = _settings.GetSingleValue("ApiKey");
        _api = new Akismet(_key, _site, "BlogEngine.net 1.5", "api.antispam.typepad.com");

        return _api.VerifyKey();
    }

    public bool Check(Comment comment)
    {
        if (_api == null) Initialize();

        AkismetComment typePadComment = GetAkismetComment(comment);
        bool isspam = _api.CommentCheck(typePadComment);
        _fallThrough = !isspam;
        return isspam;
    }

    public void Report(Comment comment)
    {
        if (_api == null) Initialize();

        AkismetComment akismetComment = GetAkismetComment(comment);

        if (comment.IsApproved)
        {
            Utils.Log(string.Format("TypePad: Reporting NOT spam from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
            _api.SubmitHam(akismetComment);
        }
        else
        {
            Utils.Log(string.Format("TypePad: Reporting SPAM from \"{0}\" at \"{1}\"", comment.Author, comment.IP));
            _api.SubmitSpam(akismetComment);
        }
    }

    public bool FallThrough
    {
        get { return _fallThrough; }
    }

    #endregion

    private static AkismetComment GetAkismetComment(Comment comment)
    {
        AkismetComment akismetComment = new AkismetComment();
        akismetComment.Blog = _settings.GetSingleValue("SiteURL");
        akismetComment.UserIp = comment.IP;
        akismetComment.CommentContent = comment.Content;
        akismetComment.CommentAuthor = comment.Author;
        akismetComment.CommentAuthorEmail = comment.Email;
        if (comment.Website != null)
        {
            akismetComment.CommentAuthorUrl = comment.Website.OriginalString;
        }
        return akismetComment;
    }

    private void InitSettings()
    {
        ExtensionSettings settings = new ExtensionSettings(this);
        settings.IsScalar = true;

        settings.AddParameter("SiteURL", "Site URL");
        settings.AddParameter("ApiKey", "API Key");

        settings.AddValue("SiteURL", "http://example.com/blog");
        settings.AddValue("ApiKey", "123456789");

        _settings = ExtensionManager.InitSettings("TypePadFilter", settings);
        ExtensionManager.SetStatus("TypePadFilter", false);
    }
}