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
    }
}
