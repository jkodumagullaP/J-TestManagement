<%@ Page Title="Add New Test Case" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AddNewTestCase.aspx.cs" Inherits="OSTMSWebsite.Analytics.AddNewTestCase" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div><asp:Label ID="MessageLabel" ForeColor="Red" runat="server" Text=""></asp:Label></div>
<asp:SqlDataSource ID="sqlAddTestCase" runat="server" 
        ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"   
        InsertCommand="" 
        OldValuesParameterFormatString="original_{0}"  >
    <InsertParameters>
        <asp:Parameter Name="testCaseId" Type="Int32" />
        <asp:Parameter Name="projectAbbreviation" Type="String" />
        <asp:Parameter Name="testCaseDescription" Type="String" />
        <asp:Parameter Name="testCaseSteps" Type="String" />
        <asp:Parameter Name="expectedResults" Type="String" />
        <asp:Parameter Name="testCaseNotes" Type="String" />
        <asp:Parameter Name="dateLastUpdated" Type="DateTime" />
        <asp:Parameter Name="updatedBy" Type="String" />
        <asp:Parameter Name="updateStory" Type="String" />
        <asp:Parameter Name="dateCreated" Type="DateTime" />
    </InsertParameters>
    </asp:SqlDataSource>

<asp:FormView ID="fvTestCaseDetails" 
        DataSourceID="sqlAddTestCase" 
        DataKeyNames="projectAbbreviation" 
        oniteminserted="fvTestCaseDetails_ItemInserted"
        RunAt="server" 
        DefaultMode="Insert" CssClass="gvTestCaseList" Width="100%">

<%--**************************** Insert Item Template ****************************--%>

  <InsertItemTemplate>
      <table width="100%" border="1">
	    <thead>
		    <tr>
			    <th colspan="4" align="left"><h1>Add New Test Case</h1></td>
		    </tr>
        </thead>
        <tbody>
            <tr>
			    <th colspan="2">Project</td>
			    <th>Date Created</td>
			    <th>Created By</td>
            </tr>
		    <tr>
			    <td colspan="2">
                <asp:DropDownList ID="ddlProject" runat="server" 
                    AppendDataBoundItems="False" 
                    DataSourceID="sqlProjects"
                    DataTextField="projectName"
                    DataValueField="projectAbbreviation"
                    onselectedindexchanged="ddlProject_SelectedIndexChanged"
                    AutoPostBack="True"
                    SelectedValue='<%# Bind("rawProjectAbbreviation") %>' 

                    Width="100%" CssClass="updateTextbox">
                </asp:DropDownList>

                <asp:SqlDataSource ID="sqlProjects" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT * FROM [Projects]  order by projectName">
                </asp:SqlDataSource>
                </td>		    
                
                <td><asp:Label ID="lblDateCreated" runat="server" Text='<%# Bind("dateCreated") %>' /></td>
			    <td><asp:Label ID="lblCreatedBy" runat="server" Text='<%# Bind("createdBy") %>' /></td>
		    </tr>
            <tr>
			    <td colspan="4"><hr /></td>
		    </tr>


		    <tr>
			    <th colspan="4">Description</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditDescription" Text='<%# Bind("rawTestCaseDescription") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
		    <tr>
			    <th colspan="4">Steps</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditTestCaseSteps" Text='<%# Bind("rawTestCaseSteps") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
		    <tr>
			    <th colspan="4">Expected Results</td>
		    </tr>
            <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditExpectedResults" Text='<%# Bind("rawExpectedResults") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
		    <tr>
			    <th colspan="4">Notes</td>
		    </tr>
		    <tr>
			    <td colspan="4"><asp:TextBox ID="txtbxEditNotes" Text='<%# Bind("rawTestCaseNotes") %>' RunAt="Server" TextMode="MultiLine" Rows="5" Width="100%" CssClass="updateTextbox" /></td>
		    </tr>
            <tr>
			    <th colspan="2">Update Sprint</td>
                <th colspan="2">Update Story</td>
		    </tr>
            
            <tr>
			    <td colspan="2">
                     <asp:DropDownList ID="ddlSprints" runat="server" 
                        AppendDataBoundItems="False" 
                        DataSourceID="sqlSprints"
                        DataTextField="sprint"
                        DataValueField="sprint"
                        onselectedindexchanged="ddlSprints_SelectedIndexChanged"
                        onDataBound="ddlSprints_DataBound"
                        AutoPostBack="False"
                        Width="100%" CssClass="updateTextbox">
                    </asp:DropDownList>


                <asp:SqlDataSource ID="sqlSprints" runat="server" 
                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                    SelectCommand="SELECT '' as [sprint]">
                </asp:SqlDataSource>

			    <td colspan="2"><asp:TextBox ID="txtbxEditUpdateStory" Text='<%# Bind("rawUpdateStory") %>' RunAt="Server" TextMode="MultiLine" Rows="3" Width="100%" CssClass="updateTextbox" /></td>
            </tr>
            <tr> 
                <td colspan="4">
                   <asp:Button ID="InsertButton" runat="server" CausesValidation="True" OnClick="InsertButton_OnClick" AutoPostBack="True" Text="Add"></asp:Button> - <asp:Button ID="InsertCancelButton" runat="server" CausesValidation="False" OnClick="CancelButton_OnClick" AutoPostBack="True" Text="Cancel"></asp:Button>
                </td>
            </tr>
	    </tbody>
    </table>
</InsertItemTemplate>
</asp:FormView>

<asp:Label ID="lblStatus" runat="server" ForeColor="Red" Text=""></asp:Label>

</asp:Content>

<%--**************************** LEFT RAIL CONTENT ****************************--%>
    

    <asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">
    
    <%--********** Releases **********--%>
        <div class="title">Releases</div>
    
    <div>
        <asp:SqlDataSource ID="sqlReleases" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [release] FROM [Releases] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [release] DESC">
            <SelectParameters>
                <%--This has to be added via Page_Load--%>
                <%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:ListBox ID="listBoxReleases" runat="server" 
            AutoPostBack="True" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlReleases"
            DataTextField="release"
            DataValueField="release"
            tooltip="Add test case to release"
            SelectionMode="Multiple"
            Width="200px" Rows="10">
        </asp:ListBox>
    </div>
    <div><hr /></div>
    

    <%--********** Sprints **********--%>
        <div class="title">Sprints</div>
    
    <div>
        <asp:SqlDataSource ID="sqlSprints2" runat="server" 
            ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
            SelectCommand="SELECT [projectAbbreviation], [sprint] FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprint] DESC">
            <SelectParameters>
                <%--This has to be added via Page_Load--%>
                <%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
            </SelectParameters>
        </asp:SqlDataSource>

        <asp:ListBox ID="listBoxSprints" runat="server" 
            AppendDataBoundItems="False" 
            DataSourceID="sqlSprints2"
            DataTextField="sprint"
            DataValueField="sprint"
            tooltip="Add test case to sprint"
            SelectionMode="Multiple"
            Width="200px" Rows="10">
        </asp:ListBox>
    </div>
    <div><hr /></div>

</asp:Content>


