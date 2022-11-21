using Xunit;
using ApiSample.Auth;
using FluentAssertions;
using FluentAssertions.Json;
using System.Net;
using TestServer.Blocks;
using TestServer.Mocks;
using TestServer.Schemas;
using Microsoft.Extensions.DependencyInjection;
using ApiSample.Persistence.Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using ApiSample.Persistence.EFCore;

namespace TestServer
{
    public class ProductV1Tests : IClassFixture<IntegrationTestScope>
    {
        readonly Product _productMock;
        readonly Category _categoryMock;
        readonly IntegrationTestScope _scope;

        public ProductV1Tests(IntegrationTestScope scope)
        {
            _scope = scope;

            _categoryMock = EntityMocks.GetCategory();
            _productMock = EntityMocks.GetProduct(_categoryMock.Id);

            _scope.InitializeDatabase(ctx =>
            {
                ctx.Categories.Add(_categoryMock);
                ctx.Products.Add(_productMock);
            });
        }

        [Fact]
        public async Task GetProducts_Should_Response_Valid_Json_Schema()
        {
            var normalUser = new UserSession { Id = 1 };
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();

            var jwtToken = authSvc.CreateJwtToken(normalUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            // Act
            var sut = await _scope.Client.GetAsync("v1/products", null, headers);

            var jschema = SchemaFile.Read("GetProduct.v1");
            sut.Should().BeSuccessful().And.BeValidSchema(jschema);
        }

        [Fact]
        public async Task GetProducts_Should_Response_Product_When_Valid_Parameters()
        {
            var normalUser = new UserSession { Id = 1 };
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();

            var jwtToken = authSvc.CreateJwtToken(normalUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            var query = new Dictionary<string, string?>
            {
                ["name"] = _productMock.Name.Substring(0, 5),
                ["categories"] = _categoryMock.Id.ToString()
            };

            // Act
            var sut = await _scope.Client.GetAsync("v1/products", query, headers);

            if (sut.IsSuccessStatusCode)
            {
                var content = await sut.Content.ReadFromJsonAsync<JToken>();

                content.Should().HaveCount(1);
            }            

            sut.Should().BeSuccessful();
        }


        [Fact]
        public async Task CreateProduct_Should_Data_Be_On_Database()
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
                price = 4040.15,
                available = true,
                name = "UNIQUE_NAME",
                categoryId = _categoryMock.Id
            };

            // Act
            var sut = await _scope.Client.PostAsync("v1/products", body, headers);

            if (sut.IsSuccessStatusCode)
            {
                var product = dbContext.Products.FirstOrDefault(x => x.Name == "UNIQUE_NAME");

                product.Should().NotBeNull();
            }

            sut.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateProduct_Should_Product_Price_Be_Same_On_Get_Product()
        {
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();
            var admUser = new UserSession { Id = 1, Profiles = new() { "Administrator" } };

            var jwtToken = authSvc.CreateJwtToken(admUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            var body = new
            {
                price = 5698.78m,
                available = true,
                name = "UNIQUE_NAME",                
                categoryId = _categoryMock.Id
            };

            var query = new Dictionary<string, string?>
            {
                ["name"] = body.name
            };

            // Act
            var sutpost = await _scope.Client.PostAsync("v1/products", body, headers);

            var sutget = await _scope.Client.GetAsync("v1/products", query, headers);

            if (sutpost.IsSuccessStatusCode && sutget.IsSuccessStatusCode)
            {
                var getRes = await sutget.Content.ReadFromJsonAsync<JToken>();

                var price = getRes?.SelectToken("[0].price")?.Value<decimal>();

                Assert.Equal(body.price, price);
            }
            
            sutpost.Should().HaveStatusCode(HttpStatusCode.OK);
            sutget.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateProduct_Should_Response_Success_Code_When_Valid_Input()
        {
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();
            var admUser = new UserSession { Id = 1, Profiles = new() { "Administrator" } };

            var jwtToken = authSvc.CreateJwtToken(admUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            var body = new
            {
                price = 4040.15,
                available = true,
                categoryId = _categoryMock.Id,
                name = "Adega Electrolux Acs34",
                description = "Adega em acabamento de aço inox"
            };

            // Act
            var sut = await _scope.Client.PostAsync("v1/products", body, headers);

            sut.Should().HaveStatusCode(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateProduct_Should_Response_Forbidden_Code_When_Invalid_Profile()
        {
            var normalUser = new UserSession { Id = 1 };
            var authSvc = _scope.Services.GetRequiredService<IAuthService>();

            var jwtToken = authSvc.CreateJwtToken(normalUser);

            var headers = new Dictionary<string, string>
            {
                ["Authorization"] = $"Bearer {jwtToken}"
            };

            // Act
            var sut = await _scope.Client.PostAsync("v1/products", new { }, headers);

            sut.Should().HaveStatusCode(HttpStatusCode.Forbidden);
        }
    }
}