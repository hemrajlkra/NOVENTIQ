using Microsoft.AspNetCore.Mvc;
using NOVENTIQ.Model;
using NOVENTIQ.Model.DTO;

namespace NOVENTIQ.Services.IServices
{
    public interface IUserService
    {
        public IEnumerable<UserDto> Getall();
        Task<string> CreateUser(User user);
        Task<UserDto> GetUser(string email);
        Task<string> UpdateUser(string id, UserUpdate user);
        Task<string> Delete(string id);
    }
}
