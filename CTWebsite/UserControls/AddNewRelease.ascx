<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AddNewRelease.ascx.cs" Inherits="CTWebsite.UserControls.AddNewRelease" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

     <asp:Table ID="tblAddNewModalTable" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Project</asp:TableHeaderCell>
            <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:Label ID="lblAddNewReleaseProject" runat="server" Text="" />
            </asp:TableCell>
        </asp:TableRow>

         <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Release Name</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtRelease" Text='<%# Bind("release") %>' runat="Server" Width="200px" AutoPostBack ="False" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Release Date</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtDateInsert" Text='<%# Bind("releaseDate") %>' runat="server" Width="200px" /> 
                <asp:ImageButton ID="imgCalDateSelectionInsert" ImageUrl="~/Images/calendar.png" class="calendarButton" Height="20" Width="20" runat="server" OnClick="imgCalDateSelectionInsert_click" BackColor="#2E4D7B" />
                <asp:Calendar ID="calDateInsert" OnSelectionChanged="calDateInsert_SelectionChanged" runat="server" Visible="False" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Release Description</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("releaseDescription") %>' runat="Server" TextMode="MultiLine" Rows="5" Width="200px" AutoPostBack ="False"/>
            </asp:TableCell>
         </asp:TableRow>


         <asp:TableRow ID="TableRow6" runat="server">
            <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button onClick="btnAddNewRelease" ID="btnOkay" AutoPostBack="true" runat="server" Text="Add Release"/>
            </asp:TableCell>
             <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
            </asp:TableCell>
         </asp:TableRow>
     </asp:Table>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>

