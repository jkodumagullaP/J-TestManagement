<%@ Page Title="Register" Language="C#" MasterPageFile="~/ThreeColumn.master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="CTWebsite.Account.Register" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    
    <asp:CreateUserWizard ID="RegisterUser" runat="server" EnableViewState="False" 
        OnCreatedUser="RegisterUser_CreatedUser" 
        oncreateusererror="RegisterUser_CreateUserError" 
        oncreatinguser="RegisterUser_CreatingUser">


        <LayoutTemplate>
            <asp:PlaceHolder ID="wizardStepPlaceholder" runat="server"></asp:PlaceHolder>
            <asp:PlaceHolder ID="navigationPlaceholder" runat="server"></asp:PlaceHolder>
        </LayoutTemplate>
        <WizardSteps>
            <asp:CreateUserWizardStep ID="RegisterUserWizardStep" runat="server">
                <ContentTemplate>
                <asp:SqlDataSource ID="InsertExtraInfo" runat="server" ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>"
                InsertCommand="INSERT INTO UserProfiles(UserId, userFirstName, userLastName, userSupervisorFirstName, userSupervisorLastName, userSupervisorEmail, defaultProjectAbbreviation) VALUES(@UserId, @userFirstName, @userLastName, @userSupervisorFirstName, @userSupervisorLastName, @userSupervisorEmail, @defaultProjectAbbreviation)">
                    <InsertParameters>
                        <asp:ControlParameter Name="userFirstname" Type="String" ControlID="FirstName" PropertyName="Text" />
                        <asp:ControlParameter Name="userLastName" Type="String" ControlID="LastName" PropertyName="Text" />
                        <asp:ControlParameter Name="defaultProjectAbbreviation" Type="String" ControlID="ddlProjects" PropertyName="Text" />
                        <asp:ControlParameter Name="userSupervisorFirstName" Type="String" ControlID="SupervisorFirstName" PropertyName="Text" />
                        <asp:ControlParameter Name="userSupervisorLastName" Type="String" ControlID="SupervisorLastName" PropertyName="Text" />
                        <asp:ControlParameter Name="userSupervisorEmail" Type="String" ControlID="SupervisorEmail" PropertyName="Text" />
                    </InsertParameters>
                </asp:SqlDataSource> 

                    <table>
                        <tr>
                            <td align="center" colspan="2"><h1>Sign Up for Your New Account</h1></td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2"><asp:Label ID="localUsername" Visible="false" runat="server">Your password for local testing is always "localtest"</asp:Label></td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                    ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                    ToolTip="User Name is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="FirstName">First Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="FirstName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" 
                                    ControlToValidate="FirstName" ErrorMessage="First Name is required." 
                                    ToolTip="First Name is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="LastNameLabel" runat="server" AssociatedControlID="LastName">Last Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="LastName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" 
                                    ControlToValidate="LastName" ErrorMessage="Last Name is required." 
                                    ToolTip="Last Name is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                                    
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" Visible="false" Text="localtest"></asp:TextBox>
                                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" Visible="false" Text="localtest"></asp:TextBox>
                                
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="EmailRequired" runat="server" 
                                    ControlToValidate="Email" ErrorMessage="E-mail is required." 
                                    ToolTip="E-mail is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="DefaultProjectLabel" runat="server" AssociatedControlID="ddlProjects">Default Project:</asp:Label>
                            </td>
                            <td>
                                <asp:DropDownList ID="ddlProjects" runat="server" 
                                    AppendDataBoundItems="False" 
                                    DataSourceID="sqlProjects"
                                    DataTextField="projectName"
                                    DataValueField="projectAbbreviation">
                                </asp:DropDownList>

                                <asp:SqlDataSource ID="sqlProjects" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:QAAConnectionString %>" 
                                    SelectCommand="SELECT * FROM [Projects] order by projectName">
                                </asp:SqlDataSource>
                                <asp:RequiredFieldValidator ID="DefaultProjectRequired" runat="server" 
                                    ControlToValidate="ddlProjects" ErrorMessage="Default Project is required." 
                                    ToolTip="Default Project is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="SupervisorFirstNameLabel" runat="server" AssociatedControlID="SupervisorFirstName">Supervisors First Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="SupervisorFirstName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="SupervisorLastNameLabel" runat="server" AssociatedControlID="SupervisorLastName">Supervisors Last Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="SupervisorLastName" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="SupervisorEmailLabel" runat="server" AssociatedControlID="SupervisorEmail">Supervisors E-mail:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="SupervisorEmail" runat="server"></asp:TextBox>
                            </td>
                        </tr>

                        <%--
                        <tr>
                            <td align="right">
                                <asp:Label ID="PasswordLabel" runat="server" Visible="false" AssociatedControlID="Password">Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password" Visible="false" Text="**********"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                    ControlToValidate="Password" ErrorMessage="Password is required." 
                                    ToolTip="Password is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" Visible="false" 
                                    AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password" Visible="false" Text="**********"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" 
                                    ControlToValidate="ConfirmPassword" 
                                    ErrorMessage="Confirm Password is required." 
                                    ToolTip="Confirm Password is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        --%>
                        <%--<tr>
                            <td align="right">
                                <asp:Label ID="QuestionLabel" runat="server" AssociatedControlID="Question">Security Question:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Question" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" 
                                    ControlToValidate="Question" ErrorMessage="Security question is required." 
                                    ToolTip="Security question is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="AnswerLabel" runat="server" AssociatedControlID="Answer">Security Answer:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" 
                                    ControlToValidate="Answer" ErrorMessage="Security answer is required." 
                                    ToolTip="Security answer is required." ValidationGroup="RegisterUser">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2">
                                <asp:CompareValidator ID="PasswordCompare" runat="server" 
                                    ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                    Display="Dynamic" 
                                    ErrorMessage="The Password and Confirmation Password must match." 
                                    ValidationGroup="RegisterUser"></asp:CompareValidator>
                            </td>
                        </tr>--%>
                        <tr>
                            <td align="center" colspan="2" style="color:Red;">
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                                <br />
                                <asp:Literal ID="ErrorMessage2" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            <CustomNavigationTemplate> 
                <asp:Button ID="StepPreviousButton" runat="server" CausesValidation="False" CommandName="MovePrevious" Text="Previous" /> 
                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" Text="Next" /> 
            </CustomNavigationTemplate> 

            </asp:CreateUserWizardStep>
<asp:CompleteWizardStep runat="server">
    <ContentTemplate>
        <table>
            <tr>
                <td align="center" colspan="2">
                    Complete</td>
            </tr>
            <tr>
                <td>
                    Your account has been successfully created.</td>
            </tr>
            <tr>
                <td align="right" colspan="2">
                    <asp:Button ID="ContinueButton" runat="server" CausesValidation="False" 
                        CommandName="Continue" Text="Continue" ValidationGroup="RegisterUser" />
                </td>
            </tr>
        </table>
    </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>





























</asp:Content>

<asp:Content ID="Content1" runat="server" contentplaceholderid="LeftColumnContent">
    
</asp:Content>


