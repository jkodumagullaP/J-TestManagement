<%@ Page Title="Test Details" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TestDetails.aspx.cs" Inherits="CTWebsite.FunctionalTesting.TestDetails" %>

<%@ Register TagPrefix="asp" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
<style type="text/css">

@media print {
    body * {
    visibility:hidden;
    }
    #printable, #printable * {
    visibility:visible;
    }
    #printable { /* aligning the printable area */
    position:absolute;
    left:40px;
    top:40px;
    }
}
</style>

</asp:Content>


<%--**************************** MAIN CONTENT ****************************--%>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Label ID="TopMessageLabel" runat="server" Text="" CssClass="Error"></asp:Label>

<div id="detailscommands">
    <asp:Label ID="lblTCID" Text="" Enabled="True" runat="server" CssClass="lblTCID"></asp:Label>
    <div class="Right">
        <asp:ImageButton ID="btnFirst" Tooltip="Jump to First Test Case" Enabled="True" Width="30" Height="30" runat="server" OnCommand="NavigationButtons_OnClientClick" CommandName="First" ImageUrl="~/Images/first.png" />
        <asp:ImageButton ID="btnBack" Tooltip="Jump to Previous Test Case" Enabled="True" Width="30" Height="30" runat="server" OnCommand="NavigationButtons_OnClientClick" CommandName="Back" ImageUrl="~/Images/previous.png"></asp:ImageButton>
        <asp:ImageButton ID="btnNext" Tooltip="Jump to Next Test Case" Enabled="True" Width="30" Height="30" runat="server" OnCommand="NavigationButtons_OnClientClick" CommandName="Next" ImageUrl="~/Images/next.png"></asp:ImageButton>
        <asp:ImageButton ID="btnLast" Tooltip="Jump to Last Test Case" Enabled="True" Width="30" Height="30" runat="server" OnCommand="NavigationButtons_OnClientClick" CommandName="Last" ImageUrl="~/Images/last.png"></asp:ImageButton>
        <asp:ImageButton ID="btnAddResults" Tooltip="Add Test Results" runat="server" Width="30" Height="30" Text="Add Manual Results" onclick="btnAddEditResults_Click" ImageUrl="~/Images/add.png"></asp:ImageButton>
        <asp:ImageButton ID="btnAddScreenshot" Tooltip="Add Screenshot" runat="server" Text="Add Screenshot" Width="30" Height="30" onclick="btnAddScreenshot_Click" ImageUrl="~/Images/addscreenshots.png"></asp:ImageButton>
        <asp:ImageButton ID="btnPrint" CommandName="" Tooltip="Print Test Case Details" Text="Print" Visible="false" Enabled="True" Width="30px" Height="30" runat="server" ImageUrl="~/Images/print.png"></asp:ImageButton>
        <asp:ImageButton ID="btnDeleteTestCase" Tooltip="Delete Test Case" CommandName="Delete" OnClientClick="return confirm('Deleting this test case will also delete all test results associated with it and remove it from the groups, sprints and releases it currently belongs to. Deleted test cases and their results as well its releases, sprints, and group associations can be restored via the admin page. Are you certain you want to delete this test case?');" Text="Delete" Enabled="True" Width="30px" Height="30" ImageUrl="~/Images/delete.png" runat="server"></asp:ImageButton>

    </div>
</div>

<div class="clear"></div>
<div>  
    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager2" runat="server"> </ajaxToolkit:ToolkitScriptManager>  
    
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

          
<ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="ajax__tab_blueGrad-theme" AutoPostBack="True">  
            
        <%--********************* DETAILS ***********************--%>            
