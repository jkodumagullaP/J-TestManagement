 <%@ Page Title="QA Dashboard" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CTWebsite._Default" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
    
    <%--********** View Dropdown**********--%>
    
<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblViewDropdownTitle" runat="server" Text="View" /></div>
    </div>
    <p><asp:DropDownList ID="ddlManualOrAutomated" runat="server" 
    AppendDataBoundItems="False" 
    AutoPostBack="True"
    onselectedindexchanged="ddlManualOrAutomated_SelectedIndexChanged" 
    CssClass="leftRailDropdown" >
    <asp:ListItem>All</asp:ListItem>
    <asp:ListItem>Automated</asp:ListItem>
    <asp:ListItem>Manual</asp:ListItem>
    </asp:DropDownList></p>
</div>
   
    <%--********** Project Dropdown **********--%>
    
    <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblProjectDropdownTitle" runat="server" Text="Project" /></div>
    </div>
    <p><asp:DropDownList ID="ddlProjects" runat="server"  
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
    
<%--********** Releases Dropdown **********--%>

    <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lvlReleaseDropdownTitle" runat="server" Text="Release" /></div>
    </div>
    <p><asp:DropDownList ID="ddlReleases" runat="server" 
                    onselectedindexchanged="ddlReleases_SelectedIndexChanged"
                    AutoPostBack="True" 
                    AppendDataBoundItems="False" 
                    DataSourceID="sqlReleases"
                    DataTextField="release"
                    DataValueField="release" 
                    CssClass="leftRailDropdown">
                </asp:DropDownList>

                <asp:SqlDataSource ID="sqlReleases" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT * FROM [Releases] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [release] DESC">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" 
                            PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </p>
        </div>


<%--********** Sprints Dropdown **********--%>
    
  <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblSprintsDropdownTitle" runat="server" Text="Sprint" /></div>
    </div>
    <p><asp:DropDownList ID="ddlSprints" runat="server" 
                    onselectedindexchanged="ddlSprints_SelectedIndexChanged"
                    AutoPostBack="True" 
                    AppendDataBoundItems="False" 
                    DataSourceID="sqlSprints"
                    DataTextField="sprint"
                    DataValueField="sprint"
                    CssClass="leftRailDropdown">
                </asp:DropDownList>

                <asp:SqlDataSource ID="sqlSprints" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT * FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprint] DESC">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" 
                            PropertyName="SelectedValue" Type="String" />
                    </SelectParameters>
                </asp:SqlDataSource>
            </p>
        </div>

<%--********** Group Dropdown **********--%>

   <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblGroupsDropdownTitle" runat="server" Text="Group" /></div>
    </div>
    <p><asp:DropDownList ID="ddlGroupTest" runat="server" 
                    onselectedindexchanged="ddlGroupTest_SelectedIndexChanged"
                    AutoPostBack="True" 
                    AppendDataBoundItems="False" 
                    DataSourceID="sqlGroupTests"
                    DataTextField="groupTestName"
                    DataValueField="groupTestAbbreviation"
                    CssClass="leftRailDropdown">
                </asp:DropDownList>

                <asp:SqlDataSource ID="sqlGroupTests" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT [projectAbbreviation], [groupTestAbbreviation], [groupTestName] FROM [GroupTests] WHERE ([projectAbbreviation] = @projectAbbreviation) and (personalGroupOwner is null or personalGroupOwner = @userName) ORDER BY [groupTestName]">
                    <SelectParameters>
                        <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
                        <%--This has to be added via Page_Load--%>
                        <%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
                    </SelectParameters>
                </asp:SqlDataSource>
            </p>
        </div>

   
