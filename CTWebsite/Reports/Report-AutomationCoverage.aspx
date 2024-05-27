<%@ Page Title="" Language="C#" MasterPageFile="~/ThreeColumn.Master" AutoEventWireup="true" CodeBehind="Report-AutomationCoverage.aspx.cs" Inherits="OSTMSWebsite.Reports.ReportsAutomationCoverage" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript" src="http://ent-mocdvtab01/javascripts/api/viz_v1.js">
</script>

<div class="tableauPlaceholder" style="width:1004px; height:836px;">
    <object class="tableauViz" width="1004" height="836" style="display:none;">
        <param name="host_url" value="http%3A%2F%2Fent-mocdvtab01%2F" />
        <param name="site_root" value="" />
        <param name="name" value="AutomatedTestCaseCoverage&#47;AutomatedTestCaseCoverage_Worksheet" />
        <param name="tabs" value="no" />
        <param name="toolbar" value="yes" />
    </object>
</div>


<%--    
    <div class="comingSoon">
        <asp:Image ID="imgComingSoon" runat="server" 
            ImageUrl="~/Images/comingsoon2.jpg" />
    </div>
--%>

<%--<div>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" 
        Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
        WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
        <LocalReport ReportPath="Reports\Report2.rdlc">
        </LocalReport>

    </rsweb:ReportViewer>

</div>--%>

</asp:Content>
