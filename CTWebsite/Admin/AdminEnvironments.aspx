<%@ Page Title="Admin Environments" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminEnvironments.aspx.cs" Inherits="CTWebsite.Admin.AdminEnvironments" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
</asp:Content>
<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="adminSectionHeader">To add an environment, select a project and click the + button.</div>
    <div class="adminSectionHeader">To edit or delete an environment, select a project and click the appropriate button.</div>
    <div class="commands">
        <asp:DropDownList ID="ddlProjects" runat="server"
            AutoPostBack="True"
            AppendDataBoundItems="False"
            DataSourceID="sqlProjects"
            DataTextField="projectName"
            DataValueField="projectAbbreviation"
            OnSelectedIndexChanged="ddlProjects_SelectedIndexChanged">
        </asp:DropDownList>

        <asp:SqlDataSource ID="sqlProjects" runat="server"
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
            SelectCommand="SELECT * FROM [Projects] order by projectName"></asp:SqlDataSource>

        <%--DO NOT DELETE THIS TEXTBOX, IT IS NEEDED FOR BINDING PURPOSES--%>
        <asp:TextBox
            ID="txtSelectedEnvironment"
            Text=''
            runat="Server"
            Visible="False" />
        &nbsp
        <asp:ImageButton ID="ibtnAdd" CommandName="AddEnvironment" runat="server"
            ImageUrl="~/Images/add.png" Height="20" Width="20" ToolTip="Add new release."
            OnClick="ibtnAdd_Click"></asp:ImageButton>

    </div>
    <div>
        <br />
    </div>
    <div>
        <asp:Label ID="lblMessage" runat="server" cssClass="failureNotification" Text="" ForeColor="Red"></asp:Label>
    </div>
    <div>
        <br />
    </div>
    <div>
        <asp:SqlDataSource ID="sqlAddUpdateEnvironment" runat="server"
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
            SelectCommand="SELECT * FROM [ProjectEnvironmentInfo] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([environment] = @environment)) ORDER BY [environment]"
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="txtSelectedEnvironment" Name="environment" PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:FormView ID="fvEnvironments"
            DataSourceID="sqlAddUpdateEnvironment"
            DataKeyNames="projectAbbreviation"
            runat="server"
            DefaultMode="Insert" CssClass="gvGlobalGridview" Width="100%"
            Caption="Add New Environment" OnDataBound="fvEnvironments_DataBound"
            OnItemDeleted="fvEnvironments_ItemDeleted" 
            OnItemInserted="fvEnvironments_ItemInserted"
            OnItemUpdated="fvEnvironments_ItemUpdated">

            <%--**************************** Insert Item Template ****************************--%>

            <InsertItemTemplate>
                <table width="100%" border="1">

                    <tr>
                        <th>Project</th>
                        <th>Environment</th>
                        <th>Base URL</th>
                        <th>Base Admin URL</th>
                        <th>Default Environment</th>
                    </tr>

                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlProject" runat="server"
                                AutoPostBack="False"
                                AppendDataBoundItems="False"
                                DataSourceID="sqlProjects"
                                DataTextField="projectName"
                                DataValueField="projectAbbreviation"
                                SelectedValue='<%# Bind("projectAbbreviation") %>'
                                Width="100%">
                            </asp:DropDownList>

                            <asp:SqlDataSource ID="sqlProjects" runat="server"
                                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
                                SelectCommand="SELECT * FROM [Projects] order by projectName"></asp:SqlDataSource>
                        </td>

                        <td>
                            <asp:TextBox ID="txtEnvironment" Text='<%# Bind("environment") %>' runat="Server" CssClass="updateTextbox" />
                        </td>

                        <td>
                            <asp:TextBox ID="txtBaseURL" Text='<%# Bind("baseURL") %>' runat="server" />
                        </td>

                        <td>
                            <asp:TextBox ID="txtBaseAdminURL" Text='<%# Bind("baseAdminURL") %>' runat="Server" CssClass="updateTextbox" />
                        </td>
                        
                        <td>
                            <asp:CheckBox ID="chkDefaultEnvironment" Checked='<%# Bind("defaultEnvironment") %>' runat="Server" CssClass="updateTextbox" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="InsertButton" runat="server" Text="Add" OnClick="InsertButton_Click"></asp:Button>
                            &nbsp
                            <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
            </InsertItemTemplate>

            <EditItemTemplate>
                <table width="100%" border="1">
                    <tr>
                        <th>Project</th>
                        <th>Environment</th>
                        <th>Base URL</th>
                        <th>Base Admin URL</th>
                        <th>Default Environment</th>
                    </tr>
                    <tr>
                        <td>
                            <asp:DropDownList ID="ddlProject" runat="server"
                                AutoPostBack="False"
                                AppendDataBoundItems="False"
                                DataSourceID="sqlProjects"
                                DataTextField="projectName"
                                DataValueField="projectAbbreviation"
                                SelectedValue='<%# Bind("projectAbbreviation") %>'
                                Width="100%" Enabled="False">
                            </asp:DropDownList>

                            <asp:SqlDataSource ID="sqlProjects" runat="server"
                                ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
                                SelectCommand="SELECT * FROM [Projects] order by projectName"></asp:SqlDataSource>
                        </td>


                        <td>
                            <asp:TextBox ID="txtEnvironment" Text='<%# Bind("environment") %>' runat="Server" CssClass="updateTextbox" ReadOnly="True" Enabled="False" />
                        </td>

                        <td>
                            <asp:TextBox ID="txtBaseURL" Text='<%# Bind("baseURL") %>' runat="server" />
                        </td>

                        <td>
                            <asp:TextBox ID="txtBaseAdminURL" Text='<%# Bind("baseAdminURL") %>' runat="server" />
                        </td>

                        <td>
                            <asp:Checkbox ID="chkDefaultEnvironment" Checked='<%# Bind("defaultEnvironment") %>' runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5">
                            <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="UpdateButton_Click" Text="Update"></asp:Button>
                            &nbsp
                            <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel" OnClick="CancelButton_Click"></asp:Button>
                        </td>
                    </tr>
                </table>
            </EditItemTemplate>
            <EmptyDataTemplate>
                <div class="NoData">
                    <asp:Image ID="NoDataImage"
                        ImageUrl="~/Images/NoData.png"
                        AlternateText="No Data Image"
                        runat="server"
                        Height="64" Width="64" />
                    <br />
                    No data available for the filters you have chosen.
                </div>
            </EmptyDataTemplate>

        </asp:FormView>
    </div>

    <div>
        <asp:SqlDataSource ID="sqlEnvironmentList" runat="server"
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
            SelectCommand="SELECT * FROM [ProjectEnvironmentInfo] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [environment] DESC">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation"
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:GridView ID="gvEnvironmentList" runat="server"
            DataSourceID="sqlEnvironmentList"
            ToolTip="Environments"
            AutoGenerateColumns="False"
            GridLines="None"
            AllowPaging="True"
            CssClass="gvGlobalGridview"
            PagerStyle-CssClass="pgr"
            AlternatingRowStyle-CssClass="alt" PageSize="50">
            <Columns>
                <asp:TemplateField HeaderText="Edit/Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="ibtnEdit" CommandName="EditEnvironment" runat="server"
                            ImageUrl="~/Images/edit.png" Height="15" Width="15" ToolTip="Edit this environment."
                            CommandArgument='<%# Eval("environment") %>' OnClick="ibtnEdit_Click"></asp:ImageButton>

                        <asp:ImageButton ID="ibtnRemove" CommandName="DeleteEnvironment" runat="server"
                            ImageUrl="~/Images/delete.png"
                            OnClientClick="return confirm('Are you certain you want to delete this environment?');"
                            Height="15" Width="15" ToolTip="Delete this environment."
                            CommandArgument='<%# Eval("environment") %>'
                            OnClick="ibtnRemove_Click"></asp:ImageButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="projectAbbreviation"
                    HeaderText="Project" SortExpression="projectAbbreviation" HtmlEncode="False" />
                <asp:BoundField DataField="environment"
                    HeaderText="Environment" SortExpression="environment" HtmlEncode="False" />
                <asp:BoundField DataField="baseURL"
                    HeaderText="Base URL" SortExpression="baseURL" HtmlEncode="False" />
                <asp:BoundField DataField="baseAdminURL"
                    HeaderText="Base Admin URL" SortExpression="baseAdminURL" HtmlEncode="False" />
                <asp:TemplateField HeaderText="Default Environment">
                    <ItemTemplate>
                        <asp:Checkbox ID="chkDefaultEnvironment" Checked='<%# Eval("defaultEnvironment") %>' runat="server" OnCheckedChanged="chkDefaultEnvironment_CheckedChanged" AutoPostBack="true"    />
                    </ItemTemplate>
                </asp:TemplateField>
                
            </Columns>
            <EmptyDataRowStyle CssClass="emptydata" />
            <EmptyDataTemplate>
                <div class="NoData">
                    <asp:Image ID="NoDataImage"
                        ImageUrl="~/Images/NoData.png"
                        AlternateText="No Data Image"
                        runat="server"
                        Height="130" Width="139" />
                    <br />
                    No environments have been created for this project.
                </div>
            </EmptyDataTemplate>
            <PagerSettings Mode="NumericFirstLast" Position="TopAndBottom" />
            <PagerStyle CssClass="pgr"></PagerStyle>

        </asp:GridView>
    </div>

</asp:Content>
<%--**************************** RIGHT RAIL CONTENT ****************************--%>
<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">
</asp:Content>
