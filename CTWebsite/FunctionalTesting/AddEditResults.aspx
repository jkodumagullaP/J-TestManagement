<%@ Page Title="Add or Edit Test Results" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AddEditResults.aspx.cs" Inherits="CTWebsite.FunctionalTesting.AddEditResults" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
   
    <asp:SqlDataSource ID="sqlTestResultDetails" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"   
        SelectCommand="Select TestResultsView.*, dbo.UserProfiles.userFirstName + ' ' + dbo.UserProfiles.userLastName as TestedByFullName from [TestResultsView] left join dbo.aspnet_Users 	on dbo.TestResultsView.testedBy = aspnet_Users.UserName left join dbo.UserProfiles on aspnet_Users.userid = UserProfiles.userid where testrunid = @testRunID" 
        InsertCommand="INSERT INTO [TestResults] ([testCaseId], [projectAbbreviation], [environment], [browserAbbreviation], [status], [reasonForStatus], [stepsToReproduce], [defectTicketNumber], [testDate], [testedBy], [testType]) VALUES (@testCaseId, @projectAbbreviation, @environment, @browserAbbreviation, @status, @reasonForStatus, @stepsToReproduce, @defectTicketNumber, @testDate, @testedBy, @testType)" 
        UpdateCommand="UPDATE [TestResults] set [environment] = @environment, [browserAbbreviation] = @browserAbbreviation, [status] = @status, [reasonForStatus] = @reasonForStatus, [stepsToReproduce] = @stepsToReproduce, [defectTicketNumber] = @defectTicketNumber, [testDate] = @testDate, [testedBy] = @testedBy, [testType] = @testType where testrunid = @testRunID" 
        OldValuesParameterFormatString="original_{0}" >
    <SelectParameters>
        <asp:QueryStringParameter Name="testRunId" QueryStringField="testRunId" 
            Type="Int32" />
    </SelectParameters>
    <UpdateParameters>
        <asp:QueryStringParameter Name="testRunId" QueryStringField="testRunId" 
            Type="Int32" />
        <asp:Parameter Name="environment" Type="String" />
        <asp:Parameter Name="browserAbbreviation" Type="String" />
        <asp:Parameter Name="status" Type="String" />
        <asp:Parameter Name="reasonForStatus" Type="String" />
        <asp:Parameter Name="stepsToReproduce" Type="String" />
        <asp:Parameter Name="defectTicketNumber" Type="String" />
        <asp:Parameter Name="testDate" Type="DateTime" />
        <asp:Parameter Name="testedBy" Type="String" />
        <asp:Parameter Name="testType" Type="String" />
    </UpdateParameters>
    <InsertParameters>
        <asp:Parameter Name="testCaseId" Type="Int32" />
        <asp:Parameter Name="projectAbbreviation" Type="String" />
        <asp:Parameter Name="environment" Type="String" />
        <asp:Parameter Name="browserAbbreviation" Type="String" />
        <asp:Parameter Name="status" Type="String" />
        <asp:Parameter Name="reasonForStatus" Type="String" />
        <asp:Parameter Name="stepsToReproduce" Type="String" />
        <asp:Parameter Name="defectTicketNumber" Type="String" />
        <asp:Parameter Name="testDate" Type="DateTime" />
        <asp:Parameter Name="testedBy" Type="String" />
        <asp:Parameter Name="testType" Type="String" />
    </InsertParameters>
    </asp:SqlDataSource>
    
    <asp:SqlDataSource ID="Environments" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT ProjectEnvironmentInfo.environment, Environments.sortOrder FROM ProjectEnvironmentInfo JOIN Environments ON ProjectEnvironmentInfo.environment = Environments.environment WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY sortOrder">
        <SelectParameters>
        <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>
        
    <asp:SqlDataSource ID="Browsers" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT Browsers.browserAbbreviation, Browsers.browserName, ProjectBrowserInfo.showBrowserColumn FROM ProjectBrowserInfo INNER JOIN Browsers ON Browsers.browserAbbreviation=ProjectBrowserInfo.browserAbbreviation WHERE (ProjectBrowserInfo.projectAbbreviation = @projectAbbreviation) AND (ProjectBrowserInfo.showBrowserColumn = 1) ORDER BY Browsers.browserName">
        <SelectParameters>
        <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:SqlDataSource ID="Statuses" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT [status] FROM [Statuses]">
    </asp:SqlDataSource>   



<asp:FormView ID="fvTestResultDetails" 
        DataSourceID="sqlTestResultDetails" 
        DataKeyNames="projectAbbreviation,testCaseId" 
        oniteminserted="fvTestResultDetails_ItemInserted"
        onitemupdated="fvTestResultDetails_ItemUpdated"
        RunAt="server" CssClass="gvGlobalGridview" Width="100%" 
        onitemcommand="fvTestResultDetails_ItemCommand">

