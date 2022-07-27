using Valour.Api.Client;
using Valour.Shared;

namespace Coca.Remora.Valour.Services;

public class ValourTokenLoginFactory : IValourTokenFactory
{
    private readonly string _email;
    private readonly string _password;

    public ValourTokenLoginFactory(string email, string password) => (_email, _password) = (email, password);

    public async Task<TaskResult<string>> GetToken() => await ValourClient.GetToken(_email, _password);
}

public class DirectValourTokenFactory : IValourTokenFactory
{
    private readonly string _token;

    public DirectValourTokenFactory(string token) => _token = token;

    public Task<TaskResult<string>> GetToken() => Task.FromResult(new TaskResult<string>(true, "Success", _token));
}

public class FuncValourTokenFactory : IValourTokenFactory
{
    private readonly Func<IServiceProvider, string> _factory;

    private readonly IServiceProvider _serviceProvider;

    public FuncValourTokenFactory(IServiceProvider provider, Func<IServiceProvider, string> factory) 
        => (_serviceProvider, _factory) = (provider, factory);

    public Task<TaskResult<string>> GetToken() => Task.FromResult(new TaskResult<string>(true, "Success", _factory(_serviceProvider)));
}

public interface IValourTokenFactory
{
    Task<TaskResult<string>> GetToken();
}
