using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework2.DiscordComponent
{
    public static class Extensions
    {
        public static string WithinChars(this string str, int amount)
        {
            if (str.Length <= amount) return str;
            string strBefore = str.Substring(0, amount / 2 - 2);
            string strAfter = str.Substring(str.Length - (int)(Math.Round(amount / 2.0) - 3));
            return string.Concat(strBefore, " ... ", strAfter);
        }
        public static string CapitalizeFirst(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }
            if (char.IsUpper(str[0]))
            {
                return str;
            }
            if (str.Length == 1)
            {
                return str.ToUpper();
            }
            return char.ToUpper(str[0]) + str.Substring(1);
        }
        public static string MakeFieldSemiReadable(this string str)
        {
            List<char> result = new List<char>();
            char[] chars = str.ToCharArray();
            result.Add(char.ToUpper(chars[0]));
            for (int i = 1; i < chars.Length; i++)
            {
                if (char.IsUpper(chars[i]))
                {
                    result.Add(' ');
                    result.Add(char.ToLower(chars[i]));
                }
                else
                {
                    result.Add(chars[i]);
                }
            }
            return new string(result.ToArray());
        }
        public static string ToStringSign(this float f)
        {
            if (f > 0) return "+" + f;
            return f.ToString();
        }
    }
}
