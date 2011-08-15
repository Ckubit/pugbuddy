<%@ page language="C#" autoeventwireup="true" inherits="login, App_Web_0zd1fnvt" title="Sign in" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphBody" Runat="Server">
<asp:Login ID="Login1" runat="server" class="loginbox" BorderPadding="25">
    <TextBoxStyle Font-Size="1em" width="150" />   
</asp:Login>

<div style="text-align:center">
  <asp:changepassword runat="server" id="changepassword1" visible="false" />
  <br /><br />
  <asp:loginstatus runat="server" id="lsLogout" visible="false" />
</div>
</asp:Content>