<%--********** Environments Dropdown **********--%>

    <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblEnvironmentDropdownTitle" runat="server" Text="Environment" /></div>
    </div>
    <p><asp:DropDownList ID="ddlEnvironment" runat="server" 
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

 <%--********** Date Range **********--%>

   <div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblDateRangeTitle" runat="server" Text="Date Range" /></div>
    </div>
            <table class="tblDateRange">
                <tr>
                    <td colspan="3"><asp:Label id="lblCalendarError" ForeColor="Red" visible="false" Text="Start date was after end date" Runat="server" /></td>
                </tr>
                <tr>
                    <td class="datetext">Start Date: </td>
                    <td><asp:TextBox id="txtStartDate" Text="Start Date" Runat="server" Width="100px" /></td>
                    <td><asp:ImageButton id="imgStartDateSelection"  ImageUrl="~/Images/calendar.png" CssClass="calendarButton" height="20" width="20" runat="server" OnClick="imgStartDateSelection_click" /></td>
                </tr>
                <tr>
                    <td colspan="3"><asp:Calendar id="calStartDate" visible="false" OnSelectionChanged="calStartDate_SelectionChanged" Runat="server" /></td>
                </tr>
                <tr>
                    <td class="datetext">End Date: </td>
                    <td><asp:TextBox id="txtEndDate" Text="End Date" Width="100px" Runat="server" /></td>
                    <td><asp:ImageButton id="imgEndDateSelection"  ImageUrl="~/Images/calendar.png" CssClass="calendarButton" height="20" width="20" runat="server" OnClick="imgEndDateSelection_click" /></td>
                </tr>
                <tr>
                    <td colspan="3"><asp:Calendar id="calEndDate" visible="false" OnSelectionChanged="calEndDate_SelectionChanged" Runat="server" /></td>
                </tr>
                </table>
        </div>

    <asp:Button ID="btnReset" runat="server" Text="Reset Filters" onclick="btnReset_Click" />   

</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
   
    <table width="100%" class="DashboardTable">
        <tr>
            <td colspan="3">
                <h1>Test Result Metrics</h1>
            </td>
        </tr>
        <thead>
            <tr>
                <td class="dashboardtitle" colspan ="3">
                    <asp:Label ID="lblDashboardTitle" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
        </thead>
        <tr>
        <%--**************************** CURRENT STATUS AREA ****************************--%>
            <td colspan="3">

            <!-- Browser Status Table-->
                <asp:Table ID="TestResultMetrics" runat="server" CssClass="CurrentStatusTable">
            
                    <asp:TableHeaderRow ID="TestResultMetrics_Header" runat="server">
                        <asp:TableCell></asp:TableCell>
                        <asp:TableCell>Pass</asp:TableCell>
                        <asp:TableCell>Fail</asp:TableCell>
                        <asp:TableCell>Other</asp:TableCell>
                        <asp:TableCell>Totals</asp:TableCell>
                    </asp:TableHeaderRow>
    
                    <asp:TableRow ID="TestResultMetrics_CHROME" runat="server">
                        <asp:TableCell CssClass="StatusLabel">Chrome</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="Chrome_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="Chrome_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="Chrome_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="Chrome_Total" runat="server" Text="Label"></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_FF" runat="server">
                        <asp:TableCell CssClass="StatusLabel">Firefox</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="Firefox_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="Firefox_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="Firefox_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="Firefox_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_IE8" runat="server">
                        <asp:TableCell CssClass="StatusLabel">IE8</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="IE8_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="IE8_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="IE8_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="IE8_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_IE9" runat="server">
                        <asp:TableCell CssClass="StatusLabel">IE9</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="IE9_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="IE9_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="IE9_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="IE9_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_IE10" runat="server">
                        <asp:TableCell CssClass="StatusLabel">IE10</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="IE10_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="IE10_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="IE10_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="IE10_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_IE11" runat="server">
                        <asp:TableCell CssClass="StatusLabel">IE11</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="IE11_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="IE11_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="IE11_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="IE11_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_MACSAF" runat="server">
                        <asp:TableCell CssClass="StatusLabel">Safari (Mac)</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="MACSAF_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="MACSAF_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="MACSAF_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="MACSAF_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableRow ID="TestResultMetrics_WINSAF" runat="server">
                        <asp:TableCell CssClass="StatusLabel">Safari (Windows)</asp:TableCell>
                        <asp:TableCell CssClass = "Pass"><asp:Label ID="WINSAF_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Fail"><asp:Label ID="WINSAF_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Other"><asp:Label ID="WINSAF_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell CssClass = "Total"><asp:Label ID="WINSAF_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableRow>

                    <asp:TableFooterRow ID="TestResultMetrics_Totals" runat="server">
                        <asp:TableCell>Totals</asp:TableCell>
                        <asp:TableCell><asp:Label ID="Totals_Pass" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:Label ID="Totals_Fail" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:Label ID="Totals_Other" runat="server" Text=""></asp:Label></asp:TableCell>
                        <asp:TableCell><asp:Label ID="Totals_Total" runat="server" Text=""></asp:Label></asp:TableCell>
                    </asp:TableFooterRow>

                </asp:Table>
    </td>
            
    <%--**************************** CHART CODE ****************************--%><%--            
        <td class="ChartTestCaseStatusCount">
                <asp:Chart ID="chrtStageStatusCount"  runat="server" Width="200px" 
                    ondatabound="chrtStageStatusCount_DataBound" Height="150px">
                    <Series></Series>
                    <ChartAreas>
                        <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                    </ChartAreas>
                    <Titles>
                        <asp:Title Alignment="TopCenter" Name="Test Cases Status" 
                            Text="Test Case Status" Font="Microsoft Sans Serif, 12pt">
                        </asp:Title>
                    </Titles>
                </asp:Chart>
            </td>
