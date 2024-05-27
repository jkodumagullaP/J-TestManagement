<%@ Page Title="Admin Selenium" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminSelenium.aspx.cs" Inherits="CTWebsite.Admin.AdminSelenium" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
 
 
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
<%--Manage Automation Test Engine --%>    
<asp:Panel ID="pnlAutomationTestEngine" runat="server" GroupingText="Manage Automation Test Engine" CssClass="dashboardPanel">

    <asp:SqlDataSource ID="sqlAutomationTestEngines" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="" />
        <asp:ListView ID="lvAutomationTestEngines" runat="server" 
            DataSourceID="sqlAutomationTestEngines" ondatabound="AutomationTestEngine_DataBound" >
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no Automation Test Engines found on the Selenium Grid.
            </div>
        </emptydatatemplate>
        <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
        <ItemTemplate>
        <tr class="TableData">
                <!-- NOTE: Don't change this ToolTip, it's used to identify this field. -->
		        <td><asp:Label ID="lblAutomationTestEngineName" runat="server" Text='<%# Eval("serverName") %>' /></td>
		        <td><asp:Label ID="lblAutomationTestEngineStatus" runat="server" Text='Started or Stopped goes here' ToolTip='<%#Eval("SeleniumServerID")%>' /></td>
                <td>                                
                    <asp:ImageButton ID="imgViewAutomationTestEngineConsole" OnClick="imgViewAutomationTestEngineConsole_OnClick" runat="server" 
                                    onclientclick="Form1.target ='_blank'"
                                    Width="24" Height="24" ImageUrl="~/Images/ConsoleButton.png"  
                                    ToolTip='View Console' ImageAlign="Top" /> 
                </td>
                <td>                                
                    <asp:ImageButton ID="imgServerStart" OnClick="imgEngineStart_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/PowerButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Start Server' ImageAlign="Top" /> 
                </td>
                <td>                                
                    <asp:ImageButton ID="imgServerStop" OnClick="imgEngineStop_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/StopButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Stop Server' ImageAlign="Top" /> 
                </td>
		        <td>                                
                    <asp:ImageButton ID="imgServerRefresh" OnClick="imgEngineRefresh_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/RefreshButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Restart Server' ImageAlign="Top" /> 
                </td>
                </tr>
            </ItemTemplate>
              <LayoutTemplate>
                <table id="tblServerList" runat="server" class="ServerListTable" width="100%">
                    <tr id="Tr1" runat="server" class="TableHeader">
                        <td id="Td1" runat="server">Server Name</td>
                        <td id="Td6" runat="server">Test Engine Status</td>
                        <td><asp:Label ID="lblAutomationTestEngineConsole" runat="server" Text='Automation Test Engine Console' /></td>
                        <td><asp:Label ID="Label1" runat="server" Text='Start' /></td>
                        <td><asp:Label ID="Label2" runat="server" Text='Stop' /></td>
                        <td><asp:Label ID="Label3" runat="server" Text='Restart' /></td>
                    </tr>
                    <tr id="ItemPlaceholder" runat="server">
                    </tr>
                </table>
            </LayoutTemplate>

        </asp:ListView>



