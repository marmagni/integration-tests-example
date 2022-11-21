using ApiSample.Persistence.Dapper.Blocks;
using ApiSample.Persistence.EFCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestServer.Blocks.TestServices;

namespace TestServer.Blocks
{
    public class IntegrationTestFactory : WebApplicationFactory<Program>
    {
        public IConfiguration Configuration;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(ConfigureServices);

            builder.ConfigureAppConfiguration((context, conf) =>
            {
                Configuration = conf.Build();
            });
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.RemoveAll<SampleContext>();
            serviceCollection.RemoveAll<DbContextOptions>();
            serviceCollection.RemoveAll<IDistributedCache>();
            serviceCollection.RemoveAll<IConnectionFactory>();

            serviceCollection.AddSingleton<SqlSandboxConnection>();
            serviceCollection.AddScoped<IConnectionFactory, Overrides.SqlConnectionFactory>();

            foreach (var option in serviceCollection.Where(s => s.ServiceType.BaseType == typeof(DbContextOptions)).ToList())
            {
                serviceCollection.Remove(option);
            }

            serviceCollection.AddDbContext<SampleContext>((provider, contextOptions) =>
            {
                var sqlConn = provider.GetRequiredService<SqlSandboxConnection>();
                contextOptions.UseSqlServer(sqlConn.Connection);
            }, ServiceLifetime.Singleton);
        }
    }
}
