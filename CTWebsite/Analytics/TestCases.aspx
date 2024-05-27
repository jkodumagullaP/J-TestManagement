<%@ Page Title="Analytics Test Cases" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TestCases.aspx.cs" Inherits="OSTMSWebsite.Analytics.TestCasesAnalytics" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <style type="text/css">
        .ExportToExcel
        {}
    </style>
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">   
<%--********** Project Dropdown **********--%>
    
    <div class="title">Project</div>
    <div class="ddlDropDownLeftRail">
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
    </div>
    
<%--********** Releases Dropdown **********--%>

    <div class="title">Release</div>
    <div class="ddlDropDownLeftRail">        
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
    </div>
    
<%--********** Sprints Dropdown **********--%>
    
    <div class="title">Sprint</div>
    <div class="ddlDropDownLeftRail">        
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
    </div>  
    
<%--********** Group Dropdown **********--%>
    
    <div class="title">Test Group</div>
    <div class="ddlDropDownLeftRail">        
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
            </SelectParameters>            
        </asp:SqlDataSource>
    </div> 

<%--********** Environments Dropdown **********--%>

    <div class="title">Environment</div>
    <div class="ddlDropDownLeftRail">
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
            SelectCommand="SELECT [environment] FROM [Environments] Order By sortOrder">
        </asp:SqlDataSource>
    </div>

<%--********** Keyword Search **********--%>

    <div class="title">Test Case Keyword Search</div>
    <div class="keywordSearch">   

        <asp:textbox id="txtKeywordSearchTextBox" runat="server" ></asp:textbox> 
        <asp:ImageButton ID="btnKeywordSearchSubmit" runat="server" 
            CssClass="btnSearch" CommandName="submit" Height="20px" 
            ImageUrl="~/Images/search.png" Width="20px" ToolTip="Search" 
            onclick="btnKeywordSearchSubmit_Click" />
    </div> 
    <div>*You may enter multiple keywords.</div>
    

<%--********** Reset Button **********--%>
    <div><hr /></div>
    <div class="Reset">
        <asp:Button ID="btnReset" runat="server" Text="Reset Filters" 
            onclick="btnReset_Click" />   
    </div>     
    <div><hr /></div>

</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <%--********** Image Menu **********--%>
 
