﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CTWebsite.ConfigurationManagement.Profiles
{

    public class ProfileConfigurationMapping
    {
        public ProfileConfiguration profile { get; set; }
        public Activation activation { get; set; }
        public Capabilities capabilities { get; set; }

        public ProfileConfigurationMapping()
        {
            profile = new ProfileConfiguration();
            activation = new Activation();
            capabilities = new Capabilities();
        }
    }

    public class ProfileConfiguration
    {
        public string profileId { get; set; }
        public string displayName { get; set; }
        public int displayPriority { get; set; }
        public bool canUseWithLocalWebdriver { get; set; }
        public bool canUseWithIWebDriver { get; set; }
    }

    public class Activation
    {
        public string browserName { get; set; }
    }



    public class Capabilities
    {
        public string[][] IWebDriver { get; set; }

        public Capabilities()
        {
            IWebDriver = new string[][] { };
        }
    }

}