<%--**************************** Item Template (Read-Only Mode) ****************************--%>
  <ItemTemplate>
      <table width="100%" border="1">
	    <thead>
		    <tr>
			    <th colspan="4"><h1>Test Results</h1></th>
		    </tr>
        </thead>
        <tbody>

		    <tr>
			    <th>Project</th>
			    <th>Test Case ID</th>
                <th>Tester</th>
                <th>Test Date</th>
		    </tr>		    
            <tr>
                <td><asp:Label ID="tbProjectAbbreviation" runat="server" Text='<%# Bind("projectAbbreviation") %>' /></td>
                <td><asp:Label ID="tbTestCaseId" runat="server" Text='<%# Bind("testCaseId") %>' /></td>
			    <td>
                    <asp:Label ID="tbLoggedInUserRaw" runat="server" Text='<%# Bind("testedBy") %>' Visible = "false" />
                    <asp:Label ID="tbLoggedInUser" runat="server" Text='<%# Bind("TestedByFullName") %>' />
                </td>
                <td><asp:Label ID="tbTestDate" runat="server" Text='<%# Bind("testDate") %>' /></td>
		    </tr>


            <tr>
			    <th>Environment</th>
			    <th>Browser</th>
			    <th>Status</th>
			    <th>Defect Ticket #</th>
		    </tr>
		    
            <tr>
			    <td>
                    <asp:Label ID="tbEnvironment" runat="server" Text='<%# Bind("environment") %>' />
                </td>
			    <td>
                    <asp:Label ID="tbBrowserAbbreviation" runat="server" Text='<%# Bind("browserAbbreviation") %>' />
                </td>
			    <td>
                    <asp:Label ID="tbStatus" runat="server" Text='<%# Bind("status") %>' />
                </td>
			    <td>
                    <asp:Label ID="tbdefectTicketNumber" runat="server" Text='<%# Bind("defectTicketNumber") %>' />
                </td>
		    </tr>
            <tr>
			    <td colspan="4"><hr /></td>
		    </tr>
            		    <tr>
			    <th colspan="4">Reason For Status</td>
		    </tr>
		    <tr>
			    <td colspan="4">
                    <asp:Label ID="tbReasonForStatus" runat="server" Text='<%# Bind("reasonForStatus") %>' />                
                </td>
		    </tr>

		    <tr>
			    <th colspan="4">Detailed Reason For Status</td>
		    </tr>
		    <tr>
			    <td colspan="4">
                    <asp:Label ID="tbReasonForStatusDetailed" runat="server" Text='<%# Bind("reasonForStatusDetailed") %>' />                
                </td>
		    </tr>
            <tr>
			    <th colspan="4">Steps to Reproduce</td>
		    </tr>
		    <tr>
			    <td colspan="4">
                    <asp:Label ID="tbStepsToReproduce" runat="server" Text='<%# Bind("stepsToReproduce") %>' />                
                </td>
		    </tr>
            <tr>
			    <th colspan="4">Screenshots</td>
		    </tr>
            <tr>
                <td colspan="4">
                    <div>     
                        <asp:DataList ID="dlTestResultScreenshots" runat="server" DataKeyField="imageURL" 
                            DataSourceID="sqlTestResultScreenshots" RepeatColumns="1" EditItemStyle-HorizontalAlign="Center" EditItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate> 
                                <asp:ImageButton ID="imgTestResultScreenshots" OnClick="imgTestResultScreenshots_OnClick" runat="server" 
                                    Width="450"
                                    ImageUrl='<%# Eval("imageURL") %>' ToolTip='<%# Eval("description") %>' 
                                    ImageAlign="Top" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" 
                                    OnClientClick="Form1.target ='_blank'"/> 
                                <asp:ImageButton ID="ibtnRemove" OnClick="ibtnRemove_OnClick" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to delete this screenshot from this test result?');" Height="20" Width="20" ToolTip="Delete this screenshot." CommandArgument='<%# Eval("projectAbbreviation") + "|" + Eval("testCaseId") + "|" + Eval("testRunId") + "|" + Eval("imageId") %>'></asp:ImageButton>
                                <br /><br /><br />
                            </ItemTemplate> 
                        </asp:DataList> 
                        <asp:SqlDataSource ID="sqlTestResultScreenshots" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"  
                            SelectCommand="SELECT * FROM TestResultScreenshots WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId) AND ([testRunId] = @testRunId))">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                                    Type="String" />
                                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                                    Type="Int32" />
                                <asp:QueryStringParameter Name="testRunId" QueryStringField="testRunId" 
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource> 
                    </div> 
                </td>
            </tr>

            <tr>
                <td colspan="4"><asp:TextBox ID="tbTestType" runat="server" Text='<%# Bind("testType") %>' Visible="False" CssClass="updateTextbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4" align="center">
                    <asp:Button ID="btnEditResult" runat="server"  
                        Text="Edit Result" Width="150px" onclick="btnEditResult_Click"></asp:Button>
                </td>
            </tr>
            <tr>
                <td colspan="4"><asp:label id="MessageLabel" forecolor="Red" runat="server" Font-Bold="True" /></td>
            </tr>

	    </tbody>
    </table>

