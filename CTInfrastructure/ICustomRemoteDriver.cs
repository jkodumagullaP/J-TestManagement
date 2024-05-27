using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CTInfrastructure
{
    interface ICustomRemoteDriver// : IWebDriver, IDisposable, ISearchContext, IJavaScriptExecutor, IFindsById, IFindsByClassName, IFindsByLinkText, IFindsByName, IFindsByTagName, IFindsByXPath, IFindsByPartialLinkText, IFindsByCssSelector, ITakesScreenshot, IHasInputDevices, IHasCapabilities, IAllowsFileDetection
    {
        OpenQA.Selenium.Remote.ICommandExecutor GetCommandExecutor();
        OpenQA.Selenium.Remote.SessionId GetSessionId();
        OpenQA.Selenium.Remote.Response PublicExecute(string driverCommandToExecute, System.Collections.Generic.Dictionary<string, object> parameters);
        String GetBrowserName();
    }
}
