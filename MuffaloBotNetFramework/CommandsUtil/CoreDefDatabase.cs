using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Text.RegularExpressions;

namespace MuffaloBotNetFramework.CommandsUtil
{
    static class CoreDefDatabase
    {
        static Regex trimmerRegex;
        static List<KeyValuePair<string, XmlDocument>> database;
        static DirectoryInfo searchDir;
        static CoreDefDatabase()
        {
            searchDir = new DirectoryInfo("Defs");
            trimmerRegex = new Regex("Defs[\\\\\\/].+"); // Defs\\\/.+
            FileInfo[] files = searchDir.GetFiles("*.xml", SearchOption.AllDirectories);
            database = new List<KeyValuePair<string, XmlDocument>>(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i].FullName);
                database.Add(new KeyValuePair<string, XmlDocument>(trimmerRegex.Match(files[i].FullName).ToString(), document));
            }
        }
        public static IEnumerable<KeyValuePair<string, XmlNode>> SelectNodesByXpath(string xpath)
        {
            return from KeyValuePair<string,XmlDocument> doc in database
                   from XmlNode node in doc.Value.SelectNodes(xpath)
                   select new KeyValuePair<string, XmlNode>(doc.Key, node);
        }
        public static string GetSummaryForNodeSelection(string xpath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var results = SelectNodesByXpath(xpath);
            int i = 0;
            foreach (var result in results)
            {
                if (i < 5)
                {
                    stringBuilder.AppendLine($"<!-- In {result.Key}: -->");
                    stringBuilder.AppendLine($"{result.Value.OuterXml}\n");
                }
                i++;
            }
            stringBuilder.AppendFormat("<!-- Summary: Found {0} results total (showing first 5) -->", i);
            return string.Concat("```xml\n", stringBuilder.ToString(), "```");
        }
    }
}
