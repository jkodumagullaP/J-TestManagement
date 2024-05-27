using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTWebsite
{
    public static class VariableName
    {
        public static string Name(IWebElement elm)
        {
            string prefix = "";
            string data = "";
            if (elm.FindElements(By.XPath(".//*")).Count == 0)
            {
                data = elm.Text;
                if (data.Contains(" "))
                {
                    string[] d = data.Split(' ');
                    data = d[0];
                }
            }
            switch (elm.TagName.ToLower())
            {
                case "a":
                    prefix = "WebLink";
                    break;
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    prefix = "WebElement";
                    break;
                case "span":
                    prefix = "span";
                    break;
                case "table":
                    prefix = "WebTable";
                    break;
                case "input":
                    prefix = Input(elm.GetAttribute("type"));
                    break;
                case "img":
                    prefix = "img";
                    break;
                case "b":
                    prefix = "b";
                    break;
                case "blockquote":
                    prefix = "blockquote";
                    break;
                case "button":
                    prefix = "WebButton";
                    break;
                case "canvas":
                    prefix = "canvas";
                    break;
                case "caption":
                    prefix = "caption";
                    break;
                case "center":
                    prefix = "WebElement";
                    break;
                case "code":
                    prefix = "WebElement";
                    break;
                case "footer":
                    prefix = "WebElement";
                    break;
                case "label":
                    prefix = "WebLabel";
                    break;
                case "nav":
                    prefix = "WebElement";
                    break;
                case "select":
                    prefix = "WebList";
                    break;
                case "strong":
                    prefix = "WebElement";
                    break;
                case "textarea":
                    prefix = "WebEdit";
                    break;
                case "title":
                    prefix = "WebElement";
                    break;
                case "time":
                    prefix = "WebElement";
                    break;
                case "i":
                    prefix = "WebElement";
                    break;

                default:
                    prefix = "WebElement";
                    break;
            }
            return prefix;
        }

        private static string Input(string type)
        {
            string retval = "";
            if (String.IsNullOrEmpty(type))
                return retval;
            switch (type)
            {
                case "button":
                    retval = "WebButton";
                    break;
                case "text":
                    retval = "WebEdit";
                    break;
                case "checkbox":
                    retval = "CheckBox";
                    break;
                case "radio":
                    retval = "Radio";
                    break;
                case "date":
                    retval = "WebEdit";
                    break;
                case "time":
                    retval = "WebEdit";
                    break;
                case "number":
                    retval = "WebEdit";
                    break;
                case "hidden":
                    retval = "hiiden";
                    break;
                case "email":
                    retval = "WebEdit";
                    break;
                case "file":
                    retval = "WebEdit";
                    break;
                case "image":
                    retval = "image";
                    break;
                case "reset":
                    retval = "reset";
                    break;
                case "range":
                    retval = "range";
                    break;
                case "submit":
                    retval = "WebButton";
                    break;
                case "tel":
                    retval = "WebEdit";
                    break;
                case "url":
                    retval = "Url";
                    break;
                case "password":
                    retval = "WebEdit";
                    break;
            }
            return retval;
        }

        public static string Locator(string type)

        {
            string retval = "";
            if (String.IsNullOrEmpty(type))
                return retval;
            switch (type)
            {
                case "id":
                    retval = "Id";
                    break;
                case "xpath":
                    retval = "XPath";
                    break;
                case "class":
                    retval = "ClassName";
                    break;
                case "name":
                    retval = "Name";
                    break;

            }
            return retval;
        }
    }
}