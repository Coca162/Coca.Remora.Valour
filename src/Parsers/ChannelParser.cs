using Remora.Commands.Parsers;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Planets.Channels;

namespace Coca.Remora.Valour.Parsers;

public class ChannelParser : MentionParser<PlanetChatChannel>
{
    public override char[] MentionTypes { get; } = new char[] { 'c' };

    public ChannelParser(PlanetMessage planetMessage) : base(planetMessage) { }

    public override ValueTask<PlanetChatChannel?> FindItem(long id)
        => PlanetChatChannel.FindAsync(id, _planetId);
}