using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NOVENTIQ.Model.DTO;
using NOVENTIQ.Services.IServices;

namespace NOVENTIQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        protected ResponseDto _responseDto;
        public AuthController(IAuthService authService)
        {
            _auth = authService;
            _responseDto = new ResponseDto();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterationRequestDto registerationRequestDto)
        {
            var errorMsg = await _auth.Register(registerationRequestDto);

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
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _auth.Login(loginRequestDto);
            if(loginResponse.User == null)
            {
                _responseDto.IsSuccess= false;
                _responseDto.Message = "Invalid email or password";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(loginResponse);

        }
        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole(RegisterRoleDto model)
        {
            var assignRoleSuccess = await _auth.AssignRole(model);
            if (!assignRoleSuccess)
            {
                _responseDto.IsSuccess=false;
                _responseDto.Message = "Something went wrong!!";
                return BadRequest(_responseDto);
            }
            else
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = "Role assigned successfully";
                return Ok(_responseDto);
            }
        }
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto refreshToken)
        {
            var rfToken = await _auth.RefreshToken(refreshToken);
            if (rfToken == null)
                return Unauthorized("Invalid or expired refresh token");
            return Ok(rfToken);
        }
    }
}
