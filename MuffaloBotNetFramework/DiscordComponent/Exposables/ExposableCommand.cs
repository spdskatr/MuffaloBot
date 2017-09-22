using MuffaloBotNetFramework.CommandsUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuffaloBotNetFramework.DiscordComponent.Exposables
{
    public class ExposableCommand
    {
        public enum CommandType
        {
            Simple,
            Regex
        }
        public CommandType commandType;
        public Dictionary<string, string> args;
        internal _DiscordMessage ProcessCommand(string input)
        {
            switch (commandType)
            {
                case CommandType.Simple:
                    if (input == args["input"])
                        return args["response"];
                    break;
                case CommandType.Regex:
                    if (Regex.IsMatch(input, args["regex"]))
                        return args["response"];
                    break;
                default:
                    break;
            }
            return null;
        }
    }
}
