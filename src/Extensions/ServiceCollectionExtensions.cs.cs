using Coca.Remora.Valour.Services;
using Coca.Remora.Valour.Commands;
using Coca.Remora.Valour.Parsers;
using Remora.Extensions.Options.Immutable;
using Remora.Commands.Tokenization;
using Remora.Commands.Extensions;
using Remora.Commands.Trees;
using Remora.Commands.Parsers;
using Valour.Api.Items.Planets.Members;
using Valour.Api.Items.Planets.Channels;
using Valour.Api.Items.Users;
using Valour.Api.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Coca.Remora.Valour.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandHandlerOptions(this IServiceCollection serviceCollection, params string[] prefixes) 
        => serviceCollection.AddCommandHandlerOptions(x => x.Prefixes = prefixes.AsReadOnly());

    public static IServiceCollection AddCommandHandlerOptions(this IServiceCollection serviceCollection, 
        Action<CommandHandlerOptions> options)
    {
        serviceCollection.Configure(options);

        return serviceCollection;
    }

    public static IServiceCollection AddValourService(this IServiceCollection serviceCollection, string token)
    {
        serviceCollection.TryAddSingleton<IValourTokenFactory>(new DirectValourTokenFactory(token));

        return serviceCollection.AddValourService();
    }

    public static IServiceCollection AddValourService(this IServiceCollection serviceCollection, string email, string password)
    {
        serviceCollection.TryAddSingleton<IValourTokenFactory>(new ValourTokenLoginFactory(email, password));

        return serviceCollection.AddValourService(); ;
    }

    public static IServiceCollection AddValourService(this IServiceCollection serviceCollection, Func<IServiceProvider, string> tokenFactory)
    {
        serviceCollection.TryAddSingleton<IValourTokenFactory>(x => new FuncValourTokenFactory(x, tokenFactory));

        return serviceCollection.AddValourService(); ;
    }

    public static IServiceCollection AddValourService(this IServiceCollection serviceCollection, IValourTokenFactory tokenFactory)
    {
        serviceCollection.TryAddSingleton(tokenFactory);

        return serviceCollection.AddValourService(); ;
    }

    private static IServiceCollection AddValourService(this IServiceCollection serviceCollection)
    {
        ValourClient.SetHttpClient(new HttpClient
        {
            BaseAddress = new Uri("https://app.valour.gg/")
        });

        serviceCollection.TryAddScoped<PlanetMessageInjector>();
        serviceCollection.TryAddScoped(x => x.GetRequiredService<PlanetMessageInjector>().PlanetMessage);

        serviceCollection.TryAddSingleton<CommandHandlerOptions>();
        serviceCollection.TryAddScoped<ICommandPrefixMatcher, DefaultPrefixMatcher>();
        serviceCollection.TryAddSingleton<ValourService>();

        serviceCollection
            .AddSingleton<IHostedService, ValourService>(serviceProvider =>
                serviceProvider.GetRequiredService<ValourService>());

        serviceCollection.Configure<TokenizerOptions>(opt => opt);
        serviceCollection.Configure<TreeSearchOptions>
        (
            opt => opt with { KeyComparison = StringComparison.OrdinalIgnoreCase }
        );

        serviceCollection.AddCommands();

        serviceCollection
            .AddTransient<ITypeParser<PlanetMember>, MemberParser>()
            .AddTransient<ITypeParser<User>, UserParser>()
            .AddTransient<ITypeParser<PlanetChatChannel>, ChannelParser>();

        return serviceCollection;
    }
}
