using HttpWatch;
using System.Collections.Generic;
using System.Data;
using Utilities;

namespace OSTMSInfrastructure
{
    public class Omniture : HttpWatchClass
    {
        // These lines are throwing a warning saying the underlines variable hides inherited member
        // This already exists in another class. Please revise.
        //public Dictionary<string, string> dctReplaceValues = new Dictionary<string, string>();
        //public Dictionary<string, string> dctReturnValues = new Dictionary<string, string>();
        //public int testID;
        //public string environment;
        //public string project;
        //public string status;
        //public string errorString;
        //public string browser;
        //public List<ResultSet> lstResults;
        //public string returnedURL;

        //public Omniture()
        //{
        //    testID = 0;
        //    status = "";
        //    errorString = "";
        //    browser = "";
        //    returnedURL = "";

        //    //dictionary values
        //    dctReplaceValues.Add(" ", "%20");
        //    dctReplaceValues.Add(":", "%3A");
        //    dctReplaceValues.Add("/", "%2F");
        //    dctReplaceValues.Add("+", "%2B");
        //    dctReplaceValues.Add("|", "%7C");
        //    dctReplaceValues.Add(",", "%2C");
        //    dctReplaceValues.Add("?", "%3F");
        //    dctReplaceValues.Add("=", "%3D");

        //}

        //public override void TestURLObjects(string URL)
        //{
        //    string omnitureString = DatabaseUtilities.GetOmnitureString(testID, environment, project);
        //    lstResults = new List<ResultSet>();

        //    foreach (Entry e in objPlugin.Log.Entries)
        //    {
        //        if (e.URL.Contains(omnitureString))
        //        {
        //            DataSet ds = DatabaseUtilities.GetAnalyticsExpectedResults(testID, environment, project);
        //            returnedURL = e.URL.ToString();

        //            foreach (DataTable dt in ds.Tables)
        //            {
        //                foreach (DataRow dr in dt.Rows)
        //                {
        //                    string returnValue = FindArgument(e.URL, dr["searchString"].ToString() + "=");
        //                    lstResults.Add(new ResultSet(dr["searchString"].ToString(), dr["expectedValue"].ToString(), returnValue));
        //                }
        //            }

        //            if (lstResults.Count > 0)
        //            {
        //                foreach (ResultSet rs in lstResults)
        //                {
        //                    if (rs.expectedResult.Contains(",") && !rs.expectedResult.Contains(rs.actualResult))
        //                    {
        //                        errorString += rs.fieldName + " expected one of " + rs.expectedResult + " but was actually " + rs.actualResult + ".\n";
        //                    }
        //                    else if (!rs.expectedResult.Contains(",") && rs.expectedResult != rs.actualResult)
        //                    {
        //                        errorString += rs.fieldName + " expected " + rs.expectedResult + " but was actually " + rs.actualResult + ".\n";
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                errorString = "Cannot find any expected results for " + URL + "!";
        //            }

        //            if (errorString.Length > 0)
        //            {
        //                status = "Fail";
        //            }
        //            else
        //            {
        //                status = "Pass";
        //            }

        //            return;
        //        }
        //    }
        //}

        //public string FindArgument(string strObject, string strFind)
        //{
        //    int intStart;
        //    string rawValue = "";

        //    if (strObject.Contains(strFind))
        //    {
        //        intStart = strObject.IndexOf(strFind);
        //        rawValue = strObject.Substring(intStart + strFind.Length, strObject.Substring(intStart + strFind.Length).IndexOf("&"));
        //    }

        //    foreach (KeyValuePair<string, string> entry in dctReplaceValues)
        //    {
        //        rawValue = rawValue.Replace(entry.Value, entry.Key);
        //    }

        //    return rawValue;
        //}

        //public class ResultSet
        //{
        //    public string fieldName;
        //    public string expectedResult;
        //    public string actualResult;

        //    public ResultSet()
        //    {
        //        fieldName = "";
        //        expectedResult = "";
        //        actualResult = "";
        //    }

        //    public ResultSet(string field, string expected, string actual)
        //    {
        //        fieldName = field;
        //        expectedResult = expected;
        //        actualResult = actual;
        //    }
        //}  

    }
}
