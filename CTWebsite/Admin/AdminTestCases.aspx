<%@ Page Title="Admin Test Cases" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminTestCases.aspx.cs" Inherits="CTWebsite.Admin.AdminTestCases" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">

 
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <table id="fileUpload">
        <tr>
            <td class ="fileUploadCell">
                <div class="boxTestCases">
                    <div class="boxTestCasesHeaderContainer">
                        <div class="left">
                            <asp:label Id="lblUploadTestCasesTitle" runat="server" Text="Upload Test Cases" />
                        </div>
                    </div>
                    <p>
                        <asp:FileUpload ID="xlsTcUpload" runat="server" Font-Size="Small" />
                        <asp:Button ID="btnUploadTc" runat="server" Text="Upload Test Cases" 
                            OnClick="btnUploadTc_Click" Width="150px" />
                    </p>
                </div>
             </td>
            <td class ="fileUploadCell">
                <div class="boxTestCases">
                    <div class="boxTestCasesHeaderContainer">
                        <div class="left">
                            <asp:label Id="lblUploadResultsTitle" runat="server" Text="Upload Test Results" />
                        </div>
                    </div>
                    <p>
                        <asp:FileUpload ID="xlsTrUpload" runat="server" Font-Size="Small" />
                        <asp:Button ID="btnUploadTr" runat="server" Text="Upload Test Results" 
                            OnClick="btnUploadTr_Click" Width="150px" />
                    </p>
                </div>
            </td>
        </tr>
        <tr>
            <td class="failureNotification" colspan="2">
                    <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
            </td>
        </tr>
    </table>

<%--********** DELETED TEST CASES **********--%>

<asp:Panel ID="pnlDeletedTestCases" runat="server" GroupingText="Deleted Test Cases" CssClass="dashboardPanel">

    <div>
        <asp:Label ID="Label2" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
    <div class="adminSectionHeader">To restore a deleted test case, select a project and click the Restore button.</div>
    
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

    </div>
    <div><br /></div>

    <%--********** Gridview **********--%>
    <asp:SqlDataSource ID="sqlDeletedTestCases" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT TestCasesHistory.*, dbo.UserProfiles.userFirstName + ' ' + dbo.UserProfiles.userLastName as LastUpdatedBy FROM [TestCasesHistory] left join dbo.aspnet_Users 	on dbo.TestCasesHistory.updatedBy = aspnet_Users.UserName left join dbo.UserProfiles on aspnet_Users.userid = UserProfiles.userid WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([changeType] = 'DELETE')) ORDER BY [testCaseId]">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" 
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
    </asp:SqlDataSource>

    <!-- DO NOT CHANGE THE COLUMN ORDER -->
    <asp:GridView ID="gvDeletedTestCases" runat="server" 
        DataKeyNames="projectAbbreviation,testCaseId" 
        DataSourceID="sqlDeletedTestCases"  
        onrowcommand="gvDeletedTestCases_RowCommand"
        Tooltip="" 
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview"  
        PagerStyle-CssClass="pgr" 
        onpageindexchanged="gvDeletedTestCases_PageIndexChanged" PageSize="50" 
        Caption="Deleted Test Cases">
        
        <Columns>
            <asp:TemplateField HeaderText="Restore">      
                <ItemTemplate> 
                    <asp:Button 
                        ID="btnRestore" 
                        runat="server" 
                        Text="Restore" 
                        CommandName="Restore" 
                        CommandArgument='<%#Eval("projectAbbreviation") + "|" + Eval("testCaseId")%>' 
                        OnClientClick="if (!confirm('Restoring this test case will also restore it to the releases, sprints, and groups it belonged to before it was deleted as well as restore any test result history associated with this test case. Are you sure you want to restore this test case?')){return false;}">
                    </asp:Button> 
                </ItemTemplate> 
            </asp:TemplateField>
            
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
            <asp:BoundField DataField="active" HeaderText="Active" 
                SortExpression="active" Visible="True" />
            <asp:BoundField DataField="testCaseOutdated" HeaderText="Test Case Outdated" 
                SortExpression="testCaseOutdated" Visible="True" />
            <asp:BoundField DataField="testScriptOutdated" HeaderText="Test Script Outdated" 
                SortExpression="testScriptOutdated" Visible="True" />
            <asp:BoundField DataField="dateLastUpdated" HeaderText="Last Updated" 
                SortExpression="dateLastUpdated" Visible="True" />
            <asp:BoundField DataField="LastUpdatedBy" HeaderText="Updated By" 
                SortExpression="LastUpdatedBy" Visible="True" />
            <asp:BoundField DataField="autoTestClass" HeaderText="autoTestClass" 
                SortExpression="autoTestClass" Visible="False" />
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
                No test cases have been deleted.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast"  position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>      


</asp:Panel>
</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
    

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">
    
    <%--********** Download Files **********--%>
    <div class="title">
        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/DownloadButton.png" Width="200" Height="72" />

    </div>
    <div class="downloadLinks"><asp:Panel ID="pnlRightColumn" runat="server"></asp:Panel></div>
    

</asp:Content>
