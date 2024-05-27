<%@ Page Title="Admin Sprints" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminSprints.aspx.cs" Inherits="CTWebsite.Admin.AdminSprints" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">


 
</asp:Content>
<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="adminSectionHeader">To add a sprint, select a project and click the + button.</div>
    <div class="adminSectionHeader">To edit or delete a sprint, select a project and click the appropriate button.</div>
    <div class="commands">
      <asp:DropDownList ID="ddlProjects" runat="server" 
        AutoPostBack="True" 
        AppendDataBoundItems="False" 
        DataSourceID="sqlProjects"
        DataTextField="projectName"
        DataValueField="projectAbbreviation" 
            onselectedindexchanged="ddlProjects_SelectedIndexChanged">
    </asp:DropDownList>

    <asp:SqlDataSource ID="sqlProjects" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT * FROM [Projects] order by projectName">
    </asp:SqlDataSource>
     
     <%--DO NOT DELETE THIS TEXTBOX, IT IS NEEDED FOR BINDING PURPOSES--%>
    <asp:TextBox 
    ID="txtSelectedSprint" 
    Text='' 
    RunAt="Server" 
    Visible="False" />
        &nbsp
        <asp:ImageButton ID="ibtnAdd" CommandName="AddSprint" runat="server" 
            ImageUrl="~/Images/add.png"  Height="20" Width="20" ToolTip="Add new sprint." 
            onclick="ibtnAdd_Click"></asp:ImageButton>

    </div>
    <div><br /></div>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
<div>
<asp:SqlDataSource ID="sqlAddUpdateSprint" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"   
        SelectCommand="SELECT * FROM [Sprints] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([sprint] = @sprint)) ORDER BY [sprint]"
        OldValuesParameterFormatString="original_{0}" >
    <SelectParameters>
        <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
        <asp:ControlParameter ControlID="txtSelectedSprint" Name="sprint" PropertyName="Text" Type="String" />
    </SelectParameters>
    <InsertParameters>
        <asp:Parameter Name="projectAbbreviation" Type="String" />
        <asp:Parameter Name="sprint" Type="String" />
        <asp:Parameter Name="sprintDescription" Type="String" />
        <asp:Parameter Name="sprintStartDate" Type="DateTime" />
        <asp:Parameter Name="sprintEndDate" Type="DateTime" />
    </InsertParameters>
    </asp:SqlDataSource>

<asp:FormView ID="fvSprints" 
        DataSourceID="sqlAddUpdateSprint" 
        DataKeyNames="projectAbbreviation" 
        RunAt="server" 
        DefaultMode="Insert" CssClass="gvGlobalGridview" Width="100%" 
        Caption="Add New Sprint" ondatabound="fvSprints_DataBound" 
        onitemdeleted="fvSprints_ItemDeleted" 
        oniteminserted="fvSprints_ItemInserted" 
        onitemupdated="fvSprints_ItemUpdated" 
        oniteminserting="fvSprints_ItemInserting" 
        onitemupdating="fvSprints_ItemUpdating">

