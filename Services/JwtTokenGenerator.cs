using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NOVENTIQ.Model;
using NOVENTIQ.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NOVENTIQ.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _options;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _options = jwtOptions.Value;
        }
        public string TokenGenerate(ApplicationUser appUser, IEnumerable<string> role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            //encoding the secret from the appsettings
            var key = Encoding.ASCII.GetBytes(_options.Secret);

            var claimsList = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, appUser.Name),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };
            claimsList.AddRange(role.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _options.Audience,
                Issuer = _options.Issuer,
                Subject = new ClaimsIdentity(claimsList),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
