using Remora.Results;
using Valour.Shared.Items.Messages.Mentions;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Messages;
using Valour.Api.Client;
using Microsoft.Extensions.Options;

namespace Coca.Remora.Valour.Commands;

public class DefaultPrefixMatcher : ICommandPrefixMatcher
{
    private readonly IReadOnlyCollection<string> _prefixes;

    private readonly Mention? _mention;

    private readonly long _planetId;

    public DefaultPrefixMatcher(IOptions<CommandHandlerOptions> options, PlanetMessage planetMessage)
    {
        _prefixes = options.Value.Prefixes;
        _mention = planetMessage.Mentions.FirstOrDefault();
        _planetId = planetMessage.PlanetId;
    }

    public ValueTask<Result<(bool Matches, int EndOfPrefix)>> MatchesPrefixAsync(string content)
    {
        if (_prefixes.Count == 0)
            return TryMatchMentionPrefix(content, (true, 0));

        foreach (var prefix in _prefixes)
            if (content.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                return new((true, prefix.Length));

        return TryMatchMentionPrefix(content, (false, -1));
    }

    public async ValueTask<Result<(bool, int)>> TryMatchMentionPrefix(string content, (bool, int) failedState)
    {
        if (_mention is not null && _mention.Position - 3 == 0)
        {
            var self = await PlanetMember.FindAsyncByUser(ValourClient.Self.Id, _planetId);

            if (_mention.TargetId == self.Id)
                return (true, content.IndexOf('»') + 1);
        }

        return failedState;
    }
}