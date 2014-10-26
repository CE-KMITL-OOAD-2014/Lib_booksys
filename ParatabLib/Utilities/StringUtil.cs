using System;
using System.Collections.Generic;
using System.Linq;

namespace ParatabLib.Utilities
{
    public class StringUtil
    {
        //Reimplementation of String.Contains to prevent null check.
        public static bool IsContains(string str1,string str2)
        {
            if (String.IsNullOrEmpty(str1))
                return String.IsNullOrEmpty(str2);
            return str1.Contains(str2);
        }

        public static bool IsAsciiCharacter(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!((str[i] >= 48 && str[i] <= 57)||(str[i] == 45 || str[i] == 95)
                    || (str[i] >= 65 && str[i] <= 90) || (str[i] >= 97 && str[i] <= 122)))
                    return false;
            }
            return true;
        }
    }
}