</ItemTemplate>

<%--**************************** Insert Item Template (Insert Mode) ****************************--%>

  <InsertItemTemplate>
      <table width="100%" border="1">
	    <thead>
		    <tr>
			    <th colspan="4"><h1>Insert Test Results</h1></th>
		    </tr>
        </thead>
        <tbody>

		    <tr>
			    <th>Project</th>
			    <th>Test Case ID</th>
                <th>Tester</th>
                <th>Test Date</th>
		    </tr>		    
            <tr>
                <td><asp:Label ID="tbProjectAbbreviation" runat="server" Text='<%# Bind("projectAbbreviation") %>' /></td>
                <td><asp:Label ID="tbTestCaseId" runat="server" Text='<%# Bind("testCaseId") %>' /></td>
			    <td>
                    <asp:Label ID="tbLoggedInUserRaw" runat="server" Text='<%# Bind("testedBy") %>' Visible = "false" />
                    <asp:Label ID="tbLoggedInUser" runat="server" Text='<%# Bind("TestedByFullName") %>' />
                </td>
                <td><asp:Label ID="tbTestDate" runat="server" Text='<%# Bind("testDate") %>' /></td>
		    </tr>


            <tr>
			    <th>Environment</th>
			    <th>Browser</th>
			    <th>Status</th>
			    <th>Defect Ticket #</th>
		    </tr>
		    
            <tr>
			    <td>
                    <asp:DropDownList ID="ddlEnvironment" runat="server" Width="175" DataSourceID="Environments" 
                    DataTextField="environment" DataValueField="environment" 
                    AppendDataBoundItems="True"
                    SelectedValue='<%# Bind("environment") %>'
                    AutoPostBack="True" CssClass="updateTextbox">

                    <asp:ListItem Value="0" Text="Select an Environment"></asp:ListItem>
                    </asp:DropDownList></td>
			    <td>
                    <asp:DropDownList ID="ddlBrowser" runat="server" Width="150" DataSourceID="Browsers" 
                    DataTextField="browserName" DataValueField="browserAbbreviation" 
                    AppendDataBoundItems="True"
                    SelectedValue='<%# Bind("browserAbbreviation") %>'
                    AutoPostBack="True" CssClass="updateTextbox">
                    <asp:ListItem Value="0" Text="Select a Browser"></asp:ListItem>
                    </asp:DropDownList></td>
			    <td>
                    <asp:DropDownList ID="ddlStatus" runat="server" Width="150" DataSourceID="Statuses" 
                    DataTextField="status" DataValueField="status" 
                    AppendDataBoundItems="True"
                    SelectedValue='<%# Bind("status") %>'
                    AutoPostBack="True" CssClass="updateTextbox">
                    <asp:ListItem Value="0" Text="Select a Status"></asp:ListItem>
                    </asp:DropDownList></td>
			    <td><asp:TextBox ID="tbdefectTicketNumber" runat="server" Text='<%# Bind("defectTicketNumber") %>' CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <td colspan="4"><hr /></td>
		    </tr>
            		    <tr>
			    <th colspan="4">Reason For Status</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="tbReasonForStatus" runat="server" Text='<%# Bind("reasonForStatus") %>' TextMode="MultiLine" Height="100" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>

		    <tr>
			    <th colspan="4">Detailed Reason For Status</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="tbReasonForStatusDetailed" runat="server" Text='<%# Bind("reasonForStatusDetailed") %>' TextMode="MultiLine" Height="100" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <th colspan="4">Steps to Reproduce</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="tbStepsToReproduce" runat="server" Text='<%# Bind("stepsToReproduce") %>' TextMode="MultiLine" Width="100%" Height="100" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
                <td colspan="4"><asp:TextBox ID="tbTestType" runat="server" Text='<%# Bind("testType") %>' Visible="False" CssClass="updateTextbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4" align="center">
                <asp:Button ID="InsertButton" runat="server" CausesValidation="True" OnClick="InsertButton_OnClick"  Text="Insert" />
                &nbsp;<asp:Button ID="InsertCancelButton" runat="server" 
                CausesValidation="False" CommandName="Cancel" Text="Cancel" /></td>
            </tr>
            <tr>
                <td colspan="4"><asp:label id="MessageLabel" forecolor="Red" runat="server" Font-Bold="True" /></td>
            </tr>

	    </tbody>
    </table>

