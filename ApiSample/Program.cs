using ApiSample.Auth;
using ApiSample.Persistence.Dapper;
using ApiSample.Persistence.Dapper.Blocks;
using ApiSample.Persistence.EFCore;
using ApiSample.Persistence.Queries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.

builder.Services.AddAuthentication(x =>
{
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    var jwtSecretKey = config.GetSection("JwtToken").Value;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(jwtSecretKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddScoped<IDapperQuery, DapperQuery>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddScoped<IProductQueries, ProductQueries>();
builder.Services.AddScoped<IConnectionFactory, SqlConnectionFactory>();

builder.Services.AddDbContext<SampleContext>((provider, options) =>
{
    var connStr = config.GetConnectionString("SqlConn");
    options.UseSqlServer(connStr);
}, ServiceLifetime.Scoped);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