--%>        </tr>
        <tr>
            <td colspan="3" class="FilterWarning">
                * These metrics are affected by the filter dropdowns in the left column where applicable.
            </td>
        </tr>
        <tr>
            <td colspan="3"><hr /></td>
        </tr>
        <tr>
            <td colspan="3">
                <h1>Automation Metrics</h1>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <table class="AutomatedContainerTable">
                    <tr>
                        <td class="AutomatedTableCellLeft">
                            <table class="AutomationMetricsLeft">
                                <tr class="Automated_Numerics">
                                    <td ># of Test Cases that can be Automated in the Future:</td>
                                    <td><asp:Label CssClass="Automated_Future" ID="Automated_Future" runat="server" Text="Label"                                  
                                            ToolTip="This is the # of test cases that are not currently automated but can be automated in the future."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Numerics">
                                    <td># of Test Cases that cannot be Automated:</td>
                                    <td><asp:Label ID="Automated_No" runat="server" Text="Label"                                     
                                            ToolTip="The number of test cases that cannot be automated."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Numerics">
                                    <td># of Test Cases already Directly Automated:</td>
                                    <td><asp:Label ID="Automated_Yes" runat="server" Text="Label" 
                                            ToolTip="The number of test cases that have already been automated."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Numerics">
                                    <td># of Child Automated Test Cases:</td>
                                    <td><asp:Label ID="Automated_Child" runat="server" Text="Label" 
                                            ToolTip="The number of test cases that get automatically passed when another automated test passes."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Numerics">
                                    <td># of Test Cases not Evaluated for Automation:</td>
                                    <td><asp:Label ID="Automated_Blank" runat="server" Text="Label" 
                                            ToolTip="The number of test cases that have not been evaluated, so we do not know if it can be automated or not."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Totals">
                                    <td># of Total Automated Test Cases:</td>
                                    <td><asp:Label ID="Automated_Total" runat="server" Text="Label" 
                                            ToolTip="The total number of test cases that have been automated"></asp:Label></td>
                                </tr>
                                <tr class="Automated_Totals">
                                    <td># of Total Manual Test Cases:</td>
                                    <td><asp:Label ID="Manual_Total" runat="server" Text="Label" 
                                            ToolTip="The total number of test cases that have to be executed manually."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Totals2">
                                    <td># of Total Test Cases:</td>
                                    <td><asp:Label ID="TestCases_Total" runat="server" Text="Label" 
                                            ToolTip="The total number of test cases that have to be executed manually."></asp:Label></td>
                                </tr>
                            </table>
                        </td>

                        <td class="AutomatedTableCellRight">
                            <table class="AutomationMetricsRight">
                                <tr class="Automated_Percentages1">
                                    <td>% of Test Cases Currently Covered By Automation:</td>
                                    <td><asp:Label ID="Current_Percentage" runat="server" Text="Label" 
                                            ToolTip="The percentage of test cases currently automated."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Percentages1">
                                    <td>Maximum % of Test Cases That Can be Automated:</td>
                                    <td><asp:Label ID="Possible_Coverage" runat="server" Text="Label" 
                                            ToolTip="When all automation is complete, this is the percentage of the total test cases that will be automated."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Percentages2">
                                    <td>% of Possible Automated Test Cases Completed (Work completed):</td>
                                    <td><asp:Label ID="Possible_Completed" runat="server" Text="Label" 
                                            ToolTip="Out of all test cases that can be automated, this is the percentage that is complete."></asp:Label></td>
                                </tr>
                                <tr class="Automated_Percentages2">
                                    <td colspan="2" class="FilterWarning">These percentages use only the evaluated test cases. See the "# of Test Cases not Evaluated for Automation" field in the left table. When this # is at 0, the percentages above are 100% accurate. </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="FilterWarning">
                * These metrics are only affected by the project filter in the left column.
            </td>
        </tr>
        <tr>
            <td colspan="3"><hr /></td>
        </tr>
        <tr>
            <td>
                <h1>Test Case Metrics</h1>
            </td>
        </tr>
        <tr>
            <td>
                 <%--**************************** PANEL: METRICS BY Project ****************************--%>
                <asp:Panel ID="Panel1" runat="server" GroupingText="Metrics By Project" 
                     CssClass="dashboardPanel">
                    <table>
                        <tr>
                            <td>Manual Cases Written:</td>
                            <td><asp:Label ID="projectManualCasesWritten" runat="server" Text="Label" 
                                    
                                    ToolTip="Manual Cases Written is affected by the Project, Release, Sprint, Group, and Date Range filters. It is not affected by the Environment filter."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Cases Written:</td>
                            <td><asp:Label ID="projectAutomatedCasesWritten" runat="server" Text="Label"                                     
                                    ToolTip="Automated Cases Written is affected by the Project, Release, Sprint, Group, and Date Range filters. It is not affected by the Environment filter."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Manual Cases Executed:</td>
                            <td><asp:Label ID="projectManualCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Manual Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Cases Executed:</td>
                            <td><asp:Label ID="projectAutomatedCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Automated Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Manual Defects Discovered:</td>
                            <td><asp:Label ID="projectManualDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Manual Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Defects Discovered:</td>
                            <td><asp:Label ID="projectAutomatedDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Automated Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns."></asp:Label></td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td>
                 <%--**************************** PANEL: METRICS BY USER ****************************--%>
                <asp:Panel ID="Panel2" runat="server" GroupingText="Metrics By User" 
                     CssClass="dashboardPanel">
                    <table>
                        <tr>
                            <td>Manual Cases Written:</td>
                            <td><asp:Label ID="userManualCasesWritten" runat="server" Text="Label" 
                                    ToolTip="Manual Cases Written is affected by the Project, Release, Sprint, Group, and Date Range filters. It is also affected by the user filter below. It is not affected by the Environment filter."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Cases Written:</td>
                            <td><asp:Label ID="userAutomatedCasesWritten" runat="server" Text="Label" 
                                    ToolTip="Automated Cases Written is affected by the Project, Release, Sprint, Group, and Date Range filters. It is also affected by the user filter below. It is not affected by the Environment filter."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Manual Cases Executed:</td>
                            <td><asp:Label ID="userManualCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Manual Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters. It is also affected by the user filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Cases Executed:</td>
                            <td><asp:Label ID="userAutomatedCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Automated Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters. It is also affected by the user filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Manual Defects Discovered:</td>
                            <td><asp:Label ID="userManualDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Manual Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns. It is also affected by the user filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Defects Discovered:</td>
                            <td><asp:Label ID="userAutomatedDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Automated Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns. It is also affected by the user filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:DropDownList ID="ddlUsers" runat="server" DataSourceID="sqlUsers" 
                                DataTextField="UserFullName" DataValueField="UserName" 
                                AppendDataBoundItems="False" 
                                AutoPostBack="True">
                                </asp:DropDownList>
                                 <asp:SqlDataSource ID="sqlUsers" runat="server" 
                                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                                SelectCommand="select UserName,  userFirstName + ' ' + userLastName as UserFullName   from aspnet_users   join userprofiles on aspnet_users.UserID = userprofiles.userid">
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td>
                 <%--**************************** PANEL: METRICS BY Browser ****************************--%>
                <asp:Panel ID="browserPanel" runat="server" GroupingText="Metrics By Browser" 
                     CssClass="dashboardPanel">
                    <table>
                        <tr>
                            <td>Manual Cases Executed:</td>
                            <td><asp:Label ID="browserManualCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Manual Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters. It is also affected by the browser filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Cases Executed:</td>
                            <td><asp:Label ID="browserAutomatedCasesExecuted" runat="server" Text="Label" 
                                    ToolTip="Automated Cases Executed is affected by the Project, Release, Sprint, Group, Environment, and Date Range filters. It is also affected by the browser filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Manual Defects Discovered:</td>
                            <td><asp:Label ID="browserManualDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Manual Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns. It is also affected by the browser filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td>Automated Defects Discovered:</td>
                            <td><asp:Label ID="browserAutomatedDefectsDiscovered" runat="server" Text="Label" 
                                    ToolTip="Automated Defects Discovered is affected by the Project, Release, Sprint, Group, Environment, and Date Range dropdowns. It is also affected by the browser filter below."></asp:Label></td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <asp:DropDownList ID="ddlBrowsers" runat="server" DataSourceID="sqlBrowsers" 
                                DataTextField="browserName" DataValueField="browserAbbreviation" 
                                AppendDataBoundItems="False" 
                                AutoPostBack="True">
                                </asp:DropDownList>
                                 
                                <asp:SqlDataSource ID="sqlBrowsers" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                                    SelectCommand="SELECT Browsers.browserAbbreviation, Browsers.browserName, ProjectBrowserInfo.showBrowserColumn FROM ProjectBrowserInfo INNER JOIN Browsers ON Browsers.browserAbbreviation=ProjectBrowserInfo.browserAbbreviation WHERE (ProjectBrowserInfo.projectAbbreviation = @projectAbbreviation) AND (ProjectBrowserInfo.showBrowserColumn = 1) ORDER BY Browsers.browserName">
                                    <SelectParameters>
                                        <asp:ControlParameter ControlID="LeftColumnContent:ddlProjects" Name="projectAbbreviation" 
                                            PropertyName="SelectedValue" Type="String" />
                                    </SelectParameters>
                                </asp:SqlDataSource>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="3" class="FilterWarning">
                * These metrics are affected by the filter dropdowns in the left column where applicable.
            </td>
        </tr>
    </table>
