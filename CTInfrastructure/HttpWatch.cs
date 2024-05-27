using HttpWatch;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Utilities;

namespace OSTMSInfrastructure
{
    public class HttpWatchClass
    {
        //    public Controller objHTTPWatch;
        //    public Plugin objPlugin;
        //    public Dictionary<string, string> dctReplaceValues = new Dictionary<string,string>();
        //    public Dictionary<string, string> dctReturnValues = new Dictionary<string, string>();
        //    public int testID;
        //    public string environment;
        //    public string project;
        //    public string status;
        //    public string errorString;
        //    public string browser; 
        //    public List<ResultSet> lstResults;
        //    public string returnedURL;

        //    public HttpWatchClass()
        //    {
        //        objHTTPWatch = new Controller();
        //        testID = 0;
        //        status = "";
        //        errorString = "";
        //        browser = "";
        //        returnedURL = "";

        //        //dictionary values
        //        dctReplaceValues.Add(" ", "%20");
        //        dctReplaceValues.Add(":", "%3A");
        //        dctReplaceValues.Add("/", "%2F");
        //        dctReplaceValues.Add("+", "%2B");
        //        dctReplaceValues.Add("|", "%7C");
        //        dctReplaceValues.Add(",", "%2C");
        //        dctReplaceValues.Add("?", "%3F");
        //        dctReplaceValues.Add("=", "%3D");
        //    }

        //    public void scanURL(string URL)
        //    {
        //        if (browser.Contains("IE"))
        //        {
        //            objPlugin = objHTTPWatch.IE.New();
        //        }
        //        else
        //        {
        //            objPlugin = objHTTPWatch.Firefox.New();
        //        }
        //        objPlugin.ClearCache();
        //        inspectURL(URL);
        //        close(URL);
        //    }

        //    public void close(string URL)
        //    {
        //        List<Process> processes = Process.GetProcessesByName("iexplorer.exe").ToList();
        //        foreach (Process p in processes)
        //        {
        //            p.Kill();
        //        }

        //        processes = Process.GetProcessesByName("firefox.exe").ToList();
        //        foreach (Process p in processes)
        //        {
        //            p.Kill();
        //        }            
        //    }

        //    public void inspectURL(string URL)
        //    {          
        //        objPlugin.Log.EnableFilter(false);
        //        objPlugin.Record();
        //        objPlugin.GotoURL(URL);
        //        objHTTPWatch.Wait(objPlugin, 15000);
        //        objPlugin.Stop();

        //        string retValue = getURLContent(URL);
        //        TestURLObjects(URL);

        //        objPlugin.ClearCache();
        //        objPlugin.Clear();            
        //    }

        //    public string getURLContent(string URL)
        //    {
        //        string result;
        //        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(URL);
        //        req.Method = "GET";

        //        HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
        //        StreamReader reader = new StreamReader(resp.GetResponseStream());
        //        result = reader.ReadToEnd();
        //        return result;
        //    }

        //    public virtual void TestURLObjects(string URL)
        //    {
        //        string omnitureString = DatabaseUtilities.GetOmnitureString(testID, environment, project);
        //        lstResults = new List<ResultSet>();

        //        foreach (Entry e in objPlugin.Log.Entries)
        //        {
        //            if (e.URL.Contains(omnitureString))
        //            {
        //                DataSet ds = DatabaseUtilities.GetAnalyticsExpectedResults(testID, environment, project);
        //                returnedURL = e.URL.ToString();

        //                foreach (DataTable dt in ds.Tables)
        //                {
        //                    foreach (DataRow dr in dt.Rows)
        //                    {
        //                        string returnValue = FindArgument(e.URL, dr["searchString"].ToString() + "=");
        //                        lstResults.Add(new ResultSet(dr["searchString"].ToString(), dr["expectedValue"].ToString(), returnValue));
        //                    }
        //                }

        //                if (lstResults.Count > 0)
        //                {
        //                    foreach (ResultSet rs in lstResults)
        //                    {
        //                        if (rs.expectedResult.Contains(",") && !rs.expectedResult.Contains(rs.actualResult))
        //                        {
        //                            errorString += rs.fieldName + " expected one of " + rs.expectedResult + " but was actually " + rs.actualResult + ".\n";
        //                        }
        //                        else if (!rs.expectedResult.Contains(",") && rs.expectedResult != rs.actualResult)
        //                        {
        //                            errorString += rs.fieldName + " expected " + rs.expectedResult + " but was actually " + rs.actualResult + ".\n";
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    errorString = "Cannot find any expected results for " + URL + "!";
        //                }

        //                if (errorString.Length > 0)
        //                {
        //                    status = "Fail";
        //                }
        //                else
        //                {
        //                    status = "Pass";
        //                }

        //                return;
        //            }                
        //        }
        //    }

        //    public string FindArgument(string strObject, string strFind)
        //    {
        //        int intStart;
        //        string rawValue = "";

        //        if (strObject.Contains(strFind))
        //        {
        //            intStart = strObject.IndexOf(strFind);
        //            rawValue = strObject.Substring(intStart + strFind.Length, strObject.Substring(intStart + strFind.Length).IndexOf("&"));
        //        }

        //        foreach (KeyValuePair<string, string> entry in dctReplaceValues)
        //        {
        //            rawValue = rawValue.Replace(entry.Value, entry.Key);
        //        }

        //        return rawValue;
        //    }

        //    public class ResultSet
        //    {
        //        public string fieldName;
        //        public string expectedResult;
        //        public string actualResult;

        //        public ResultSet()
        //        {
        //            fieldName = "";
        //            expectedResult = "";
        //            actualResult = "";
        //        }

        //        public ResultSet(string field, string expected, string actual)
        //        {
        //            fieldName = field;
        //            expectedResult = expected;
        //            actualResult = actual;
        //        }
        //    }       

        //}
    }
}