<ajaxToolkit:TabPanel ID="TabPanel1" HeaderText="Details" runat="server" CssClass="ajax__tab_blueGrad-theme">  
    <ContentTemplate> 
        <asp:FormView ID="fvTestCaseDetails" DataSourceID="sqlTestCaseDetails" 
                    DataKeyNames="projectAbbreviation,testCaseId" RunAt="server" 
                    CssClass=""
                    OnPreRender="fvTestCaseDetails_PreRender" Width="100%">

        <%--**************************** Item Template (Read-Only Mode) ****************************--%>
        <ItemTemplate>
            <table id="detailsHeader">
                <tr>
                    <td class="details-tcid">
                        <asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' />
                    </td>
                    <td class="details-title">
                        <asp:Label ID="lblItemDescription" runat="server" Text='<%# Bind("testCaseDescription") %>' />                    
                    </td>
                    <td class="details-title2">
                        <asp:Button ID="btnUpdateTestCase" CommandName="Edit" Text="Edit" Enabled="True" Width="40px" runat="server"></asp:Button>                    
                    </td>
                </tr>
            </table>
            <br />
            <table id="detailsInfo">
                <tr>
                    <th class="detailsInfo-title">
                        Active
                    </th>
                    <th class="detailsInfo-title">
                        Test Case Outdated
                    </th>
                    <th class="detailsInfo-title">
                        Automated Script Outdated
                    </th>
                    <th class="detailsInfo-title">
                        Last Tested
                    </th>
                    <th class="detailsInfo-title">
                        Tested By
                    </th >
                    <th class="detailsInfo-title">
                        Last Updated
                    </th>
                    <th class="detailsInfo-title">
                        Updated By
                    </th>
                </tr>
                <tr>
                    <td class="detailsInfo-element">
                       <asp:CheckBox ID="chkActive" runat="server" Enabled="true" OnCheckedChanged="chkActive_CheckedChanged" Checked='<%# Bind("active") %>' AutoPostBack="True" />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:CheckBox ID="chkTestCaseOutdated" runat="server" Enabled="true" OnCheckedChanged="chkTestCaseOutdated_CheckedChanged"                          Checked='<%# Bind("testCaseOutdated") %>' AutoPostBack="True" />
                    </td>
                    <td class="detailsInfo-element"> <asp:CheckBox ID="chkTestScriptOutdated" runat="server" Enabled="true"                                                 OnCheckedChanged="chkTestScriptOutdated_CheckedChanged" Checked='<%# Bind("testScriptOutdated") %>' AutoPostBack="True" />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:Label ID="lblItemTestDate" runat="server" Text='<%# Bind("testDate") %>' />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:Label ID="lblItemLastTestedBy" runat="server" Text='<%# Bind("lastTestedBy") %>' />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:Label ID="lblDateLastUpdated" runat="server" Text='<%# Bind("dateLastUpdated") %>' />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:Label ID="lblItemUpdatedBy" runat="server" Text='<%# Bind("updatedBy") %>' />
                    </td>
                </tr>
            </table>
            <br />
            <table id="detailsInfo2">
                <tr>
                    <td class="details-info-titles">
                         Test Case Steps
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                        <asp:Label ID="lblItemTestCaseSteps" runat="server" Text='<%# Bind("testCaseSteps") %>' />
                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Expected Results
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                        <asp:Label ID="lblItemExpectedResults" runat="server" Text='<%# Bind("expectedResults") %>' />
                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Notes
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                        <asp:Label ID="lblItemTestCaseNotes" runat="server" Text='<%# Bind("testCaseNotes") %>' />
                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Screenshots
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                    <div>     
                        <asp:DataList ID="dlTestCaseScreenshots" runat="server" DataKeyField="imageURL" 
                            DataSourceID="sqlTestCaseScreenshots" RepeatColumns="1" EditItemStyle-HorizontalAlign="Center" EditItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate> 
                                <asp:ImageButton ID="imgTestCaseScreenshots" OnClick="imgTestCaseScreenshots_OnClick" runat="server" 
                                    Width="90%"
                                    ImageUrl='<%# Eval("imageURL") %>' ToolTip='<%# Eval("description") %>' 
                                    ImageAlign="Top" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" 
                                    OnClientClick="Form1.target ='_blank'"/> 
                                <asp:ImageButton ID="ibtnRemove" OnClick="ibtnRemove_OnClick" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to delete this screenshot from this test case?');" Height="20" Width="20" ToolTip="Delete this screenshot." CommandArgument='<%# Eval("projectAbbreviation") + "|" + Eval("testCaseId") + "|" + Eval("imageId") %>'></asp:ImageButton>
                                <br /><br /><br />
                            </ItemTemplate> 
                        </asp:DataList> 
                        <asp:SqlDataSource ID="sqlTestCaseScreenshots" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"  
                            SelectCommand="SELECT * FROM TestCaseSCreenshots WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId))">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                                    Type="String" />
                                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource> 
                    </div> 
                    </td>
                </tr>
            </table>
        </ItemTemplate>
  <%--**************************** Edit Item Template (Edit Mode) ****************************--%>
        <EditItemTemplate>
            <table id="detailsHeader">
                <tr>
                    <td class="details-tcid">
                        <asp:Label ID="lblProject" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblTestCaseId" runat="server" Text='<%# Eval("testCaseId") %>' />
                    </td>
                    <td class="details-title" colspan="2">
                        <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("rawTestCaseDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="2" CssClass="details-update-description" />
                    <asp:RequiredFieldValidator ID="DescriptionRequired" runat="server" 
                         ControlToValidate="txtbxEditDescription" ErrorMessage="Description is required." 
                         ToolTip="Description is required." ValidationGroup="UpdateTestCase">
                    </asp:RequiredFieldValidator>
                    
                    </td>
                </tr>
            </table>
            <br />
            <table id="detailsInfo">
                <tr>
                    <th class="detailsInfo-title">
                        Active
                    </th>
                    <th class="detailsInfo-title">
                        Test Case Outdated
                    </th>
                    <th class="detailsInfo-title">
                        Script Outdated
                    </th >
                    <th class="detailsInfo-title">
                        Test Category
                    </th>
                </tr>
                <tr>
                    <td class="detailsInfo-element">
                       <asp:CheckBox ID="chkActive" runat="server" Text="" Checked='<%# Bind("active") %>' />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:CheckBox ID="chkTestCaseOutdated" runat="server" Text="" Checked='<%# Bind("testCaseOutdated") %>' />
                    </td>
                    <td class="detailsInfo-element">
                        <asp:CheckBox ID="chkTestScriptOutdated" runat="server" Text="" Checked='<%# Bind("testScriptOutdated") %>'/>
                    </td>
                    <td class="detailsInfo-element">
                        <asp:TextBox ID="txtCategory" runat="server" 
                        Text='<%# Bind("testCategory") %>'
                        Width="100px" CssClass="textbox"
                        Enabled="true">
                    </asp:TextBox>
                    </td>
                </tr>
            </table>
            <br />
            <table id="detailsInfo2">
                <tr>
                    <td class="details-info-titles">
                         Test Case Steps
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                    <asp:TextBox ID="txtbxEditTestCaseSteps" Text='<%# Bind("rawTestCaseSteps") %>' RunAt="Server" TextMode="MultiLine" Rows="10" Width="100%"/>
                    <asp:RequiredFieldValidator ID="StepsRequired" runat="server" 
                         ControlToValidate="txtbxEditTestCaseSteps" ErrorMessage="Test Case Steps is required." 
                         ToolTip="Test Case Steps is required." ValidationGroup="UpdateTestCase">
                    </asp:RequiredFieldValidator>
                    <ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender1"
                        TargetControlID="txtbxEditTestCaseSteps"
                        runat="server" EnableSanitization="False" />
                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Expected Results
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                    <asp:TextBox ID="txtbxEditExpectedResults" Text='<%# Bind("rawExpectedResults") %>' RunAt="Server" TextMode="MultiLine" Rows="10" Width="100%"/>
                    <asp:RequiredFieldValidator ID="ExpectedResultsRequired" runat="server" 
                         ControlToValidate="txtbxEditExpectedResults" ErrorMessage="Expected Results is required." 
                         ToolTip="Expected Results is required." ValidationGroup="UpdateTestCase">
                    </asp:RequiredFieldValidator>
                    <ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender2"
                        TargetControlID="txtbxEditExpectedResults"
                        runat="server" EnableSanitization="False" />
                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Notes
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
<asp:TextBox ID="txtbxEditNotes" Text='<%# Bind("rawTestCaseNotes") %>' RunAt="Server" TextMode="MultiLine" Rows="10" Width="100%" />
                    <ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender3"
                        TargetControlID="txtbxEditNotes"
                        runat="server" EnableSanitization="False" />                    </td>
                </tr>
                <tr>
                    <td class="details-info-titles">
                         Screenshots
                    </td>
                </tr>
                <tr>
                    <td class="details-info-elements">
                    <div>     
                        <asp:DataList ID="dlTestCaseScreenshots" runat="server" DataKeyField="imageURL" 
                            DataSourceID="sqlTestCaseScreenshots" RepeatColumns="1" EditItemStyle-HorizontalAlign="Center" EditItemStyle-VerticalAlign="Middle"> 
                            <ItemTemplate> 
                                <asp:ImageButton ID="imgTestCaseScreenshots" OnClick="imgTestCaseScreenshots_OnClick" runat="server" 
                                    Width="90%"
                                    ImageUrl='<%# Eval("imageURL") %>' ToolTip='<%# Eval("description") %>' 
                                    ImageAlign="Top" BorderColor="Black" BorderStyle="Solid" BorderWidth="2px" 
                                    OnClientClick="Form1.target ='_blank'"/> 
                                <asp:ImageButton ID="ibtnRemove" OnClick="ibtnRemove_OnClick" runat="server" ImageUrl="~/Images/delete.png"             OnClientClick="return confirm('Are you certain you want to delete this screenshot from this test case?');" Height="20" Width="20" ToolTip="Delete this screenshot." CommandArgument='<%# Eval("projectAbbreviation") + "|" + Eval("testCaseId") + "|" + Eval("imageId") %>'></asp:ImageButton>
                                <br /><br /><br />
                            </ItemTemplate> 
                        </asp:DataList> 
                        <asp:SqlDataSource ID="sqlTestCaseScreenshots" runat="server" 
                            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"  
                            SelectCommand="SELECT * FROM TestCaseSCreenshots WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId))">
                            <SelectParameters>
                                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                                    Type="String" />
                                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                                    Type="Int32" />
                            </SelectParameters>
                        </asp:SqlDataSource> 
                    </div> 
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_OnClick" Text="Update"></asp:Button> - <asp:Button ID="UpdateCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:Button>
                        <asp:Label ID="MessageLabel" runat="server" Text="" CssClass="Error"></asp:Label>

                    </td>
                </tr>
            </table>
        </EditItemTemplate>
     </asp:FormView>
    </ContentTemplate>  
</ajaxToolkit:TabPanel>  

        <%--********************* RESULTS ***********************--%>            
        <ajaxToolkit:TabPanel ID="TabPanel2" HeaderText="Results" runat="server" >  
            <ContentTemplate>

                <asp:Label id="lblResultTestCaseDescription" runat="server" Text =""></asp:Label>
                <asp:SqlDataSource ID="sqlSingleTestCaseHistory" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand=""
        UpdateCommand="UPDATE [TestResultsView] SET [reasonForStatus] = @reasonForStatus, [stepsToReproduce] = @stepsToReproduce, [defectTicketNumber] = @defectTicketNumber WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId) AND ([testDate] = @testDate) AND ([environment] = @environment))"> 
        <SelectParameters>
            <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                Type="String" />
            <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                Type="Int32" />
            <asp:Parameter DefaultValue="QA" Name="environment" Type="String" />
        </SelectParameters>
    </asp:SqlDataSource>

    <!-- DO NOT CHANGE THE COLUMN ORDER -->
    <asp:GridView ID="gvResultHistory" runat="server" 
        DataKeyNames="testRunId" 
        DataSourceID="sqlSingleTestCaseHistory" 
        OnRowDataBound="gvResultHistory_RowDataBound" 
        onrowcommand="gvResultHistory_RowCommand"
        AutoGenerateColumns="False" 
        GridLines="None"
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview" 
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvResultHistory_PageIndexChanged" 
        AlternatingRowStyle-CssClass="alt" >
        <Columns>
            <asp:BoundField DataField="testDate" HeaderText="Date" 
                SortExpression="testDate" ItemStyle-CssClass="TestDateColumn" ReadOnly="True" />
            
            <asp:BoundField HtmlEncode="False"  DataField="reasonForStatus" HeaderText="Reason for Status" 
                ItemStyle-CssClass="ReasonForStatusColumn" SortExpression="reasonForStatus" />
            
            <asp:BoundField HtmlEncode="False" DataField="stepsToReproduce" HeaderText="Steps To Reproduce" 
                ItemStyle-CssClass="StepsToReproduceColumn" SortExpression="stepsToReproduce" />
            
            <asp:hyperlinkfield headertext="Defect #" 
                  datatextfield="defectTicketNumber" 
                  DataNavigateUrlFields="defectTicketNumber"
                  ItemStyle-CssClass="DefectTicketColumn"
                  Target="_blank"
                  datanavigateurlformatstring=""/> 
            <asp:BoundField DataField="status" HeaderText="Status" 
                SortExpression="status" ReadOnly="True" ItemStyle-CssClass="StatusColumn"/>
            <asp:BoundField DataField="environment" HeaderText="Environment" 
                SortExpression="environment" ReadOnly="True" ItemStyle-CssClass="EnvironmentColumn"/>
            <asp:BoundField DataField="browserAbbreviation" 
                HeaderText="Browser" SortExpression="browserAbbreviation" 
                ReadOnly="True" ItemStyle-CssClass="BrowserColumn"/>
            <asp:BoundField DataField="testRunId" HeaderText="testRunId" 
                InsertVisible="False" ReadOnly="True" SortExpression="testRunId" 
                Visible="False" />
            <asp:BoundField DataField="testCaseId" HeaderText="testCaseId" 
                SortExpression="testCaseId" Visible="False" />
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="projectAbbreviation" SortExpression="projectAbbreviation" 
                Visible="False" />
            <asp:BoundField DataField="TestedByFullName" HeaderText="Tester" 
                SortExpression="TestedByFullName" ReadOnly="True" ItemStyle-CssClass="TestedByFullNameColumn" />
            <asp:BoundField DataField="testType" HeaderText="Test Type" 
                SortExpression="testType" ReadOnly="True" ItemStyle-CssClass="TestTypeColumn"/>
            <asp:BoundField DataField="testedBy" HeaderText="RawTester" 
                SortExpression="testedBy" ReadOnly="True" Visible="False"/>
            <asp:BoundField DataField="elapsedSeconds" HeaderText="Run Time (HH:MM:SS)" 
                SortExpression="elapsedSeconds" ReadOnly="True" Visible="True" ItemStyle-CssClass="ElapsedSecondsColumn" />
            <asp:BoundField DataField="automationNode" HeaderText="Automation Node" 
                SortExpression="automationNode" ReadOnly="True" Visible="True" ItemStyle-CssClass="AutomationNodeColumn"/>
            <asp:TemplateField HeaderText="Edit" ItemStyle-CssClass="EditButtonColumn">      
                <ItemTemplate> 
                    <asp:Button ID="EditButton" runat="server" PostBackUrl='<%# "~/FunctionalTesting/AddEditResults.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") +"&testRunId="+ Eval("testRunId") %>' Text="Details"></asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>

        </Columns>
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There is no test result history for the filters you have chosen.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast" position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
            </ContentTemplate>              
        </ajaxToolkit:TabPanel> 
            
        <%--********************* WORK ITEMS ***********************--%>          
        <ajaxToolkit:TabPanel ID="TabPanel3" HeaderText="Work Items" runat="server">  
            <ContentTemplate>  
                    <div class="NoDataSidebar">
                        <asp:image id="NoDataImage"   
                        imageurl="~/Images/comingsoon2.jpg"
                        alternatetext="Coming Soon" 
                        runat="server"
                         />
                        </div>
            </ContentTemplate>              
        </ajaxToolkit:TabPanel>
            
        <%--********************* DEFECTS ***********************--%>            
        <ajaxToolkit:TabPanel ID="TabPanel4" HeaderText="Defects" runat="server" >  
            <ContentTemplate>

            <asp:SqlDataSource ID="sqlDefectTickets" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                SelectCommand="SELECT testRunId, defectTicketNumber, max(testDate) testDate, projectAbbreviation, testCaseId, environment, reasonForStatus, browserAbbreviation FROM TestResults WHERE (projectAbbreviation = @projectAbbreviation) AND (testCaseId = @testCaseId) AND (defectTicketNumber <> '') AND (defectTicketNumber IS NOT NULL) GROUP BY defectTicketNumber, projectAbbreviation, testCaseId, testRunId, environment, reasonForStatus, browserAbbreviation ORDER BY testDate Desc, defectTicketNumber ASC">
                <SelectParameters>
                    <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                        Type="String" />
                    <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                        Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>

                
    <asp:GridView ID="gvTestCaseDefects" runat="server" 
        DataKeyNames="testRunId" 
        DataSourceID="sqlDefectTickets" 
        AutoGenerateColumns="False" 
        GridLines="None"
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview" 
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvDefectTickets_PageIndexChanged" 
        AlternatingRowStyle-CssClass="alt" >
        <Columns>
            <asp:hyperlinkfield headertext="Defect #" 
                  datatextfield="defectTicketNumber"
                    target="_blank" 
                  DataNavigateUrlFields="defectTicketNumber"
                  ItemStyle-CssClass="DefectTicketColumn"
                  datanavigateurlformatstring=""/> 
            
            <asp:TemplateField HeaderText="Status" ItemStyle-CssClass="StatusColumn">      
                <ItemTemplate> 
                    <asp:Label ID="DefectStatus" runat="server" Text="Coming Soon"></asp:Label>
                </ItemTemplate> 
            </asp:TemplateField>

            <asp:BoundField DataField="testDate" HeaderText="Test Date" 
                SortExpression="testDate" ItemStyle-CssClass="TestDateColumn" ReadOnly="True" />
            
            <asp:BoundField HtmlEncode="False"  DataField="reasonForStatus" HeaderText="Reason for Status" 
                ItemStyle-CssClass="ReasonForStatusColumn" SortExpression="reasonForStatus" />

            <asp:BoundField DataField="environment" HeaderText="Environment" 
                SortExpression="environment" ReadOnly="True" ItemStyle-CssClass="EnvironmentColumn"/>
            
            <asp:BoundField DataField="browserAbbreviation" 
                HeaderText="Browser" SortExpression="browserAbbreviation" 
                ReadOnly="True" ItemStyle-CssClass="BrowserColumn"/>
            
            <asp:TemplateField HeaderText="View Test Result" ItemStyle-CssClass="EditButtonColumn">      
                <ItemTemplate> 
                    <asp:Button ID="EditButton" runat="server" PostBackUrl='<%# "~/FunctionalTesting/AddEditResults.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") +"&testRunId="+ Eval("testRunId") %>' Text="Test Result"></asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>

        </Columns>
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no defects for this test case.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast" position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
          </ContentTemplate>              
        </ajaxToolkit:TabPanel>

        <%--********************* HISTORY ***********************--%>            
        <ajaxToolkit:TabPanel ID="TabPanel5" HeaderText="History" runat="server" >  
            <ContentTemplate>  
    
        <asp:SqlDataSource ID="sqlTestCasesHistory" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT *, dbo.FixEndlines(testCaseDescription) as fixed_endline_testCaseDescription, dbo.FixEndlines(testCaseSteps) as fixed_endline_testCaseSteps, dbo.FixEndlines(expectedResults) as fixed_endline_expectedResults, dbo.FixEndlines(testCaseNotes) as fixed_endline_testCaseNotes FROM [TestCasesHistory] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId)) order by dateLastUpdated desc">
        <SelectParameters>
            <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                Type="String" />
            <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

    <asp:GridView ID="gvUpdateHistory" runat="server" AutoGenerateColumns="False" 
        DataSourceID="sqlTestCasesHistory" 
        AllowPaging="True" AllowSorting="True" 
        CaptionAlign="Top" PageSize="50"
        CssClass="gvGlobalGridview" 
        Width="100%"
        AlternatingRowStyle-CssClass="alt">
        <%--ViewStateMode="Enabled" --%>
        <Columns>
            <asp:BoundField HtmlEncode="False" DataField="changeType" HeaderText="Change Type" 
                SortExpression="changeType" />
            <asp:BoundField DataField="dateLastUpdated" HeaderText="Date Changed" 
                SortExpression="dateLastUpdated" Visible="True" />
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_testCaseDescription" HeaderText="Description" 
                SortExpression="fixed_endline_testCaseDescription" />
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_testCaseSteps" HeaderText="Steps" 
                SortExpression="fixed_endline_testCaseSteps" />
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_expectedResults" HeaderText="Expected Results" 
                SortExpression="fixed_endline_expectedResults" />
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_testCaseNotes" HeaderText="Notes" 
                SortExpression="fixed_endline_testCaseNotes" Visible="True" />
            <asp:BoundField DataField="updatedBy" HeaderText="Updated By" 
                SortExpression="updatedBy" Visible="True" />
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
                No data available for the filters you have chosen.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast"  position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
            </ContentTemplate>              
        </ajaxToolkit:TabPanel>  
        
    <%--********************* AUTOMATION ***********************--%>          
        <ajaxToolkit:TabPanel ID="TabPanel6" HeaderText="Automation" runat="server">  
            <ContentTemplate>  
    <table>
        <tr>
            <td>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td>Test Case ID</td>
            <td>
                <asp:Label ID="lblAutomationTestCaseProject" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblAutomationTestCaseId" runat="server" Text='<%# Eval("testCaseId") %>' />
            </td>
        </tr>
        <tr>
            <td>Description</td><td>
                <asp:Label ID="lblDescription" runat="server" Width="400" Text=""></asp:Label></td>
        </tr>
        <tr>
        <td>AutoTestClass</td><td>
            <asp:TextBox ID="txtAutoTestClass" CssClass="textbox" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>autoMetaDataTable</td><td>
            <asp:TextBox ID="txtautoMetaDataTable" CssClass="textbox" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>autoMetaDataRow</td><td>
            <asp:TextBox ID="txtautoMetaDataRow" CssClass="textbox" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>Automated</td><td>
            <asp:DropDownList ID="ddlAutomated" runat="server"  
                CssClass="dropdown" 
                Width="414" >
            </asp:DropDownList></td>
        </tr>
        <tr>
        <td>Automation Reason</td><td>
            <asp:TextBox ID="txtAutomationReason" CssClass="textbox" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>Child Test Case ID</td><td>
            <asp:TextBox ID="txtChildTestCaseID" CssClass="textbox" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>                
            <asp:Button ID="btnInstallAutoTestCase" runat="server" Text="Install Automated Test Case"
             AutoPostBack="True" onclick="btnInstallAutoTestCase_Click" />
        </td>
        <td>
            <asp:Button ID="btnAddChild" runat="server" Text="Add Automated Child"
             AutoPostBack="True" onclick="btnAddChild_Click" />
        </td>

        </tr>
    </table>
    
