using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.CommandsUtil
{
    // Ye olde ThingDefDatabase code
    static class ThingDefDatabase
    {
        const string fieldRegex = "@General_Fields[^@]*{0};([^;\\n@<>]*)";
        const string statRegex = "@{0}[^@]*{0};([^;\\n@<>]*)";
        const string statBasesRegex = "@{0}([^@<>]+)";
        static readonly Regex statsDisassemblerRegex = new Regex("[^;\\n@<>]+;([^;\\n@<>]+);([^;\\n@<>]+)");
        internal static string Request(string label)
        {
            try
            {
                return File.ReadAllText($"ThingDefDatabase/{label.ToLower()}.txt");
            }
            catch (IOException)
            {
                return "";
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("[ERR] Other exception when requesting file from ThingDefDatabase: " + e);
                return "";
            }
        }
        internal static string RequestField(string label, string field)
        {
            return Regex.Match(Request(label), string.Format(fieldRegex, field.ToLower())).Groups[1].Value;
        }
        internal static IEnumerable<KeyValuePair<string, string>> RequestStatBases(string label) => ProcessRequest(label, "Stat_Bases");
        internal static IEnumerable<KeyValuePair<string, string>> RequestStatFactors(string label) => ProcessRequest(label, "Stat_Factors");
        internal static IEnumerable<KeyValuePair<string, string>> RequestStatOffsets(string label) => ProcessRequest(label, "Stat_Offsets");
        static IEnumerable<KeyValuePair<string, string>> ProcessRequest(string label, string markerName)
        {
            var results = statsDisassemblerRegex.Matches(Regex.Match(Request(label), string.Format(statBasesRegex, markerName)).Groups[1].Value);
            if (results.Count == 0) return null;
            return EnumerateResultsInternal(results);
        }
        static IEnumerable<KeyValuePair<string, string>> EnumerateResultsInternal(MatchCollection results)
        {
            for (int i = 0; i < results.Count; i++)
            {
                yield return new KeyValuePair<string, string>(results[i].Groups[1].Value, results[i].Groups[2].Value);
            }
        }
    }
}
