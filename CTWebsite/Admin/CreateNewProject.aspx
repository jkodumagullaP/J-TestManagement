<%@ Page Title="Create New Project" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="CreateNewProject.aspx.cs" Inherits="CTWebsite.Admin.CreateNewProject" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Panel ID="pnlCreateNewProject" runat="server" GroupingText="Create New Project" CssClass="dashboardPanel">
    <table>
        <tr>
            <td colspan="2">
                    <div class="adminSectionHeader">Project Names and Abbreviations are designed to match those in defect management systems for future integration. The Project Abbreviation should match the Key for a given project.</div> 

            </td>
        </tr>
        <tr>
            <td colspan="2"><asp:label id="MessageLabel" forecolor="Red" runat="server" Font-Bold="True" /></td>
        </tr>
        <tr>
            <td>Project Name</td>
            <td><asp:TextBox ID="txtProjectName" runat="server" Width="250"></asp:TextBox></td>
        </tr>
        <tr>
            <td></td>
            <td class="subscript">Max Characters: 100</td>
        </tr>
        <tr>
            <td>Project Abbreviation</td>
            <td>
                <asp:TextBox ID="txtProjectAbbreviation" runat="server" Width="250"></asp:TextBox>
                
            </td>
        </tr>
        <tr>
            <td></td>
            <td class="subscript">Max Characters: 20</td>
        </tr>
        <tr>
        <td colspan="2">
            <asp:Button ID="btnCreateNewProject" runat="server" Text="Create New Project"
                AutoPostBack="True" CausesValidation="True" onclick="btnCreateNewProject_Click" />
        </td>
        </tr>
    </table>
    </asp:Panel>

</asp:Content>


