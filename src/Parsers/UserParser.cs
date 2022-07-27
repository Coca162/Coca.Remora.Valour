using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Messages;
using Valour.Api.Items.Users;

namespace Coca.Remora.Valour.Parsers;

public sealed class UserParser : MentionParser<User>
{
    public override char[] MentionTypes { get; } = new char[] { 'm', 'u' };

    public UserParser(PlanetMessage planetMessage) : base(planetMessage) { }

    public override async ValueTask<User?> FindItem(long id)
    {
        User? user = await User.FindAsync(id);
        return user is not null ? user : await (await PlanetMember.FindAsync(id, _planetId)).GetUserAsync();
    }
}