<%@ Page Title="Add New Test Case" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="AddNewTestCase.aspx.cs" Inherits="CTWebsite.FunctionalTesting.AddNewTestCase" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<%@ Register Src="~/UserControls/AddNewGroup.ascx" TagPrefix="uc1" TagName="AddNewGroup" %>
<%@ Register Src="~/UserControls/AddNewRelease.ascx" TagPrefix="uc1" TagName="AddNewRelease" %>
<%@ Register Src="~/UserControls/AddNewSprint.ascx" TagPrefix="uc1" TagName="AddNewSprint" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div>
		<asp:Label ID="MessageLabel" ForeColor="Red" runat="server" Text=""></asp:Label>
        <br />

        <br />
    </div>
	<div>
		<ajaxControlToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server"></ajaxControlToolkit:ToolkitScriptManager>
		<asp:SqlDataSource ID="sqlAddTestCase" runat="server"
			ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
			InsertCommand=""
			OldValuesParameterFormatString="original_{0}">
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

		<ajaxToolkit:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" CssClass="ajax__tab_blueGrad-theme" AutoPostBack="True">
			<%--********************* DETAILS ***********************--%>
			<ajaxToolkit:TabPanel ID="TabPanel1" HeaderText="Add New Test Case" runat="server" CssClass="ajax__tab_blueGrad-theme">
				<ContentTemplate>
					<asp:FormView ID="fvTestCaseDetails"
						DataSourceID="sqlAddTestCase"
						DataKeyNames="projectAbbreviation"
						OnItemInserted="fvTestCaseDetails_ItemInserted"
						runat="server"
						DefaultMode="Insert"
						CssClass=""
						Width="100%">
						<InsertItemTemplate>
							<%--**************************** Insert Item Template ****************************--%>
							<table id="detailsInfo1">
								<tr>
									<th class="detailsInfo-title">
										Project
									</th>
									<th class="detailsInfo-title">
										Description
									</th>
								</tr>
								<tr>
									<td class="detailsInfo-element">
										<asp:DropDownList ID="ddlProject" runat="server"
											AppendDataBoundItems="False"
											DataSourceID="sqlProjects"
											DataTextField="projectName"
											DataValueField="projectAbbreviation"
											OnSelectedIndexChanged="ddlProject_SelectedIndexChanged"
											AutoPostBack="True"
											SelectedValue='<%# Bind("rawProjectAbbreviation") %>'
											CssClass="details-new-project">
										</asp:DropDownList>

										<asp:SqlDataSource ID="sqlProjects" runat="server"
											ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
											SelectCommand="SELECT * FROM [Projects]  order by projectName"></asp:SqlDataSource>
									</td>
									<td class="detailsInfo-element" >
										<asp:TextBox
											ID="txtbxEditDescription"
											Text='<%# Bind("rawTestCaseDescription") %>'
											runat="Server"
											TextMode="MultiLine"
											Rows="1"
											cssClass="textbox" Width="350" />
										<ajaxToolkit:TextBoxWatermarkExtender ID="DescriptionWatermark" runat="server"
											TargetControlID="txtbxEditDescription"
											WatermarkText="Test Case Description"
											WatermarkCssClass="watermarked" />
										<asp:RequiredFieldValidator
											ID="DescriptionRequired"
											runat="server"
											ControlToValidate="txtbxEditDescription"
											ErrorMessage="Description is required."
											ToolTip="Description is required."
											ValidationGroup="UpdateTestCase">
										</asp:RequiredFieldValidator>
									</td>
								</tr>
							</table>
							<br />
							<table id="detailsInfo">
								<tr>
									<th class="detailsInfo-title">Active</th>
									<th class="detailsInfo-title">Date Created</th>
									<th class="detailsInfo-title">Created By</th>
									<th class="detailsInfo-title">Test Category</th>
								</tr>
								<tr>
									<td class="detailsInfo-element">
										<asp:CheckBox ID="chkActive" runat="server" Enabled="true" Checked='<%# Bind("active") %>' />
									</td>
									<td class="detailsInfo-element">
										<asp:Label ID="lblDateCreated" runat="server" Text='<%# Bind("dateCreated") %>' />
									</td>
									<td class="detailsInfo-element">
										<asp:Label ID="lblCreatedBy" runat="server" Text='<%# Bind("createdBy") %>' />
									</td>
									<td class="detailsInfo-element">
										<asp:TextBox ID="txtCategory" runat="server" Text='<%# Bind("testCategory") %>'
											CssClass="textbox" Enabled="true">
										</asp:TextBox>
								</tr>
							</table>
							<br />
							<table id="detailsInfo2">
								<tr>
									<td class="details-info-titles">Test Case Steps
									</td>
								</tr>
								<tr>
									<td class="details-info-elements">
										<asp:TextBox ID="txtbxEditTestCaseSteps" Text='<%# Bind("rawTestCaseSteps") %>'
											runat="Server" TextMode="MultiLine" Rows="10" Width="100%" />
										<asp:RequiredFieldValidator ID="StepsRequired" runat="server"
											ControlToValidate="txtbxEditTestCaseSteps" ErrorMessage="Test Case Steps is required."
											ToolTip="Test Case Steps is required." ValidationGroup="UpdateTestCase">
										</asp:RequiredFieldValidator>
										<ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender1"
											TargetControlID="txtbxEditTestCaseSteps"
											runat="server" EnableSanitization="False" />
									</td>
								</tr>
								<tr>
									<td class="details-info-titles">Expected Results
									</td>
								</tr>
								<tr>
									<td class="details-info-elements">
										<asp:TextBox ID="txtbxEditExpectedResults" Text='<%# Bind("rawExpectedResults") %>' runat="Server" TextMode="MultiLine" Rows="10" Width="100%" />
										<asp:RequiredFieldValidator ID="ExpectedResultsRequired" runat="server"
											ControlToValidate="txtbxEditExpectedResults" ErrorMessage="Expected Results is required."
											ToolTip="Expected Results is required." ValidationGroup="UpdateTestCase">
										</asp:RequiredFieldValidator>
										<ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender2"
											TargetControlID="txtbxEditExpectedResults"
											runat="server" EnableSanitization="False" />
									</td>
								</tr>
								<tr>
									<td class="details-info-titles">Notes
									</td>
								</tr>
								<tr>
									<td class="details-info-elements">
										<asp:TextBox ID="txtbxEditNotes" Text='<%# Bind("rawTestCaseNotes") %>'
											runat="Server" TextMode="MultiLine" Rows="10" Width="100%" />
										<ajaxToolkit:HtmlEditorExtender ID="HtmlEditorExtender3"
											TargetControlID="txtbxEditNotes"
											runat="server" EnableSanitization="False" />
									</td>
								</tr>
								<tr>
									<td class="details-info-titles">Screenshots
									</td>
								</tr>
								<tr>
									<td class="details-info-elements"></td>
								</tr>
								<tr>
									<td>
										<asp:Label ID="lblStatus" runat="server" ForeColor="Red" Text=""></asp:Label>
										<asp:Panel ID="pnlAddNewScreenshot" runat="server" GroupingText="Upload Screenshot" CssClass="dashboardPanel">
											<table>
												<tr>
													<td>File</td>
													<td>
														<asp:FileUpload ID="imgUploadScreenshot" runat="server" /></td>
												</tr>
												<tr>
													<td>Description</td>
													<td>
														<asp:TextBox ID="txtUploadImageDescription" runat="server" Width="400"></asp:TextBox>
													</td>
												</tr>
												<tr>
													<td colspan="2">
														<asp:Button ID="btnUploadScreenshot" runat="server" Text="Upload Screenshot"
															OnClick="btnUploadScreenshot_Click" Width="200" /></td>
												</tr>
											</table>
										</asp:Panel>
										<asp:Panel ID="pnlAddScreenshotURL" runat="server" GroupingText="Add Screenshot URL" CssClass="dashboardPanel">
											<table>
												<tr>
													<td>Image URL</td>
													<td>
														<asp:TextBox ID="txtImageUrl" runat="server" Width="400"></asp:TextBox>
													</td>
												</tr>
												<tr>
													<td>Description</td>
													<td>
														<asp:TextBox ID="txtImageUrlDescription" runat="server" Width="400"></asp:TextBox>
													</td>
												</tr>
												<tr>
													<td colspan="2">
														<asp:Button ID="btnAddImageUrl" runat="server" Text="Add Screenshot URL"
															AutoPostBack="True" OnClick="btnAddImageUrl_Click" Width="200" /></td>
												</tr>
											</table>
										</asp:Panel>

										<asp:ListBox ID="listBoxScreenshots" runat="server"
											Width="600px" Rows="5"></asp:ListBox>
										<br />
										<asp:Button ID="deleteScreenshotButton" runat="server" Text="Delete Screenshot"
											AutoPostBack="True" OnClick="deleteScreenshotButton_Click" Width="200" />
									</td>
								</tr>
								<tr>
									<td colspan="4">
										<asp:Button ID="InsertButton" runat="server"
											CausesValidation="True" OnClick="InsertButton_OnClick"
											AutoPostBack="True" Text="Add"></asp:Button>
										-
										<asp:Button ID="InsertCancelButton"
											runat="server" CausesValidation="False"
											OnClick="CancelButton_OnClick" AutoPostBack="True" Text="Cancel"></asp:Button>
									</td>
								</tr>
							</table>
						</InsertItemTemplate>
					</asp:FormView>
				</ContentTemplate>
			</ajaxToolkit:TabPanel>
		</ajaxToolkit:TabContainer>
	</div>
