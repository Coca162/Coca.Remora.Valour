namespace Coca.Remora.Valour.Commands;

public class CommandHandlerOptions
{
    public bool AllowSelf { get; set; } = false;

    public bool AllowBots { get; set; } = false;

    public IReadOnlyCollection<string> Prefixes { get; set; } = new string[] { "/" }.AsReadOnly();
}