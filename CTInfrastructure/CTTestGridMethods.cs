/*!
 * Crystal Test CTTestGridMethods.cs
 * 
 *     https://crystaltest.codeplex.com
 *
 * Distributed in whole under the terms of the Apache 2.0 License
 *
 *     Copyright 2014, Pixeltrix
 *
 * Licensed under the GNU General Public License version 2 (GPLv2) (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     https://crystaltest.codeplex.com/license
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 *     Date: Fri Mar 28 2014 09:33:33 -0500
 *     
 * Includes Selenium
 * http://docs.seleniumhq.org/
 * Released under the Apache 2.0 License.
 */

using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Configuration;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Web.Script.Serialization;
using Utilities;

namespace CTInfrastructure
{
    public static class CTTestGridMethods
    {
        public static int implicitWaitTime = 30;

        public static string GetBaseUrlForEnvironment(string environment, string projectAbbreviation)
        {
            return DatabaseUtilities.GetTextFromQuery("select [baseURL] from [ProjectEnvironmentInfo] where [environment] = '" + environment + "' and [projectAbbreviation] = '" + projectAbbreviation + "'");
        }

        public static string GetBaseAdminUrlForEnvironment(string environment, string projectAbbreviation)
        {
            return DatabaseUtilities.GetTextFromQuery("select [baseAdminURL] from [ProjectEnvironmentInfo] where [environment] = '" + environment + "' and [projectAbbreviation] = '" + projectAbbreviation + "'");
        }


        public static void SetUpSeleniumDriver(IWebDriver driver, string initialURL)
        {
            driver.Manage().Timeouts().ImplicitlyWait(new TimeSpan(0, 0, implicitWaitTime));
            driver.Navigate().GoToUrl(initialURL);

            Thread.Sleep(2000);

            //Open site in full screen to avoid elements off screen errors.
            //SeleniumSharedMethods.MaximizeSimple(driver);
        }

        public static string GetEngineStatus(string hostName)
        {
            try
            {
                ManagementClass services = GetServices(hostName);

                foreach (ManagementObject service in services.GetInstances())
                {
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    string serviceName = service["Name"].ToString().ToLower().Trim();
                    string serviceState = service["State"].ToString().Trim();

                    if (TestRunEnvironment == "PROD")
                    {
                        if (serviceName.Contains("automationtestengineprod")) //  
                        {
                            return serviceState;
                        }
                    }
                    else if (TestRunEnvironment == "STAGE")
                    {
                        if (serviceName.Contains("automationtestenginestage")) //  
                        {
                            return serviceState;
                        }
                    }
                    else if (TestRunEnvironment == "LOCAL")
                    {
                        if (serviceName.Contains("automationtestenginelocal")) //  
                        {
                            return serviceState;
                        }
                    }
                }

                return "UNKNOWN";
            }
            catch (Exception)
            {
                return "OFFLINE";
            }
        }

        public static string GetHubStatus(string hostName)
        {
            try
            {
                ManagementClass services = GetServices(hostName);

                foreach (ManagementObject service in services.GetInstances())
                {
                    string serviceName = service["Name"].ToString().ToLower().Trim();
                    string serviceState = service["State"].ToString().Trim();

                    if (serviceName.Contains("seleniumhub")) //  
                    {
                        return serviceState;
                    }
                }

                return "UNKNOWN";
            }
            catch (Exception)
            {
                return "OFFLINE";
            }
        }

        public static string GetNodeStatus(string hostName)
        {
            try
            {
                ManagementClass services = GetServices(hostName);

                foreach (ManagementObject service in services.GetInstances())
                {
                    string serviceName = service["Name"].ToString().ToLower().Trim();
                    string serviceState = service["State"].ToString().Trim();

                    if (serviceName.Contains("seleniumnode")) //  
                    {
                        return serviceState;
                    }
                }

                return "UNKNOWN";
            }
            catch (Exception)
            {
                return "OFFLINE";
            }
        }

