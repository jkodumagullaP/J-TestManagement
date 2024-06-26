﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RazorEngine;
using CTWebsite.UI.CodeGeneration;
using CTWebsite.WebDriver;

namespace CTWebsite.UI
{
    public class TemplateViewModel
    {
        public SwdPageObject PageObject { get; set; }
    }

    public class CSharpPageObjectGenerator
    {
        public string[] Generate(SwdPageObject pageObject, string fullTemplatePath)
        {
            var template = File.ReadAllText(fullTemplatePath);

            object model = new TemplateViewModel() { PageObject = pageObject };

            string result = "not parsed";
            try
            {
                result = Razor.Parse(template, model);
            }
            catch
            {
                throw;
            }
            return Utils.SplitSingleLineToMultyLine(result);
        }
    }
}