<%--**************************** Insert Item Template ****************************--%>

  <InsertItemTemplate>
      <table width="100%" border="1">

            <tr>
			    <th>Project</th>
			    <th>Sprint Title</th>
            </tr>
		    <tr>
			    <td>
                    <asp:DropDownList ID="ddlProject" runat="server" 
                        AutoPostBack="False" 
                        AppendDataBoundItems="False" 
                        DataSourceID="sqlProjects"
                        DataTextField="projectName"
                        DataValueField="projectAbbreviation"
                        SelectedValue='<%# Bind("projectAbbreviation") %>'
                        Width="100%">
                    </asp:DropDownList>

                    <asp:SqlDataSource ID="sqlProjects" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                        SelectCommand="SELECT * FROM [Projects] order by projectName">
                    </asp:SqlDataSource>
                </td>		    
                <td><asp:TextBox ID="txtSprint" Text='<%# Bind("sprint") %>' RunAt="Server" CssClass="updateTextbox" /></td>
            
            <tr>
			    <th>Start Date</th>
			    <th>End Date</th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox id="txtStartDateInsert" Text='<%# Bind("sprintStartDate") %>' Runat="server" />
                    <asp:ImageButton id="imgStartDateSelectionInsert"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgStartDateSelectionInsert_click" />
                        <asp:Calendar id="calStartDateInsert" OnSelectionChanged="calStartDateInsert_SelectionChanged" Runat="server" Visible="False" />
                </td>
                <td>
                    <asp:TextBox id="txtEndDateInsert" Text='<%# Bind("sprintEndDate") %>' Runat="server" />
                    <asp:ImageButton id="imgEndDateSelectionInsert"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgEndDateSelectionInsert_click" />
                        <asp:Calendar id="calEndDateInsert" OnSelectionChanged="calEndDateInsert_SelectionChanged" Runat="server" Visible="False" />
                </td>
            </tr>

		    <tr>
			    <th colspan="3">Sprint Description</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("sprintDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
                <td colspan="4">
                   <asp:Button ID="InsertButton" runat="server" CausesValidation="True" OnClick="InsertButton_Click" Text="Add"></asp:Button> &nbsp <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                </td>
            </tr>
    </table>
</InsertItemTemplate>

<EditItemTemplate>
       <table width="100%" border="1">

            <tr>
			    <th>Project</th>
			    <th>Sprint Title</th>
            </tr>
		    <tr>
			    <td>
                    <asp:DropDownList ID="ddlProject" runat="server" 
                        AutoPostBack="False" 
                        AppendDataBoundItems="False" 
                        DataSourceID="sqlProjects"
                        DataTextField="projectName"
                        DataValueField="projectAbbreviation"
                        SelectedValue='<%# Bind("projectAbbreviation") %>'
                        Width="100%" Enabled="False">
                    </asp:DropDownList>

                    <asp:SqlDataSource ID="sqlProjects" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                        SelectCommand="SELECT * FROM [Projects] order by projectName">
                    </asp:SqlDataSource>
                </td>		    
                <td><asp:TextBox ID="txtSprint" Text='<%# Bind("sprint") %>' RunAt="Server" CssClass="updateTextbox" ReadOnly="True"  Enabled="False" /></td>
            
            <tr>
			    <th>Start Date</th>
			    <th>End Date</th>
            </tr>
            <tr>
                <td>
                    <asp:TextBox id="txtStartDateEdit" Text='<%# Bind("sprintStartDate") %>' Runat="server" />
                    <asp:ImageButton id="imgStartDateSelectionEdit"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgStartDateSelectionEdit_click" />
                        <asp:Calendar id="calStartDateEdit" OnSelectionChanged="calStartDateEdit_SelectionChanged" Runat="server" Visible="False" />
                </td>
                <td>
                    <asp:TextBox id="txtEndDateEdit" Text='<%# Bind("sprintEndDate") %>' Runat="server" />
                    <asp:ImageButton id="imgEndDateSelectionEdit"  ImageUrl="~/Images/calendar.png" class="calendarButton" height="20" width="20" runat="server" OnClick="imgEndDateSelectionEdit_click" />
                        <asp:Calendar id="calEndDateEdit" OnSelectionChanged="calEndDateEdit_SelectionChanged" Runat="server" Visible="False" />
                </td>
            </tr>

		    <tr>
			    <th colspan="3">Sprint Description</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("sprintDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
                <td colspan="4">
                   <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="EditButton_Click" Text="Update"></asp:Button> &nbsp <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                </td>
            </tr>
    </table>
</EditItemTemplate>
</asp:FormView>
</div>

<div>
        <asp:SqlDataSource ID="sqlSprintList" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT * FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprintEndDate] DESC">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" 
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

                <asp:GridView ID="gvSprintList" runat="server" 
                    DataSourceID="sqlSprintList" 
                    Tooltip="Sprints" 
                    AutoGenerateColumns="False"  
                    GridLines="None"  
                    AllowPaging="True"  
                    CssClass="gvGlobalGridview"  
                    PagerStyle-CssClass="pgr"  
                    AlternatingRowStyle-CssClass="alt" PageSize="50">
                    <Columns>
                        <asp:TemplateField HeaderText="Edit/Delete">      
                            <ItemTemplate> 
                                <asp:ImageButton ID="ibtnEdit" CommandName="EditSprint" runat="server" 
                                    ImageUrl="~/Images/edit.png" Height="15" Width="15" ToolTip="Edit this sprint." 
                                    CommandArgument='<%# Eval("sprint") %>' onclick="ibtnEdit_Click"></asp:ImageButton>

                                <asp:ImageButton ID="ibtnRemove" CommandName="DeleteSprint" runat="server" 
                                    ImageUrl="~/Images/delete.png" 
                                    OnClientClick="return confirm('Are you certain you want to delete this sprint?');" 
                                    Height="15" Width="15" ToolTip="Delete this sprint." 
                                    CommandArgument='<%# Eval("sprint") %>' 
                                    onclick="ibtnRemove_Click"></asp:ImageButton>
                            </ItemTemplate> 
                        </asp:TemplateField>
                        <asp:BoundField DataField="projectAbbreviation" 
                            HeaderText="Project" SortExpression="projectAbbreviation" HtmlEncode="False"/>
                        <asp:BoundField DataField="sprint" 
                            HeaderText="Sprint" SortExpression="sprint" HtmlEncode="False"/>
                        <asp:BoundField DataField="sprintDescription" 
                            HeaderText="Description" SortExpression="sprintDescription" HtmlEncode="False"/>
                        <asp:BoundField DataField="sprintStartDate" HeaderText="Start Date" SortExpression="sprintStartDate" DataFormatString="{0:d}" HtmlEncode="false" />
                        <asp:BoundField DataField="sprintEndDate" HeaderText="End Date" SortExpression="sprintEndDate" DataFormatString="{0:d}" HtmlEncode="False"/>
                    
                    </Columns>
                    <emptydatarowstyle CssClass="emptydata"/>
                    <emptydatatemplate>
                        <div class="NoData">
                            <asp:image id="NoDataImage"   
                            imageurl="~/Images/NoData.png"
                            alternatetext="No Data Image" 
                            runat="server"
                            Height="130" Width="139" />
                            <br />
                            No sprints have been created for this project.
                        </div>
                    </emptydatatemplate>
                    <PagerSettings Mode="NumericFirstLast"  position="TopAndBottom"/>
                    <PagerStyle CssClass="pgr"></PagerStyle>
                </asp:GridView>
</div>


</asp:Content>
<%--**************************** RIGHT RAIL CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">

</asp:Content>
