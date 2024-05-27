<%@ Page Title="Admin Releases" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AdminReleases.aspx.cs" Inherits="CTWebsite.Admin.AdminReleases" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
</asp:Content>
<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="adminSectionHeader">To add a release, select a project and click the + button.</div>
    <div class="adminSectionHeader">To edit or delete a release, select a project and click the appropriate button.</div>
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
            ID="txtSelectedRelease"
            Text=''
            runat="Server"
            Visible="False" />
        &nbsp
        <asp:ImageButton ID="ibtnAdd" CommandName="AddRelease" runat="server"
            ImageUrl="~/Images/add.png" Height="20" Width="20" ToolTip="Add new release."
            OnClick="ibtnAdd_Click"></asp:ImageButton>

    </div>
    <div>
        <br />
    </div>
    <div>
        <asp:Label ID="lblMessage" runat="server" Text="" ForeColor="Red"></asp:Label>
    </div>
    <div>
        <asp:SqlDataSource ID="sqlAddUpdateRelease" runat="server"
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
            SelectCommand="SELECT * FROM [Releases] WHERE (([projectAbbreviation] = @projectAbbreviation) AND ([release] = @release)) ORDER BY [release]"
            OldValuesParameterFormatString="original_{0}">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation" PropertyName="SelectedValue" Type="String" />
                <asp:ControlParameter ControlID="txtSelectedRelease" Name="release" PropertyName="Text" Type="String" />
            </SelectParameters>
            <InsertParameters>
                <asp:Parameter Name="projectAbbreviation" Type="String" />
                <asp:Parameter Name="release" Type="String" />
                <asp:Parameter Name="releaseDescription" Type="String" />
                <asp:Parameter Name="releaseDate" Type="DateTime" />
            </InsertParameters>
        </asp:SqlDataSource>

        <asp:FormView ID="fvReleases"
            DataSourceID="sqlAddUpdateRelease"
            DataKeyNames="projectAbbreviation"
            runat="server"
            DefaultMode="Insert" CssClass="gvGlobalGridview" Width="100%"
            Caption="Add New Release" OnDataBound="fvReleases_DataBound"
            OnItemDeleted="fvReleases_ItemDeleted" OnItemInserted="fvReleases_ItemInserted"
            OnItemUpdated="fvReleases_ItemUpdated">

            <%--**************************** Insert Item Template ****************************--%>

            <InsertItemTemplate>
                <table width="100%" border="1">

                    <tr>
                        <th>Project</th>
                        <th>Release Title</th>
                        <th>Release Date</th>
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
                            <asp:TextBox ID="txtRelease" Text='<%# Bind("release") %>' runat="Server" CssClass="updateTextbox" /></td>
                        <td>
                            <asp:TextBox ID="txtDateInsert" Text='<%# Bind("releaseDate") %>' runat="server" />
                            <asp:ImageButton ID="imgCalDateSelectionInsert" ImageUrl="~/Images/calendar.png" class="calendarButton" Height="20" Width="20" runat="server" OnClick="imgCalDateSelectionInsert_click" />
                            <asp:Calendar ID="calDateInsert" OnSelectionChanged="calDateInsert_SelectionChanged" runat="server" Visible="False" />
                        </td>
                    </tr>

                    <tr>
                        <th colspan="3">
                        Release Description</td>
                    </tr>

                    <tr>
                        <td colspan="3">
                            <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("releaseDescription") %>' runat="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <asp:Button ID="InsertButton" runat="server" CausesValidation="True" OnClick="InsertButton_Click" Text="Add"></asp:Button>
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
                        <th>Release Title</th>
                        <th>Release Date</th>
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
                            <asp:TextBox ID="txtRelease" Text='<%# Bind("release") %>' runat="Server" CssClass="updateTextbox" ReadOnly="True" Enabled="False" /></td>
                        <td>
                            <asp:TextBox ID="txtDateEdit" Text='<%# Bind("releaseDate") %>' runat="server" />
                            <asp:ImageButton ID="imgCalDateSelectionEdit" ImageUrl="~/Images/calendar.png" class="calendarButton" Height="20" Width="20" runat="server" OnClick="imgCalDateSelectionEdit_click" />
                            <asp:Calendar ID="calDateEdit" OnSelectionChanged="calDateEdit_SelectionChanged" runat="server" Visible="False" />
                        </td>
                    </tr>
                    <tr>
                        <th colspan="3">
                        Release Description</td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("releaseDescription") %>' runat="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" OnClick="EditButton_Click" Text="Update"></asp:Button>
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
        <asp:SqlDataSource ID="sqlReleaseList" runat="server"
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
            SelectCommand="SELECT * FROM [Releases] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [releaseDate] DESC">
            <SelectParameters>
                <asp:ControlParameter ControlID="ddlProjects" Name="projectAbbreviation"
                    PropertyName="SelectedValue" Type="String" />
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:GridView ID="gvReleaseList" runat="server"
            DataSourceID="sqlReleaseList"
            ToolTip="Releases"
            AutoGenerateColumns="False"
            GridLines="None"
            AllowPaging="True"
            CssClass="gvGlobalGridview"
            PagerStyle-CssClass="pgr"
            AlternatingRowStyle-CssClass="alt" PageSize="50">
            <Columns>
                <asp:TemplateField HeaderText="Edit/Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="ibtnEdit" CommandName="EditRelease" runat="server"
                            ImageUrl="~/Images/edit.png" Height="15" Width="15" ToolTip="Edit this release."
                            CommandArgument='<%# Eval("release") %>' OnClick="ibtnEdit_Click"></asp:ImageButton>

                        <asp:ImageButton ID="ibtnRemove" CommandName="DeleteRelease" runat="server"
                            ImageUrl="~/Images/delete.png"
                            OnClientClick="return confirm('Are you certain you want to delete this release?');"
                            Height="15" Width="15" ToolTip="Delete this release."
                            CommandArgument='<%# Eval("release") %>'
                            OnClick="ibtnRemove_Click"></asp:ImageButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="projectAbbreviation"
                    HeaderText="Project" SortExpression="projectAbbreviation" HtmlEncode="False" />
                <asp:BoundField DataField="release"
                    HeaderText="Release" SortExpression="release" HtmlEncode="False" />
                <asp:BoundField DataField="releaseDescription"
                    HeaderText="Description" SortExpression="releaseDescription" HtmlEncode="False" />
                <asp:BoundField DataField="releaseDate"
                    HeaderText="Release Date" SortExpression="releaseDate" DataFormatString="{0:d}" HtmlEncode="False" />

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
                    No releases have been created for this project.
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
