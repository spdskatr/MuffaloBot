using DSharpPlus;
using Newtonsoft.Json.Linq;

namespace MuffaloBotNetFramework2.DiscordComponent
{
    public interface IInternalModule
    {
        void BindToClient(DiscordClient client);
        void InitializeFronJson(JObject jObject);
    }
}
