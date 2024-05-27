<%@ Page Title="Add New Automation Test Case" MasterPageFile="" Language="C#" AutoEventWireup="true"  CodeBehind="NewTest.aspx.cs" Inherits="CTWebsite.NewTest" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxControlToolkit" %>
<%@ Register Src="~/UserControls/AddNewGroup.ascx" TagPrefix="uc1" TagName="AddNewGroup" %>
<%@ Register Src="~/UserControls/AddNewRelease.ascx" TagPrefix="uc1" TagName="AddNewRelease" %>
<%@ Register Src="~/UserControls/AddNewSprint.ascx" TagPrefix="uc1" TagName="AddNewSprint" %>

 
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <!--Bootstrap CSS-->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-T3c6CoIi6uLrA9TneNEoa7RxnatzjcDSCmG1MXxSR1GAsXEV/Dwwykc2MPK8M2HN" crossorigin="anonymous" />
    <!--jQuery-->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js" integrity="sha384-KyZXEAg3QhqLMpG8r+J2Wk5vqXn3Fm/z2N1r8f6VZJ4T3Hdvh4kXG1j4fZ6IsU2f5" crossorigin="anonymous"></script>
    <!--AJAX JS-->
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.7.1/jquery.min.js"></script>
    <!--Bootstrap JS-->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-C6RzsynM9kWDrMNeT87bh95OGNyZPhcTNXj1NW7RuBCsyN/o0jlpcV8Qyq46cDfL" crossorigin="anonymous"></script>
    <style type="text/css">
.floatLeft { float: left; }
</style>
<style type="text/css">
.floatRight { float: right; }
</style>
<style type="text/css">
        .auto-style1 {
            border: 2px solid #5D7FC0;
            border-radius: 10px;
            background-color: #D5E1F0;
            margin-top: 10px;
            margin-bottom: 10px;
            height: 1456px;
            width: 370px;
        }
        .auto-style9 {
        width: 1024px;
    }
        .auto-style10 {
        height: 36px;
    }
        .auto-style11 {
        width: 198px;
    }
        .auto-style12 {
        width: 1022px;
        height: 23px;
    }
        .auto-style13 {
        width: 654px;
    }
    .auto-style14 {
        width: 1024px;
        height: 116px;
    }
        .auto-style16 {
        width: 1276px;
    }
    .auto-style17 {
        width: 1026px;
    }
        </style>
            
    <script type="text/javascript" LANGUAGE="JavaScript">
function executeCommands()
{
// Instantiate the Shell object and invoke
//its execute method.

var oShell = new ActiveXObject("Shell.Application");

var commandtoRun = "./node.bat";

// Invoke the execute method.
oShell.ShellExecute(commandtoRun, "", "", "open", "1");
}
</script>

</head>

<body>

    <form id="form1" runat="server">
   <div id="header">
            <%--********** Title **********--%>
            <table id="SiteHeader">
                <tr>
                    <td class="logocontainer">
                        <asp:Image ID="logo" runat="server" ImageUrl="~/Images/ctlogo.png" Height="150px" Width="268px" AlternateText="Crystal Test" Visible="true" />
                    </td>
                    <td class="auto-style16">
                        <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/masthead.png" Height="150px" Width="1099px" AlternateText="Crystal Test" Visible="true" />
                    </td>
                </tr>
                </table>
        </div>
       
        <asp:Panel ID="treeviewMenu" Width="25%"  Height="909px" runat="server" OnClientClick="return false;"   ScrollBars="Both" HorizontalAlign="Left" CssClass="floatLeft">    <%--********** View Dropdown**********--%>
    <div class="auto-style1">
<div class="boxTestCases">
    <div class="boxTestCasesHeaderContainer">
        <div class="left"><strong>
            <asp:Label ID="lblViewDropdownTitle" runat="server" Text="Projects List" />
            </strong></div>
    </div>
    <asp:DropDownList ID="DropDownList1" runat="server" OnSelectedIndexChanged="DropDownList1_SelectedIndexChanged"></asp:DropDownList>
        <asp:Button ID="Button1" runat="server" Text="Browse Test Cases" OnClick="Button1_Click1" />
    <asp:ListBox ID="ListBox1" runat="server" Width="309px" OnSelectedIndexChanged="ListBox1_SelectedIndexChanged" Height="384px" ></asp:ListBox>
        

</div>
            </div>
</asp:Panel>


<asp:Panel ID="qvObjektMenu" Width="75%" Height="137px" runat="server"   HorizontalAlign="Right" CssClass="float-start">
	 <table  border="1" style="border: thick double #669900; background-color: #99CCFF; " class="auto-style14">
              
         <tr>
        <td>
   
                             <strong>
            <asp:Label ID="Label5" runat="server" CssClass="floatLeft" Text="Create New Project" />
            </strong></td>
                  </tr><tr>
<td>                         <asp:Label CssClass="floatLeft"  ID="Label3" runat="server" ForeColor="Blue" Text="Enter Project Name   "></asp:Label></td>
                           <td class="auto-style13">  <asp:TextBox CssClass="floatLeft"  ID="TextBox3" runat="server" OnTextChanged="TextBox3_TextChanged" Height="33px" Width="644px"></asp:TextBox></td> 
