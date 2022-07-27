using Coca.Remora.Valour.Commands;
using Remora.Commands.Tokenization;
using Remora.Commands.Services;
using Remora.Commands.Trees;
using Valour.Api.Client;
using Valour.Api.Items.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Coca.Remora.Valour.Services;

public class ValourService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly CommandService _commandService;
    private readonly IValourTokenFactory _tokenFactory;

    private readonly CommandHandlerOptions _commandHandlerOptions;
    private readonly TokenizerOptions _tokenizerOptions;
    private readonly TreeSearchOptions _treeSearchOptions;


    public ValourService
    (
        IServiceProvider serviceProvider,
        CommandService commandService,
        IValourTokenFactory tokenFactory,

        IOptions<CommandHandlerOptions> commandHandlerOptions,
        IOptions<TokenizerOptions> tokenizerOptions,
        IOptions<TreeSearchOptions> treeSearchOptions
    )
    {
        _serviceProvider = serviceProvider;
        _commandService = commandService;
        _tokenFactory = tokenFactory;

        _commandHandlerOptions = commandHandlerOptions.Value;
        _tokenizerOptions = tokenizerOptions.Value;
        _treeSearchOptions = treeSearchOptions.Value;
    }


    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var tokenResult = await _tokenFactory.GetToken();

        var token = tokenResult.Success ? tokenResult.Data : throw new ValourTokenNotFoundException(tokenResult.Message);

        var setUpResult = await ValourClient.InitializeUser(token);

        if (!setUpResult.Success)
            throw new ValourSetUpFailureException(setUpResult.Message);

        await ValourClient.InitializeSignalR("https://app.valour.gg/planethub");
        await ValourClient.JoinAllChannelsAsync();

        ValourClient.OnMessageRecieved += OnMessageRecieved;
    }

    private async Task OnMessageRecieved(PlanetMessage msg)
    {
        if (!_commandHandlerOptions.AllowSelf && msg.AuthorUserId == ValourClient.Self.Id ||
            !_commandHandlerOptions.AllowBots && (await msg.GetAuthorUserAsync()).Bot)
            return;

        using IServiceScope scope = _serviceProvider.CreateScope();
        scope.ServiceProvider.GetRequiredService<PlanetMessageInjector>().PlanetMessage = msg;

        var prefixMatcher = scope.ServiceProvider.GetRequiredService<ICommandPrefixMatcher>();
        var checkPrefix = await prefixMatcher.MatchesPrefixAsync(msg.Content);

        if (!checkPrefix.IsDefined(out var check) || !check.Matches)
            return;
        
        string content = msg.Content.AsSpan()[check.EndOfPrefix..].TrimStart().ToString();

        var executeResult = await _commandService.TryExecuteAsync
        (
            content,
            scope.ServiceProvider,
            _tokenizerOptions,
            _treeSearchOptions
        );
    }
}
