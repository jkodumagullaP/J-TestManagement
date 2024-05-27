<%@ Page Title="Profile" Language="C#" MasterPageFile="~/ThreeColumn.master" AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs" Inherits="CTWebsite.Account.Profile" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>

<%@ Register Src="~/UserControls/AddNewGroup.ascx" TagPrefix="uc1" TagName="AddNewGroup" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">

<%--********** Project Dropdown **********--%>
<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblProjectDropdownTitle" runat="server" Text="Project" /></div>
    </div>
    <p>
        <asp:DropDownList ID="ddlProjects" runat="server"  
            DataSourceID="sqlProjects" 
            DataTextField="projectName" 
            DataValueField="projectAbbreviation" 
            AppendDataBoundItems="False" 
            onselectedindexchanged="ddlProject_SelectedIndexChanged"
            AutoPostBack="True"
            CssClass="leftRailDropdown">
        </asp:DropDownList>
            
        <asp:SqlDataSource ID="sqlProjects" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [projectName] FROM [Projects] order by projectName">
        </asp:SqlDataSource>
    </p>
</div>

<%--********** Environments Dropdown **********--%>

<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblEnvironmentDropdownTitle" runat="server" Text="Environment" /></div>
    </div>
    <p>
        <asp:DropDownList ID="ddlEnvironment" runat="server" 
            DataSourceID="sqlEnvironments" 
            DataTextField="environment" 
            DataValueField="environment" 
            AppendDataBoundItems="False" 
            AutoPostBack="True"
            onselectedindexchanged="ddlEnvironment_SelectedIndexChanged" 
            CssClass="leftRailDropdown">
        </asp:DropDownList>
            
        <asp:SqlDataSource ID="sqlEnvironments" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT ProjectEnvironmentInfo.environment, Environments.sortOrder FROM ProjectEnvironmentInfo JOIN Environments ON ProjectEnvironmentInfo.environment = Environments.environment WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY sortOrder">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
    </p>
</div>
<%--********** Groups **********--%>
<div class="box">
	<div class="boxHeaderContainer">
		<div class="left"><asp:label Id="Label4" runat="server" Text=" Personal Groups" /></div>
		<div class="right"><asp:ImageButton ID="btnAddNewGroup" runat="server" Tooltip="Add New Group" Width="30" Height="30" ImageUrl="~/Images/add.png" /></div>
	</div>
	<p>
		<asp:SqlDataSource ID="sqlPersonalTestCaseGroups" runat="server" 
			ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
			SelectCommand="SELECT * FROM GroupTests
							WHERE (personalGroupOwner = @userName)
							ORDER BY groupTestName ASC">
		</asp:SqlDataSource>

		<asp:ListView ID="lvPersonalTestCaseGroups" runat="server" 
			DataSourceID="sqlPersonalTestCaseGroups" onitemcommand="lvPersonalTestCaseGroups_ItemCommand">
		    <emptydatatemplate>
			    <div class="NoDataSidebar">
				    <asp:image id="NoDataImage"   
				    imageurl="~/Images/NoData.png"
				    alternatetext="No Data Image" 
				    runat="server"
				    Height="64" Width="64" />
				    <br />
				    You do not have any personal groups.
			    </div>
		    </emptydatatemplate>
			<ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
			<ItemTemplate>
			    <div class="ProfileCTDetailsBoxText">
				    <asp:ImageButton ID="ibtnRemove" CommandName="RemovePersonalGroup" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this group?');" Height="14" Width="14" ToolTip="Delete this test case from this group." CommandArgument='<%#Eval("projectAbbreviation") + "|" + Eval("groupTestAbbreviation") %>'></asp:ImageButton>
				    <a href="../FunctionalTesting/TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&group=<%# Eval("groupTestAbbreviation") %>">
					    <asp:Label ID="lblGroup" runat="server" Text='<%# Eval("groupTestName") %>' /></a>
				    <br />
				    <asp:Label ID="lblGroupDescription" runat="server" Text='<%# Eval("groupTestDescription") %>'></asp:Label>
			    </div>
		    </ItemTemplate>
		    <LayoutTemplate>
			    <div ID="itemPlaceholderContainer" runat="server" style="">
				    <div runat="server" id="itemPlaceholder" />
			    </div>
			</LayoutTemplate>
		</asp:ListView>
	</p>
</div>

<ajaxControlToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server"></ajaxControlToolkit:ToolkitScriptManager>
<asp:panel id="Panel1" runat="server" AutoPostBack="true">
    <div class="AddNewModal">
	    <div class="PopupHeader" id="PopupHeader1">Add New Group</div>
	    <div class="PopupBody">
		    <uc1:AddNewGroup ID="AddNewGroup" runat="server"/>
	    </div>
    </div>