<%--Child Automated Test Cases--%>
<asp:Panel ID="pnlChildAutomatedTestCases" runat="server" GroupingText="Child Automated Test Cases" CssClass="dashboardPanel">
    <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
    
    <asp:SqlDataSource ID="sqlChildAutomatedTestCases" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="" />
        
    <asp:ListView ID="lvChildAutomatedTestCases" runat="server" DataSourceID="sqlChildAutomatedTestCases" ondatabound="lvChildAutomatedTestCases_DataBound" >
    <emptydatatemplate>
        <div class="NoData">
            <asp:image id="NoDataImage"   
            imageurl="~/Images/NoData.png"
            alternatetext="No Data Image" 
            runat="server"
            Height="64" Width="64" />
            <br />
            There are no child test cases belonging to this parent automated test case.
        </div>
    </emptydatatemplate>
    <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
    
    <ItemTemplate>
    <tr class="TableData">
            <td><asp:Label ID="lblProjectAbbreviation" runat="server" Text='<%# Eval("projectAbbreviation") %>' /></td>
            <td><asp:Label ID="lblChildTestCaseId" runat="server" Text='<%# Eval("childTestCaseId") %>' /></td>
            <td><asp:Label ID="lblDescription" runat="server" Text='<%# Eval("testCaseDescription") %>' ToolTip='<%#Eval("testCaseDescription")%>' /></td>
            <td>                                
                <asp:ImageButton ID="btnRemoveChild" OnClick="btnRemoveChild_OnClick" runat="server" 
                                Width="24" Height="24" ImageUrl="~/Images/StopButton.png" 
                                OnClientClick="return confirm('Are you certain you want to remove this child test case from this automated parent test case?');"
                                CommandArgument='<%#Eval("childTestCaseId")%>' 
                                ImageAlign="Top" /> 

            </td>
    </tr>
    </ItemTemplate>
    <LayoutTemplate>
    <table id="tblServerList" runat="server" class="ServerListTable" width="100%">
        <tr id="Tr1" runat="server" class="TableHeader">
            <td id="Td1" runat="server">Project</td>
            <td id="Td2" runat="server">ID</td>
            <td id="Td6" runat="server">Description</td>
            <td id="Td7" runat="server">Remove</td>
        </tr>
        <tr id="ItemPlaceholder" runat="server">
        </tr>
    </table>
