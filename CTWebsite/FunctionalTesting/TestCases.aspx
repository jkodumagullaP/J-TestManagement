<%@ Page Title="Test Cases" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TestCases.aspx.cs" Inherits="CTWebsite.FunctionalTesting.TestCases" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .ExportToExcel
        {}
    </style>
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

<%--********** Releases Dropdown **********--%>

<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblReleaseDropdownTitle" runat="server" Text="Release" /></div>
    </div>
    <p>
        <asp:DropDownList ID="ddlReleases" runat="server" 
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
        <div class="left"><asp:label Id="lblSprintDropdownTitle" runat="server" Text="Sprint" /></div>
    </div>
        <p>
            <asp:DropDownList ID="ddlSprints" runat="server" 
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
        <div class="left"><asp:label Id="lblGroupDropdownTitle" runat="server" Text="Group" /></div>
    </div>
    <p>
        <asp:DropDownList ID="ddlGroupTest" runat="server" 
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


<%--********** Keyword Search **********--%>

<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblKeywordSearchTitle" runat="server" Text="Search" /></div>
    </div>
    <p class="keywordSearch">
        <asp:textbox CssClass="leftRailTextbox" id="txtKeywordSearchTextBox" runat="server" ></asp:textbox> 
        <asp:ImageButton ID="btnKeywordSearchSubmit" runat="server" 
            CssClass="btnSearch" CommandName="submit" Height="20px" 
            ImageUrl="~/Images/search.png" Width="20px" ToolTip="Search" 
            onclick="btnKeywordSearchSubmit_Click" />
    </p>
    <div class="message">*You may enter multiple keywords.</div>
</div>

<%--********** Reset Button **********--%>
    <asp:Button ID="btnReset" runat="server" Text="Reset Filters" onclick="btnReset_Click" />   

</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<%--Hidden Value to determine project page--%>
<asp:HiddenField ID="hdnProject" runat="server" Value="" />
    <%--********** Image Menu **********--%>
 
<table class="testCaseListIcons">
    <tr>
        <td><h1>Test Case List</h1></td>
        <td>
            <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
            <div class="imgButtonMenu">
                <asp:ImageButton ID="btnMassPass" runat="server"
                    Height="30px"
                    ImageUrl="~/Images/masspass.png" Width="30px"
                    ToolTip="Automatically Pass Selected Tests in all supported browsers" onclick="MassPass"
                    Visible="true" />
        
                <asp:ImageButton ID="btnAddToAutomatedQueue" runat="server" 
                    CssClass="AddToQueue" Height="30px" 
                    ImageUrl="~/Images/AutoQueueStart.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to Automated Queue" onclick="AddToAutomatedQueue" 
                    Visible="False" />

                <asp:ImageButton ID="btnRemoveFromAutomatedQueue" runat="server" 
                    CssClass="AddToQueue" Height="30px" 
                    ImageUrl="~/Images/AutoQueueStop.png" Width="30px" 
                    ToolTip="Remove Selected Test Cases from Automated Queue" onclick="RemoveFromAutomationQueue" 
                    Visible="False" />

                <asp:ImageButton ID="btnExportToExcel" runat="server" 
                    CssClass="ExportToExcel" Height="30px" 
                    ImageUrl="~/Images/ExportToExcel2.png" Width="30px" 
                    ToolTip="Create Test Case Import Sheet with Selected Test Cases" onclick="ExportToExcel" TabIndex="1" />

                <asp:ImageButton ID="btnAddToRelease" runat="server" 
                    CssClass="DownloadExcel" Height="30px" 
                    ImageUrl="~/Images/AddToRelease2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Release" onclick="AddToRelease" />

                <asp:ImageButton ID="btnAddToSprint" runat="server" 
                    CssClass="DownloadExcel" Height="30px" 
                    ImageUrl="~/Images/AddToSprint2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Sprint" onclick="AddToSprint" />
        
                <asp:ImageButton ID="btnAddToGroup" runat="server" 
                    CssClass="DownloadExcel" Height="30px" 
                    ImageUrl="~/Images/AddToGroup2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Group" onclick="AddToGroup" />
        
                <asp:ImageButton ID="btnAssign" runat="server" 
                    CssClass="Assign" Height="30px" 
                    ImageUrl="~/Images/Assign2.png" Width="30px" 
                    ToolTip="Assign Selected Test Cases to a User" onclick="AssignTestCase" 
                    Visible="True" />

                <asp:ImageButton ID="btnBulkDelete" runat="server" 
                    CssClass="DownloadExcel" Height="30px" 
                    ImageUrl="~/Images/Delete.png" Width="30px" 
                    ToolTip="Delete Selected Test Cases" onclick="BulkDelete"
                    OnClientClick="if (!confirm('Deleting these test cases will also delete all test results associated with it and remove it from the groups, sprints and releases it currently belongs to. Deleted test cases and their results as well its releases, sprints, and group associations can be restored via the admin page. Are you certain you want to delete this test case?')){return false;}" />
            </div>
            <div class="imgButtonMenu">
                <asp:Label ID="lblPleaseSelectAProject" ForeColor="Red" runat="server" Text="Please select a project first"></asp:Label>
                <asp:DropDownList ID="ddlAddToaBunchOfThings" 
                    runat="server" 
                    DataSourceID="sqlAddToaBunchOfThings" 
                    AutoPostBack="True" 
                    onselectedindexchanged="ddlAddToaBunchOfThings_SelectedIndexChanged">
            </asp:DropDownList>
            </div>
            <asp:SqlDataSource ID="sqlAddToaBunchOfThings" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                SelectCommand="">
            </asp:SqlDataSource>

        </td>
    </tr>