</tr><tr>
    <td>
                              <asp:Button ID="Button4" CssClass="floatLeft"  runat="server" OnClick="Button4_Click" Text="New Project" Width="200" /></td>
</tr>
         </table>
    <table class="auto-style9" style="background-color: #99CCFF; border: thick double #99CC00" title="ConFigure Agents">
              <tr>
        <td>
            <strong>
            <asp:Label ID="Label1" runat="server" CssClass="floatLeft" Text="Configure Agents" />
            </strong></td>
        </tr><tr>
                  <td>

                   <asp:Button CssClass="floatLeft" ID="Button7" runat="server" Text="Configure Node" OnClick="Button7_Click" OnClientClick="executeCommands" />
              </td>
                                        <td class="auto-style11">
                                            &nbsp;</td>
              </tr>
        </table>
    <table class="auto-style17" style="background-color: #99CCFF; border: thick double #669900" title="Design Test Cases">
        <tr><td>
          <strong>
            <asp:Label ID="Label2" runat="server" CssClass="floatLeft" Text="Design/Execute" />
            </strong></td></tr><tr>
        		<td>
	<asp:Button ID="Button2" CssClass="floatLeft" runat="server" Text="Start Agent"
															OnClick="Button1_Click" Width="200" /> </td>
                                 <td>    <asp:Button CssClass="floatLeft" ID="Button3" runat="server" Text="Record"
															OnClick="btnStartVisualSearch_Click" Width="200" />   </td>  

     
<td class="auto-style10"><asp:Button ID="Button6" CssClass="floatLeft" runat="server" Text="Generate Test Case " OnClick="Button6_Click" /></td> 		        
                		          <td class="auto-style10">   <asp:Button CssClass="floatLeft" ID="Run" runat="server" Text="Execute " OnClick="Run_Click" /></td> 		        
       
        </table>
    
    <table class="auto-style12">
          <tr><td>
           <strong>
            <asp:Label ID="Label4" runat="server" CssClass="floatLeft" Text="Develop / Configure" />
            </strong></td></tr>
        <tr>
      <td>  <asp:Button ID="btnUpload"  CssClass="floatLeft" runat="server" OnClick="btnUpload_Click" Text="Edit Test Cases"    AutoPostback ="true"   />    
          <asp:Button ID="Downloadlocal" runat="server" CssClass="float-start" OnClick="Downloadlocal_Click" OnClientClick="executeCommands()" Text="Edit Test Configuration" Width="219px" />
          <asp:Button ID="Button5" runat="server" CssClass="float-start" OnClick="Button5_Click" Text="Edit Test Runner" Width="196px" />
      </td>
                <td>  <asp:Button ID="btnSave" CssClass="floatLeft" runat="server" OnClick="btnSave_Click"  Text="Save"   AutoPostback = "false"   />    </td>
      </tr>
        </table><table>
          
            <tr>
            <asp:GridView  ShowHeaderWhenEmpty="true" ID="gridDView"  EnableModelValidation="True"  runat="server" AutoGenerateColumns="true" OnRowUpdating ="gridDView_RowUpdating" OnRowEditing="gridDView_RowEditing" CssClass="table table-bordered border border-1 border-dark-subtle table-hover text-center" OnSelectedIndexChanged="gridDView_SelectedIndexChanged">
                <HeaderStyle CssClass="align-middle table-primary"  />
                <Columns> 

                    <asp:CommandField  ShowEditButton="True" HeaderText="Action" ItemStyle-CssClass="col-md-2 align-middle fw-light" ControlStyle-CssClass="btn btn-light border border-dark-subtle rounded-1 fs-6 fw-light shadow-sm" />


                </Columns>
                                <HeaderStyle BackColor="#df5015" Font-Bold="true" ForeColor="White" />

            </asp:GridView>
                   </td>
               </tr>
            <tr>
               <td>
                     <asp:GridView ID="dataGridView2" runat="server">
                             </asp:GridView>
               </td>
          </tr>
             <tr>
               <td>
                  <asp:GridView  ShowHeaderWhenEmpty="true" ID="GridView1"  EnableModelValidation="True"  runat="server" AutoGenerateColumns="true" OnRowUpdating ="gridDView_RowUpdating" OnRowEditing="gridDView_RowEditing" CssClass="table table-bordered border border-1 border-dark-subtle table-hover text-center" OnSelectedIndexChanged="gridDView_SelectedIndexChanged">
                <HeaderStyle CssClass="align-middle table-primary" />
                
                                <HeaderStyle BackColor="#df5015" Font-Bold="true" ForeColor="White" />

            </asp:GridView>
            
                   </td>
               
                 </tr>  
        </table>
    
    <table>
            <tr>
            <td>

                  
                </td></tr>
            <tr><td>
        <asp:GridView ID="dataGridView1" runat="server" Width="1133px">
                             </asp:GridView>
                            
                           
           </td>
                </tr></table>
   
        </asp:Panel>
    </form>
</body>
</html>






