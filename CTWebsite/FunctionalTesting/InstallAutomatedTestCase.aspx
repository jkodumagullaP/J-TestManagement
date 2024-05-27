<%@ Page Title="Install Automated Test Case" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="InstallAutomatedTestCase.aspx.cs" Inherits="CTWebsite.FunctionalTesting.InstallAutomatedTestCase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<asp:Panel ID="pnlInstallAutomatedTestCase" runat="server" GroupingText="Install Automated Test Case" CssClass="dashboardPanel">
    <table>
        <tr>
            <td>
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
        <td>Project</td><td>
            <asp:DropDownList ID="ddlProjects" runat="server"  
                DataSourceID="sqlProjects" 
                DataTextField="projectName" 
                DataValueField="projectAbbreviation" 
                AppendDataBoundItems="False" 
                AutoPostBack="True"
                CssClass="leftRailDropdown" 
                onselectedindexchanged="ddlProjects_SelectedIndexChanged" Width="400" >
            </asp:DropDownList>
            
            <asp:SqlDataSource ID="sqlProjects" runat="server" 
                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                SelectCommand="SELECT [projectAbbreviation], [projectName] FROM [Projects] order by projectName">
            </asp:SqlDataSource>
        </td>
        </tr>
        <tr>
        <td>Test Case ID</td><td>
            <asp:TextBox ID="txtTestCaseID" runat="server" 
                AutoPostBack="True"
                ontextchanged="txtTestCaseID_TextChanged" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
            <td>Description</td><td>
                <asp:Label ID="lblDescription" runat="server" Width="400" Text=""></asp:Label></td>
        </tr>
        <tr>
        <td>AutoTestClass</td><td>
            <asp:TextBox ID="txtAutoTestClass" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>autoMetaDataTable</td><td>
            <asp:TextBox ID="txtautoMetaDataTable" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>autoMetaDataRow</td><td>
            <asp:TextBox ID="txtautoMetaDataRow" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>Automated</td><td>
            <asp:DropDownList ID="ddlAutomated" runat="server"  
                CssClass="leftRailDropdown" 
                Width="400" >
            </asp:DropDownList></td>
        </tr>
        <tr>
        <td>Automation Reason</td><td>
            <asp:TextBox ID="txtAutomationReason" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>Child Test Case ID</td><td>
            <asp:TextBox ID="txtChildTestCaseID" runat="server" Width="400"></asp:TextBox></td>
        </tr>
        <tr>
        <td>                
            <asp:Button ID="Button2" runat="server" Text="Install Automated Test Case"
             AutoPostBack="True" onclick="Button1_Click" />
        </td>
        <td>
            <asp:Button ID="btnAddChild" runat="server" Text="Add Automated Child"
             AutoPostBack="True" onclick="Button2_Click" />
        </td>

        </tr>
    </table>
    </asp:Panel>
    
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



</asp:Content>


