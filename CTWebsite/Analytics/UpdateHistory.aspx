<%@ Page Title="Update History" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="UpdateHistory.aspx.cs" Inherits="OSTMSWebsite.Analytics.UpdateHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:SqlDataSource ID="sqlTestCasesHistory" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT *, dbo.FixEndlines(testCaseDescription) as fixed_endline_testCaseDescription, dbo.FixEndlines(testCaseSteps) as fixed_endline_testCaseSteps, dbo.FixEndlines(expectedResults) as fixed_endline_expectedResults, dbo.FixEndlines(testCaseNotes) as fixed_endline_testCaseNotes, dbo.FixEndlines(updateSprint) as fixed_endline_updateSprint, dbo.FixEndlines(updateStory) as fixed_endline_updateStory FROM [TestCasesHistory] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([testCaseId] = @testCaseId)) order by dateLastUpdated desc">
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
        CssClass="gvTestCaseList" 
        Width="100%">
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
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_updateSprint" HeaderText="Update Sprint" 
                SortExpression="fixed_endline_updateSprint" Visible="True" />
            <asp:BoundField HtmlEncode="False" DataField="fixed_endline_updateStory" HeaderText="Update Story" 
                SortExpression="fixed_endline_updateStory" Visible="True" />
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

</asp:Content>


