using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Messages;

namespace Coca.Remora.Valour.Parsers;

public class MemberParser : MentionParser<PlanetMember>
{
    public override char[] MentionTypes { get; } = new char[] { 'm', 'u' };

    public MemberParser(PlanetMessage planetMessage) : base(planetMessage) { }

    public override async ValueTask<PlanetMember?> FindItem(long id)
    {
        PlanetMember? member = await PlanetMember.FindAsync(id, _planetId);
        return member is not null ? member : await PlanetMember.FindAsyncByUser(id, _planetId);
    }
}