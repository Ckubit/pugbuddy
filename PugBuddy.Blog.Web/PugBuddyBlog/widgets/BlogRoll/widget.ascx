<%@ Import namespace="BlogEngine.Core"%>
<%@ control language="C#" autoeventwireup="true" inherits="widgets_BlogRoll_widget, App_Web_4beokoul" %>
 <blog:Blogroll ID="Blogroll1" runat="server" />
  <a href="<%=Utils.AbsoluteWebRoot %>opml.axd" style="display:block;text-align:right" title="Download OPML file" >Download OPML file <img src="<%=Utils.AbsoluteWebRoot %>pics/opml.png" alt="OPML" /></a>