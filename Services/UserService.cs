using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NOVENTIQ.Data;
using NOVENTIQ.Model;
using NOVENTIQ.Model.DTO;
using NOVENTIQ.Services.IServices;

namespace NOVENTIQ.Services
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
        public async Task<UserDto> GetUser(string email)
        {
            var user =  await _userManager.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user == null)
            {
                return null;
            }
            UserDto dto = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            return dto;
        }
        public async Task<string> UpdateUser(string id, UserUpdate user)
        {
            var appUser = await _userManager.FindByIdAsync(id);
            if (appUser == null)
            {
                return "User not found";
            }
            //if email change check if not already exist
            if(!string.Equals(appUser.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailExist = await _userManager.FindByEmailAsync(user.Email);
                if(emailExist != null && emailExist.Id != id)
                {
                    return "Email already in use";
                }
            }
            //check if userName is unique
            if(!string.Equals(appUser.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var userNameExist = await _userManager.FindByNameAsync(user.UserName);
                if(userNameExist != null && userNameExist.Id != id)
                {
                    return $"{user.UserName} in use";
                }
            }
            appUser.Name = user.Name;
            appUser.Email = user.Email;
            appUser.PhoneNumber = user.PhoneNumber;
            appUser.UserName = user.UserName;
            appUser.NormalizedEmail = user.Email.ToUpperInvariant();
            appUser.NormalizedUserName = user.UserName.ToUpperInvariant();
            try
            {
                var result = await _userManager.UpdateAsync(appUser);
                if(result.Succeeded)
                {
                    return string.Empty;
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
                return $"Error updating user {ex.Message}";
            }

        }
        public async Task<string> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return "User doesn't exists";
            try
            {
                //delete role First
                var roles = await _userManager.GetRolesAsync(user);
                if(roles != null && roles.Count > 0)
                {
                    var removeUserRole = await _userManager.RemoveFromRolesAsync(user, roles);
                    if(!removeUserRole.Succeeded)
                    {
                        return removeUserRole.Errors.FirstOrDefault()?.Description ?? "Failed to deletee role";
                    }
                }
                // Then delete User
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return string.Empty;
                else
                    return result.Errors.FirstOrDefault()?.Description ?? "Failed to delete User";
                
            }
            catch (Exception ex)
            {
                return $"Error deleting {ex.Message}";
            }
        }

    }
}
