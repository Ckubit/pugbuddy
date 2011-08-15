// Copyright (c) 2007 Adrian Godong, Ben Maurer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted for dotnetblogengine by Filip Stanek ( http://www.bloodforge.com )

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BlogEngine.Core;
using BlogEngine.Core.Providers;
using Recaptcha;

namespace Controls
{
    public class RecaptchaControl : WebControl, IValidator
    {

        #region Private Fields

        private const string RECAPTCHA_CHALLENGE_FIELD = "recaptcha_challenge_field";
        private const string RECAPTCHA_RESPONSE_FIELD = "recaptcha_response_field";

        private const string RECAPTCHA_SECURE_HOST = "https://api-secure.recaptcha.net";
        private const string RECAPTCHA_HOST = "http://api.recaptcha.net";

        private RecaptchaResponse recaptchaResponse;

        private string publicKey;
        private string privateKey;
        private string theme;
        private bool skipRecaptcha = true;
        private string errorMessage;
        private string userUniqueIdentifier;

        #endregion

        #region Public Properties

        public string Theme
        {
            get { return theme; }
            set { theme = value; }
        }

        public string UserUniqueIdentifier
        {
            get { return userUniqueIdentifier; }
            set { userUniqueIdentifier = value; }
        }

        /// <summary>
        /// Returns whether the control has been enabled via the Extension Manager
        /// </summary>
        public bool RecaptchaEnabled
        {
            get
            {
                ManagedExtension captchaExtension = ExtensionManager.GetExtension("Recaptcha");
                return captchaExtension.Enabled;
            }
        }

        /// <summary>
        /// Returns whether the recaptcha needs to be displayed for the current user
        /// </summary>
        public bool RecaptchaNecessary
        {
            get
            {
                ExtensionSettings Settings = ExtensionManager.GetSettings("Recaptcha");
                return !Page.User.Identity.IsAuthenticated || Convert.ToBoolean(Settings.GetSingleValue("ShowForAuthenticatedUsers"));
            }
        }

        public int MaxLogEntries
        {
            get
            {
                ExtensionSettings Settings = ExtensionManager.GetSettings("Recaptcha");
                return Convert.ToInt32(Settings.GetSingleValue("MaxLogEntries"));
            }
        }

        public bool RecaptchaLoggingEnabled
        {
            get
            {
                ExtensionSettings Settings = ExtensionManager.GetSettings("Recaptcha");
                return MaxLogEntries > 0;
            }
        }

