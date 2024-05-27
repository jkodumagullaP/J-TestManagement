<%@ Page Title="Result History" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="ResultHistory.aspx.cs" Inherits="OSTMSWebsite.Analytics.ResultHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContent" runat="server">
    
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
		CssClass="gvTestResultList" 
		PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvResultHistory_PageIndexChanged" 
        AlternatingRowStyle-CssClass="alt" >
        <Columns>
            <asp:TemplateField HeaderText="Edit" ItemStyle-CssClass="EditButtonColumn">      
                <ItemTemplate> 
                    <asp:Button ID="EditButton" runat="server" PostBackUrl='<%# "~/FunctionalTesting/AddEditResults.aspx?project="+ Eval("projectAbbreviation") +"&testCase="+ Eval("testCaseId") +"&testRunId="+ Eval("testRunId") %>' Text="Details"></asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>
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
                  datanavigateurlformatstring="http://trgteam2:8080/tfs/TFS2008/QAA_TMS/_workItems#_a=edit&id={0}&triage=true"/> <%--TODO: This needs updated %>

            
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



    

</asp:Content>

<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">

    <%--********** Environments Dropdown **********--%>

    <div class="title">Environment</div>
    <div>
        <asp:DropDownList ID="ddlEnvironments" runat="server" 
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
            SelectCommand="SELECT Environments.environment, Environments.environment
                            FROM Environments order by sortOrder">
                            
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>

    
    <%--********** Browser Dropdown **********--%>
    <div class="title">Browser</div>
    <div>
        <asp:DropDownList ID="ddlBrowsers" runat="server" 
            DataSourceID="sqlBrowsers" 
            DataTextField="browserName" 
            DataValueField="browserAbbreviation" 
            AppendDataBoundItems="False" 
            AutoPostBack="True"
            onselectedindexchanged="ddlBrowsers_SelectedIndexChanged" 
            CssClass="leftRailDropdown">
        </asp:DropDownList>
            
        <asp:SqlDataSource ID="sqlBrowsers" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT TestResults.browserAbbreviation, Browsers.browserName
                            FROM TestResults
                            INNER JOIN Browsers
                            ON TestResults.browserAbbreviation=Browsers.browserAbbreviation
                            WHERE (TestResults.projectAbbreviation=@projectAbbreviation) AND (TestResults.testCaseId=@testCaseId)
                            GROUP BY TestResults.browserAbbreviation, Browsers.browserName
                            ORDER BY TestResults.browserAbbreviation">
                            
            <SelectParameters>
                <asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" 
                    Type="String" />
                <asp:QueryStringParameter Name="testCaseId" QueryStringField="testCase" 
                    Type="Int32" />
            </SelectParameters>
        </asp:SqlDataSource>
    </div>
</asp:Content>