</asp:Panel>
    
    
<%--Manage Selenium HUB--%>
<asp:Panel ID="pnlSeleniumHUB" runat="server" GroupingText="Manage Selenium HUB" CssClass="dashboardPanel">
<!-- Contents of http://localhost:4444/grid/console goes here. -->

    <asp:SqlDataSource ID="sqlSeleniumHUBs" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="" />
        <asp:ListView ID="lvSeleniumHUBs" runat="server" 
            DataSourceID="sqlSeleniumHUBs" ondatabound="ListView1_DataBound" >
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no servers on the Selenium Grid.
            </div>
        </emptydatatemplate>
        <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
        <ItemTemplate>
        <tr class="TableData">
                <!-- NOTE: Don't change this ToolTip, it's used to identify this field. -->
		        <td><asp:Label ID="lblHubHostName" runat="server" Text='<%# Eval("serverName") %>' /></td>
		        <td><asp:Label ID="lblHubStatus" runat="server" Text='Started or Stopped goes here' ToolTip='<%#Eval("SeleniumServerID")%>' /></td>
                <td>                                
                    <asp:ImageButton ID="imgViewHubConsole" OnClick="imgViewHubConsole_OnClick" runat="server" 
                                    onclientclick="Form1.target ='_blank'"
                                    Width="24" Height="24" ImageUrl="~/Images/ConsoleButton.png"  
                                    ToolTip='View Console' ImageAlign="Top" /> 
                </td>
                </tr>
            </ItemTemplate>
              <LayoutTemplate>
                <table id="tblServerList" runat="server" class="ServerListTable" width="100%">
                    <tr id="Tr1" runat="server" class="TableHeader">
                        <td id="Td1" runat="server">Server Name</td>
                        <td id="Td6" runat="server">HUB Status</td>
                        <td><asp:Label ID="lblHubConsole" runat="server" Text='Hub Console' /></td>
                    </tr>
                    <tr id="ItemPlaceholder" runat="server">
                    </tr>
                </table>
            </LayoutTemplate>

        </asp:ListView>



</asp:Panel>
<%--Manage Selenium Nodes--%>
<asp:Panel ID="pnlSeleniumNodes" runat="server" GroupingText="Manage Selenium Nodes" CssClass="dashboardPanel">
    <asp:Label ID="lblMessage" ForeColor="Red" runat="server" Text=""></asp:Label>
        <asp:SqlDataSource ID="sqlSeleniumServers" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="" />
        <asp:ListView ID="lvSeleniumServers" runat="server" 
            DataSourceID="sqlSeleniumServers" 
        ondatabound="lvSeleniumServers_DataBound" >
        <emptydatatemplate>
            <div class="NoData">
                <asp:image id="NoDataImage"   
                imageurl="~/Images/NoData.png"
                alternatetext="No Data Image" 
                runat="server"
                Height="64" Width="64" />
                <br />
                There are no servers on the Selenium Grid.
            </div>
        </emptydatatemplate>
        <ItemSeparatorTemplate><br /></ItemSeparatorTemplate>
        <ItemTemplate>
        <tr class="TableData">
		        <td><asp:Label ID="lblServerName" runat="server" Text='<%# Eval("serverName") %>' /></td>
		        <td><asp:Label ID="lblBrowsers" runat="server" Text='<%# Eval("Browsers") %>' /></td>
                
                <!-- NOTE: Don't change this ToolTip, it's used to identify this field. -->
		        <td><asp:Label ID="lblServerStatus" runat="server" Text='Started or Stopped goes here' ToolTip='<%#Eval("SeleniumServerID")%>' /></td>
                
                <td>                                
                    <asp:ImageButton ID="imgViewNodeConsole" OnClick="imgViewNodeConsole_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/ConsoleButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='View Console' ImageAlign="Top" /> 
                </td>
                <td>                                
                    <asp:ImageButton ID="imgServerStart" OnClick="imgServerStart_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/PowerButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Start Server' ImageAlign="Top" /> 
                </td>
                <td>                                
                    <asp:ImageButton ID="imgServerStop" OnClick="imgServerStop_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/StopButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Stop Server' ImageAlign="Top" /> 
                </td>
		        <td>                                
                    <asp:ImageButton ID="imgServerRefresh" OnClick="imgServerRefresh_OnClick" runat="server" 
                                    Width="24" Height="24" ImageUrl="~/Images/RefreshButton.png" 
                                    CommandArgument='<%#Eval("SeleniumServerID")%>' 
                                    ToolTip='Restart Server' ImageAlign="Top" /> 
                </td>
                </tr>
            </ItemTemplate>
              <LayoutTemplate>
                <table id="tblServerList" runat="server" class="ServerListTable" width="100%">
                    <tr id="Tr1" runat="server" class="TableHeader">
                        <td id="Td1" runat="server">Server Name</td>
                        <td id="Td2" runat="server">Browsers</td>
                        <td id="Td6" runat="server">Node Status</td>
                        <td id="Td7" runat="server">Console</td>
                        <td id="Td3" runat="server">Start</td>
                        <td id="Td4" runat="server">Stop</td>
                        <td id="Td5" runat="server">Restart</td>
                    </tr>
                    <tr id="ItemPlaceholder" runat="server">
                    </tr>
                </table>
            </LayoutTemplate>

        </asp:ListView>
    </asp:Panel>


