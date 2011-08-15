<%@ control language="C#" autoeventwireup="true" inherits="widgets_Calendar_widget, App_Web_ezkpt4je" %>
<%@ Import Namespace="BlogEngine.Core" %>

<div style="text-align:center">
  <blog:PostCalendar ID="PostCalendar1" runat="Server" NextMonthText=">>" DayNameFormat="FirstTwoLetters" FirstDayOfWeek="monday" PrevMonthText="<<" CssClass="calendar" BorderWidth="0" WeekendDayStyle-CssClass="weekend" OtherMonthDayStyle-CssClass="other" UseAccessibleHeader="true" EnableViewState="false" />
  <br />
  <a href="<%=Utils.AbsoluteWebRoot %>calendar/default.aspx">View posts in large calendar</a>
</div>