</asp:panel>    
	
<ajaxToolkit:modalpopupextender id="ModalPopupExtender1" runat="server" 
    targetcontrolid="btnAddNewGroup" popupcontrolid="Panel1" 
    popupdraghandlecontrolid="PopupHeader1" drag="true" 
    backgroundcssclass="ModalPopupBG">
</ajaxToolkit:modalpopupextender>

</asp:Content>


<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <%--**************************** PANEL: Profile Data ****************************--%>
    <asp:Panel ID="pnlProfileData" runat="server" GroupingText="Profile Data" CssClass="dashboardPanel">
    
    <asp:SqlDataSource ID="sqlProfileData" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"   
        SelectCommand="SELECT UserName, userFirstName, userLastName, Email, userSupervisorFirstName, userSupervisorLastName, userSupervisorEmail, defaultProjectAbbreviation, projectName, PasswordQuestion, PasswordAnswer
                      FROM [aspnet_Users]
                      JOIN [aspnet_Membership] 
                      ON aspnet_Users.UserId = aspnet_Membership.UserId
                      JOIN dbo.UserProfiles
                      ON aspnet_Users.UserId = UserProfiles.UserId
                      JOIN Projects
                      ON UserProfiles.defaultProjectAbbreviation = Projects.projectAbbreviation
                      WHERE UserName = @username"
        UpdateCommand="" >
    <SelectParameters>
        <%--This has to be added via Page_Load--%>
        <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
    </SelectParameters>
</asp:SqlDataSource>


<asp:FormView ID="fvProfileData" DataSourceID="sqlProfileData" 
        RunAt="server" 
        CssClass="fvProfileData" onitemcommand="fvProfileData_ItemCommand" 
            ondatabinding="fvProfileData_DataBinding">

