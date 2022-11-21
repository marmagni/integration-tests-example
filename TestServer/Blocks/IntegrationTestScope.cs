using ApiSample.Persistence.EFCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestServer.Blocks.TestServices;
using Xunit;
using TestHost = Microsoft.AspNetCore.TestHost;

namespace TestServer.Blocks
{
    public class IntegrationTestScope : IAsyncLifetime
    {
        private IServiceScope _scope;
        private static object _locker = new object();
        private readonly TestHost.TestServer _server;
        private readonly IntegrationTestFactory _factory;

        public HttpClient Client { get; }
        public IConfiguration Config { get; }
        public IServiceProvider Services { get; }

        public IntegrationTestScope()
        {
            _factory = new IntegrationTestFactory();

            lock (_locker)
            {                
                _server = _factory.Server;
                _scope = _factory.Server.Services.CreateScope();
            }

            Config = _factory.Configuration;
            Client = _factory.CreateClient();
            Services = _scope.ServiceProvider;
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            _scope?.Dispose();
            _server?.Dispose();
            await _factory.DisposeAsync();
        }

        async Task IAsyncLifetime.InitializeAsync()
        {
            var db = Services.GetRequiredService<SqlSandboxConnection>();

            await db.CreateDatabaseAsync();

            var dbcontext = Services.GetRequiredService<SampleContext>();

            await dbcontext.Database.EnsureCreatedAsync();
        }

        public void InitializeDatabase(Action<SampleContext> initDatabase)
        {
            var db = Services.GetRequiredService<SqlSandboxConnection>();
            var dbcontext = Services.GetRequiredService<SampleContext>();

            foreach (var entry in dbcontext.ChangeTracker.Entries())
            {
                dbcontext.Remove(entry.Entity);
            }

            db.OpenConnection();

            initDatabase?.Invoke(dbcontext);

            dbcontext.SaveChanges();            
        }
    }
}