</table>     
 
<div>
    <asp:label Id="lblLegend" runat="server" Text="Test Cases in light grey indicate that they are not active. Test cases in red indicate that they need updating. As long as a test case is inactive or needs updating, test results can not be applied to it nor can its automated test be ran. You can update its status on its detail page." />
</div>
<div>
    <asp:label Id="lblCrystalTestCases" runat="server" Text="" Forecolor="red" Font-Bold="True" />
</div>
       
    <%--********** Gridview **********--%>
    <asp:SqlDataSource ID="sqlTestCasesByProject" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="" onselected="sqlTestCasesByProject_Selected">
    </asp:SqlDataSource>

    <%--<asp:button id="myButton" runat="server" OnClick="myButton_Click" style="visible:hidden;" />--%>
    <!-- DO NOT CHANGE THE COLUMN ORDER -->
    <asp:GridView ID="gvTestCaseList" runat="server" 
        DataKeyNames="projectAbbreviation,testCaseId" 
        DataSourceID="sqlTestCasesByProject"  
        OnRowDataBound="gvTestCaseList_RowDataBound"
        onrowcommand="gvTestCaseList_RowCommand"
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview"  
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvTestCaseList_PageIndexChanged" PageSize="50"
        AlternatingRowStyle-CssClass="alt">
        
        <Columns>

            <%-- Column 0 --%>
            <asp:TemplateField ItemStyle-CssClass="CheckAllColumn">      
                <HeaderTemplate>
                    <asp:CheckBox ID="chkHeader" runat="server" Text="All" AutoPostBack="true" OnCheckedChanged="chkHeader_CheckedChanged"  />
                </HeaderTemplate>
                <ItemTemplate> 
                    <asp:CheckBox ID="chkSelected" runat="server" />
                </ItemTemplate> 
            </asp:TemplateField> 

            <%-- Column 1 --%>
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="Project" ReadOnly="True" 
                SortExpression="projectAbbreviation" ItemStyle-CssClass="ProjectColumn" />

            <%-- Column 2 --%>
            <asp:BoundField DataField="testCaseId" HeaderText="ID" ReadOnly="True" 
                SortExpression="testCaseId" ItemStyle-CssClass="IdColumn" />

            <%-- Column 3 --%>
            <asp:BoundField HtmlEncode="False" DataField="testCaseDescription" 
                HeaderText="Description" SortExpression="testCaseDescription" ItemStyle-CssClass="DescriptionColumn" />

            <%-- Column 4 --%>
            <asp:BoundField DataField="testDate" HeaderText="Last Test Date" 
                SortExpression="testDate" Visible="True" ItemStyle-CssClass="LastTestDateColumn" />
            
            <%-- Column 5 --%>
            <asp:BoundField DataField="Browser1Status" HeaderText="Chrome" FooterText="CHROME"
                SortExpression="Browser1Status" Visible="True" />

            <%-- Column 6 --%>
            <asp:BoundField DataField="Browser2Status" HeaderText="Firefox" FooterText="FF"
                SortExpression="Browser2Status" Visible="True" />

            <%-- Column 7 --%>
            <asp:BoundField DataField="Browser3Status" HeaderText="IE8" FooterText="IE8"
                SortExpression="Browser3Status" Visible="False" />

            <%-- Column 8 --%>
            <asp:BoundField DataField="Browser4Status" HeaderText="IE9" FooterText="IE9"
                SortExpression="Browser4Status" Visible="True" />

            <%-- Column 9 --%>
            <asp:BoundField DataField="Browser5Status" HeaderText="IE10" FooterText="IE10"
                SortExpression="Browser5Status" Visible="False" />

            <%-- Column 10 --%>
            <asp:BoundField DataField="Browser6Status" HeaderText="IE11" FooterText="IE11"
                SortExpression="Browser6Status" Visible="False" />

            <%-- Column 11 --%>
            <asp:BoundField DataField="Browser7Status" HeaderText="Mac Safari" FooterText="MACSAF"
                SortExpression="Browser7Status" Visible="False" />

            <%-- Column 12 --%>
            <asp:BoundField DataField="Browser8Status" HeaderText="Win Safari" FooterText="WINSAF"
                SortExpression="Browser8Status" Visible="False" />


            <%-- Column 13 --%>
            <asp:TemplateField HeaderText="Test">      
                <ItemTemplate> 
                    <asp:Button ID="RunTest" runat="server" Text="Test" CommandName="RunTest" CommandArgument='<%#String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",Eval("projectAbbreviation"),Eval("testCaseId"),Eval("autoTestClass"),Eval("autoMetaDataTable"),Eval("autoMetaDataRow"),Eval("environment"),Eval("childTestCaseId"), Eval("active"), Eval("testCaseOutdated"), Eval("testScriptOutdated"))%>'></asp:Button> 
                </ItemTemplate> 
             </asp:TemplateField>  

            <%-- Column 14 --%>
            <asp:TemplateField HeaderText="Test Case Details" ItemStyle-CssClass="DetailButtonColumn">      
                <ItemTemplate> 
                    <asp:Button ID="DetailsButton" runat="server" PostBackUrl='<%# "~/FunctionalTesting/TestDetails.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") %>' Text="Details"></asp:Button> 
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
                Height="130" Width="139" />
                <br />
                No data available for the filters you have chosen.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast" position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>
    <asp:Label ID="lblRecordCount" runat="server" Text="" CssClass="GridviewTotals" />

