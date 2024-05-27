<%@ Page Title="Analytics Test Details" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TestDetails.aspx.cs" Inherits="OSTMSWebsite.Analytics.AnalyticsTestDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>


<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<asp:Label ID="TopMessageLabel" runat="server" Text="" CssClass="Error"></asp:Label>
    
    <asp:SqlDataSource ID="sqlTestCaseDetails" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"   
        SelectCommand="SELECT * FROM [TestCaseWithStatus] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId) AND ([environment] = @environment)) ORDER BY [testCaseId]"
        UpdateCommand="" 
        DeleteCommand="exec sp_DeleteTestCaseWithHistory @user, @projectAbbreviation, @testCaseId" 
        ondeleting="sqlTestCaseDetails_Deleting" 
        ondeleted="sqlTestCaseDetails_Deleted">             
    <SelectParameters>
        <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
            Type="String" />
        <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
            Type="Int32" />
        <asp:Parameter DefaultValue="QA" Name="environment" Type="String" />
    </SelectParameters>
</asp:SqlDataSource>


<asp:FormView ID="fvTestCaseDetails" DataSourceID="sqlTestCaseDetails" 
        DataKeyNames="projectAbbreviation,testCaseId" RunAt="server" 
        CssClass="gvTestCaseList">

<%--**************************** Item Template (Read-Only Mode) ****************************--%>
  <ItemTemplate>
      <table border="1" width="100%">
	    <thead>
		    <tr>
			    <th colspan="3" align="left"><h1>Test Case Details</h1></th>
                <th align="right"><h1><asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' /></h1></th>
		    </tr>
        </thead>
        <tbody>
            <tr>
			    <th>Test Case ID</td>
			    <th>Current Stage Status</td>
			    <th>Last Tested</td>
			    <th>Last Tested By</td>
            </tr>
		    <tr>
			    <td><asp:Label ID="lblItemProject" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseId" runat="server" Text='<%# Eval("testCaseId") %>' /></td>
			    <td><asp:Label ID="lblItemStatus" runat="server" Text='<%# Bind("status") %>' /></td>
			    <td><asp:Label ID="lblItemTestDate" runat="server" Text='<%# Bind("testDate") %>' /></td>
			    <td><asp:Label ID="lblItemLastTestedBy" runat="server" Text='<%# Bind("lastTestedBy") %>' /></td>
		    </tr>
            <tr>
			    <th>Last Updated</td>
                <th>Updated By</td>
			    <th>Update Sprint</td>
			    <th>Updated Story</td>
		    </tr>
		    <tr>
			    <td><asp:Label ID="lblItemDateLastTested" runat="server" Text='<%# Bind("dateLastUpdated") %>' /></td>
			    <td><asp:Label ID="lblItemUpdatedBy" runat="server" Text='<%# Bind("updatedBy") %>' /></td>
			    <td><asp:Label ID="lblItemUpdateSprint" runat="server" Text='<%# Bind("updateSprint") %>' /></td>
			    <td><asp:Label ID="lblItemUpdateStory" runat="server" Text='<%# Bind("updateStory") %>' /></td>
		    </tr>
            <tr>
                <td colspan="2" class="commands"><asp:Button ID="btnUpdateTestCase" CommandName="Edit" Text="Update Test Case" Enabled="True" Width="150px" runat="server"></asp:Button></td>
                <td colspan="2" class="commands"><asp:Button ID="btnDeleteTestCase" CommandName="Delete" OnClientClick="return confirm('Deleting this test case will also delete all test results associated with it and remove it from the groups, sprints and releases it currently belongs to. Deleted test cases and their results as well its releases, sprints, and group associations can be restored via the admin page. Are you certain you want to delete this test case?');" Text="Delete Test Case" Width="150px" runat="server"></asp:Button></td>
            </tr>
		    <tr>
			    <th colspan="4">Description</td>
		    </tr>
		    <tr>
			    <td colspan="4"><b><asp:Label ID="lblItemDescription" runat="server" Text='<%# Bind("testCaseDescription") %>' /></b></td>
		    </tr>
		    <tr>
			    <th colspan="4">Expected Results</td>
		    </tr>
            <tr>
			    <td colspan="4"><asp:Label ID="lblItemExpectedResults" runat="server" Text='<%# Bind("expectedResults") %>' /></td>
		    </tr>
		    <tr>
			    <th colspan="4">Notes</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:Label ID="lblItemTestCaseNotes" runat="server" Text='<%# Bind("testCaseNotes") %>' /></td>
		    </tr>
	    </tbody>
    </table>

  </ItemTemplate>

  <%--**************************** Edit Item Template (Edit Mode) ****************************--%>
    <EditItemTemplate>
      <table border="1" width="100%">
	    <thead>
		    <tr>
			    <th colspan="3" align="left"><h1>Test Case Details</h1></td>
                <th align="right"><h1><asp:Label ID="lblProject" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblTestCaseId" runat="server" Text='<%# Eval("testCaseId") %>' /></h1></td>
		    </tr>
        </thead>
        <tbody>
            <tr>
			    <th>Project</td>
			    <th>Current Status</td>
			    <th>Last Tested</td>
			    <th>Last Tested By</td>
            </tr>
		    <tr>
			    <td><asp:Label ID="lblEditProject" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblEditTestCaseId" runat="server" Text='<%# Eval("testCaseId") %>' /></td>
			    <td><asp:Label ID="lblEditStatus" runat="server" Text='<%# Bind("status") %>' /></td>
			    <td><asp:Label ID="lblEditTestDate" runat="server" Text='<%# Bind("testDate") %>' /></td>
			    <td><asp:Label ID="lblEditLastTestedBy" runat="server" Text='<%# Bind("lastTestedBy") %>' /></td>
		    </tr>
            <tr>
			    <th>Last Updated</td>
                <th>Updated By</td>
			    <th>Update Sprint</td>
			    <th>Updated Story</td>
		    </tr>
		    <tr>
			    <td><asp:Label ID="lblEditDateLastUpdated" runat="server" Text='<%# Bind("dateLastUpdated") %>' /></td>
			    <td><asp:Label ID="lblEditUpdatedBy" runat="server" Text='<%# Bind("updatedBy") %>' /></td>
			    <td><asp:Label ID="lblEditUpdateSprint" runat="server" Text='<%# Bind("updateSprint") %>' /></td>
			    <td><asp:Label ID="lblEditUpdateStory" runat="server" Text='<%# Bind("updateStory") %>' /></td>
		    </tr>
            <tr>
			    <td colspan="4"><hr /></td>
		    </tr>


		    <tr>
			    <th colspan="4">Description</td>
		    </tr>
		    <tr>
			    <td colspan="4">
                    <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("rawTestCaseDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" />
                    <asp:RequiredFieldValidator ID="DescriptionRequired" runat="server" 
                         ControlToValidate="txtbxEditDescription" ErrorMessage="Description is required." 
                         ToolTip="Description is required." ValidationGroup="UpdateTestCase">
                    </asp:RequiredFieldValidator>
                </td>
		    </tr>
		    <tr>
			    <th colspan="4">Expected Results</td>
		    </tr>
            <tr>
			    <td colspan="4">
                    <asp:TextBox ID="txtbxEditExpectedResults" Text='<%# Bind("rawExpectedResults") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" />
                    <asp:RequiredFieldValidator ID="ExpectedResultsRequired" runat="server" 
                         ControlToValidate="txtbxEditExpectedResults" ErrorMessage="Expected Results is required." 
                         ToolTip="Expected Results is required." ValidationGroup="UpdateTestCase">
                    </asp:RequiredFieldValidator>
                </td>
		    </tr>
		    <tr>
			    <th colspan="4">Notes</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditNotes" Text='<%# Bind("rawTestCaseNotes") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <th colspan="2">Update Sprint</td>
                <th colspan="2">Update Story</td>
		    </tr>
            
            <tr>
			    <td colspan="2">


                     <asp:DropDownList ID="ddlSprints" runat="server" 
                        AppendDataBoundItems="False" 
                        DataSourceID="sqlSprints"
                        DataTextField="sprint"
                        DataValueField="sprint"
                        SelectedValue='<%# Bind("rawUpdateSprint") %>'
                        Width="100px" CssClass="updateTextbox">
                    </asp:DropDownList>


                <asp:SqlDataSource ID="sqlSprints" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT '' as sprint union SELECT [sprint] FROM [Sprints] where projectAbbreviation = @projectAbbreviation">
                    <SelectParameters>
                        <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
			    <td colspan="2"><asp:TextBox ID="txtbxEditUpdateStory" Text='<%# Bind("rawUpdateStory") %>' RunAt="Server" TextMode="MultiLine" Rows="3" Width="100%" CssClass="updateTextbox" /></td>
            </tr>
            <tr> 
                <td colspan="4">
                   <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_OnClick" Text="Update"></asp:Button> - <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:Button>
                   <asp:Label ID="MessageLabel" runat="server" Text="" CssClass="Error"></asp:Label>
                </td>
            </tr>
	    </tbody>
    </table>
  </EditItemTemplate>
</asp:FormView>
</asp:Content>



<%--**************************** LEFT RAIL CONTENT ****************************--%>
    

    <asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
    
    
    <%--<%--********** Groups **********--%>
    <div class="title">Groups</div>
    
    <div>
        <asp:SqlDataSource ID="sqlTestCaseGroups" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT GroupTestCases.projectAbbreviation, GroupTestCases.testCaseId, GroupTestCases.groupTestAbbreviation, GroupTests.groupTestName, GroupTests.groupTestDescription
                            FROM GroupTestCases
                            INNER JOIN GroupTests
                            ON GroupTestCases.projectAbbreviation=GroupTests.projectAbbreviation 
                                AND GroupTestCases.groupTestAbbreviation=GroupTests.groupTestAbbreviation
                            WHERE (GroupTestCases.projectAbbreviation=@projectAbbreviation) AND (GroupTestCases.testCaseId=@testCaseId)
                                and (personalGroupOwner is null or personalGroupOwner = @userName)
                            ORDER BY groupTestAbbreviation ASC">
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" Type="Int32" />
                <%--This has to be added via Page_Load--%>
                <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
            </SelectParameters>
        </asp:SqlDataSource>


            <asp:ListView ID="lvTestCaseGroups" runat="server" 
            DataSourceID="sqlTestCaseGroups" onitemcommand="lvTestCaseGroups_ItemCommand">
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                This test case does not belong to a group.
            </div>
        </emptydatatemplate>
            <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
            <ItemTemplate>
                <li style="">
                    <a href="TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&group=<%# Eval("groupTestAbbreviation") %>">
                        <asp:Label ID="lblGroup" runat="server" Text='<%# Eval("groupTestName") %>' /></a>
                    <br />
                    <b>Description: </b><asp:Label ID="lblGroupDescription" runat="server" Text='<%# Eval("groupTestDescription") %>'></asp:Label>
                    <br />
                    <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromGroup" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this group?');" Height="12" Width="12" ToolTip="Delete this test case from this group." CommandArgument='<%# Eval("groupTestAbbreviation") %>'></asp:ImageButton>
                    <br />
                </li>
            </ItemTemplate>
            <LayoutTemplate>
                <ul ID="itemPlaceholderContainer" runat="server" style="">
                    <li runat="server" id="itemPlaceholder" />
                </ul>
                <div style="">
                </div>
            </LayoutTemplate>
        </asp:ListView>
    </div>    
    
    <div class="commands">
     <asp:DropDownList ID="ddlGroupTests" runat="server" 
            AutoPostBack="True" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlGroupTests"
            DataTextField="groupTestName"
            DataValueField="groupTestAbbreviation"
            tooltip="Add test case to group"
            Width="140px">
        </asp:DropDownList>

        <asp:SqlDataSource ID="sqlGroupTests" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [groupTestAbbreviation], [groupTestName] FROM [GroupTests] WHERE ([projectAbbreviation] = @projectAbbreviation)  and (personalGroupOwner is null or personalGroupOwner = @userName) ORDER BY [groupTestName]">
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
                <%--This has to be added via Page_Load--%>
                <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Button ID="btnAddGroup" runat="server" tooltip="Add test case to group."
            Text="Add" Width="80px" onclick="btnAddGroup_Click" ></asp:Button>
    </div>
    <div><hr /></div>

    
    <%--********** Releases **********--%>
    <div class="title">Releases</div>
    <div>
        <asp:SqlDataSource ID="sqlTestCaseReleases" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT ReleaseTestCases.projectAbbreviation, ReleaseTestCases.testCaseId, ReleaseTestCases.release, Releases.releaseDescription, Releases.releaseDate
                            FROM ReleaseTestCases
                            INNER JOIN Releases
                            ON ReleaseTestCases.projectAbbreviation=Releases.projectAbbreviation 
                                AND ReleaseTestCases.release=Releases.release
                            WHERE (ReleaseTestCases.projectAbbreviation=@projectAbbreviation) AND (ReleaseTestCases.testCaseId=@testCaseId)
                            ORDER BY release DESC">
            
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        
        <asp:ListView ID="lvTestCaseReleases" runat="server" 
            DataSourceID="sqlTestCaseReleases" 
            onitemcommand="lvTestCaseReleases_ItemCommand">
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                This test case does not belong to a release.
            </div>
        </emptydatatemplate>
            <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
            <ItemTemplate>
                <li style="">
                    <a href="TestCases-API.aspx?project=<%# Eval("projectAbbreviation") %>&release=<%# Eval("release") %>">
                        <asp:Label ID="lblRelease" runat="server" Text='<%# Eval("release") %>' /></a>
                    <br /> 
                    <b>Release Date: </b><asp:Label ID="lblReleaseDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("releaseDate")).ToString("M/d/yyyy")%>'></asp:Label>
                    <br />                    
                    <b>Description: </b><asp:Label ID="lblReleaseDescription" runat="server" Text='<%# Eval("releaseDescription") %>'></asp:Label>
                    <br />
                    <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromRelease" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this release?');" Height="12" Width="12" ToolTip="Remove this test case from this release." CommandArgument='<%# Eval("release") %>'></asp:ImageButton>
                    <br />
                </li>
            </ItemTemplate>
            <LayoutTemplate>
                <ul ID="itemPlaceholderContainer" runat="server" style="">
                    <li runat="server" id="itemPlaceholder" />
                </ul>
                <div style="">
                </div>
            </LayoutTemplate>
        </asp:ListView>
    </div>   
    
    <div class="commands">
     <asp:DropDownList ID="ddlReleases" runat="server" 
            AutoPostBack="True" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlReleases"
            DataTextField="release"
            DataValueField="release"
            tooltip="Add test case to release"
            Width="140px">
        </asp:DropDownList>

        <asp:SqlDataSource ID="sqlReleases" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [release] FROM [Releases] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [release] DESC">
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Button ID="btnAddRelease" runat="server" tooltip="Add test case to release"
            Text="Add" Width="80px" onclick="btnAddRelease_Click" ></asp:Button>
    </div>
    

    <%--********** Sprints **********--%>
    <div class="title">Sprints</div>
    <div>
        <asp:SqlDataSource ID="sqlSprintTestCases" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT Sprints.projectAbbreviation, SprintTestCases.testCaseId, SprintTestCases.sprint, Sprints.sprintStartDate, Sprints.sprintEndDate
                            FROM SprintTestCases
                            INNER JOIN Sprints
                            ON SprintTestCases.projectAbbreviation=Sprints.projectAbbreviation 
                                AND SprintTestCases.sprint=Sprints.sprint
                            WHERE (SprintTestCases.projectAbbreviation=@projectAbbreviation) AND (SprintTestCases.testCaseId=@testCaseId)
                            ORDER BY sprint DESC">
            
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        
        <asp:ListView ID="lvSprints" runat="server" DataSourceID="sqlSprintTestCases" 
            onitemcommand="lvSprints_ItemCommand">
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                This test case does not belong to a sprint.
            </div>
        </emptydatatemplate>
            <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
            <ItemTemplate>
                <li style="">
                    <a href="TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&sprint=<%# Eval("sprint") %>">
                        <asp:Label ID="lblSprint" runat="server" Text='<%# Eval("sprint") %>' /></a>
                    <br /> 
                    <b>Start Date: </b><asp:Label ID="lblSprintStartDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("sprintStartDate")).ToString("M/d/yyyy")%>'></asp:Label>
                    <br />                    
                    <b>End Date: </b><asp:Label ID="lblSprintEndDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("sprintEndDate")).ToString("M/d/yyyy")%>'></asp:Label>
                    <br />
                    <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromSprint" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this sprint?');" Height="12" Width="12" ToolTip="Remove this test case from this sprint." CommandArgument='<%# Eval("sprint") %>'></asp:ImageButton>
                    <br />
                </li>
            </ItemTemplate>
            <LayoutTemplate>
                <ul ID="itemPlaceholderContainer" runat="server" style="">
                    <li runat="server" id="itemPlaceholder" />
                </ul>
                <div style="">
                </div>
            </LayoutTemplate>
        </asp:ListView>
    </div>   
    
    <div class="commands">
     <asp:DropDownList ID="ddlSprints" runat="server" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlSprints"
            DataTextField="sprint"
            DataValueField="sprint"
            tooltip="Add test case to sprint"
            Width="140px">
        </asp:DropDownList>

        <asp:SqlDataSource ID="sqlSprints" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [sprint] FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprint] DESC">
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
        <asp:Button ID="btnAddSprint" runat="server" 
            Text="Add" Width="80px" tooltip="Add test case to sprint" onclick="btnAddSprint_Click" ></asp:Button>
    </div>