</asp:Content>

<%--**************************** LEFT RAIL CONTENT ****************************--%>


<asp:Content ID="Content3" ContentPlaceHolderID="LeftColumnContent" runat="server">

    <%--This has to be added via Page_Load--%>
<div class="box">
	<div class="boxHeaderContainer">
		<div class="left"><asp:label Id="Label4" runat="server" Text="Groups" /></div>
		<div class="right"><asp:ImageButton ID="btnAddNewGroup" runat="server" Tooltip="Add New Group" Width="30" Height="30" ImageUrl="~/Images/add.png" /></div>
	</div>
	<p>
		<asp:SqlDataSource ID="sqlGroupTests" runat="server"
			ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
			SelectCommand="SELECT [projectAbbreviation], [groupTestAbbreviation], [groupTestName] FROM [GroupTests] WHERE ([projectAbbreviation] = @projectAbbreviation)  and (personalGroupOwner is null or personalGroupOwner = @userName) ORDER BY [groupTestName]">
			<SelectParameters>
				<%--This has to be added via Page_Load--%>
				<%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
				<%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
			</SelectParameters>
		</asp:SqlDataSource>
		<asp:ListBox ID="listBoxGroupTests" runat="server"
			DataSourceID="sqlGroupTests"
			DataTextField="groupTestName"
			DataValueField="groupTestAbbreviation"
			ToolTip="Add test case to group"
			SelectionMode="Multiple"
			Width="200px" Rows="8"></asp:ListBox>
	</p>