</InsertItemTemplate>

  <%--**************************** Edit Item Template (Edit Mode) ****************************--%>
  <EditItemTemplate>
      <table width="100%" border="1">
	    <thead>
		    <tr>
			    <th colspan="4"><h1>Edit Test Results</h1></th>
		    </tr>
        </thead>
        <tbody>

		    <tr>
			    <th>Project</th>
			    <th>Test Case ID</th>
                <th>Tester</th>
                <th>Test Date</th>
		    </tr>		    
            <tr>
                <td><asp:Label ID="tbProjectAbbreviation" runat="server" Text='<%# Bind("projectAbbreviation") %>' /></td>
                <td><asp:Label ID="tbTestCaseId" runat="server" Text='<%# Bind("testCaseId") %>' /></td>
			    <td>
                    <asp:Label ID="tbLoggedInUserRaw" runat="server" Text='<%# Bind("testedBy") %>' Visible = "false" />
                    <asp:Label ID="tbLoggedInUser" runat="server" Text='<%# Bind("TestedByFullName") %>' />
                </td>
                <td><asp:Label ID="tbTestDate" runat="server" Text='<%# Bind("testDate") %>' /></td>
		    </tr>


            <tr>
			    <th>Environment</th>
			    <th>Browser</th>
			    <th>Status</th>
			    <th>Defect Ticket #</th>
		    </tr>
		    
            <tr>
			    <td>
                    <asp:Label ID="lblEnvironment" runat="server" Text='<%# Bind("environment") %>' />
                </td>
			    <td>
                    <asp:Label ID="lblBrowserAbbreviation" runat="server" Text='<%# Bind("browserAbbreviation") %>' />
                </td>
			    <td>
                    <asp:DropDownList ID="ddlStatus" runat="server" Width="150" DataSourceID="Statuses" 
                    DataTextField="status" DataValueField="status" 
                    AppendDataBoundItems="True"
                    SelectedValue='<%# Bind("status") %>'
                    AutoPostBack="True" CssClass="updateTextbox">
                    <asp:ListItem Value="0" Text="Select a Status"></asp:ListItem>
                    </asp:DropDownList></td>
			    <td><asp:TextBox ID="tbdefectTicketNumber" runat="server" Text='<%# Bind("defectTicketNumber") %>' CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <td colspan="4"><hr /></td>
		    </tr>
            		    <tr>
			    <th colspan="4">Reason For Status</td>
		    </tr>
		    
            <tr>
			    <td colspan="4"><asp:TextBox ID="tbReasonForStatus" runat="server" Text='<%# Bind("reasonForStatus") %>' TextMode="MultiLine" Height="100" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>

		    <tr>
			    <th colspan="4"> Detailed Reason For Status</td>
		    </tr>
		    
            <tr>
			    <td colspan="4"><asp:TextBox ID="tbReasonForStatusDetailed" runat="server" Text='<%# Bind("reasonForStatusDetailed") %>' TextMode="MultiLine" Height="100" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <th colspan="4">Steps to Reproduce</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="tbStepsToReproduce" runat="server" Text='<%# Bind("stepsToReproduce") %>' TextMode="MultiLine" Width="100%" Height="100" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
                <td colspan="4"><asp:TextBox ID="tbTestType" runat="server" Text='<%# Bind("testType") %>' Visible="False" CssClass="updateTextbox" />
                </td>
            </tr>
            <tr>
                <td colspan="4" align="center">
                <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_OnClick" Text="Update" />
                &nbsp;<asp:Button ID="EditCancelButton" runat="server" 
                CausesValidation="False" CommandName="Cancel" Text="Cancel" /></td>
            </tr>
            <tr>
                <td colspan="4"><asp:label id="MessageLabel" forecolor="Red" runat="server" Font-Bold="True" /></td>
            </tr>

	    </tbody>
    </table>

</EditItemTemplate>


 
</asp:FormView>
    
</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">

    <%--********** Commands **********--%>
    <div class="title"><asp:label id="lblCommands" runat="server" text="Commands" /></div>
    
    <div class="commands">
        <asp:Button ID="btnAddScreenshot" runat="server"  
            Text="Add Screenshot" Width="150px" onclick="btnAddScreenshot_Click"></asp:Button>
    </div>    
</asp:Content>



