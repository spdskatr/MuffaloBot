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
using MuffaloBot.DiscordComponent;
using System.Net.Http;
using System.IO.Compression;

namespace MuffaloBot.ClientModules
{
    class XmlDatabase : BaseModule
    {
        List<KeyValuePair<string, XmlDocument>> database = new List<KeyValuePair<string, XmlDocument>>();
        public XmlDatabase()
        {
            HttpClient client = new HttpClient();
            using (MemoryStream memory = new MemoryStream(client.GetByteArrayAsync("https://github.com/spdskatr/MuffaloBot/raw/master/files/Defs.zip").GetAwaiter().GetResult()))
            using (ZipArchive archive = new ZipArchive(memory))
            {
                database = new List<KeyValuePair<string, XmlDocument>>(archive.Entries.Count);
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".xml"))
                    {
                        XmlDocument document = new XmlDocument();
                        document.Load(entry.Open());
                        database.Add(new KeyValuePair<string, XmlDocument>(entry.FullName, document));
                    }
                }
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