</div>
   
<asp:panel id="Panel1" runat="server" AutoPostBack="true">
	<div class="AddNewModal">
		<div class="PopupHeader" id="PopupHeader1">Add New Group</div>
		<div class="PopupBody">
			<uc1:AddNewGroup ID="AddNewGroup" runat="server"/>
		</div>
	</div>
</asp:panel>    
	
<ajaxToolkit:modalpopupextender id="ModalPopupExtender1" runat="server" 
	targetcontrolid="btnAddNewGroup" popupcontrolid="Panel1" 
	popupdraghandlecontrolid="PopupHeader1" drag="true" 
	backgroundcssclass="ModalPopupBG">
</ajaxToolkit:modalpopupextender>


<%--<asp:Parameter Name="userName" runat="server"  DefaultValue='<%# Eval("User.Identity.Name") %>' />--%>
<div class="box">
	<div class="boxHeaderContainer">
		<div class="left"><asp:label Id="Label1" runat="server" Text="Releases" /></div>
		<div class="right"><asp:ImageButton ID="btnAddNewRelease" runat="server" Tooltip="Add New Release" Width="30" Height="30" ImageUrl="~/Images/add.png" /></div>
	</div>
	<p>
		<asp:SqlDataSource ID="sqlReleases" runat="server"
			ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
			SelectCommand="SELECT [projectAbbreviation], [release] FROM [Releases] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [release] DESC">
			<SelectParameters>
				<%--This has to be added via Page_Load--%>
				<%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
			</SelectParameters>
		</asp:SqlDataSource>

		<asp:ListBox ID="listBoxReleases" runat="server"
			DataSourceID="sqlReleases"
			DataTextField="release"
			DataValueField="release"
			ToolTip="Add test case to release"
			SelectionMode="Multiple"
			Width="200px" Rows="8"></asp:ListBox>
	</p>
