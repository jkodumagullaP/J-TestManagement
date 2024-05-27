<%@ Page Title="Admin Projects" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminProjects.aspx.cs" Inherits="CTWebsite.Admin.AdminProjects" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">

<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><asp:label Id="lblActionsTitle" runat="server" Text="Actions" /></div>
    </div>
    <p>
        <asp:Button ID="btnCreateNewProject" runat="server" 
            Text="Create New Project" Width="200px" onclick="btnCreateNewProject_Click"></asp:Button>
        <asp:Button ID="btnAddExampleData" runat="server" Text="Add Example Data" Width="200px" onclick="btnAddExampleData_Click"></asp:Button>
        <asp:Button ID="btnRemoveExampleData" runat="server" Text="Remove Example Data" Width="200px" onclick="btnRemoveExampleData_Click"></asp:Button>
        <asp:Button ID="btnResetCrystalTest" runat="server" Text="Reset Crystal Test" Width="200px" onclick="btnResetCrystalTest_Click" 
                    OnClientClick="if (!confirm('WARNING!! Clicking this button will remove ALL Projects and their data including all test cases, releases, groups, sprints, automation data, etc. This will reset the application back to the point of original installation. This can not be undone. Are you sure you want to reset Crystal Test?')){return false;}"></asp:Button>
    </p>
</div>

</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Message area--%>
    <div class="failureNotification">
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>

<%--********** PROJECT LIST **********--%>
    <div class="adminSectionHeader">Project Names and Abbreviations are designed to match those in defect management systems for future integration. The Project Abbreviation should match the Key for a given project for systems that require it such as Jira. TFS does not require a project key to link directly to a bug, but it does require it to link to a project.</div> 
    <%--********** Gridview **********--%>
    <asp:SqlDataSource ID="sqlProjects" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT * FROM [Projects] order by projectName">
    </asp:SqlDataSource>
    
    <!-- DO NOT CHANGE THE COLUMN ORDER -->
    <asp:GridView ID="gvProjects" runat="server" 
        DataSourceID="sqlProjects"  
        Tooltip="" 
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="True" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview"  
        PagerStyle-CssClass="pgr" 
        PageSize="50" 
        Caption="Projects">
        
        <Columns>
            <asp:BoundField DataField="projectName" 
                HeaderText="Project Name" ReadOnly="True" 
                SortExpression="projectName" />
            
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="Project Abbreviation" ReadOnly="True" 
                SortExpression="projectAbbreviation" />
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
                There are no projects in the system.
            </div>
        </emptydatatemplate>
        <PagerSettings Mode="NumericFirstLast"  position="TopAndBottom"/>
        <PagerStyle CssClass="pgr"></PagerStyle>
    </asp:GridView>      



</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>
    

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">

</asp:Content>