</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
    

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">
    <%--********** Command Buttons **********--%>

<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblActionsTitle" runat="server" Text="Actions" /></div>
    </div>
    <p>
        <asp:Button ID="btnAddNewTestCase" runat="server" Text="Add New Test Case" Width="200px" onclick="btnAddNewTestCase_Click"></asp:Button>
        <asp:Button ID="btnUploadTestCases" runat="server" Text="Upload Bulk Test Cases" Width="200px" onclick="btnUploadTestCases_Click"></asp:Button>
        <asp:Button ID="btnUploadTestResults" runat="server" Text="Upload Bulk Test Results" Width="200px" onclick="btnUploadTestResults_Click"></asp:Button>
    </p>
</div>

<div runat="server" ID="BrowserSelectionHeader" class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="Label2" runat="server" Text="Automated Browser Selection" /></div>
    </div>
    <p>
        <asp:ListBox ID="lstbxBrowserSelection" runat="server" DataSourceID="sqlBrowserSelection" 
            DataTextField="browserName" DataValueField="browserAbbreviation" 
            SelectionMode="Multiple"   Width="200px"
            ondatabound="lstbxBrowserSelection_DataBound" Rows="6"></asp:ListBox>
        <asp:SqlDataSource ID="sqlBrowserSelection" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT Browsers.browserAbbreviation, Browsers.browserName, ProjectBrowserInfo.showBrowserColumn FROM ProjectBrowserInfo INNER JOIN Browsers ON Browsers.browserAbbreviation=ProjectBrowserInfo.browserAbbreviation WHERE (ProjectBrowserInfo.projectAbbreviation = @projectAbbreviation) AND (ProjectBrowserInfo.showBrowserColumn = 1) AND (Browsers.browserSupportedByAutomation = 1) ORDER BY Browsers.browserName">
            <SelectParameters>
                <asp:ControlParameter ControlID="LeftColumnContent:ddlProjects" Name="projectAbbreviation" 
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>
         
    </p>
