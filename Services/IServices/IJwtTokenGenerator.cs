using NOVENTIQ.Model;

namespace NOVENTIQ.Services.IServices
{
    public interface IJwtTokenGenerator
    {
        public string TokenGenerate(ApplicationUser appUser, IEnumerable<string> role);
    }
}