        public static ManagementClass GetServices(string hostName)
        {
            try
            {
                ConnectionOptions op = new ConnectionOptions();

                string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                if (String.Compare(hostName, "localhost", true) != 0)
                {
                    op.Username = "@'" + Constants.QAANetworkServiceUserName.ToString() + "'";
                    op.Password = "@'" + Constants.QAANetworkServiceUserPassword.ToString() + "'";
                }

                //ManagementScope scope = new ManagementScope(@"\\Servername.Domain\root\cimv2", op);
                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2", op);
                scope.Connect();

                ManagementPath serviceManagementPath = new ManagementPath("Win32_Service");
                return new ManagementClass(scope, serviceManagementPath, null);
            }
            catch (Exception)
            {
                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2");
                scope.Connect();

                ManagementPath serviceManagementPath = new ManagementPath("Win32_Service");
                return new ManagementClass(scope, serviceManagementPath, null);
            }
        }

        public static ManagementClass GetProcesses(string hostName)
        {
            try
            {
                ConnectionOptions op = new ConnectionOptions();

                if (String.Compare(hostName, "localhost", true) != 0)
                {
                    op.Username = "@'" + Constants.QAANetworkServiceUserName.ToString() + "'";
                    op.Password = "@'" + Constants.QAANetworkServiceUserPassword.ToString() + "'";
                }

                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2", op);
                scope.Connect();

                ManagementPath processManagementPath = new ManagementPath("Win32_Process");
                return new ManagementClass(scope, processManagementPath, null);
            }
            catch (Exception)
            {
                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2");
                scope.Connect();

                ManagementPath processManagementPath = new ManagementPath("Win32_Process");
                return new ManagementClass(scope, processManagementPath, null);
            }
        }

        public static void RebootComputer(string hostName)
        {
            try
            {
                ConnectionOptions op = new ConnectionOptions();

                if (String.Compare(hostName, "localhost", true) != 0)
                {
                    op.Username = "@'" + Constants.QAANetworkServiceUserName.ToString() + "'";
                    op.Password = "@'" + Constants.QAANetworkServiceUserPassword.ToString() + "'";
                }

                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2", op);
                scope.Connect();

                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_OperatingSystem");
                ManagementClass processClass = new ManagementClass(scope, managementPath, objectGetOptions);

                // http://msdn.microsoft.com/en-us/library/windows/desktop/aa394058(v=vs.85).aspx
                int REBOOT = 2;
                int FORCEACTION = 4;

                ManagementBaseObject inParams = processClass.GetMethodParameters("Win32Shutdown");
                inParams["Flags"] = REBOOT + FORCEACTION;

                ManagementBaseObject outParams;

                // http://bytes.com/topic/c-sharp/answers/813855-why-does-wmi-invoke-not-work
                foreach (ManagementObject OSInstance in processClass.GetInstances())
                {
                    outParams = OSInstance.InvokeMethod("Win32Shutdown", inParams, null);
                }
            }
            catch (Exception)
            {
                ManagementScope scope = new ManagementScope(@"\\" + hostName + @"\root\cimv2");
                scope.Connect();

                ObjectGetOptions objectGetOptions = new ObjectGetOptions();
                ManagementPath managementPath = new ManagementPath("Win32_OperatingSystem");
                ManagementClass processClass = new ManagementClass(scope, managementPath, objectGetOptions);

                int REBOOT = 2;
                int FORCEACTION = 4;

                ManagementBaseObject inParams = processClass.GetMethodParameters("Win32Shutdown");
                inParams["Flags"] = REBOOT + FORCEACTION;

                ManagementBaseObject outParams;

                foreach (ManagementObject OSInstance in processClass.GetInstances())
                {
                    outParams = OSInstance.InvokeMethod("Win32Shutdown", inParams, null);
                }

            }
        }
    
        public static void GetNodeHostNameAndPort(String hubHostName, int hubPort, string sessionID, out string hostName, out int port)
        {
            string sessionURL = "http://" + hubHostName + ":" + hubPort + "/grid/api/testsession?session=" + sessionID;

            string response = NetworkingUtilities.HttpRequestAndResponse(sessionURL);

            var serializer = new JavaScriptSerializer();
            serializer.RegisterConverters(new[] { new DynamicJsonConverter() });

            dynamic dynamicObject = serializer.Deserialize(response, typeof(object));

            string hostNameAndPortString = dynamicObject.proxyId.ToString();

            hostNameAndPortString = hostNameAndPortString.ToUpper().Replace("HTTP://", "").Replace("HTTPS://", "".TrimEnd(new[] {'/'}));

            string[] hostNameAndPort = hostNameAndPortString.Split(new[] { ':' });

            hostName = hostNameAndPort[0];
            port = Convert.ToInt32(hostNameAndPort[1]);
        }

