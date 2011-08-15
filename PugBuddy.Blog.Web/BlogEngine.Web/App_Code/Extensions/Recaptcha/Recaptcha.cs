using BlogEngine.Core.Web.Controls;

namespace Recaptcha
{
    /// <summary>
    /// Builds the recaptcha control ( http://www.Recaptcha.net )
    /// </summary>
    [Extension("Settings for the Recaptcha control", "1.0", "<a href=\"http://www.bloodforge.com\">Bloodforge.com</a>")]
    public class Recaptcha
    {
        static protected ExtensionSettings _settings;

        public Recaptcha()
        {
            InitSettings();
        }

        public string JScript
        {
            get
            {
                string result = @"function showRecaptchaLog() {
        window.scrollTo(0, 0);
        var width = document.documentElement.clientWidth + document.documentElement.scrollLeft;
        var height = document.documentElement.clientHeight + document.documentElement.scrollTop;

        var layer = document.createElement('div');
        layer.style.zIndex = 1002;
        layer.id = 'RecaptchaLogLayer';
        layer.style.position = 'absolute';
        layer.style.top = '0px';
        layer.style.left = '0px';
        layer.style.height = document.documentElement.scrollHeight + 'px';
        layer.style.width = width + 'px';
        layer.style.backgroundColor = 'black';
        layer.style.opacity = '.6';
        layer.style.filter += ('progid:DXImageTransform.Microsoft.Alpha(opacity=60)');
        document.body.style.position = 'static';
        document.body.appendChild(layer);

        var size = { 'height': 500, 'width': 750 };
        var iframe = document.createElement('iframe');
        iframe.name = 'Recaptcha Log Viewer';
        iframe.id = 'RecaptchaLogDetails';
        iframe.src = '../Pages/RecaptchaLogViewer.aspx';
        iframe.style.height = size.height + 'px';
        iframe.style.width = size.width + 'px';
        iframe.style.position = 'fixed';
        iframe.style.zIndex = 1003;
        iframe.style.backgroundColor = 'white';
        iframe.style.border = '4px solid silver';
        iframe.frameborder = '0';

        iframe.style.top = ((height + document.documentElement.scrollTop) / 2) - (size.height / 2) + 'px';
        iframe.style.left = (width / 2) - (size.width / 2) + 'px';

        document.body.appendChild(iframe);
        return false;
    }";
                return result;
            }
        }

        #region private methods

        public void InitSettings()
        {
            ExtensionSettings settings = new ExtensionSettings(this);
            settings.IsScalar = true;

            settings.AddParameter("PublicKey", "Public Key", 50, true, true, ParameterType.String);
            settings.AddValue("PublicKey", "YOURPUBLICKEY");

            settings.AddParameter("PrivateKey", "Private Key", 50, true, true, ParameterType.String);
            settings.AddValue("PrivateKey", "YOURPRIVATEKEY");

            settings.AddParameter("ShowForAuthenticatedUsers", "Show Captcha For Authenticated Users", 1, true, false, ParameterType.Boolean);
            settings.AddValue("ShowForAuthenticatedUsers", false);

            settings.AddParameter("MaxLogEntries", "Logging: Maximum successful recaptcha attempts to store (set to 0 to disable logging)", 4, true, false, ParameterType.Integer);
            settings.AddValue("MaxLogEntries", 50);

            settings.AddParameter("Theme", "Theme", 20, true, false, ParameterType.DropDown);
            settings.AddValue("Theme", new string[] { "red", "white", "blackglass", "clean" }, "white");

            settings.Help = "\n<script language='javascript' type='text/javascript'>\n" + JScript + "\n</script>\n" + "You can create your own public key at <a href='http://www.Recaptcha.net'>http://www.Recaptcha.net</a>. This is used for communication between your website and the recapcha server.<br /><br />Please rememeber you need to <span style=\"color:red\">enable extension</span> for reCaptcha to show up on the comments form.<br /><br />You can see some statistics on Captcha solving by storing successful attempts. If you're getting spam, this should also confirm that the spammers are at least solving the captcha.<br /><br /><a href='../Pages/RecaptchaLogViewer.aspx' target='_blank' onclick='return showRecaptchaLog()'>Click here to view the log</a>";
            _settings = ExtensionManager.InitSettings("Recaptcha", settings);

            ExtensionManager.SetStatus("Recaptcha", false);
        }

        #endregion
    }


}