</div>

<asp:panel id="Panel2" runat="server" AutoPostBack="true">
	<div class="AddNewModal">
		<div class="PopupHeader" id="PopupHeader2">Add New Release</div>
		<div class="PopupBody">
			<uc1:AddNewRelease ID="AddNewRelease" runat="server" />
		</div>
	</div>
</asp:panel>    
	
<ajaxToolkit:modalpopupextender id="ModalPopupExtender2" runat="server" 
	targetcontrolid="btnAddNewRelease" popupcontrolid="Panel2" 
	popupdraghandlecontrolid="PopupHeader2" drag="true" 
	backgroundcssclass="ModalPopupBG">
</ajaxToolkit:modalpopupextender>


<%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
<div class="box">
	<div class="boxHeaderContainer">
		<div class="left"><asp:label Id="Label2" runat="server" Text="Sprints" /></div>
		<div class="right"><asp:ImageButton ID="btnAddNewSprint" runat="server" Tooltip="Add New Sprint" Width="30" Height="30" ImageUrl="~/Images/add.png" /></div>
	</div>
	<p>
		<asp:SqlDataSource ID="sqlSprints2" runat="server"
			ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
			SelectCommand="SELECT [projectAbbreviation], [sprint] FROM [Sprints] WHERE ([projectAbbreviation] = @projectAbbreviation) ORDER BY [sprint] DESC">
			<SelectParameters>
				<%--This has to be added via Page_Load--%>
				<%--<asp:QueryStringParameter Name="projectAbbreviation" QueryStringField="project" Type="String" />--%>
			</SelectParameters>
		</asp:SqlDataSource>

		<asp:ListBox ID="listBoxSprints" runat="server"
			DataSourceID="sqlSprints2"
			DataTextField="sprint"
			DataValueField="sprint"
			ToolTip="Add test case to sprint"
			SelectionMode="Multiple"
			Width="200px" Rows="8"></asp:ListBox>
	</p>
</div>

<asp:panel id="Panel3" runat="server" AutoPostBack="true">
	<div class="AddNewModal">
		<div class="PopupHeader" id="PopupHeader3">Add New Sprint</div>
		<div class="PopupBody">
			<uc1:AddNewSprint ID="AddNewSprint" runat="server" />
		</div>
	</div>
</asp:panel>    
	
<ajaxToolkit:modalpopupextender id="ModalPopupExtender3" runat="server" 
	targetcontrolid="btnAddNewSprint" popupcontrolid="Panel3" 
	popupdraghandlecontrolid="PopupHeader3" drag="true" 
	backgroundcssclass="ModalPopupBG">
</ajaxToolkit:modalpopupextender>


</asp:Content>


