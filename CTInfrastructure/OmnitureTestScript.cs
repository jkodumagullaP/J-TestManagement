using System;
using Utilities;

namespace OSTMSInfrastructure
{
    public class AnalyticsOmniture : SeleniumTestBase
    {

        //protected override void TestSpecific_RunSeleniumTest()
        //{
        //    string project = DatabaseUtilities.MakeSQLSafe(projectAbbreviation);
        //    string URL = DatabaseUtilities.GetURL(testCaseId, environment, project);
        //    Omniture watch = new Omniture();

        //    if (URL != "")
        //    {
        //        try
        //        {                    
        //            watch.testID = testCaseId;
        //            watch.project = project;
        //            watch.environment = environment;
        //            watch.browser = browserAbbreviation;
        //            watch.scanURL(URL);
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception("Error Message: ", e);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("No URL to inspect!");
        //    }

        //    if (watch.errorString != "")
        //    {
        //        throw new Exception(watch.errorString);
        //    }
        //}

    }
}