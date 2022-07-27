using Remora.Commands.Extensions;
using Coca.Remora.Valour.Extensions;
using Coca.Remora.Valour.Sample.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Coca.Remora.Valour.Sample;

internal class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args).UseConsoleLifetime().ConfigureServices(SetUpServices).Build().RunAsync();
    }

    static void SetUpServices(IServiceCollection serviceProvider)
    {
        //In a actual project you would have these be kept seperately from your source code
        serviceProvider.AddValourService("email", "password");
        //Alternate way to authenticate is directly with the token or through a function
        //serviceProvider.AddValourService("token");
        //serviceProvider.AddValourService(serviceProvider => service.Provider.GetRequiredService<GetThingForToken>());

        //These are the defaults which you do not need to put in if you wish to use them
        serviceProvider.AddCommandHandlerOptions(x =>
        {
            x.Prefixes = new string[] { "/" };
            x.AllowBots = false;
            x.AllowSelf = false;
        });

        serviceProvider
            .AddCommandTree()
            .WithCommandGroup<DevCommands>()
            .WithCommandGroup<PingCommand>()
            .WithCommandGroup<SayCommand>()
            .Finish();
    }
}