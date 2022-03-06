// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using Revolt;

namespace RevoltBotExample;

public static class Program
{
    public static RevoltClient Client { get; private set; }
    public static Configuration Config { get; private set; }
    public static async Task Main()
    {
        Config = JsonConvert.DeserializeObject<Configuration>(await File.ReadAllTextAsync("./config.json"))!;
        Client = new RevoltClient();
        await Client.LoginAsync(TokenType.Bot, Config.Token);
        Client.OnReady += async () =>
        {
            Console.WriteLine("Ready!");
        };
        await Client.ConnectWebSocketAsync();

        await CommandHandler.InitializeAsync();
        Console.WriteLine("Commands initialized!");

        // prevent the program exiting
        await Task.Delay(-1);
    }
}