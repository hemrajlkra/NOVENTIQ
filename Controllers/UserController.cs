using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NOVENTIQ.Model;
using NOVENTIQ.Model.DTO;
using NOVENTIQ.Services.IServices;

namespace NOVENTIQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        protected ResponseDto _responseDto;
        public UserController(UserManager<ApplicationUser> userManager, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _responseDto = new ResponseDto();
        }

        [Authorize(Roles ="ADMIN")]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var users =  _userService.Getall();
            return Ok(users);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(User user)
        {
            var errorMsg = await _userService.CreateUser(user);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = errorMsg;
                return BadRequest(errorMsg);
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Successfully Created";
                return Ok(_responseDto);
            }
            
        }
        [Authorize(Roles ="EMPLOYEE,ADMIN")]
        [HttpGet("getuser")]
        public async Task<IActionResult> GetUser(string email)
        {
            var response = await _userService.GetUser(email);
            if(response == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "User doesn't exist";
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Successfully Retrieved";
                _responseDto.Result = response;
                return Ok(_responseDto);
            }

        }
        [Authorize(Roles ="ADMIN")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(string id, UserUpdate user)
        {
            var result = await _userService.UpdateUser(id, user);
            if (!string.IsNullOrEmpty(result))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = result;
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Updated successfully";
                return Ok(_responseDto);
            }

        }
        [Authorize(Roles ="ADMIN")]
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.Delete(id);
            if (!string.IsNullOrEmpty(result))
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = result;
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Deleted successfully";
                return Ok(_responseDto);
            }

        }
    }
}
