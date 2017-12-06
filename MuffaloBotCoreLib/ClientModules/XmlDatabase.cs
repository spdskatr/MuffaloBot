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
using MuffaloBotNetFramework2.DiscordComponent;

namespace MuffaloBotCoreLib.ClientModules
{
    class XmlDatabase : BaseModule
    {
        Regex trimmerRegex;
        List<KeyValuePair<string, XmlDocument>> database;
        DirectoryInfo searchDir;
        public XmlDatabase()
        {
            searchDir = new DirectoryInfo("Defs");
            trimmerRegex = new Regex("Defs[\\\\\\/].+"); // Defs[\/].+
            string[] filenames = Directory.GetFiles("Defs", "*.xml", SearchOption.AllDirectories);
            database = new List<KeyValuePair<string, XmlDocument>>(filenames.Length);
            for (int i = 0; i < filenames.Length; i++)
            {
                XmlDocument document = new XmlDocument();
                document.Load(filenames[i]);
                database.Add(new KeyValuePair<string, XmlDocument>(trimmerRegex.Match(filenames[i]).ToString(), document));
            }
        }
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

        protected override void Setup(DiscordClient client)
        {
            Client = client;
        }
    }
}
