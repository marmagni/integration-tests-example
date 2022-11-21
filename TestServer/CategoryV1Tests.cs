using ApiSample.Auth;
using ApiSample.Persistence.EFCore;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using TestServer.Blocks;
using Xunit;

namespace TestServer
{
    public class CategoryV1Tests : IClassFixture<IntegrationTestScope>
    {
        readonly IntegrationTestScope _scope;

        public CategoryV1Tests(IntegrationTestScope scope)
        {
            _scope = scope;
        }

        [Fact]
        public async Task CreateCategory_Should_Data_Be_On_Database()
        {
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();
            var dbContext = _scope.Services.GetRequiredService<SampleContext>();
            var admUser = new UserSession { Id = 1, Profiles = new() { "Administrator" } };

            var jwtToken = authSvc.CreateJwtToken(admUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            var body = new
            {
                name = "Eletronics",
            };

            // Act
            var sut = await _scope.Client.PostAsync("v1/categories", body, headers);

            if (sut.IsSuccessStatusCode)
            {
                var product = dbContext.Categories.FirstOrDefault(x => x.Name == "Eletronics");

                product.Should().NotBeNull();
            }

            sut.Should().HaveStatusCode(HttpStatusCode.OK);
        }
    }
}