<%--**************************** Item Template ****************************--%>

  <ItemTemplate>
      <table border="1" width="100%">
            <tr>
			    <th>Username</th>
			    <td><asp:Label ID="lblUsername" runat="server" Text='<%# Bind("UserName") %>' /></td>
                <td rowspan="9" class="ibtnEditProfile"><asp:ImageButton ID="ibtnEditPersonalData" CommandName="Edit" runat="server" ImageUrl="~/Images/EditUser.png" Height="64" Width="64" ToolTip="Edit My Profile." AlternateText="Edit My Profile" /></td>
            </tr>
            <tr>
			    <th>First Name</th>
			    <td><asp:Label ID="lblFirstName" runat="server" Text='<%# Bind("userFirstName") %>' /></td>
            </tr>
            <tr>
			    <th>Last Name</th>
			    <td><asp:Label ID="lblLastName" runat="server" Text='<%# Bind("userLastName") %>' /></td>
            </tr>
            <tr>
			    <th>Email</th>
			    <td><asp:Label ID="lblEmail" runat="server" Text='<%# Bind("email") %>' /></td>
            </tr>
            <tr>
			    <th>Supervisor's First Name</th>
			    <td><asp:Label ID="lblSupervisorFirstName" runat="server" Text='<%# Bind("userSupervisorFirstName") %>' /></td>
            </tr>
            <tr>
			    <th>Supervisor's Last Name</th>
			    <td><asp:Label ID="lblSupervisorLastName" runat="server" Text='<%# Bind("userSupervisorLastName") %>' /></td>
            </tr>
            <tr>
			    <th>Supervisor's Email</th>
			    <td><asp:Label ID="lblSupervisorEmail" runat="server" Text='<%# Bind("userSupervisorEmail") %>' /></td>
            </tr>
            <tr>
			    <th>Default Project</th>
			    <td><asp:Label ID="lblDefaultProject" runat="server" Text='<%# Bind("projectName") %>' /></td>
            </tr>
    </table>

  </ItemTemplate>

  <%--**************************** Edit Item Template ****************************--%>
    <EditItemTemplate>
      
      <table border="1" width="100%">
            <tr>
			    <th><asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="lblUsername">Username:</asp:Label></th>
			    <td><asp:Label ID="lblUsername" runat="server" Text='<%# Bind("UserName") %>' /></td>
            </tr>
                    
          
          <tr>
			    <th>
                    <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="txtLastName">First Name:</asp:Label>
                </th>
			    
              <td>
                    
                  <asp:TextBox ID="txtFirstName" Text='<%# Bind("userFirstName") %>' RunAt="Server" Width="100%" CssClass="updateTextbox" />
                 <asp:RequiredFieldValidator ID="FirstNameValidator"
                     Display="Dynamic" 
                     ValidationGroup="EditProfileFields"
                     CSSClass="Error"
                     ControlToValidate="txtFirstName"
                     ErrorMessage="First Name is Required"
                     InitialValue=""
                     Text=""
                     runat="server"/>
              </td>
            </tr>
            
          
          <tr>
			    <th>
                    <asp:Label ID="LastNameLabel" runat="server" AssociatedControlID="txtLastName">Last Name:</asp:Label>
                </th>
			    
              <td>
                    <asp:TextBox ID="txtLastName" Text='<%# Bind("userLastName") %>' RunAt="Server" Width="100%" CssClass="updateTextbox" />
                    <asp:RequiredFieldValidator ID="LastNameValidator"
                     Display="Dynamic" 
                     ValidationGroup="EditProfileFields"
                     CSSClass="Error"
                     ControlToValidate="txtLastName"
                     ErrorMessage="Last Name is Required"
                     InitialValue=""
                     Text=""
                     runat="server"/>

                </td>
            </tr>


            <tr>
			    <th><asp:Label ID="EmailLabel" runat="server" AssociatedControlID="lblEmail">Email:</asp:Label></th>
			    <td><asp:Label ID="lblEmail" Text='<%# Bind("email") %>' RunAt="Server" /></td>

            </tr>
            <tr>
			    <th><asp:Label ID="SupervisorsFirstNameLabel" runat="server" AssociatedControlID="txtSupervisorFirstName">Supervisor's First Name:</asp:Label></th>
			    <td><asp:TextBox ID="txtSupervisorFirstName" Text='<%# Bind("userSupervisorFirstName") %>' RunAt="Server" Width="100%" CssClass="updateTextbox" /></td>
            </tr>
            <tr>
			    <th><asp:Label ID="SupervisorsLastNameLabel" runat="server" AssociatedControlID="txtSupervisorLastName">Supervisor's Last Name:</asp:Label></th>
			    <td><asp:TextBox ID="txtSupervisorLastName" Text='<%# Bind("userSupervisorLastName") %>' RunAt="Server" Width="100%" CssClass="updateTextbox" /></td>
            </tr>
            <tr>
			    <th><asp:Label ID="SupervisorsEmailLabel" runat="server" AssociatedControlID="txtSupervisorEmail">Supervisor's Last Name:</asp:Label></th>
			    <td><asp:TextBox ID="txtSupervisorEmail" Text='<%# Bind("userSupervisorEmail") %>' RunAt="Server" Width="100%" CssClass="updateTextbox" /></td>
            </tr>
            <tr>
			    <th>
                    <asp:Label ID="DefaultProjectLabel" runat="server" AssociatedControlID="ddlDefaultProjects">Default Project</asp:Label>
                </th>
			    <td>
                    <asp:SqlDataSource ID="sqlDefaultProject" runat="server" 
                        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                        SelectCommand="SELECT * FROM [Projects] order by projectName">
                    </asp:SqlDataSource>

                    <asp:DropDownList ID="ddlDefaultProjects" runat="server" 
                        AutoPostBack="False" 
                        AppendDataBoundItems="False" 
                        DataSourceID="sqlDefaultProject"
                        DataTextField="projectName"
                        DataValueField="projectAbbreviation"
                        SelectedValue='<%# Bind("defaultProjectAbbreviation") %>'>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                   <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CommandName="EditPersonalData" 
                        ValidationGroup="EditProfileFields" Text="Update"></asp:Button> 
                    <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:Button>
                </td>
            </tr>
            <tr>
                <td>
                </td>
            </tr>
    </table>
    <asp:ValidationSummary ID="ValidationSummary1"
        CSSClass="Error"
        HeaderText="You must enter a value in the following fields:"
        DisplayMode="List"
        EnableClientScript="true"
        runat="server"/>
  </EditItemTemplate>
</asp:FormView>
</asp:Panel>

<div><hr /></div>

<%--**************************** PANEL: METRICS BY USER ****************************--%>
<asp:Panel ID="pnlMetricsByUser" runat="server" GroupingText="Personal Metrics for Current Month" 
        CssClass="fvProfileData">
    <table>
        <tr>
            <th>Manual Cases Written:</th>
            <td><asp:Label ID="userManualCasesWritten" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <th>Automated Cases Written:</th>
            <td><asp:Label ID="userAutomatedCasesWritten" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <th>Manual Cases Executed:</th>
            <td><asp:Label ID="userManualCasesExecuted" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <th>Automated Cases Executed:</th>
            <td><asp:Label ID="userAutomatedCasesExecuted" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <th>Manual Defects Discovered:</th>
            <td><asp:Label ID="userManualDefectsDiscovered" runat="server" Text="Label"></asp:Label></td>
        </tr>
        <tr>
            <th>Automated Defects Discovered:</th>
            <td><asp:Label ID="userAutomatedDefectsDiscovered" runat="server" Text="Label"></asp:Label></td>
        </tr>
    </table>