</asp:Content>
<%--**************************** RIGHT RAIL CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">




 <%--**************************** PANEL: Automated Tests In Progress ****************************--%>
    <asp:Panel ID="pnlInProgress" runat="server" GroupingText="Automated Tests In Progress" CssClass="dashboardRightPanel">    
      <div class="multicolumnTestCaseList">
        <asp:SqlDataSource ID="sqlInProgress" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>

        <asp:ListView ID="lvInProgress" runat="server" 
           DataSourceID="sqlInProgress" GroupItemCount="3">
        
        <emptydatatemplate>
            <div class="NoDataGood">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no automated tests in progress.
            </div>
        </emptydatatemplate>
           <LayoutTemplate>
              <table>
                 <tr>
                    <td>
                       <table border="0" cellpadding="5" id="tblInProgress">
                          <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                       </table>
                    </td>
                 </tr>
              </table>
           </LayoutTemplate>

           <GroupTemplate>
              <tr>
                 <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
              </tr>
           </GroupTemplate>

           <ItemTemplate>
              <td>
                    <a href="/FunctionalTesting/TestDetails.aspx?project=<%# Eval("projectAbbreviation") %>&testCase=<%# Eval("testCaseId") %>">
                        <asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' /></a>
              </td>
           </ItemTemplate>
        </asp:ListView> 
    </div>    
