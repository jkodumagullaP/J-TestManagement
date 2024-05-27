using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;

namespace CTWebsite
{
    public class WrappedElement
    {
        public string Tag;
        public string Id;
        public string Name;
        public string ClassName;
        public string Text;
        public string Type;
        public Dictionary<string, object> Attributes;
        public IWebElement Parent;
        public IWebElement GrandParent;
        public IWebElement FollowingSibling;
        public IWebElement PrecedingSibling;
        public ReadOnlyCollection<IWebElement> Children;

        public WrappedElement(IWebDriver driver, IWebElement elm)
        {
            Attributes = new Dictionary<string, object>();
            try
            {
                Tag = elm.TagName.ToLower();
            }
            catch { }
            try
            {
                Id = elm.GetAttribute("id");
            }
            catch { }
            try
            {
                Name = elm.GetAttribute("name");
            }
            catch { }
            try
            {
                ClassName = elm.GetAttribute("class");
            }
            catch { }
            try
            {
                Type = elm.GetAttribute("type");
            }
            catch { }
            try
            {
                Text = elm.Text;
            }
            catch { }
            try
            {
                IJavaScriptExecutor je = (IJavaScriptExecutor)driver;

                Dictionary<string, object> attrs = je.ExecuteScript("var items = {}; for (index = 0; index < arguments[0].attributes.length; ++index) { items[arguments[0].attributes[index].name] = arguments[0].attributes[index].value }; return items;", elm) as Dictionary<string, object>;

                foreach (var attr in attrs)
                {
                    string[] excludes = { "width", "height", "autocapitalize", "autocomplete", "autocorrect", "spellcheck" };
                    if (!excludes.ToArray<string>().Contains(attr.Key))
                        Attributes.Add(attr.Key, attr.Value);
                }

            }

            catch { }

            try
            {
                Parent = elm.FindElement(By.XPath("./.."));

                if (Parent.TagName == "body")
                    Parent = null;
                if (Parent != null)
                {
                    GrandParent = Parent.FindElement(By.XPath("./.."));
                    if (GrandParent.TagName == "body")
                        GrandParent = null;
                }
                By ByFS = By.XPath("following-sibling::*");
                if (driver.FindElements(ByFS).Count > 0)
                    FollowingSibling = elm.FindElement(ByFS);
                By ByPS = By.XPath("preceding-sibling::*");
                if (driver.FindElements(ByPS).Count > 0)
                    PrecedingSibling = elm.FindElement(ByPS);
                Children = elm.FindElements(By.XPath(".//*"));
            }
            catch { }
        }
    }
}