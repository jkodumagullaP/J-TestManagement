<%@ Page Title="Analytics Test URL Details" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="TestURLDetails.aspx.cs" Inherits="OSTMSWebsite.Analytics.AnalyticsTestURLDetails" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<%--**************************** LEFT RAIL CONTENT ****************************--%>
<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">   
    <%--********** Environments Dropdown **********--%>

    <div class="title">Environment</div>
    <div class="ddlDropDownLeftRail">
        <asp:DropDownList ID="ddlEnvironment" runat="server" 
            DataTextField="environment" 
            DataValueField="environment" 
            AppendDataBoundItems="False" 
            AutoPostBack="True"
            onselectedindexchanged="ddlEnvironment_SelectedIndexChanged" 
            CssClass="leftRailDropdown">
        </asp:DropDownList>
    </div>
</asp:Content>

<%--**************************** MAIN CONTENT ****************************--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:Label ID="TopMessageLabel" runat="server" Text="" CssClass="Error"></asp:Label>
    <asp:Label ID="lblURLHeader" runat="server" width="100%" CssClass="headerURL" />
    <asp:GridView ID="gvURLList" runat="server"
            AutoGenerateColumns="false"
            onrowdatabound="gvURLList_RowDataBound"
            onrowediting="gvURLList_RowEditing"
            onrowcancelingedit="gvURLList_RowCancelingEdit"
            onrowupdating="gvURLList_RowUpdating"
            onrowdeleting="gvURLList_RowDeleting"
            CssClass="gvTestCaseList">
        <Columns>
            <asp:CommandField ShowEditButton="true" />
            <asp:CommandField ShowDeleteButton="true" />
            <asp:TemplateField HeaderText="Environment" SortExpression="environment">
                <ItemTemplate>
                    <asp:Label ID="lblEnvironment" runat="server" Text='<%# Bind("environment") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="lblEnvironments" runat="server" Text='<%# Bind("environment") %>'></asp:Label>
                </EditItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="URL" SortExpression="URL">
                <ItemTemplate>
                    <asp:Label ID="lblURL" runat="server" Text='<%# Bind("URL") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtURL" runat="server" Text='<%# Bind("URL") %>' Width="90%"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Omniture String" SortExpression="OmnitureString">
                <ItemTemplate>
                    <asp:Label ID="lblOmniture" runat="server" Text='<%# Bind("OmnitureString") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtOmniture" runat="server" Text='<%# Bind("OmnitureString") %>' Width="90%"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField> 
        </Columns>
    </asp:GridView>            

    <asp:Button ID="btnAddURL" runat="server" OnClick="btnAddURL_Click" Text="Add URL" />
    <asp:Panel ID="pnlNewURL" runat="server" Visible="false">
        <table class="gvTestCaseList">
            <tr>
                <th width="50%">URL</th>
                <th width="50%">Omniture String</th>
            </tr>
            <tr>
                <td width="49%">
                    <asp:TextBox ID="txtURL" runat="server" Width="98%"></asp:TextBox>
                </td>
                <td width="49%">
                    <asp:TextBox ID="txtOmnitureString" runat="server" Width="98%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:LinkButton ID="lnkSubmit" runat="server" OnClick="lnkSubmit_Click" Text="Submit"></asp:LinkButton>
                </td>
                <td>
                    <asp:LinkButton ID="lnkCancel" runat="server" OnClick="lnkCancel_Click" Text="Cancel"></asp:LinkButton>
                </td>
                <td>
                </td>
            </tr>
        </table>    
    </asp:Panel>

    <br />
    <hr />
    <br />

    <asp:Label ID="lblExpectedResultsHeader" runat="server" width="100%" CssClass="headerURL" />
    <asp:GridView ID="gvExpectedResults" runat="server"
            AutoGenerateColumns="false"
            onrowdatabound="gvExpectedResults_RowDataBound"
            onrowediting="gvExpectedResults_RowEditing"
            onrowcancelingedit="gvExpectedResults_RowCancelingEdit"
            onrowupdating="gvExpectedResults_RowUpdating"
            onrowdeleting="gvExpectedResults_RowDeleting"
            CssClass="gvTestCaseList">
        <Columns>
            <asp:CommandField ShowEditButton="true" />
            <asp:CommandField ShowDeleteButton="true" />
            <asp:TemplateField HeaderText="Field Name" SortExpression="fieldName">
                <ItemTemplate>
                    <asp:Label ID="lblFieldName" runat="server" Text='<%# Bind("fieldName") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtFieldName" runat="server" Text='<%# Bind("fieldName") %>'></asp:TextBox>
                </EditItemTemplate>                
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Search String" SortExpression="searchString">
                <ItemTemplate>
                    <asp:Label ID="lblSearchString" runat="server" Text='<%# Bind("searchString") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtSearchString" runat="server" Text='<%# Bind("searchString") %>' Width="90%"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Expected Value" SortExpression="expectedValue">
                <ItemTemplate>
                    <asp:Label ID="lblExpectedValue" runat="server" Text='<%# Bind("expectedValue") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="txtExpectedValue" runat="server" Text='<%# Bind("expectedValue") %>' Width="90%"></asp:TextBox>
                </EditItemTemplate>
            </asp:TemplateField> 
            <asp:TemplateField HeaderText="Old Field Name" SortExpression="fieldName" Visible="false">
                <ItemTemplate>
                    <asp:Label ID="lblOldField" runat="server" Text='<%# Bind("fieldName") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:Label ID="lblOldFieldName" runat="server" Text='<%# Bind("fieldName") %>' Width="90%"></asp:Label>
                </EditItemTemplate>
            </asp:TemplateField> 
        </Columns>
    </asp:GridView>            

    <asp:Button ID="btnAddExpectedResult" runat="server" OnClick="btnAddExpectedResult_Click" Text="Add Result" />
    <asp:Panel ID="pnlNewExpectedResult" runat="server" Visible="false">
        <table class="gvTestCaseList">
            <tr>
                <th width="33%">Field Name</th>
                <th width="33%">Search String</th>
                <th width="33%">Expected Value</th>
            </tr>
            <tr>
                <td width="33%">
                    <asp:TextBox ID="txtFieldName" runat="server" Width="98%"></asp:TextBox>
                </td>
                <td width="33%">
                    <asp:TextBox ID="txtSearchString" runat="server" Width="98%"></asp:TextBox>
                </td>
                <td width="33%">
                    <asp:TextBox ID="txtExpectedValue" runat="server" Width="98%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:LinkButton ID="lnkSubmitResult" runat="server" OnClick="lnkSubmitResult_Click" Text="Submit"></asp:LinkButton>
                </td>
                <td>
                    <asp:LinkButton ID="lnkCancelResult" runat="server" OnClick="lnkCancelResult_Click" Text="Cancel"></asp:LinkButton>
                </td>
                <td>
                </td>
            </tr>
        </table>    
    </asp:Panel>
</asp:Content>

<%--**************************** RIGHT RAIL CONTENT ****************************--%>

<asp:Content ID="Content4" ContentPlaceHolderID="RightColumnContent" runat="server">
    <%--********** Command Buttons **********--%>
    <div class="title">Notes</div>
    To enter multiple values for an Expected Value, separate them with a comma with no spaces in between.
    <br />
    For example:  Monday,Tuesday,Wednesday
    
    

</asp:Content>