</div>

<div class="tcChart">
    <div class="ChartTotalPassed"> 
        <table id="ChartInfo">
            <tr>
                <td class="left">
                    <asp:Label ID="PercentOfTotalTestCasesPassed" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
                <td class="center">
                    <asp:Label ID="PercentOfTotalTested" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
                <td class="right">
                    <asp:Label ID="PercentOfTotalTestCasesTested" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="alignleft">
                    <asp:Label ID="Label3" CssClass="ChartTotalPassedWords" runat="server" Text="% of Total<br/>Tests<br/>Passed"></asp:Label>
                </td>
                <td class="aligncenter">
                    <asp:Label ID="Label1" CssClass="ChartTotalPassedWords" runat="server" Text="% of Tested<br/>Tests<br/>Passed"></asp:Label>
                </td>
                <td class="alignright">
                    <asp:Label ID="Label4" CssClass="ChartTotalPassedWords" runat="server" Text="% of Total<br/>Tests<br/>Tested"></asp:Label>
                </td>
            </tr>
        </table> 
    </div> 

    <ajaxToolkit:ToolkitScriptManager 
        ID="ToolkitScriptManager1" 
        runat="server"  >
    </ajaxToolkit:ToolkitScriptManager>
    
    <ajaxToolkit:BarChart ID="BarChart1" runat="server" 
        ChartHeight="300" ChartWidth="200" ChartType="Column"
        ChartTitle="Status at a Glance" 
        CategoriesAxis="Status"  
        ChartTitleColor="#89AAD6" CategoryAxisLineColor="#D5E1F0" 
        ValueAxisLineColor="#D5E1F0" BaseLineColor="#D5E1F0" >
    </ajaxToolkit:BarChart>  
    
    <div class="TotalTestCases"> 
        <table id="TestCaseInfo">
            <tr>
                <td class="left">
                    <asp:Label ID="lblNumberOfTestCasesNumbers" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
                <td class="center">
                    <asp:Label ID="lblNumberOfBrowsersNumbers" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
                <td class="right">
                    <asp:Label ID="lblNumberOfTotalTestsNumbers" CssClass="ChartTotalPassedPercent" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="alignleft">
                    <asp:Label ID="lblNumberOfTestCasesWords" CssClass="ChartTotalPassedWords" runat="server" Text="Number<br/>of Test<br/>Cases"></asp:Label>
                </td>
                <td class="aligncenter">
                    <asp:Label ID="lblNumberOfBrowsersWords" CssClass="ChartTotalPassedWords" runat="server" Text="Number<br/>of<br/>Browsers"></asp:Label>
                </td>
                <td class="alignright">
                    <asp:Label ID="lblNumberOfTotalTestsWords" CssClass="ChartTotalPassedWords" runat="server" Text="Number<br/>of Total<br/>Tests"></asp:Label>
                </td>
            </tr>
        </table> 
    </div> 
    
    <%--
        Did not use piechart because the Ajax Control Toolkit Pie chart is broken
        when any value is more than 50% of the whole. Consider using after issue is fixed.
        http://ajaxcontroltoolkit.codeplex.com/workitem/27373

        <ajaxToolkit:PieChart ID="pieChart1" runat="server" ChartHeight="300" 
        ChartWidth="220" ChartTitle="Test Coverage" 
        ChartTitleColor="#89AAD6" BorderStyle="None" Font-Size="Larger"> 
        <PieChartValues>
            <ajaxToolkit:PieChartValue Category="Pass" Data="45" 
            PieChartValueColor="#00FF00" PieChartValueStrokeColor="black" />
            <ajaxToolkit:PieChartValue Category="Fail" Data="25" 
            PieChartValueColor="#FF0000" PieChartValueStrokeColor="black" />
            <ajaxToolkit:PieChartValue Category="Other" Data="30" 
            PieChartValueColor="#D1D1D1" PieChartValueStrokeColor="black" />
        </PieChartValues> 
        </ajaxToolkit:PieChart >
    --%>
</div>


</asp:Content>