namespace MuffaloBotInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                System.Console.WriteLine(MuffaloBotNetFramework.DiscordComponent.DiscordRoot.ProcessString(System.Console.ReadLine(), 0, 0)?.message);
            }
        }
    }
}
