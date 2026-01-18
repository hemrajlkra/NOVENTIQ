using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NOVENTIQ.Data;
using NOVENTIQ.Model;
using NOVENTIQ.Services.IServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NOVENTIQ.Services
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _options;
        private readonly AppDbContext _context;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions, AppDbContext context)
        {
            _options = jwtOptions.Value;
            _context = context;
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
        public async Task<RefreshToken> TokenRefresh(string userId)
        {
            var refreshToken = new RefreshToken()
            {
                UserId = userId,
                Token = GenerateRandomToken(),
                ExpireAt = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
            };
            _context.RefreshTokens.Add(refreshToken);

            await _context.SaveChangesAsync();
            return refreshToken;
        }
        public string GenerateRandomToken()
        {
            var randomBytes = new byte[64];
            using (var range = RandomNumberGenerator.Create())
            {
                range.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<RefreshToken> ValidateRefreshToken(string Token)
        {
            var refreshTokn = await _context.RefreshTokens.FirstOrDefaultAsync(x=>x.Token == Token);
            if (refreshTokn == null || refreshTokn.IsRevoked || refreshTokn.ExpireAt < DateTime.Now)
                return null;
            return refreshTokn;
        }
        public async Task<bool> RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token);

            if (refreshToken == null)
                return false;

            refreshToken.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
