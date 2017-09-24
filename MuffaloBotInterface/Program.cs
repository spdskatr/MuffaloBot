namespace MuffaloBotInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                System.Console.WriteLine(MuffaloBotNetFramework.DiscordComponent.DiscordRoot.ProcessString(System.Console.ReadLine(), 0, 0)?.message);
                /* If you want to test reddit components...
                    var match = MuffaloBotNetFramework.RedditComponent.RedditBase.commandRegex.Match(System.Console.ReadLine());
                    System.Console.WriteLine(MuffaloBotNetFramework.RedditComponent.RedditRoot.ProcessCommand(match.Groups[1].Value, match.Groups[2].Value));
                 */
            }
        }
    }
}
