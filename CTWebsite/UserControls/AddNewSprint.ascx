<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddNewSprint.ascx.cs" Inherits="CTWebsite.UserControls.AddNewSprint" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxtoolkit" %>

     <asp:Table ID="tblAddNewModalTable" runat="server">
        <asp:TableRow ID="TableRow1" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Project</asp:TableHeaderCell>
            <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:Label ID="lblAddNewSprintProject" runat="server" Text="" />
            </asp:TableCell>
        </asp:TableRow>

         <asp:TableRow ID="TableRow2" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Sprint Name</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtSprint" Text='<%# Bind("sprint") %>' runat="Server" Width="200px" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow3" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Sprint Start Date</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                    <asp:TextBox id="txtStartDateInsert" Text='<%# Bind("sprintStartDate") %>' Runat="server" />
                    <asp:ImageButton id="imgStartDateSelectionInsert"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgStartDateSelectionInsert_click" />
                        <asp:Calendar id="calStartDateInsert" OnSelectionChanged="calStartDateInsert_SelectionChanged" Runat="server" Visible="False" />
            </asp:TableCell>
         </asp:TableRow>


         <asp:TableRow ID="TableRow4" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Release Description</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox id="txtEndDateInsert" Text='<%# Bind("sprintEndDate") %>' Runat="server" />
                <asp:ImageButton id="imgEndDateSelectionInsert"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgEndDateSelectionInsert_click" />
                    <asp:Calendar id="calEndDateInsert" OnSelectionChanged="calEndDateInsert_SelectionChanged" Runat="server" Visible="False" />
            </asp:TableCell>
         </asp:TableRow>

         <asp:TableRow ID="TableRow5" runat="server">
            <asp:TableHeaderCell CssClass="thAddNewModalHeaderRow">Release Description</asp:TableHeaderCell>
             <asp:TableCell CssClass="tcAddNewModalFormRow">
                <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("sprintDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" />                        </asp:TableCell>
         </asp:TableRow>


         <asp:TableRow ID="TableRow6" runat="server" BackColor="#2E4D7B">
            <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button onClick="btnAddNewSprint" ID="btnOkay" AutoPostBack="true" runat="server" Text="Add Sprint"/>
            </asp:TableCell>
             <asp:TableCell CssClass="tcAddNewModalFormButtons">
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
            </asp:TableCell>
         </asp:TableRow>
     </asp:Table>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