        internal DateTime PageLoadTime
        {
            get
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "PageLoadTime"] != null)
                {
                    return Convert.ToDateTime(HttpContext.Current.Cache[UserUniqueIdentifier + "PageLoadTime"]);
                }
                else
                {
                    return DateTime.Now;
                }
            }
            set
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "PageLoadTime"] != null)
                {
                    HttpContext.Current.Cache[UserUniqueIdentifier + "PageLoadTime"] = value;
                }
                else
                {
                    HttpContext.Current.Cache.Add(UserUniqueIdentifier + "PageLoadTime", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Low, null);
                }
            }
        }

        internal DateTime RecaptchaRenderTime
        {
            get
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaRenderTime"] != null)
                {
                    return Convert.ToDateTime(HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaRenderTime"]);
                }
                else
                {
                    return DateTime.Now;
                }
            }
            set
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaRenderTime"] != null)
                {
                    HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaRenderTime"] = value;
                }
                else
                {
                    HttpContext.Current.Cache.Add(UserUniqueIdentifier + "RecaptchaRenderTime", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Low, null);
                }
            }
        }

        internal UInt16 RecaptchaAttempts
        {
            get
            {

                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaAttempts"] != null)
                {
                    return Convert.ToUInt16(HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaAttempts"]);
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaAttempts"] != null)
                {
                    HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaAttempts"] = value;
                }
                else
                {
                    HttpContext.Current.Cache.Add(UserUniqueIdentifier + "RecaptchaAttempts", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 15, 0), System.Web.Caching.CacheItemPriority.Low, null);
                }
            }
        }

        internal string RecaptchaChallengeValue
        {
            get
            {

                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaChallengeValue"] != null)
                {
                    return Convert.ToString(HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaChallengeValue"]);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaChallengeValue"] != null)
                {
                    HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaChallengeValue"] = value;
                }
                else
                {
                    HttpContext.Current.Cache.Add(UserUniqueIdentifier + "RecaptchaChallengeValue", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                }
            }
        }

        internal string RecaptchaResponseValue
        {
            get
            {

                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaResponseValue"] != null)
                {
                    return Convert.ToString(HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaResponseValue"]);
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaResponseValue"] != null)
                {
                    HttpContext.Current.Cache[UserUniqueIdentifier + "RecaptchaResponseValue"] = value;
                }
                else
                {
                    HttpContext.Current.Cache.Add(UserUniqueIdentifier + "RecaptchaResponseValue", value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 1, 0), System.Web.Caching.CacheItemPriority.NotRemovable, null);
                }
            }
        }

        #endregion

        public RecaptchaControl()
        {
        }

        public void UpdateLog(Comment comment)
        {
            if (RecaptchaLoggingEnabled && !skipRecaptcha)
            {
                RecaptchaLogItem logItem = new RecaptchaLogItem();
                logItem.Response = RecaptchaResponseValue;
                logItem.Challenge = RecaptchaChallengeValue;
                logItem.CommentID = comment.Id;
                logItem.Enabled = RecaptchaEnabled;
                logItem.Necessary = RecaptchaNecessary;
                logItem.NumberOfAttempts = RecaptchaAttempts;
                logItem.TimeToComment = DateTime.Now.Subtract(PageLoadTime).TotalSeconds;
                logItem.TimeToSolveCapcha = DateTime.Now.Subtract(RecaptchaRenderTime).TotalSeconds;

                Stream s = (Stream)BlogService.LoadFromDataStore(BlogEngine.Core.DataStore.ExtensionType.Extension, "RecaptchaLog");
                List<RecaptchaLogItem> log = new List<RecaptchaLogItem>();
                if (s != null)
                {
                    System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<RecaptchaLogItem>));
                    log = (List<RecaptchaLogItem>)serializer.Deserialize(s);
                    s.Close();
                }
                log.Add(logItem);

                if (log.Count > MaxLogEntries)
                {
                    log.RemoveRange(0, log.Count - MaxLogEntries);
                }

                BlogService.SaveToDataStore(BlogEngine.Core.DataStore.ExtensionType.Extension, "RecaptchaLog", log);

                RecaptchaAttempts = 0;
                PageLoadTime = DateTime.Now;
                HttpContext.Current.Cache.Remove(UserUniqueIdentifier + "RecaptchaChallengeValue");
                HttpContext.Current.Cache.Remove(UserUniqueIdentifier + "RecaptchaResponseValue");

            }
        }

        #region Overriden Methods

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ExtensionSettings Settings = ExtensionManager.GetSettings("Recaptcha");
            publicKey = Settings.GetSingleValue("PublicKey");
            privateKey = Settings.GetSingleValue("PrivateKey");

            if (String.IsNullOrEmpty(Theme))
            {
                Theme = Settings.GetSingleValue("Theme");
            }

            if (RecaptchaEnabled && RecaptchaNecessary)
            {
                skipRecaptcha = false;
            }

            if (String.IsNullOrEmpty(publicKey) || String.IsNullOrEmpty(privateKey))
            {
                throw new ApplicationException("reCAPTCHA needs to be configured with a public & private key.");
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (!skipRecaptcha)
            {
                RenderContents(writer);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            if (RecaptchaLoggingEnabled)
            {
                if (!Page.IsCallback)
                {
                    PageLoadTime = DateTime.Now;
                    RecaptchaAttempts = 0;
                }
                RecaptchaRenderTime = DateTime.Now;
            }
            base.OnUnload(e);
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.AddAttribute("type", "text/javascript");
            output.AddAttribute("src", "http://api.recaptcha.net/js/recaptcha_ajax.js");
            output.AddAttribute("defer", "defer");
            output.RenderBeginTag("script");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, "spnCaptchaIncorrect");
            output.AddAttribute(HtmlTextWriterAttribute.Style, "color:Red;display:none;");
            output.RenderBeginTag("span");
            output.WriteLine("The captcha text was not valid. Please try again.");
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Id, "recaptcha_placeholder");
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.RenderEndTag();

            output.AddAttribute(HtmlTextWriterAttribute.Type, "text/javascript");
            output.RenderBeginTag(HtmlTextWriterTag.Script);
            output.WriteLine("function showRecaptcha() {");
            output.WriteLine("Recaptcha.create('" + publicKey + "', 'recaptcha_placeholder', {");
            output.WriteLine("theme: '{0}',", Theme);
            output.WriteLine("tabindex: {0}", TabIndex.ToString());
            output.WriteLine("})");
            output.WriteLine("}");

            output.WriteLine("var rc_oldonload = window.onload;");
            output.WriteLine("if (typeof window.onload != 'function') {");
            output.WriteLine("window.onload = showRecaptcha;");
            output.WriteLine("}");
            output.WriteLine("else {");
            output.WriteLine("window.onload = function() {");
            output.WriteLine("rc_oldonload();");
            output.WriteLine("showRecaptcha();");
            output.WriteLine("}");
            output.WriteLine("}");

            output.RenderEndTag();
        }

        #endregion

        #region IValidator Members

        public string ErrorMessage
        {
            get
            {
                if (errorMessage != null)
                {
                    return errorMessage;
                }
                return "The verification words are incorrect.";
            }
            set
            {
                errorMessage = value;
            }
        }

        public bool IsValid
        {
            get { return recaptchaResponse != null && recaptchaResponse.IsValid; }
            set { }
        }

        public void Validate()
        {
            if (skipRecaptcha)
            {
                recaptchaResponse = RecaptchaResponse.Valid;
            }
            else
            {
                RecaptchaValidator validator = new RecaptchaValidator();
                validator.PrivateKey = privateKey;
                validator.RemoteIP = Page.Request.UserHostAddress;
                if (String.IsNullOrEmpty(RecaptchaChallengeValue) && String.IsNullOrEmpty(RecaptchaResponseValue))
                {
                    validator.Challenge = Context.Request.Form[RECAPTCHA_CHALLENGE_FIELD];
                    validator.Response = Context.Request.Form[RECAPTCHA_RESPONSE_FIELD];
                }
                else
                {
                    validator.Challenge = RecaptchaChallengeValue;
                    validator.Response = RecaptchaResponseValue;
                }

                recaptchaResponse = validator.Validate();
            }
        }

        #endregion


        private string GenerateChallengeUrl(bool noScript)
        {
            StringBuilder urlBuilder = new StringBuilder();
            urlBuilder.Append(Context.Request.IsSecureConnection ? RECAPTCHA_SECURE_HOST : RECAPTCHA_HOST);
            urlBuilder.Append(noScript ? "/noscript?" : "/challenge?");
            urlBuilder.AppendFormat("k={0}", publicKey);
            if (recaptchaResponse != null && recaptchaResponse.ErrorCode != "")
            {
                urlBuilder.AppendFormat("&error={0}", recaptchaResponse.ErrorCode);
            }
            return urlBuilder.ToString();
        }

        public bool ValidateAsync(string response, string challenge)
        {
            if (RecaptchaLoggingEnabled)
            {
                RecaptchaAttempts++;
            }
            RecaptchaResponseValue = response;
            RecaptchaChallengeValue = challenge;
            Validate();
            return IsValid;
        }

    }
}