using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestLibrary.Utilities
{
    public class StringUtil
    {
        //Reimplementation of String.Contains to prevent null check. 
        public static bool IsContains(string str1, string str2)
        {
            if (String.IsNullOrEmpty(str1))
                return String.IsNullOrEmpty(str2);
            return str1.Contains(str2);
        }
    }
}