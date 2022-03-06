using Revolt;
using Revolt.Commands;
using Revolt.Commands.Extensions;
using Revolt.Commands.Info;
using Revolt.Commands.Results;

namespace RevoltBotExample;

public static class CommandHandler
{
    public static CommandService Commands { get; private set; }

    private static string _mentionPrefix;

    // store version with space so we don't allocate each time we check
    private static string _mentionPrefixSpace;

    public static async Task InitializeAsync()
    {
        var commands = Commands = new CommandService(new CommandServiceConfig()
        {
            DefaultRunMode = RunMode.Async
        });
        await commands.AddModulesAsync(typeof(Program).Assembly, null);
        commands.CommandExecuted += _commandExecuted;
        _mentionPrefix = $"<@{Program.Client.User._id}>";
        _mentionPrefixSpace = _mentionPrefix + " ";
        Program.Client.MessageReceived += _messageReceived;
    }

    private static async Task _commandExecuted(Optional<CommandInfo> arg1, ICommandContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            if (result is ExecuteResult { Error: CommandError.Exception } executeResult)
            {
                var exception = executeResult.Exception;
                await context.Channel.SendMessageAsync($@"> ## An internal exception occurred
> 
> ```csharp
> ({exception.GetType().FullName}) {exception.Message.Replace("\n", "\n> ")}
> ```");
            }
            else if (result.Error == CommandError.UnmetPrecondition)
            {
                await context.Channel.SendMessageAsync(result.ToString()!,
                    replies: new[] { new MessageReply(context.Message._id) });
            }
        }
    }

    private static bool HasPrefix(this string args, ref int argPos)
        => args.HasStringPrefix("._.", ref argPos) ||
           args.HasStringPrefix(_mentionPrefixSpace, ref argPos) ||
           args.HasStringPrefix(_mentionPrefix, ref argPos);

    private static async Task _messageReceived(Message message)
    {
        var context = new RevoltCommandContext(message);
        int argPos = 0;
        if (
            context.User.Bot != null ||
            context.Message.AuthorId == context.Client.User._id ||
            !message.Content.HasPrefix(ref argPos)
        )
            return;

        await Commands.ExecuteAsync(context, message.Content.Substring(argPos), null, MultiMatchHandling.Best);
    }
}