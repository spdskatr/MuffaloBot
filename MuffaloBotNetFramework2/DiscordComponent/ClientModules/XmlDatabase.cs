using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace MuffaloBotNetFramework2.DiscordComponent.ClientModules
{
    class XmlDatabase : IClientModule
    {
        Regex trimmerRegex;
        List<KeyValuePair<string, XmlDocument>> database;
        DirectoryInfo searchDir;
        public IEnumerable<KeyValuePair<string, XmlNode>> SelectNodesByXpath(string xpath)
        {
            return from KeyValuePair<string, XmlDocument> doc in database
                   from XmlNode node in doc.Value.SelectNodes(xpath)
                   select new KeyValuePair<string, XmlNode>(doc.Key, node);
        }
        public string GetSummaryForNodeSelection(string xpath)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var results = SelectNodesByXpath(xpath);
            int i = 0;
            foreach (var result in results)
            {
                if (i < 5)
                {
                    stringBuilder.AppendLine($"<!-- In {result.Key}: -->");
                    stringBuilder.AppendLine($"{result.Value.OuterXml.WithinChars(100)}\n");
                }
                i++;
            }
            stringBuilder.AppendFormat("<!-- Summary: Found {0} results total (showing first 5 if applicable) -->", i);
            return string.Concat("```xml\n", stringBuilder.ToString(), "```");
        }

        public void BindToClient(DiscordClient client)
        {
        }

        public void InitializeFronJson(JObject jObject)
        {
            searchDir = new DirectoryInfo("Defs");
            trimmerRegex = new Regex("Defs[\\\\\\/].+"); // Defs[\/].+
            FileInfo[] files = searchDir.GetFiles("*.xml", SearchOption.AllDirectories);
            database = new List<KeyValuePair<string, XmlDocument>>(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(files[i].FullName);
                database.Add(new KeyValuePair<string, XmlDocument>(trimmerRegex.Match(files[i].FullName).ToString(), document));
            }
        }
    }
}
