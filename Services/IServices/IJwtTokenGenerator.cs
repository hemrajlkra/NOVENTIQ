using NOVENTIQ.Model;

namespace NOVENTIQ.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        public string TokenGenerate(ApplicationUser appUser, IEnumerable<string> role);
        Task<RefreshToken> TokenRefresh(string userId);
        Task<RefreshToken> ValidateRefreshToken(string Token);
        Task<bool> RevokeRefreshTokenAsync(string token);
    }
}
