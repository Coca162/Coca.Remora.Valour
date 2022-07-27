using Remora.Commands.Parsers;
using Remora.Results;
using Valour.Api.Items.Messages;

namespace Coca.Remora.Valour.Parsers;

public abstract class MentionParser<T> : AbstractTypeParser<T>
{
    public static readonly int LongestLong = long.MaxValue.ToString().Length;

    public abstract char[] MentionTypes { get; }

    public long _planetId;

    public MentionParser(PlanetMessage planetMessage) => _planetId = planetMessage.PlanetId;

    public override async ValueTask<Result<T>> TryParseAsync(string mentionString, CancellationToken ct)
    {
        long result;
        if (mentionString.Length <= LongestLong)
        {
            if (!long.TryParse(mentionString, out result))
                return new ArgumentInvalidError(nameof(mentionString), $"{nameof(T)} is smaller then a mention and is not a long");
        }
        else
        {
            if (!MentionTypes.Contains(mentionString[2]))
                return new ArgumentInvalidError(nameof(mentionString), $"Mention type {nameof(T)} does not contain '{mentionString[2]}' in 2nd character");

            if (!TryParseMention(mentionString.AsSpan(), out result))
                return new ArgumentInvalidError(nameof(mentionString), "Mention cannot be parsed to a long");
        }

        T? member = await this.FindItem(result);

        return member is not null
            ? Result<T>.FromSuccess(member)
            : new ArgumentInvalidError(nameof(mentionString), $"Cannot get {nameof(T)} from Valour.Api");
    }

    public abstract ValueTask<T?> FindItem(long id);

    public static bool TryParseMention(ReadOnlySpan<char> mentionSpan, out long result)
        => long.TryParse(mentionSpan[4..^1], out result);
}