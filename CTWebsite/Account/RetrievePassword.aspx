<%@ Page Title="Password Recovery" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="RetrievePassword.aspx.cs" Inherits="CTWebsite.Account.RetrievePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    
    <asp:PasswordRecovery OnVerifyingUser="validateUserEmail" 
      SuccessText="Your password was successfully reset and emailed to you."
      QuestionFailureText="Incorrect answer. Please try again." 
      runat="server" ID="PWRecovery" 
      UserNameFailureText="Username not found.">
    <MailDefinition IsBodyHtml="true" BodyFileName="passwordrecoveryemail.txt"
           From="YourEmailAddress@YourDomain.com" 
           Subject="Password Reset" 
           Priority="High">
    </MailDefinition>
    <UserNameTemplate>
        <asp:Panel ID="Panel1" runat="server" GroupingText="Password Recovery">
        <p>Fill in your username and email address below and click the submit button. You will then be asked to answer the security question you set up upon registration. If you answer the question correctly, you will be emailed a new password.</p>
        <dl>
            <dt>Username</dt>
            <dd>
                <asp:TextBox ID="Username" runat="server" />
            </dd>
            <dt>Email</dt>
            <dd>
                <asp:TextBox ValidationGroup="PWRecovery" 
                   runat="server" ID="txtEmail">
                </asp:TextBox>
            </dd>
            <dt></dt>
            <dd>
                <asp:Button ID="submit" 
                   CausesValidation="true" 
                   ValidationGroup="PWRecovery" 
                   runat="server"
                   CommandName="Submit" 
                   Text="Submit" />
            </dd>
            <dt></dt>
            <dd>
                <p class="Error"><asp:Literal ID="ErrorLiteral" 
                         runat="server"></asp:Literal>
                </p>
            </dd>
        </dl>
        </asp:Panel>
    </UserNameTemplate>
    <QuestionTemplate>
    <asp:Panel ID="Panel1" runat="server" GroupingText="Password Recovery">
        <p>
            You must answer your recovery question 
            in order to have a new email sent to you.
        </p>
        <dl>
            <dt>Question:</dt>
            <dd>
                <asp:Literal runat="server" ID="Question" />
            </dd>
            <dt></dt>
            <dt>Answer:</dt>
            <dd>
                <asp:TextBox runat="server" ID="Answer" />
            </dd>
            <dt></dt>
            <dd>
                <asp:Button runat="server" ID="submit" 
                  Text="Submit" CommandName="submit" />
            </dd>
            <dt></dt>
            <dd>
                <p class="Error">
                    <asp:Literal ID="FailureText" runat="server">
    </asp:Literal>
    </p>
            </dd>
        </dl>
        </asp:Panel>
    </QuestionTemplate>
</asp:PasswordRecovery>
</asp:Content>
