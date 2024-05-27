<%@ Page Title="" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="ReportsMain.aspx.cs" Inherits="OSTMSWebsite.Reports.ReportsMain" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<asp:Panel ID="pnlReports" runat="server" GroupingText="Reports" CssClass="dashboardPanel">
    <div class="comingSoon">
        <asp:Image ID="imgComingSoon" runat="server" 
            ImageUrl="~/Images/comingsoon2.jpg" />
    </div>
</asp:Panel>
</asp:Content>