        public static void StartHUB(string hostName)
        {
            ManagementClass services = GetServices(hostName);

            // Start hub, if it exists
            foreach (ManagementObject service in services.GetInstances())
            {
                string serviceName = service["Name"].ToString().ToLower().Trim();
                string serviceCaption = service["Caption"].ToString().ToLower().Trim();
                string serviceState = service["State"].ToString().ToLower().Trim();

                if (serviceName.Contains("seleniumhub")) //  
                {
                    if (!(serviceState.Equals("running")))
                    {
                        // It's not running, so we can start it
                        service.InvokeMethod("StartService", null, null);
                    }
                }
            }
        }

        public static void StartNode(string hostName)
        {
            ManagementClass services = GetServices(hostName);

            // Start node, if it exists
            foreach (ManagementObject service in services.GetInstances())
            {
                string serviceName = service["Name"].ToString().ToLower().Trim();
                string serviceCaption = service["Caption"].ToString().ToLower().Trim();
                string serviceState = service["State"].ToString().ToLower().Trim();

                if (serviceName.Contains("seleniumnode")) //  
                {
                    if (!(serviceState.Equals("running")))
                    {
                        // It's not running, so we can start it
                        service.InvokeMethod("StartService", null, null);
                    }
                }
            }
        }

        public static void StartEngine(string hostName)
        {
            if (Constants.QAANetworkDomain.ToString() != "")
            {
                using (new Impersonator(Constants.QAANetworkDomain.ToString(), Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                {
                    string serviceName = null;
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    if (TestRunEnvironment == "PROD")
                    {
                        serviceName = "AutomationTestEngineProd";
                    }
                    else if (TestRunEnvironment == "STAGE")
                    {
                        serviceName = "AutomationTestEngineStage";
                    }
                    else if (TestRunEnvironment == "LOCAL")
                    {
                        serviceName = "AutomationTestEngineLocal";
                    }

                    ServiceController service = new ServiceController(serviceName);

                    // Start the service
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10.0));
                        Thread.Sleep(2000);
                    }
                }
            }
            else 
            {
                using (new Impersonator(Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                {
                    string serviceName = null;
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    if (TestRunEnvironment == "PROD")
                    {
                        serviceName = "AutomationTestEngineProd";
                    }
                    else if (TestRunEnvironment == "STAGE")
                    {
                        serviceName = "AutomationTestEngineStage";
                    }
                    else if (TestRunEnvironment == "LOCAL")
                    {
                        serviceName = "AutomationTestEngineLocal";
                    }

                    ServiceController service = new ServiceController(serviceName);

                    // Start the service
                    if (service.Status == ServiceControllerStatus.Stopped)
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10.0));
                        Thread.Sleep(2000);
                    }
                }
            }
        }
        
        
        public static void StopNode(string hostName)
        {
            ManagementClass services = GetServices(hostName);

            // Stop node, if it exists
            foreach (ManagementObject service in services.GetInstances())
            {
                string serviceName = service["Name"].ToString().ToLower().Trim();
                string serviceCaption = service["Caption"].ToString().ToLower().Trim();
                string serviceState = service["State"].ToString().ToLower().Trim();

                if (serviceName.Contains("seleniumnode")) //  
                {
                    if (!(serviceState.Equals("stopped")))
                    {
                        // It's not stopped, so we can stop it
                        service.InvokeMethod("StopService", null, null);
                    }
                }
            }

            // Kill browsers, if any exist
            // Skip this step on localhost, or else our website would close too
            if (hostName.ToLower() != "localhost")
            {
                TerminateBrowsers(hostName);
            }
        }

        public static void StopHUB(string hostName)
        {
            ManagementClass services = GetServices(hostName);

            // Stop hub, if it exists
            foreach (ManagementObject service in services.GetInstances())
            {
                string serviceName = service["Name"].ToString().ToLower().Trim();
                string serviceCaption = service["Caption"].ToString().ToLower().Trim();
                string serviceState = service["State"].ToString().ToLower().Trim();

                if (serviceName.Contains("seleniumhub"))
                {
                    if (!(serviceState.Equals("stopped")))
                    {
                        // It's not stopped, so we can stop it
                        service.InvokeMethod("StopService", null, null);
                    }
                }
            }
        }

        public static void StopEngine(string hostName)
        {
            if (Constants.QAANetworkDomain.ToString() != "")
            {
                using (new Impersonator(Constants.QAANetworkDomain.ToString(), Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                {
                    string serviceName = null;
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    if (TestRunEnvironment == "PROD")
                    {
                        serviceName = "AutomationTestEngineProd";
                    }
                    else if (TestRunEnvironment == "STAGE")
                    {
                        serviceName = "AutomationTestEngineStage";
                    }
                    else if (TestRunEnvironment == "LOCAL")
                    {
                        serviceName = "AutomationTestEngineLocal";
                    }

                    ServiceController service = new ServiceController(serviceName);

                    //Stop the service
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10.0));
                        Thread.Sleep(2000);
                    }
                }
            }
            else
            {
                using (new Impersonator(Constants.QAANetworkServiceUserName.ToString(), Constants.QAANetworkServiceUserPassword.ToCharArray()))
                {
                    string serviceName = null;
                    string TestRunEnvironment = ConfigurationManager.AppSettings["TestRunEnvironment"];

                    if (TestRunEnvironment == "PROD")
                    {
                        serviceName = "AutomationTestEngineProd";
                    }
                    else if (TestRunEnvironment == "STAGE")
                    {
                        serviceName = "AutomationTestEngineStage";
                    }
                    else if (TestRunEnvironment == "LOCAL")
                    {
                        serviceName = "AutomationTestEngineLocal";
                    }

                    ServiceController service = new ServiceController(serviceName);

                    //Stop the service
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        service.Stop();
                        service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10.0));
                        Thread.Sleep(2000);
                    }
                }
            }
        }

        public static void ResetSpecificBrowserProcessesOnNode(string nodeHost, string browserName)
        {
            string browserProcessName;

            switch (browserName)
            {
                case "internet explorer":
                    browserProcessName = "iexplore";
                    break;
                case "firefox":
                    browserProcessName = "firefox";
                    break;
                case "chrome":
                    browserProcessName = "chrome";
                    break;
                default:
                    throw new Exception("Unrecognized browser name: " + browserName);
            }

            ManagementClass processes = GetProcesses(nodeHost);

            // Kill browsers, if any exist
            foreach (ManagementObject process in processes.GetInstances())
            {
                string serviceName = process["Name"].ToString().ToLower().Trim();

                // Don't kill any "driver" processes
                if (serviceName.StartsWith(browserProcessName)
                    && !serviceName.Contains("driver"))
                {
                    try
                    {
                        process.InvokeMethod("Terminate", null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        
        public static void TerminateBrowsers(string hostName)
        {
            ManagementClass processes = GetProcesses(hostName);

            // Kill browsers, if any exist
            foreach (ManagementObject process in processes.GetInstances())
            {
                string serviceName = process["Name"].ToString().ToLower().Trim();

                if (serviceName.StartsWith("iexplore") || serviceName.StartsWith("firefox") || serviceName.StartsWith("chrome") || serviceName.StartsWith("IEDriverServer")
                     || serviceName.StartsWith("dumprep") || serviceName.StartsWith("dwwin") || serviceName.StartsWith("cmd") || serviceName.StartsWith("selenium"))
                {
                    try
                    {
                        process.InvokeMethod("Terminate", null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        public static void TerminateJava(string hostName)
        {
            ManagementClass processes = GetProcesses(hostName);

            // Kill browsers, if any exist
            foreach (ManagementObject process in processes.GetInstances())
            {
                string serviceName = process["Name"].ToString().ToLower().Trim();

                if (serviceName.StartsWith("java.exe"))
                {
                    try
                    {
                        process.InvokeMethod("Terminate", null);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

    }
}
