using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTWebsite.WebDriver.JsCommand
{
    public class AddElement : BrowserCommand
    {
        public string ElementId { get; set; }
        public string ElementCodeName {get; set;}
        public string ElementXPath { get; set; }
        public string ElementCssSelector { get; set; }
    }
}