</asp:Panel>
<div><hr /></div>
    <%--********** Gridview **********--%>
    <div><h1>Personal Task List</h1></div>
    <div>Test Cases that have been tested and passed within the past 7 days will be highlighted in green.</div>
    <asp:SqlDataSource ID="sqlTaskList" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="">
    <SelectParameters>
        <%--This has to be added via Page_Load--%>
        <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
    </SelectParameters>
    </asp:SqlDataSource>

    <!-- DO NOT CHANGE THE COLUMN ORDER -->
    <asp:GridView ID="gvTaskList" runat="server" 
        DataKeyNames="projectAbbreviation,testCaseId" 
        DataSourceID="sqlTaskList"  
        OnRowDataBound="gvTaskList_RowDataBound"
        onrowcommand="gvTaskList_RowCommand"
        Tooltip="" 
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview"
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvTaskList_PageIndexChanged" PageSize="50" 
        Caption="Task List">
        
        <Columns>
<%--            <asp:TemplateField HeaderText="Delete">      
                <ItemTemplate> 
                    <asp:CheckBox ID="chkSelected" OnCheckedChanged="chkSelected_CheckedChanged"  runat="server" /> 
                </ItemTemplate> 
            </asp:TemplateField> --%>

            
            
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="Project" ReadOnly="True" 
                SortExpression="projectAbbreviation" />
            
            <asp:BoundField DataField="testCaseId" HeaderText="ID" ReadOnly="True" 
                SortExpression="testCaseId" ItemStyle-HorizontalAlign="Center">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            
            <asp:BoundField HtmlEncode="False" DataField="testCaseDescription" 
                HeaderText="Description" SortExpression="testCaseDescription" 
                ItemStyle-HorizontalAlign="Center">
                <ItemStyle HorizontalAlign="Center"></ItemStyle>
            </asp:BoundField>
            
            <asp:BoundField DataField="testDate" HeaderText="Last Test Date" 
                SortExpression="testDate" Visible="True" />

            <asp:BoundField DataField="DateAssigned" HeaderText="DateAssigned"
                SortExpression="DateAssigned" Visible="True" />

            <%-- Column 6 --%>
            <asp:BoundField DataField="Browser1Status" HeaderText="Chrome" FooterText="CHROME"
                SortExpression="Browser1Status" Visible="True" />

            <%-- Column 7 --%>
            <asp:BoundField DataField="Browser2Status" HeaderText="Firefox" FooterText="FF"
                SortExpression="Browser2Status" Visible="True" />

            <%-- Column 8 --%>
            <asp:BoundField DataField="Browser3Status" HeaderText="IE8" FooterText="IE8"
                SortExpression="Browser3Status" Visible="False" />

            <%-- Column 9 --%>
            <asp:BoundField DataField="Browser4Status" HeaderText="IE9" FooterText="IE9"
                SortExpression="Browser4Status" Visible="True" />

            <%-- Column 10 --%>
            <asp:BoundField DataField="Browser5Status" HeaderText="IE10" FooterText="IE10"
                SortExpression="Browser5Status" Visible="False" />

            <%-- Column 11 --%>
            <asp:BoundField DataField="Browser6Status" HeaderText="IE11" FooterText="IE11"
                SortExpression="Browser6Status" Visible="False" />

            <%-- Column 12 --%>
            <asp:BoundField DataField="Browser7Status" HeaderText="Mac Safari" FooterText="MACSAF"
                SortExpression="Browser7Status" Visible="False" />

            <%-- Column 13 --%>
            <asp:BoundField DataField="Browser8Status" HeaderText="Win Safari" FooterText="WINSAF"
                SortExpression="Browser8Status" Visible="False" />


            <asp:BoundField DataField="TestCategory" HeaderText="TestCategory"
                SortExpression="testCategory" Visible="False" />

            <asp:TemplateField HeaderText="Details">      
                <ItemTemplate> 
                    <asp:Button ID="DetailsButton" runat="server" PostBackUrl='<%# "~/FunctionalTesting/TestDetails.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") %>' Text="Details"></asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Remove">      
                <ItemTemplate> 
                    <asp:Button 
                        ID="btnDelete" 
                        runat="server" 
                        Text="Remove" 
                        CommandName="Unassign" 
                        CommandArgument='<%#Eval("projectAbbreviation") + "|" + Eval("testCaseId")%>' 
                        OnClientClick="if (!confirm('Are you sure you want to remove this test case from your task list?')){return false;}">
                    </asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>

        </Columns>
        <emptydatarowstyle CssClass="emptydata"/>
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
        <PagerSettings Mode="NumericFirstLast"  position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>      
</asp:Content>
<%--**************************** RIGHT RAIL CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">
   
   
</asp:Content>