<%--Reset Selenium Grid--%>
<asp:Panel ID="pnlRestartAutomation" runat="server" GroupingText="Reset Selenium Grid" CssClass="dashboardPanelWarning">
    <b>Warning:</b> Resetting the Selenium Grid affects ALL automation tests that have the status of In Progress or In Queue 
    for ALL projects and should only be done at a LAST resort. Before resetting the entire Selenium Grid, locate the node on which 
    the affected test is stuck by checking the Automation Node field, on the result history page and reset only that node. Then retry the test. 
    If you must reset the entire Selenium Grid, you will need to restart any automated tests that were in progess or in queue 
    previously. These tests will have a status of Retest.
     <br />Restarting the Selenium Grid will do the following:
    <ol>
        <li>Reset any automated tests with the status of In Progress or In Queue and change their status to "Retest".</li>
        <li>Shutdown all browser processes on all Selenium Node servers</li>
        <li>Shut down all Selenium Nodes</li>
        <li>Shut down the Selenium HUB</li>
        <li>Restart the Selenium HUB</li>
        <li>Restart the Selenium Nodes</li>
    </ol>
    <asp:Button ID="btnResetSeleniumGrid" runat="server" Text="Reset Selenium Grid" OnClientClick="return confirm('Resetting the Selenium Grid affects ALL automation tests that have the status of In Progress or In Queue for ALL projects and should only be done at a LAST resort. Before resetting the entire Selenium Grid locate the node on which the affected test is stuck by checking the Automation Node field on the result history page and reset only that node. Then retry the test. If you must reset the entire Selenium Grid you will need to restart any automated tests that were in progess or in queue previously. These tests will have a status of Retest. Are you certain you want to reset the Selenium Grid?');" onclick="btnResetSeleniumGrid_Click" />
    <br />
    <br />
    This button is even more drastic, it literally reboots the servers that are hosting the hub and the nodes. If you click this, click it only once and expect a minimum 5 minutes for the grid to reboot.
    <br />
    <asp:Button ID="btnResetSeleniumGridWithReboot" runat="server" 
        onclick="btnResetSeleniumGridWithReboot_Click" 
        OnClientClick="return confirm('Resetting the Selenium Grid affects ALL automation tests that have the status of In Progress or In Queue for ALL projects and should only be done at a LAST resort. Before resetting the entire Selenium Grid locate the node on which the affected test is stuck by checking the Automation Node field on the result history page and reset only that node. Then retry the test. If you must reset the entire Selenium Grid you will need to restart any automated tests that were in progess or in queue previously. These tests will have a status of Retest. Are you certain you want to reset the Selenium Grid?');" 
        Text="Full Selenium Grid Server Reboot" />
</asp:Panel>
<asp:Panel ID="Panel1" runat="server" GroupingText="Reset Website" CssClass="dashboardPanelWarning">
            <b>Warning:</b> This will restart the entire website for ALL projects, 
            terminating any running tests and any active users&#39; sessions. This, too, should 
            only be used as a LAST resort. If you do a website reset, you should also do a 
            full selenium grid reset afterwards to ensure any partially-completed tests are 
            removed from the nodes.
        <br /><br />
    <asp:Button ID="btnResetWebsite" runat="server" OnClientClick="return confirm('This will restart the entire website for ALL projects, terminating any running tests and any active users sessions. This should only be used as a LAST resort. If you do a website reset, you should also do a full selenium grid reset afterwards to ensure any partially-completed tests are removed from the nodes. Are you certain you want to reset the Website?');"
        onclick="btnResetWebsite_Click" Text="Reset Website" />
</asp:Panel>

</asp:Content>