</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
    

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">

    <%--********** Commands **********--%>
    <div class="title">Commands</div>
    <div class="commands">
        <asp:Button ID="btnAddEditResults" runat="server"  
            Text="Add Manual Results" Width="150px" onclick="btnAddEditResults_Click"></asp:Button>
    </div>   
    
    <div class="commands">
        <asp:Button ID="btnResultHistory" runat="server" 
            Text="View Result History" Width="150px" onclick="btnViewResultHistory_Click"></asp:Button>
    </div>
    
    <div class="commands">
        <asp:Button ID="btnUpdateHistory" runat="server" 
            Text="View Update History" Width="150px" onclick="btnViewUpdateHistory_Click"></asp:Button>
    </div>   
    
    <div class="commands">
        <asp:Button ID="btnURLList" runat="server" 
            Text="View URL List" Width="150px" onclick="btnViewURLList_Click"></asp:Button>
    </div>  
    <div><hr /></div>

        <%--********** Defects **********--%>
    <div class="title">Defect Tickets</div>
    <div>
        <asp:SqlDataSource ID="TestCaseDefectsTickets" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT defectTicketNumber, max(testDate) testDate, projectAbbreviation, testCaseId, environment FROM TestResults WHERE (projectAbbreviation = @projectAbbreviation) AND (testCaseId = @testCaseId) AND (defectTicketNumber <> '') AND (defectTicketNumber IS NOT NULL) GROUP BY defectTicketNumber, projectAbbreviation, testCaseId, environment ORDER BY testDate Desc, defectTicketNumber ASC">
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
        
        <asp:ListView ID="DefectsTickets" runat="server" DataSourceID="TestCaseDefectTickets">
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no Defect Tickets associated with this test case.
            </div>
        </emptydatatemplate>
            <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
            <ItemTemplate>
                <li style="">
                    <a href="http://trgteam2:8080/tfs/TFS2008/QAA_TMS/_workItems#_a=edit&id=<%# Eval("defectTicketNumber") %>&triage=true">
                        <asp:Label ID="defectTicketNumberLabel" runat="server" 
                        Text='<%# Eval("defectTicketNumber") %>' /></a>
                    <br />
                    <b>Test Date:</b>
                    <asp:Label ID="testDateLabel" runat="server" Text='<%# Eval("testDate") %>' />
                    <br />
                    <b>Environment:</b>
                    <asp:Label ID="environmentLabel" runat="server" 
                        Text='<%# Eval("environment") %>' />
                    <br />
                    <b>Ticket Status:</b> Coming Soon
                    <br />
                </li>
            </ItemTemplate>
            <LayoutTemplate>
                <ul ID="itemPlaceholderContainer" runat="server" style="">
                    <li runat="server" id="itemPlaceholder" />
                </ul>
                <div style="">
                </div>
            </LayoutTemplate>
        </asp:ListView>
    </div>


</asp:Content>

