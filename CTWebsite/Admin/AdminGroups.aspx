<%@ Page Title="Admin Groups" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminGroups.aspx.cs" Inherits="CTWebsite.Admin.AdminGroups" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">


</asp:Content>
<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="adminSectionHeader">To add a group, select a project and click the + button.</div>
    <div class="adminSectionHeader">To edit or delete a group, select a project and click the appropriate button.</div>
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
     
<%--********** Group Dropdown **********--%>
<%--DO NOT DELETE THIS TEXTBOX, IT IS NEEDED FOR BINDING PURPOSES--%>
    <asp:TextBox 
    ID="txtSelectedGroup" 
    Text='' 
    RunAt="Server" 
    Visible="False" />
    
        &nbsp
        <asp:ImageButton ID="ibtnAdd" CommandName="AddGroup" runat="server" 
            ImageUrl="~/Images/add.png"  Height="20" Width="20" ToolTip="Add new group." 
            onclick="ibtnAdd_Click"></asp:ImageButton>

    </div>
    <div><br /></div>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
<div>

<asp:SqlDataSource ID="sqlAddUpdateGroup" runat="server"
    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
    SelectCommand="SELECT * FROM [GroupTests] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([groupTestAbbreviation] = @groupTestAbbreviation)) ORDER BY [groupTestAbbreviation]"
    OldValuesParameterFormatString="original_{0}">
    <SelectParameters>
        <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
        <asp:ControlParameter ControlID="txtSelectedGroup" Name="groupTestAbbreviation" PropertyName="Text" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>


<asp:FormView ID="fvGroups"
    DataSourceID="sqlAddUpdateGroup"
    DataKeyNames="projectAbbreviation"
    runat="server"
    DefaultMode="Insert" CssClass="gvGlobalGridview" Width="100%"
    Caption="Add New Group" OnDataBound="fvGroups_DataBound"
    OnItemDeleted="fvGroups_ItemDeleted" 
    OnItemInserted="fvGroups_ItemInserted"
    OnItemUpdated="fvGroups_ItemUpdated">

<%--**************************** Insert Item Template ****************************--%>

  <InsertItemTemplate>
<%--      <table width="100%" border="1">--%>
            <tr>
			    <th>Project</th>
			    <th>Group Abbreviation</th>
			    <th>Group Name</th>
                <th>Personal Group?</th>
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
                <td><asp:TextBox ID="txtGroupAbbreviation" Text='<%# Bind("groupTestAbbreviation") %>' RunAt="Server" CssClass="updateTextbox" /></td>
                <td><asp:TextBox ID="txtGroupTestName" Text='<%# Bind("groupTestName") %>' RunAt="Server" CssClass="updateTextbox" /></td>
                <td><asp:CheckBox ID="insertChkSelected" runat="server" Text="Personal Group?" /></td>
            </tr>
            <tr>
			    <th colspan="4">Group Description</th>
            </tr>
            <tr>
                <td colspan="4"><asp:TextBox ID="txtGroupTestDescription" Text='<%# Bind("groupTestDescription") %>' RunAt="Server" CssClass="updateTextbox" Width="100%" TextMode="MultiLine" Rows="5" /></td>
            </tr>
            <tr>
                <td colspan="4">
                   <asp:Button ID="InsertButton" runat="server" Text="Add" OnClick="InsertButton_Click" ></asp:Button> &nbsp 
                   <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                </td>
            </tr>
<%--    </table>--%>
</InsertItemTemplate>

