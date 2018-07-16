using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MuffaloBot.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace MuffaloBot.Commands
{
    public class XPathCommands
    {
        [Command("xpath"), Description("Searches the RimWorld Xml database for xml nodes that match the xpath. **Examples:**\n`!xpath Defs/ThingDef[defName=\"Steel\"]/description`\n`!xpath Defs/ThingDef[@Name=\"BuildingBase\"]`\n`!xpath //*`")]
        public async Task XPathCommand(CommandContext ctx)
        {
            if (string.IsNullOrWhiteSpace(ctx.RawArgumentString)) return;
            try
            {
                await ctx.RespondAsync(ctx.Client.GetModule<XmlDatabaseModule>().GetSummaryForNodeSelection(ctx.RawArgumentString));
            }
            catch (System.Xml.XPath.XPathException ex)
            {
                await ctx.RespondAsync("Invalid XPath! Error: " + ex.Message);
            }
        }

        [Command("iteminfo"), Description("Displays the stats for an item or building in RimWorld.")]
        public async Task InfoCommand(CommandContext ctx, [RemainingText, Description("The name of the item.")] string itemName)
        {
            if (!new Regex("^[a-zA-Z0-9\\-_ ]*$").IsMatch(itemName))
            {
                await ctx.RespondAsync("Invalid name! Only letters, numbers, spaces, underscores or dashes allowed.");
                return;
            }
            XmlDatabaseModule xmlDatabase = ctx.Client.GetModule<XmlDatabaseModule>();
            IEnumerable<KeyValuePair<string, XmlNode>> results =
                xmlDatabase
                .SelectNodesByXpath($"Defs/ThingDef[translate(defName,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')=\"{itemName.ToLower()}\"]")
                .Concat(xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[translate(label,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')=\"{itemName.ToLower()}\"]"))
                .Concat(xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[contains(label, \"{itemName.ToLower()}\")]"))
                .Distinct();
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithColor(DiscordColor.Azure);
            List<string> didYouMean = new List<string>();
            foreach (KeyValuePair<string, XmlNode> item in results)
            {
                if (string.IsNullOrEmpty(builder.Title))
                {
                    builder.WithTitle($"Info for item \"{item.Value["label"].InnerXml.CapitalizeFirst()}\" (defName: {item.Value["defName"].InnerXml})");
                    builder.WithDescription(item.Value["description"]?.InnerXml ?? "No description.");
                    if (InnerXmlOfPathFromDef(xmlDatabase, item.Value, "category") == "Item")
                    {
                        StringBuilder itemStatsBuilder = new StringBuilder();
                        itemStatsBuilder.AppendLine("Stack limit: " + InnerXmlOfPathFromDef(xmlDatabase, item.Value, "stackLimit", "1"));
                        itemStatsBuilder.AppendLine("Automatically haulable: " + InnerXmlOfPathFromDef(xmlDatabase, item.Value, "alwaysHaulable", "false"));
                        if (item.Value["stuffProps"] != null)
                        {
                            itemStatsBuilder.AppendLine("Small volume: " + InnerXmlOfPathFromDef(xmlDatabase, item.Value, "smallVolume", "false"));
                        }
                        builder.AddField("Item stats", itemStatsBuilder.ToString());
                    }
                    StringBuilder stringBuilder = new StringBuilder();
                    AllStatBasesForThingDef(xmlDatabase, item.Value, stringBuilder, new HashSet<string>());
                    string str = stringBuilder.ToString();
                    if (!string.IsNullOrEmpty(str))
                    {
                        builder.AddField("Base stats", str);
                    }
                    if (item.Value["stuffProps"] != null)
                    {
                        stringBuilder = new StringBuilder();
                        DiscordColor color = builder.Color;
                        AllStuffPropertiesForThingDef(xmlDatabase, item.Value, stringBuilder, new HashSet<string>(), ref color);
                        builder.Color = color;
                        string result = stringBuilder.ToString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            builder.AddField("Stuff properties - General", result);
                        }
                        stringBuilder = new StringBuilder();
                        AllStatFactorsForThingDef(xmlDatabase, item.Value, stringBuilder, new HashSet<string>());
                        result = stringBuilder.ToString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            builder.AddField("Stat modifiers - Factors", result);
                        }
                        stringBuilder = new StringBuilder();
                        AllStatOffsetsForThingDef(xmlDatabase, item.Value, stringBuilder, new HashSet<string>());
                        result = stringBuilder.ToString();
                        if (!string.IsNullOrEmpty(result))
                        {
                            builder.AddField("Stat modifiers - Offsets", result);
                        }
                    }
                }
                else
                {
                    didYouMean.Add($"`{item.Value["defName"].InnerXml}`");
                }
            }
            string didYouMeanStr = string.Join(", ", didYouMean);
            if (!string.IsNullOrEmpty(didYouMeanStr))
            {
                builder.AddField("Did you mean", didYouMeanStr);
            }
            if (string.IsNullOrEmpty(builder.Title))
            {
                await ctx.RespondAsync("No results.");
            }
            else
            {
                await ctx.RespondAsync(embed: builder.Build());
            }
        }

        void AllStuffPropertiesForThingDef(XmlDatabaseModule xmlDatabase, XmlNode node, StringBuilder stringBuilder, HashSet<string> foundProps, ref DiscordColor color)
        {
            XmlNodeList nodeList = node["stuffProps"]?.ChildNodes;
            for (int i = 0; i < (nodeList?.Count ?? 0); i++)
            {
                if (nodeList[i].NodeType == XmlNodeType.Element && !foundProps.Contains(nodeList[i].Name))
                {
                    switch (nodeList[i].Name)
                    {
                        case "color":
                            string str = nodeList[i].InnerXml;
                            str = str.TrimStart(new char[] { '(', 'R', 'G', 'B', 'A' });
                            str = str.TrimEnd(new char[] { ')' });
                            string[] array2 = str.Split(new char[] { ',' });
                            float f1 = float.Parse(array2[0]);
                            float f2 = float.Parse(array2[1]);
                            float f3 = float.Parse(array2[2]);
                            if (f1 > 1f || f2 > 1f || f3 > 1f)
                            {
                                color = new DiscordColor((byte)f1, (byte)f2, (byte)f3);
                            }
                            else
                            {
                                color = new DiscordColor(f1, f2, f3);
                            }
                            stringBuilder.AppendLine($"Color: {nodeList[i].InnerXml}");
                            foundProps.Add(nodeList[i].Name);
                            break;
                        default:
                            if (!nodeList[i].ChildNodes.Cast<XmlNode>().Any(xml => xml.NodeType != XmlNodeType.Text) && !string.IsNullOrEmpty(nodeList[i].InnerXml))
                            {
                                stringBuilder.AppendLine($"{nodeList[i].Name.MakeFieldSemiReadable()}: {nodeList[i].InnerXml}");
                                foundProps.Add(nodeList[i].Name);
                            }
                            break;
                    }
                }
            }
            XmlAttribute parent = node.Attributes["ParentName"];
            if (parent != null)
            {
                XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[@Name=\"{parent.InnerXml}\"]").FirstOrDefault().Value;
                AllStuffPropertiesForThingDef(xmlDatabase, xmlNode, stringBuilder, foundProps, ref color);
            }
        }

        void AllStatBasesForThingDef(XmlDatabaseModule xmlDatabase, XmlNode node, StringBuilder stringBuilder, HashSet<string> foundStats)
        {
            XmlNode statBasesNode = node["statBases"];
            if (statBasesNode != null)
            {
                foreach (XmlNode child in statBasesNode.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        string statDefName = child.Name;
                        if (foundStats.Contains(statDefName))
                        {
                            continue;
                        }
                        foundStats.Add(statDefName);

                        XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/StatDef[defName=\"{statDefName}\"]/label").FirstOrDefault().Value;
                        if (xmlNode != null) statDefName = xmlNode.InnerXml;
                        stringBuilder.AppendLine($"{statDefName.CapitalizeFirst()}: {child.InnerXml}");
                    }
                }
            }
            XmlAttributeCollection attributes = node.Attributes;
            XmlAttribute parentAttr = attributes["ParentName"];
            if (parentAttr != null)
            {
                XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[@Name=\"{parentAttr.InnerXml}\"]").FirstOrDefault().Value;
                AllStatBasesForThingDef(xmlDatabase, xmlNode, stringBuilder, foundStats);
            }
        }
        void AllStatFactorsForThingDef(XmlDatabaseModule xmlDatabase, XmlNode node, StringBuilder stringBuilder, HashSet<string> foundStats)
        {
            if (node["stuffProps"] != null)
            {
                XmlNode statFactors = node["stuffProps"]["statFactors"];
                if (statFactors != null)
                {
                    foreach (XmlNode child in statFactors.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element)
                        {
                            string statDefName = child.Name;
                            if (foundStats.Contains(statDefName))
                            {
                                continue;
                            }
                            foundStats.Add(statDefName);

                            XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/StatDef[defName=\"{statDefName}\"]/label").FirstOrDefault().Value;
                            if (xmlNode != null) statDefName = xmlNode.InnerXml;
                            stringBuilder.AppendLine($"{statDefName.CapitalizeFirst()}: x{child.InnerXml}");
                        }
                    }
                }
            }
            XmlAttributeCollection attributes = node.Attributes;
            XmlAttribute parentAttr = attributes["ParentName"];
            if (parentAttr != null)
            {
                XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[@Name=\"{parentAttr.InnerXml}\"]").FirstOrDefault().Value;
                AllStatFactorsForThingDef(xmlDatabase, xmlNode, stringBuilder, foundStats);
            }
        }
        void AllStatOffsetsForThingDef(XmlDatabaseModule xmlDatabase, XmlNode node, StringBuilder stringBuilder, HashSet<string> foundStats)
        {
            if (node["stuffProps"] != null)
            {
                XmlNode statFactors = node["stuffProps"]["statOffsets"];
                if (statFactors != null)
                {
                    foreach (XmlNode child in statFactors.ChildNodes)
                    {
                        if (child.NodeType == XmlNodeType.Element)
                        {
                            string statDefName = child.Name;
                            if (foundStats.Contains(statDefName))
                            {
                                continue;
                            }
                            foundStats.Add(statDefName);

                            XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/StatDef[defName=\"{statDefName}\"]/label").FirstOrDefault().Value;
                            if (xmlNode != null) statDefName = xmlNode.InnerXml;
                            string val = child.InnerXml;
                            val = float.Parse(val).ToStringSign();
                            stringBuilder.AppendLine($"{statDefName.CapitalizeFirst()}: {val}");
                        }
                    }
                }
            }
            XmlAttributeCollection attributes = node.Attributes;
            XmlAttribute parentAttr = attributes["ParentName"];
            if (parentAttr != null)
            {
                XmlNode xmlNode = xmlDatabase.SelectNodesByXpath($"Defs/ThingDef[@Name=\"{parentAttr.InnerXml}\"]").FirstOrDefault().Value;
                AllStatOffsetsForThingDef(xmlDatabase, xmlNode, stringBuilder, foundStats);
            }
        }
        string InnerXmlOfPathFromDef(XmlDatabaseModule database, XmlNode def, string xpath, string defaultValue = null)
        {
            XmlNode result = null;
            while (result == null)
            {
                XmlNodeList list = def.SelectNodes(xpath);
                if (list.Count > 0)
                {
                    result = list.Item(0);
                }
                else
                {
                    XmlAttributeCollection attributeCollection = def.Attributes;
                    if (attributeCollection["ParentName"] != null)
                    {
                        def = database.SelectNodesByXpath($"Defs/ThingDef[@Name=\"{attributeCollection["ParentName"].InnerXml}\"]").FirstOrDefault().Value;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            return result?.InnerXml ?? defaultValue;
        }
    }
}