</asp:Panel>
        
<%--********** Automated Tests In Queue **********--%>    
    <asp:Panel ID="pnlInQueue" runat="server" GroupingText="Automated Tests In Queue" CssClass="dashboardRightPanel">    
      <div class="multicolumnTestCaseList">
        <asp:SqlDataSource ID="sqlInQueue" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>
        
        <asp:ListView ID="lvInQueue" runat="server" 
           DataSourceID="sqlInQueue" GroupItemCount="3">
        <emptydatatemplate>
            <div class="NoDataGood">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no automated tests in queue.
            </div>
        </emptydatatemplate>
           <LayoutTemplate>
              <table>
                 <tr>
                    <td>
                       <table border="0" cellpadding="5">
                          <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                       </table>
                    </td>
                 </tr>
              </table>
           </LayoutTemplate>

           <GroupTemplate>
              <tr>
                 <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
              </tr>
           </GroupTemplate>

           <ItemTemplate>
              <td>
                    <a href="/FunctionalTesting/TestDetails.aspx?project=<%# Eval("projectAbbreviation") %>&testCase=<%# Eval("testCaseId") %>">
                        <asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' /></a>
              </td>
           </ItemTemplate>
        </asp:ListView> 
    </div>
    </asp:Panel>
    
<%--********** Test Scripts Needing Updated - Automated **********--%>    
    <asp:Panel ID="pnlScriptsNeedUpdated" runat="server" GroupingText="Scripts Needing Updated" CssClass="dashboardRightPanel">    
      <div class="multicolumnTestCaseList">
        <asp:SqlDataSource ID="sqlNeedsUpdatedAuto" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>
       
        <asp:ListView ID="lvNeedsUpdatedAuto" runat="server" 
           DataSourceID="sqlNeedsUpdatedAuto" GroupItemCount="3">
        <emptydatatemplate>
            <div class="NoDataGood">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no automated test scripts needing updated.
            </div>
        </emptydatatemplate>

           <LayoutTemplate>
              <table>
                 <tr>
                    <td>
                       <table border="0" cellpadding="5">
                          <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                       </table>
                    </td>
                 </tr>
              </table>
           </LayoutTemplate>

           <GroupTemplate>
              <tr>
                 <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
              </tr>
           </GroupTemplate>

           <ItemTemplate>
              <td>
                    <a href="/FunctionalTesting/TestDetails.aspx?project=<%# Eval("projectAbbreviation") %>&testCase=<%# Eval("testCaseId") %>&index=5">
                        <asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' /></a>
              </td>
           </ItemTemplate>
        </asp:ListView> 
    </div>
        </asp:Panel>
    
    