<%--**************************** Edit Item Template ****************************--%>
<EditItemTemplate>
      <%--<table width="100%" border="1">--%>
            <tr>
			    <th>Project</th>
			    <th>Group Abbreviation</th>
			    <th>Group Name</th>
                <th>Personal Group?</th>
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
                        Width="100%" EnableTheming="True" Enabled="False">
                    </asp:DropDownList>

                    <asp:SqlDataSource ID="sqlProjects" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                        SelectCommand="SELECT * FROM [Projects] order by projectName">
                    </asp:SqlDataSource>
                </td>		    
                <td><asp:TextBox ID="txtGroupAbbreviation" Text='<%# Bind("groupTestAbbreviation") %>' RunAt="Server" CssClass="updateTextbox" ReadOnly="True"  Enabled="False" /></td>
                <td><asp:TextBox ID="txtGroupTestName" Text='<%# Bind("groupTestName") %>' RunAt="Server" CssClass="updateTextbox" /></td>
                <td><asp:CheckBox ID="editChkSelected" runat="server" Text="Personal Group?" /></td>
            </tr>
            <tr>
			    <th colspan="4">Group Description</th>
            </tr>
            <tr>
                <td colspan="4"><asp:TextBox ID="txtGroupTestDescription" Text='<%# Bind("groupTestDescription") %>' RunAt="Server" CssClass="updateTextbox" Width="100%" TextMode="MultiLine" Rows="5" /></td>
            </tr>
                <td colspan="4">
                   
                    <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click" Text="Update"></asp:Button>
                   &nbsp 
                   <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                </td>
            </tr>
<%--    </table>--%>
</EditItemTemplate>
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                No data available for the filters you have chosen.
            </div>
        </emptydatatemplate>

</asp:FormView>

</div>


<div>
            <asp:SqlDataSource ID="sqlGroups" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            
                    SelectCommand="SELECT * FROM [GroupTests] WHERE ([projectAbbreviation] = @projectAbbreviation)  and (personalGroupOwner is null or personalGroupOwner = @userName) ORDER BY groupTestName">
                <SelectParameters>
                    <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
                    <%--This has to be added via Page_Load--%>
                    <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
                </SelectParameters>
            </asp:SqlDataSource>

                <asp:GridView ID="gvGroupTests" runat="server" 
                    DataSourceID="sqlGroups" 
                    Tooltip="GroupTests" 
                    AutoGenerateColumns="False"  
                    GridLines="None"  
                    AllowPaging="True"  
                    CssClass="gvGlobalGridview"  
                    PagerStyle-CssClass="pgr"  
                    PageSize="100">
                    <Columns>
                         <asp:TemplateField HeaderText="Edit/Delete">      
                            <ItemTemplate> 
                                <asp:ImageButton ID="ibtnEdit" CommandName="EditGroup" runat="server" 
                                    ImageUrl="~/Images/edit.png" Height="15" Width="15" ToolTip="Edit this group." 
                                    CommandArgument='<%# Eval("groupTestAbbreviation") %>' onclick="ibtnEdit_Click"></asp:ImageButton>

                                <asp:ImageButton ID="ibtnRemove" CommandName="DeleteGroup" runat="server" 
                                    ImageUrl="~/Images/delete.png" 
                                    OnClientClick="return confirm('Are you certain you want to delete this group?');" 
                                    Height="15" Width="15" ToolTip="Delete this group." 
                                    CommandArgument='<%# Eval("groupTestAbbreviation") %>' 
                                    onclick="ibtnRemove_Click"></asp:ImageButton>
                            </ItemTemplate> 
                        </asp:TemplateField>
                        <asp:BoundField DataField="projectAbbreviation" 
                            HeaderText="Project" SortExpression="projectAbbreviation" HtmlEncode="False"/>
                        <asp:BoundField DataField="groupTestAbbreviation" 
                            HeaderText="Group Abbreviation" SortExpression="groupTestAbbreviation" HtmlEncode="False"/>
                        <asp:BoundField DataField="groupTestName" 
                            HeaderText="Group Name" SortExpression="groupTestName" HtmlEncode="False"/>
                        <asp:BoundField DataField="groupTestDescription" 
                            HeaderText="Description" SortExpression="groupTestDescription" HtmlEncode="False"/>
                        <asp:BoundField DataField="personalGroupOwner" 
                            HeaderText="Private Owner" SortExpression="personalGroupOwner" HtmlEncode="False"/>
                        
                    
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
                            No groups have been created for this project.
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