</LayoutTemplate>

    </asp:ListView>
        </asp:Panel>

            </ContentTemplate>              
        </ajaxToolkit:TabPanel>
            </ajaxToolkit:TabContainer>  

    </div>  
</asp:Content>



<%--**************************** LEFT RAIL CONTENT ****************************--%>
    
    <asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
    
        <%--********** Environments Dropdown **********--%>

    <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblEnvironmentDropdownTitle" runat="server" Text="Environment" /></div>
    </div>
    <p><asp:DropDownList ID="ddlEnvironments" runat="server" 
                DataSourceID="sqlEnvironments" 
                DataTextField="environment" 
                DataValueField="environment" 
                AppendDataBoundItems="False" 
                AutoPostBack="True"
                onselectedindexchanged="ddlEnvironments_SelectedIndexChanged" 
                CssClass="leftRailDropdown">
            </asp:DropDownList>
            <asp:SqlDataSource ID="sqlEnvironments" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT ProjectEnvironmentInfo.environment, Environments.sortOrder FROM ProjectEnvironmentInfo JOIN Environments ON ProjectEnvironmentInfo.environment = Environments.environment WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY sortOrder">
                <SelectParameters>
                    <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                        Type="String" />
                    <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                        Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
        </p>
        </div>
    
    <%--********** Browser Dropdown **********--%>
    <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblBrowserDropdownTitle" runat="server" Text="Browsers" /></div>
    </div>
    <p><asp:DropDownList ID="ddlBrowsers" runat="server" 
                DataSourceID="sqlBrowsers" 
                DataTextField="browserName" 
                DataValueField="browserAbbreviation" 
                AppendDataBoundItems="False" 
                AutoPostBack="True"
                onselectedindexchanged="ddlBrowsers_SelectedIndexChanged" 
                CssClass="leftRailDropdown">
            </asp:DropDownList>
            <%--  The browser selection list only includes browsers that are project-enabled AND have test results --%>
            <asp:SqlDataSource ID="sqlBrowsers" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                SelectCommand="SELECT TestResults.browserAbbreviation, Browsers.browserName, ProjectBrowserInfo.showBrowserColumn
                                FROM TestResults
                                INNER JOIN Browsers
                                ON TestResults.browserAbbreviation=Browsers.browserAbbreviation
                                INNER JOIN ProjectBrowserInfo
                                ON TestResults.browserAbbreviation = ProjectBrowserInfo.browserAbbreviation
                                WHERE (ProjectBrowserInfo.projectAbbreviation=@projectAbbreviation) 
                                AND (TestResults.testCaseId=@testCaseId) AND (ProjectBrowserInfo.showBrowserColumn = 1)
                                GROUP BY TestResults.browserAbbreviation, Browsers.browserName, ProjectBrowserInfo.showBrowserColumn
                                ORDER BY TestResults.browserAbbreviation">
                <SelectParameters>
                    <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />
                    <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" Type="Int32" />
                </SelectParameters>
            </asp:SqlDataSource>
        </p>
        </div>

    <%--********** Groups **********--%>

  
