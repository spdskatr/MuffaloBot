using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.DiscordComponent
{
    partial class DiscordRoot
    {
        static Random random = new Random();
        public const string exception = "An exception happened.\n{0}";
        public const string notFound = "Could not find the description for '{0}'.";
        public const string searchQuery = "http://rimworldwiki.com/api.php?action=query&list=search&format=xml&srlimit=30&srprop=&srsearch={0}";
        public const string wikiPageFiller = "http://rimworldwiki.com/wiki/{0}";
        internal static readonly Regex commandBreakdown = new Regex("^!([^\\s]+) (.*)$");
        internal static readonly Regex commandArgsSeparator = new Regex("([^\\s]+) (.+)");
        internal static readonly Regex mediawikiapi = new Regex("<p ns=\"0\" title=\"([^<>\"']+)\"");
        //--------------------------------------------------|     field         =       value  |
        static Regex mediawikiStatDisassembler = new Regex("\\|[\\s]*([^=\\|]+)[\\s]*=[\\s]*([^=\\|\\n]*)[\\s]*\\|", RegexOptions.IgnoreCase | RegexOptions.Singleline);
        const ulong RimWorldServer = 214523379766525963;
        internal const string help_msg = @"```md
# MuffaloBot Commands #
!basestats <item>
> Gets the base stats for an item or building
!mbhelp
> Displays this message
!desc <item>
> Gets the description for the item
!field <field> <item>
> Gets the specified (xml) field for an item. Usage: !field stacklimit steel
!stuffstats <item>
> Returns the stuff stats for an item (i.e. the stat modifiers that items and buildings made out of it will get
!usage <command>
> Returns the usage for a command.
!wiki
> Returns the website for the RimWorld wiki
!wikisearch <search term>
> Returns the first 30 hits off the wiki for the search term.
!wolfy 
> ???
!wshopsearch
> Searches the RimWorld Steam Workshop for a specified name and returns the first 5 results.
<secret command>
> MuffaloBot has got many secrets hidden around...
```";
    }
}