<%--********** Test Cases Needing Updated **********--%>    
    <asp:Panel ID="pnlCasesNeedUpdated" runat="server" GroupingText="Test Cases Needing Updated" CssClass="dashboardRightPanel">    
      <div class="multicolumnTestCaseList">
        <asp:SqlDataSource ID="sqlTestCaseNeedsUpdated" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="">
        </asp:SqlDataSource>
        <asp:ListView ID="lvTestCaseNeedsUpdated" runat="server" 
           DataSourceID="sqlTestCaseNeedsUpdated" GroupItemCount="3">
        <emptydatatemplate>
            <div class="NoDataGood">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no test cases needing updated.
            </div>
        </emptydatatemplate>        
           <LayoutTemplate>
              <table>
                 <tr>
                    <td>
                       <table border="0" cellpadding="5">
                          <asp:PlaceHolder runat="server" ID="groupPlaceHolder"></asp:PlaceHolder>
                       </table>
                    </td>
                 </tr>
              </table>
           </LayoutTemplate>

           <GroupTemplate>
              <tr>
                 <asp:PlaceHolder runat="server" ID="itemPlaceHolder"></asp:PlaceHolder>
              </tr>
           </GroupTemplate>

           <ItemTemplate>
              <td>
                    <a href="/FunctionalTesting/TestDetails.aspx?project=<%# Eval("projectAbbreviation") %>&testCase=<%# Eval("testCaseId") %>">
                        <asp:Label ID="lblItemProjectHeader" runat="server" Text='<%# Eval("projectAbbreviation") %>' />-<asp:Label ID="lblItemTestCaseIdHeader" runat="server" Text='<%# Eval("testCaseId") %>' /></a>
              </td>
           </ItemTemplate>
        </asp:ListView> 
    </div>
        </asp:Panel>
    <div><hr /></div>
</asp:Content>