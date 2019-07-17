using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CodinovaAssignment.DataTransferObject;
using CodinovaAssignment.Helper;
using CodinovaAssignment.Model;
using CodinovaAssignment.Sevices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CodinovaAssignment.Controllers
{
    public class UserDetailsController : Controller
    {
        [Authorize]
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private IUserService _userService;
            private readonly AppSettings _appSettings;
            public UsersController(IUserService userService,
                IOptions<AppSettings> appSettings
                )
            {
                _userService = userService;
                _appSettings = appSettings.Value;
            }

            [AllowAnonymous]
            [HttpPost]
            [Route("Login")]
            public IActionResult Login([FromBody]UserDto userDto)
            {
                var user = _userService.Authenticate(userDto.Username, userDto.Password);
                if (user == null)
                    return BadRequest(new { message = "Username or password is incorrect" });

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Ok(new
                {
                    Id = user.UserId,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });
            }

            [AllowAnonymous]
            [HttpPost]
            [Route("Register")]
            public IActionResult Register([FromBody]UserDto userDto)
            {
                var user = userDto;
                UserDetails _useInfo = new UserDetails
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    UserName = userDto.Username,
                };
                try
                {
                    _userService.Create(_useInfo, userDto.Password);
                    return Ok("Register Successfully");
                }
                catch (AppException ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }

            [HttpGet]
            public IActionResult GetAll()
            {
                var users = _userService.GetAll();
                return Ok(users);
            }
        }
    }
}