using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using CTInfrastructure;
using Utilities;
using System.IO;

namespace CTWebsite.FunctionalTesting
{
    public partial class AddNewScreenshot : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
                
        }

        protected void btnAddImageUrl_Click(object sender, EventArgs e)
        {
            AddScreenshot(txtImageUrl.Text, txtImageUrlDescription.Text);
        }

        private void AddScreenshot(string imageURL, string description)
        {
            Label lblStatus = (Label)AspUtilities.FindControlRecursive(this, "lblStatus");

            if (!imageURL.ToLower().EndsWith(".jpg")
             && !imageURL.ToLower().EndsWith(".gif")
             && !imageURL.ToLower().EndsWith(".png"))
            {
                lblStatus.Text = "Screenshot URLs must end with .jpg, .png, or .gif";
                return;
            }

            string typeQueryStringValue = Request.QueryString["type"];
            string projectQueryStringValue = Request.QueryString["project"];
            int testCaseQueryStringValue = Convert.ToInt32(Request.QueryString["testCase"]);

            try
            {
                //If a project was given in the query string, choose it now
                if (typeQueryStringValue == "TestResult")
                {
                    int testRunIDQueryStringValue = Convert.ToInt32(Request.QueryString["testRunID"]);
                    bool created = SeleniumTestBase.AddSingleTestResultScreenshotIfNeeded(testCaseQueryStringValue, projectQueryStringValue, testRunIDQueryStringValue, imageURL, description);

                    if (!created)
                    {
                        lblStatus.Text = "This is a duplicate of an existing screenshot URL on this test result";
                        return;
                    }

                    Response.Redirect("~/FunctionalTesting/AddEditResults.aspx?project=" + projectQueryStringValue + "&testCase=" + testCaseQueryStringValue + "&testRunId=" + testRunIDQueryStringValue);
                }
                else
                {
                    bool created = CTMethods.AddSingleTestCaseScreenshotIfNeeded(testCaseQueryStringValue, projectQueryStringValue, imageURL, description);

                    if (!created)
                    {
                        lblStatus.Text = "This is a duplicate of an existing screenshot URL on this test case";
                        return;
                    }

                    Response.Redirect("~/FunctionalTesting/TestDetails.aspx?project=" + projectQueryStringValue + "&testCase=" + testCaseQueryStringValue);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Unexpected error: " + ex.Message;
                return;
            }
        }

        protected void btnUploadScreenshot_Click(object sender, EventArgs e)
        {

            string TestCaseScreenshotPath = ConfigurationManager.AppSettings["TestCaseScreenshotPath"];
            string TestResultScreenshotPath = ConfigurationManager.AppSettings["TestResultScreenshotPath"];
            
            if (imgUploadScreenshot.HasFile)
            {
                string typeQueryStringValue = Request.QueryString["type"];

                string rawURL;
                if (typeQueryStringValue == "TestResult")
                {
                    //rawURL = TestResultScreenshotPath + imgUploadScreenshot.FileName.ToString();
                    rawURL = "~/Admin/UploadedTestResultScreenshots/" + imgUploadScreenshot.FileName.ToString();
                }
                else
                {
                    //rawURL = TestCaseScreenshotPath + imgUploadScreenshot.FileName.ToString();
                    rawURL = "~/Admin/UploadedTestCaseScreenshots/" + imgUploadScreenshot.FileName.ToString();
                }

                string uploadedFileLocalPath = Server.MapPath(rawURL);
                string uploadedFilePath = ResolveClientUrl(rawURL);

                imgUploadScreenshot.SaveAs(uploadedFileLocalPath);

                AddScreenshot(uploadedFilePath, txtUploadImageDescription.Text);
            }
            else
            {
                lblStatus.Text = "Please select a file first";
                return;
            }
        }
    }
}