using DSharpPlus;
using Newtonsoft.Json.Linq;

namespace MuffaloBot.DiscordComponent
{
    public interface IInternalModule
    {
        void BindToClient(DiscordClient client);
        void InitializeFronJson(JObject jObject);
    }
}
