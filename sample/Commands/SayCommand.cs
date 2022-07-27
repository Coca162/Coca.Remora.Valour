using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;
using Valour.Api.Items.Messages;

namespace Coca.Remora.Valour.Sample.Commands;

public class SayCommand : CommandGroup
{
    private readonly PlanetMessage _planetMessage;

    public SayCommand(PlanetMessage msg) => _planetMessage = msg;

    [Command("say")]
    public async Task<IResult> Say([Greedy] string say)
    {
        IEnumerable<string> args = say.Split(null);

        if (int.TryParse(args.ElementAt(0), out int times))
            args = args.Skip(1);
        else
            times = 1;

        await Say(times, string.Join(' ', args));
        return Result.FromSuccess();
    }

    public Task Say(int times, string say)
    {
        if (times > 5)
            return _planetMessage.ReplyAsync("Too many times!");

        Task[] tasks = new Task[times];

        for (int i = 0; i < times; i++)
            tasks[i] = _planetMessage.ReplyAsync(say);

        return Task.WhenAll(tasks);
    }
}