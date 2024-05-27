using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CTWebsite.Tests
{
    public class Data
    {
        public static T[] EmptyArray<T>() where T : class
        {
            return new T[] { };
        }
    }
}
