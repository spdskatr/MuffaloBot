using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.CommandsUtil
{
    static class StringUtil
    {
        internal static string CapFirst(this string str)
        {
            if (str.Length == 0) return str;
            if (str.Length > 1)
            {
                return str[0].ToString().ToUpper() + str.Substring(1);
            }
            return str.ToUpper();
        }
        static readonly Regex whitespaceRegex = new Regex("^\\s*$");
        internal static bool EmptyOrContainsOnlyWhitespace(this string str)
        {
            return (str.Length > 0) ? whitespaceRegex.Match(str).Success : true;
        }
        internal static string FormatNewLinesForReddit(this string str)
        {
            return str.Replace("\n", "  \n");
        }
        internal static string WithinChars(this string str, int amount)
        {
            if (str.Length <= amount) return str;
            string strBefore = str.Substring(0, amount / 2 - 2);
            string strAfter = str.Substring(str.Length - (int)(Math.Round(amount / 2.0) - 3));
            return string.Concat(strBefore, " ... ", strAfter);
        }
    }
}
