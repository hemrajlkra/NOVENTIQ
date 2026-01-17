using NOVENTIQ.Model.DTO;

namespace NOVENTIQ.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegisterationRequestDto registerationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(RegisterRoleDto registerRole);
    }
}
