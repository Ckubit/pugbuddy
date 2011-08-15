#region using declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Controls;
using BlogEngine.Core;
using BlogEngine.Core.Providers;
using Recaptcha;

#endregion

public partial class admin_Pages_RecaptchaLogViewer : System.Web.UI.Page
{
    private const string GravatarImage = "<img class=\"photo\" src=\"{0}\" alt=\"{1}\" />";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!System.Threading.Thread.CurrentPrincipal.Identity.IsAuthenticated)
        {
            Response.Redirect(BlogEngine.Core.Utils.RelativeWebRoot);
        }
        if (!IsPostBack)
        {
            BindGrid();
        }
    }

    private void BindGrid()
    {
        Stream s = (Stream)BlogService.LoadFromDataStore(BlogEngine.Core.DataStore.ExtensionType.Extension, "RecaptchaLog");
        List<RecaptchaLogItem> log = new List<RecaptchaLogItem>();
        if (s != null)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<RecaptchaLogItem>));
            log = (List<RecaptchaLogItem>)serializer.Deserialize(s);
            s.Close();
        }

        Dictionary<Guid, Comment> comments = new Dictionary<Guid, Comment>();
        foreach (Post post in Post.Posts)
        {
            foreach (Comment comment in post.Comments)
            {
                comments.Add(comment.Id, comment);
            }
        }

        DataTable dtLogView = new DataTable("LogView");
        dtLogView.Columns.Add("Email");
        dtLogView.Columns.Add("Date", typeof(DateTime));
        dtLogView.Columns.Add("Author");
        dtLogView.Columns.Add("Website");
        dtLogView.Columns.Add("IP");
        dtLogView.Columns.Add("RecaptchaAttempts", typeof(UInt16));
        dtLogView.Columns.Add("CommentTime", typeof(Double));
        dtLogView.Columns.Add("RecaptchaTime", typeof(Double));

        List<RecaptchaLogItem> orphanedRecords = new List<RecaptchaLogItem>();

        foreach (RecaptchaLogItem item in log)
        {
            if (comments.ContainsKey(item.CommentID))
            {
                Comment comment = comments[item.CommentID];
                dtLogView.Rows.Add(comment.Email, comment.DateCreated, comment.Author, comment.Website, comment.IP, item.NumberOfAttempts, item.TimeToComment, item.TimeToSolveCapcha);
            }
            else
            {
                orphanedRecords.Add(item);
            }
        }

        if (orphanedRecords.Count > 0)
        {
            foreach (RecaptchaLogItem orphan in orphanedRecords)
            {
                log.Remove(orphan);
            }
            BlogService.SaveToDataStore(BlogEngine.Core.DataStore.ExtensionType.Extension, "RecaptchaLog", log);
        }

        DataView view = new DataView(dtLogView);
        view.Sort = "Date DESC";
        RecaptchaLog.DataSource = view;
        RecaptchaLog.DataBind();
    }

    #region helper methods

    protected string Gravatar(string email, string author)
    {
        if (BlogSettings.Instance.Avatar == "none")
            return null;

        if (String.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            return "<img src=\"" + Utils.AbsoluteWebRoot + "themes/" + BlogSettings.Instance.Theme + "/noavatar.jpg\" alt=\"" + author + "\" width=\"28\" height=\"28\" />";
        }

        string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(email.ToLowerInvariant().Trim(), "MD5").ToLowerInvariant();
        string gravatar = "http://www.gravatar.com/avatar/" + hash + ".jpg?s=28&amp;d=";

        string link = string.Empty;
        switch (BlogSettings.Instance.Avatar)
        {
            case "identicon":
                link = gravatar + "identicon";
                break;

            case "wavatar":
                link = gravatar + "wavatar";
                break;

            default:
                link = gravatar + "monsterid";
                break;
        }

        return string.Format(CultureInfo.InvariantCulture, GravatarImage, link, author);
    }

    public static string GetWebsite(object website)
    {
        if (website == null) return "";

        const string templ = "<a href='{0}' target='_new' rel='{0}'>{1}</a>";

        string site = website.ToString();
        site = site.Replace("http://www.", "");
        site = site.Replace("http://", "");
        site = site.Length < 20 ? site : site.Substring(0, 17) + "...";

        return string.Format(templ, website, site);
    }

    #endregion
}
