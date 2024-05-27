<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddNewGroup.ascx.cs" Inherits="CTWebsite.UserControls.AddNewGroup" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

     <asp:Table ID="tblAddNewModalTable" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Project</asp:TableHeaderCell>
            <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:Label ID="lblAddNewGroupProject" runat="server" Text="" />
            </asp:TableCell>
        </asp:TableRow>

         <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Group Abbreviation</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtGroupTestAbbreviation" runat="server" Width="200" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Group Name</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtGroupTestName" runat="server" Width="200" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Group Description</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtGroupTestDescription" runat="server" Width="200" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Personal Group?</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:CheckBox ID="chkPersonalGroup" runat="server" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableFooterRow ID="tblFooterRow" runat="server" BackColor="#5D7FC0">
            <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button onClick="btnAddNewGroup" ID="btnOkay" AutoPostBack="true" runat="server" Text="Add Group"/>
            </asp:TableCell>
             <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
            </asp:TableCell>
         </asp:TableFooterRow>
     </asp:Table>
