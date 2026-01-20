using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NOVENTIQ.Data;
using NOVENTIQ.Model;
using NOVENTIQ.Model.DTO;

namespace NOVENTIQ.Services.IServices
{
    public class UserService :IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        public UserService(AppDbContext dbContext, UserManager<ApplicationUser> userManager) 
        {
            _db = dbContext;
            _userManager = userManager;
        }

        public async Task<string> CreateUser(User user)
        {
            ApplicationUser appUser = new()
            {
                Name = user.Name,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                NormalizedEmail = user.Email.ToUpper()
            };
            try
            {
                var result = await _userManager.CreateAsync(appUser, user.Password);
                if (result.Succeeded)
                {
                    var usertoReturn = _db.ApplicationUsers.FirstOrDefault(x => x.Email == user.Email);
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
        public IEnumerable<UserDto> Getall()
        {
            List<UserDto> users = _userManager.Users.Select(x =>
            new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber
            }).ToList();
            return users;
        }

    }
}
