using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Results;
using Valour.Api.Items.Planets.Channels;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Users;

namespace Coca.Remora.Valour.Sample.Commands;

public class DevCommands : CommandGroup
{
    private readonly PlanetMessage _planetMessage;

    public DevCommands(PlanetMessage msg) => _planetMessage = msg;


    [Command("channel")]
    public async Task<IResult> Channel(PlanetChatChannel channel)
    {
        await _planetMessage.ReplyAsync($"{channel.Name}: {channel.Id}");
        return Result.FromSuccess();
    }

    [Command("user")]
    public async Task<IResult> User(User user)
    {
        await _planetMessage.ReplyAsync($"{user.Name}: {user.Id}");
        return Result.FromSuccess();
    }

    [Command("member")]
    public async Task<IResult> Member(PlanetMember member)
    {
        await _planetMessage.ReplyAsync($"{await member.GetNameAsync()}: {member.Id}");
        return Result.FromSuccess();
    }
}