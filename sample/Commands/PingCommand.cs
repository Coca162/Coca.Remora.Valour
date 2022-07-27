using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;
using Valour.Api.Items.Messages;

namespace Coca.Remora.Valour.Sample.Commands;

public class PingCommand : CommandGroup
{
    private readonly PlanetMessage _planetMessage;

    public PingCommand(PlanetMessage msg) => _planetMessage = msg;


    [Command("ping")]
    public async Task<IResult> Ping()
    {
        await _planetMessage.ReplyAsync("Ping!");
        return Result.FromSuccess();
    }
}