<ajaxToolkit:Accordion ID="Accordion1" 
    runat="server"
    HeaderCssClass="accordionHeader"  
    HeaderSelectedCssClass="accordionHeaderSelected"  
    ContentCssClass="accordionContent"
    >

<Panes>
    <ajaxToolkit:AccordionPane ID="AccordionPane1" runat="server">
        <Header>Groups</Header>
        <Content>
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
                    <div class="NoDataSidebar">
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
                    <div class="CTDetailsBoxText">
                        <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromGroup" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this group?');" Height="14" Width="14" ToolTip="Delete this test case from this group." CommandArgument='<%# Eval("groupTestAbbreviation") %>'></asp:ImageButton>
                        <a href="TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&group=<%# Eval("groupTestAbbreviation") %>">
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
            <br />
            <asp:DropDownList ID="ddlGroupTests" runat="server" 
                AutoPostBack="True" 
                AppendDataBoundItems="False" 
                DataSourceID="sqlGroupTests"
                DataTextField="groupTestName"
                DataValueField="groupTestAbbreviation"
                tooltip="Add test case to group"
                Width="218px">
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
        </Content>
    </ajaxToolkit:AccordionPane>


        <%--********** Releases **********--%>

    <ajaxToolkit:AccordionPane ID="AccordionPane2" runat="server">
        <Header>Releases</Header>
        <Content>
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
                    <div class="NoDataSidebar">
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
                <div class="CTDetailsBoxText">
                <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromRelease" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this release?');" Height="12" Width="12" ToolTip="Remove this test case from this release." CommandArgument='<%# Eval("release") %>'></asp:ImageButton>
                <a href="TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&release=<%# Eval("release") %>">
                <asp:Label ID="lblRelease" runat="server" Text='<%# Eval("release") %>' /></a>
                <br /> 
                <b>Release Date: </b><asp:Label ID="lblReleaseDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("releaseDate")).ToString("d")%>'></asp:Label>
                </div>
            </ItemTemplate>
                <LayoutTemplate>
                    <div ID="itemPlaceholderContainer" runat="server" style="">
                        <div runat="server" id="itemPlaceholder" />
                    </div>
                </LayoutTemplate>
            </asp:ListView>
            <br />
     <asp:DropDownList ID="ddlReleases" runat="server" 
            AutoPostBack="True" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlReleases"
            DataTextField="release"
            DataValueField="release"
            tooltip="Add test case to release"
            Width="218px">
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
        </Content>
    </ajaxToolkit:AccordionPane>


        <%--********** Sprints **********--%>

    <ajaxToolkit:AccordionPane ID="AccordionPane3" runat="server">
        <Header>Sprints</Header>
        <Content>
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
                <div class="NoDataSidebar">
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
                    <div class="CTDetailsBoxText">
                        <asp:ImageButton ID="ibtnRemove" CommandName="RemoveFromSprint" runat="server" ImageUrl="~/Images/delete.png" OnClientClick="return confirm('Are you certain you want to remove this test case from this sprint?');" Height="12" Width="12" ToolTip="Remove this test case from this sprint." CommandArgument='<%# Eval("sprint") %>'></asp:ImageButton>
                        <a href="TestCases.aspx?project=<%# Eval("projectAbbreviation") %>&sprint=<%# Eval("sprint") %>">
                            <asp:Label ID="lblSprint" runat="server" Text='<%# Eval("sprint") %>' /></a>
                        <br /> 
                        <b>Start Date: </b><asp:Label ID="lblSprintStartDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("sprintStartDate")).ToString("d")%>'></asp:Label>
                        <br />                    
                        <b>End Date: </b><asp:Label ID="lblSprintEndDate" runat="server" DataFormatString="{0:d}" HtmlEncode="false" Text='<%# Convert.ToDateTime(Eval("sprintEndDate")).ToString("d")%>'></asp:Label>
                    </div>
                </ItemTemplate>
                <LayoutTemplate>
                    <div ID="itemPlaceholderContainer" runat="server" style="">
                        <div runat="server" id="itemPlaceholder" />
                    </div>
                </LayoutTemplate>
            </asp:ListView>  
            <br />
            <asp:DropDownList ID="ddlSprints" runat="server" 
                AppendDataBoundItems="False" 
                DataSourceID="sqlSprints"
                DataTextField="sprint"
                DataValueField="sprint"
                tooltip="Add test case to sprint"
                Width="218px">
            </asp:DropDownList>

            <asp:SqlDataSource ID="sqlSprints" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                SelectCommand="SELECT [projectAbbreviation], [sprint] FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprint] DESC">
                <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                Type="String" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:Button ID="btnAddSprint" runat="server" Text="Add" Width="80px" tooltip="Add test case to sprint" onclick="btnAddSprint_Click" ></asp:Button>
        </Content>
    </ajaxToolkit:AccordionPane>
</Panes>
</ajaxToolkit:Accordion>
</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
    

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">


</asp:Content>


