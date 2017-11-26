namespace MuffaloBotNetFramework2.DiscordComponent
{
    interface IClientModule
    {
        void BindToClient(DSharpPlus.DiscordClient client);
        void InitializeFronJson(Newtonsoft.Json.Linq.JObject jObject);
    }
}
