using Remora.Results;

namespace Coca.Remora.Valour.Commands;

public interface ICommandPrefixMatcher
{
    ValueTask<Result<(bool Matches, int EndOfPrefix)>> MatchesPrefixAsync(string content);
}