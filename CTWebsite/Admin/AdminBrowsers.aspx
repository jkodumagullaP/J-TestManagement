<%@ Page Title="Admin Browsers" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminBrowsers.aspx.cs" Inherits="CTWebsite.Admin.AdminBrowsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Message area--%>
    <div class="failureNotification">
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
                    <ajaxToolkit:ToolkitScriptManager ID="ToolkitScriptManager1" runat="Server" />
    </div>

<%--********** DELETED TEST CASES **********--%>

<asp:Panel ID="pnlBrowsers" runat="server" GroupingText="Project Supported Browsers" CssClass="dashboardPanel">

    <div>
        <asp:Label ID="Label2" runat="server" Text="" ForeColor="Red"></asp:Label>

    </div>
    <div class="adminSectionHeader">Select which browsers this project supports. The browsers selected here will display results and metrics on the Test Cases page, Dashboard Result Metrics panel, and Personal Task List.</div>
    
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
    <asp:SqlDataSource ID="sqlProjectSupportedBrowsers" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
        SelectCommand="SELECT Browsers.browserName, ProjectBrowserInfo.browserAbbreviation, ProjectBrowserInfo.showBrowserColumn, ProjectBrowserInfo.projectAbbreviation
                        FROM ProjectBrowserInfo 
                        JOIN Browsers
                        ON Browsers.browserAbbreviation = ProjectBrowserInfo.browserAbbreviation
                        WHERE ([projectAbbreviation] = @projectAbbreviation)">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" 
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
            <UpdateParameters>
                <asp:Parameter Name="projectAbbreviation" Type="String" />
                <asp:Parameter Name="browserAbbreviation" Type="String" />
                <asp:Parameter Name="showBrowserColumn" Type="String" />
            </UpdateParameters>
    </asp:SqlDataSource>

    <asp:GridView ID="gvProjectSupportedBrowsers" runat="server" 
        DataKeyNames="projectAbbreviation, browserAbbreviation, showBrowserColumn"
        DataSourceID="sqlProjectSupportedBrowsers"  
        OnRowDataBound="gvProjectSupportedBrowsers_RowDataBound"
        OnRowCommand="gvProjectSupportedBrowsers_RowCommand"
        Tooltip="" 
        AutoGenerateColumns="False"  
        GridLines="None"  
        AllowPaging="False" 
        AllowSorting="True" 
        CssClass="gvGlobalGridview" Width="500" ShowHeader="False">
        
        <Columns>
            <asp:BoundField DataField="projectAbbreviation" 
                HeaderText="Project" ReadOnly="True" 
                SortExpression="projectAbbreviation" />

            <asp:BoundField DataField="browserName" 
                HeaderText="Browser Name" ReadOnly="True" 
                SortExpression="browserName" />

            <asp:BoundField DataField="browserAbbreviation" 
                HeaderText="Browser Abbreviation" ReadOnly="True" 
                SortExpression="browserAbbreviation" />

            <asp:TemplateField HeaderText="Project Supported">      
                <ItemTemplate> 
                    <asp:ImageButton 
                        ID="imgbtnBrowserToggle" 
                        DataField="showBrowserColumn"
                        runat="server" 
                        AutoPostBack="True" 
                        CommandName="Toggle"
                        ToolTip='<%# Container.DataItemIndex %>'
                        CommandArgument='<%# Eval("projectAbbreviation") + "|" + Eval("browserAbbreviation") + "|" +  Eval("showBrowserColumn")%>'
                        ImageUrl="~/Images/onbutton.png" />
                        

<%--                    <ajaxToolkit:ToggleButtonExtender ID="ToggleBrowserSupport" runat="server"
                        TargetControlID="editChkSelected"
                        ImageWidth="50"
                        ImageHeight="15"
                        CheckedImageAlternateText='<%# Eval("projectAbbreviation") + "|" + Eval("browserAbbreviation") + "|" +  Eval("showBrowserColumn")%>'
                        UncheckedImageAlternateText='<%# Eval("projectAbbreviation") + "|" + Eval("browserAbbreviation") + "|" +  Eval("showBrowserColumn")%>'
                        UncheckedImageUrl="~/Images/offbutton.png"
                        CheckedImageUrl="~/Images/onbutton.png" />                
--%>
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
                Height="64" Width="64" />
                <br />
                No test cases have been deleted.
            </div>
        </emptydatatemplate>
    </asp:GridView>      


</asp:Panel>
</asp:Content>