<table class="testCaseListIcons">
    <tr>
        <td><h1>Analytics Test Case List</h1></td>
        <td>
            <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
            <div class="imgButtonMenu">

                <asp:ImageButton ID="btnAddToAutomatedQueue" runat="server" 
                    CssClass="AddToQueue" Height="25px" 
                    ImageUrl="~/Images/AutoQueueStart.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to Automated Queue" onclick="AddToAutomatedQueue"  />

                <asp:ImageButton ID="btnRemoveFromAutomatedQueue" runat="server" 
                    CssClass="AddToQueue" Height="25px" 
                    ImageUrl="~/Images/AutoQueueStop.png" Width="30px" 
                    ToolTip="Remove Selected Test Cases from Automated Queue" onclick="RemoveFromAutomationQueue" />

                <asp:ImageButton ID="btnExportToExcel" runat="server" 
                    CssClass="ExportToExcel" Height="25px" 
                    ImageUrl="~/Images/ExportToExcel2.png" Width="30px" 
                    ToolTip="Create Test Case Import Sheet with Selected Test Cases" onclick="ExportToExcel" TabIndex="1" />

                <asp:ImageButton ID="btnAddToRelease" runat="server" 
                    CssClass="DownloadExcel" Height="25px" 
                    ImageUrl="~/Images/AddToRelease2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Release" onclick="AddToRelease" />

                <asp:ImageButton ID="btnAddToSprint" runat="server" 
                    CssClass="DownloadExcel" Height="25px" 
                    ImageUrl="~/Images/AddToSprint2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Sprint" onclick="AddToSprint" />
        
                <asp:ImageButton ID="btnAddToGroup" runat="server" 
                    CssClass="DownloadExcel" Height="25px" 
                    ImageUrl="~/Images/AddToGroup2.png" Width="30px" 
                    ToolTip="Add Selected Test Cases to a Group" onclick="AddToGroup" />
        
                <asp:ImageButton ID="btnAssign" runat="server" 
                    CssClass="Assign" Height="25px" 
                    ImageUrl="~/Images/Assign2.png" Width="30px" 
                    ToolTip="Assign Selected Test Cases to a User" onclick="AssignTestCase" 
                    Visible="True" />

                <asp:ImageButton ID="btnBulkDelete" runat="server" 
                    CssClass="DownloadExcel" Height="25px" 
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
    
    <%--********** Gridview **********--%>
    <asp:SqlDataSource ID="sqlTestCasesByProject" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="" onselected="sqlTestCasesByProject_Selected">
    </asp:SqlDataSource>

    <asp:GridView ID="gvTestCaseList" runat="server" 
        DataKeyNames="projectAbbreviation,testCaseId" 
        DataSourceID="sqlTestCasesByProject"  
        OnRowDataBound="gvTestCaseList_RowDataBound"
        onrowcommand="gvTestCaseList_RowCommand"
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvTestCaseList"  
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvTestCaseList_PageIndexChanged" PageSize="100"
        AlternatingRowStyle-CssClass="alt">
        
        <Columns>
            <asp:TemplateField HeaderText="Test Case Details" ItemStyle-CssClass="DetailButtonColumn">      
                <ItemTemplate> 
                    <asp:Button ID="DetailsButton" runat="server" PostBackUrl='<%# "~/Analytics/TestDetails.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") %>' Text="Details"></asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>
            
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="Project" ReadOnly="True" 
                SortExpression="projectAbbreviation" ItemStyle-CssClass="ProjectColumn" />
            
            <asp:BoundField DataField="testCaseId" HeaderText="ID" ReadOnly="True"
                SortExpression="testCaseId" ItemStyle-CssClass="IdColumn" />
            
            <asp:BoundField HtmlEncode="False" DataField="testCaseDescription" 
                HeaderText="Description" SortExpression="testCaseDescription" ItemStyle-CssClass="DescriptionColumn" />

            <asp:BoundField DataField="testDate" HeaderText="Last Test Date" 
                SortExpression="testDate" Visible="True" ItemStyle-CssClass="LastTestDateColumn" />

            <asp:BoundField DataField="Browser1Status" HeaderText="Chrome" 
                SortExpression="Browser1Status" Visible="False" />

            <asp:BoundField DataField="Browser2Status" HeaderText="Firefox" 
                SortExpression="Browser2Status" Visible="True" />

            <asp:BoundField DataField="Browser3Status" HeaderText="IE8" 
                SortExpression="Browser3Status" Visible="False" />

            <asp:BoundField DataField="Browser4Status" HeaderText="IE9" 
                SortExpression="Browser4Status" Visible="False" />
            
            <asp:BoundField DataField="Browser5Status" HeaderText="IE10" 
                SortExpression="Browser5Status" Visible="True" />

            <asp:BoundField DataField="Browser6Status" HeaderText="Mac Safari" 
                SortExpression="Browser6Status" Visible="False" />

            <asp:BoundField DataField="Browser7Status" HeaderText="Win Safari" 
                SortExpression="Browser7Status" Visible="False" />

            <asp:BoundField DataField="Browser8Status" HeaderText="None" 
                SortExpression="Browser8Status" Visible="False" />
            
            <asp:TemplateField ItemStyle-CssClass="CheckAllColumn">      
                <HeaderTemplate>
                    <asp:CheckBox ID="chkHeader" runat="server" Text="All" AutoPostBack="true" OnCheckedChanged="chkHeader_CheckedChanged"  />
                </HeaderTemplate>
                <%-- http://forums.asp.net/t/1721045.aspx/1 --%>
                <ItemTemplate> 
                    <asp:CheckBox ID="chkSelected" runat="server" />
                </ItemTemplate> 
            </asp:TemplateField> 
            
            <asp:TemplateField HeaderText="Test">      
                <ItemTemplate> 
                    <asp:Button ID="RunTest" runat="server" Text="Test" CommandName="RunTest" CommandArgument='<%#String.Format("{0}|{1}",Eval("projectAbbreviation"),Eval("testCaseId"))%>'></asp:Button> 
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
    <div class="title">Commands</div>
    
    <div class="commands">
        <asp:Button ID="btnAddNewTestCase" runat="server" 
            Text="Add New Test Case" Width="200px" onclick="btnAddNewTestCase_Click"></asp:Button>
    </div>
    
    <div class="commands">
        <asp:Button ID="btnUploadTestCases" runat="server" 
            Text="Upload Bulk Test Cases" Width="200px" onclick="btnUploadTestCases_Click"></asp:Button>
    </div>

    <div class="commands">
        <asp:Button ID="btnUploadTestResults" runat="server" 
            Text="Upload Bulk Test Results" Width="200px" onclick="btnUploadTestResults_Click"></asp:Button>
    </div>
    <div><hr /></div>
    <div  runat="server" ID="BrowserSelectionHeader" class="title"><asp:Literal runat="server">Automated Browser Selection</asp:Literal></div>

    <div class="commands">
        <asp:ListBox ID="lstbxBrowserSelection" runat="server" DataSourceID="sqlBrowserSelection" 
            DataTextField="browserName" DataValueField="browserAbbreviation" 
            SelectionMode="Multiple"   Width="200px"
            ondatabound="lstbxBrowserSelection_DataBound" Rows="6"></asp:ListBox>
    </div>
    <asp:SqlDataSource ID="sqlBrowserSelection" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT [browserName], [browserAbbreviation], [browserSupportedByAnalytics] FROM [Browsers] WHERE ([browserSupportedByAnalytics] = @browserSupportedByAnalytics)">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="browserSupportedByAnalytics" 
                Type="Int32" />
        </SelectParameters>
    </asp:SqlDataSource>

</asp:Content>