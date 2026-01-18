using Microsoft.AspNetCore.Identity;
using NOVENTIQ.Data;
using NOVENTIQ.Model;
using NOVENTIQ.Model.DTO;
using NOVENTIQ.Services.IServices;

namespace NOVENTIQ.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtToken;
        public AuthService(AppDbContext dbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtToken = jwtTokenGenerator;
        }

        public async Task<string> Register(RegisterationRequestDto registerationRequestDto)
        {
            ApplicationUser appUser = new()
            {
                Name = registerationRequestDto.Name,
                Email = registerationRequestDto.Email,
                UserName = registerationRequestDto.Email,
                PhoneNumber = registerationRequestDto.PhoneNumber,
                NormalizedEmail = registerationRequestDto.Email.ToUpper()
            };
            try
            {
                var result = await _userManager.CreateAsync(appUser, registerationRequestDto.Password);
                if (result.Succeeded)
                {
                    var usertoReturn = _db.ApplicationUsers.FirstOrDefault(x => x.Email == registerationRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Id = usertoReturn.Id,
                        Name = usertoReturn.Name,
                        Email = usertoReturn.Email,
                        PhoneNumber = usertoReturn.PhoneNumber
                    };
                    return string.Empty;
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
                
            }
            catch (Exception ex) 
            {

            }
            return "Error Encountered";
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == loginRequestDto.Username);
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if(user == null || !isValid)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }
            var userRoles = await _userManager.GetRolesAsync(user);
            var token = _jwtToken.TokenGenerate(user, userRoles);
            UserDto userDto = new()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name
            };
            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token,
            };
            return loginResponseDto;
        }
        public async Task<bool> AssignRole(RegisterRoleDto registerRole)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email == registerRole.Email);
            bool isValid = await _userManager.CheckPasswordAsync(user, registerRole.Password);
            if (user != null && isValid)
            {
                //Checking if role existss if not then will create that role and assign
                if (!_roleManager.RoleExistsAsync(registerRole.Role).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(registerRole.Role)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, registerRole.Role);
                return true;
            }
            return false;
        }

    }
}
