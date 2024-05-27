<%@ Page Title="Add New Screenshot" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AddNewScreenshot.aspx.cs" Inherits="CTWebsite.FunctionalTesting.AddNewScreenshot" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<asp:Label ID="lblStatus" runat="server" ForeColor="Red" Text=""></asp:Label>
<asp:Panel ID="pnlAddNewScreenshot" runat="server" GroupingText="Upload Screenshot" CssClass="dashboardPanel">
    <table>
		<tr>
            <td>File</td>
            <td><asp:FileUpload ID="imgUploadScreenshot" runat="server"/></td>
        </tr>
        <tr>
            <td>Description</td>
            <td>
                <asp:TextBox ID="txtUploadImageDescription" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2"><asp:Button ID="btnUploadScreenshot" runat="server" Text="Upload Screenshot" 
                        OnClick="btnUploadScreenshot_Click" Width="200" /></td>
        </tr>
    </table>
</asp:Panel>
<asp:Panel ID="pnlAddScreenshotURL" runat="server" GroupingText="Add Screenshot URL" CssClass="dashboardPanel">
    <table>
        <tr>
            <td>Image URL</td>
            <td>
                <asp:TextBox ID="txtImageUrl" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Description</td>
            <td>
                <asp:TextBox ID="txtImageUrlDescription" runat="server" Width="400"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="2"><asp:Button ID="btnAddImageUrl" runat="server" Text="Add Screenshot URL"
                AutoPostBack="True" onclick="btnAddImageUrl_Click" Width="200" /></td>
        </tr>
    </table>
</asp:Panel>

</asp:Content>


