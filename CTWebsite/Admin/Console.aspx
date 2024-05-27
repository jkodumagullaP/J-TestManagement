<%@ Page Title="Create New Project" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Console.aspx.cs" Inherits="CTWebsite.Admin.Console" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">


        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminTestCases" runat="server" NavigateUrl="~/Admin/AdminTestCases.aspx">Administer Test Cases</asp:HyperLink>
        </div>
        <div><hr /></div>
        
        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminReleases" runat="server" NavigateUrl="~/Admin/AdminReleases.aspx">Administer Releases</asp:HyperLink>
        </div>
        <div><hr /></div>

        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminSprints" runat="server" NavigateUrl="~/Admin/AdminSprints.aspx">Administer Sprints</asp:HyperLink>
        </div>
        <div><hr /></div>

        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminGroups" runat="server" NavigateUrl="~/Admin/AdminGroups.aspx">Administer Groups</asp:HyperLink>
        </div>
        <div><hr /></div>

        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminProjects" runat="server" NavigateUrl="~/Admin/AdminProjects.aspx">Administer Projects</asp:HyperLink>
        </div>
        <div><hr /></div>

        <div class="adminMenu">
            <asp:HyperLink ID="hlAdminSelenium" runat="server" NavigateUrl="~/Admin/AdminSelenium.aspx">Administer Selenium</asp:HyperLink>
        </div> 
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Label ID="lblMessage" runat="server" Text="Refresh the page to update the console view."></asp:Label>
<asp:Panel ID="pnlConsole" runat="server" GroupingText="ServerName Console" CssClass="dashboardPanel">
    <asp:TextBox ID="txtServerConsole" Text='' RunAt="Server" CssClass="consoleTextbox" Width="100%" TextMode="MultiLine" Rows="35" />
    </asp:Panel>

</asp:Content>


