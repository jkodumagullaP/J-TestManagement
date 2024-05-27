using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utilities;

namespace OSTMSWebsite.Analytics
{
    public partial class UpdateHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            String tbProjectAbbreviationValue = Request.QueryString["project"];
            String tbTestCaseIdValue = Request.QueryString["testCase"];
            gvUpdateHistory.Caption = "Update History for " + tbProjectAbbreviationValue + "-" + tbTestCaseIdValue;
        }
    }
}