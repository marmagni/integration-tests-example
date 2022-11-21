using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using static Dapper.SqlMapper;
using System.Text;

namespace ApiSample.Auth
{
    public class AuthService : IAuthService
    {
        private readonly string _jwtSecret;
        public AuthService(IConfiguration configuration)
        {
            _jwtSecret = configuration.GetSection("JwtToken").Value;
        }

        public string CreateJwtToken(UserSession userSession)
        {
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userSession.Id.ToString()));
            userSession.Profiles.ForEach(profile => claims.AddClaim(new Claim("roles", profile)));
            
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSecret));
            var credentials  = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                SigningCredentials = credentials,
                Expires = DateTime.UtcNow.AddHours(1),
            };

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.CreateJwtSecurityToken(descriptor);

            return handler.WriteToken(jwt);
        }
    }
}
