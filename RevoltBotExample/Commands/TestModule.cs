using Revolt.Commands;
using Revolt.Commands.Attributes;

namespace RevoltBotExample.Commands;

public class TestModule : ModuleBase
{
    [Command("test")]
    public async Task Test()
        => await Context.Channel.SendMessageAsync("Hello, wonderful world of Revolt!");
}