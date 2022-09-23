using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TVShowApplication.Data.Options;
using TVShowApplication.Services.Interfaces;

namespace TVShowApplication.Services.Authentication
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly JwtOptions _options;

        public JwtGenerator(IOptions<JwtOptions> options)
        {
            _options = options.Value;
        }

        public string GenerateToken(IDictionary<string, string> claims)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_options.Secret!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims.Select(x => new Claim(x.Key, x.Value))),
                Expires = DateTime.UtcNow.AddSeconds(_options.ExpirationSeconds!.Value),
                Issuer = _options.Issuer,
                Audience = _options.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha512Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }
    }
}
