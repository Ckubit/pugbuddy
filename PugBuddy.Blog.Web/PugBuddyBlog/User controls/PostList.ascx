<%@ control language="C#" autoeventwireup="true" enableviewstate="false" inherits="User_controls_PostList, App_Web_xw4d4qep" %>
<div runat="server" id="posts" class="posts" />

<div id="postPaging">
  <a runat="server" ID="hlPrev" style="float:left">&lt;&lt; <%=Resources.labels.previousPosts %></a>
  <a runat="server" ID="hlNext" style="float:right"><%=Resources.labels.nextPosts %> &gt;&gt;</a>
</div>