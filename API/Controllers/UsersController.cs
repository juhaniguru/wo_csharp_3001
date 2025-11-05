using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService _userService, IMapper _mapper) : ControllerBase
    {
        // localhost:portti/api/users/
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(
                    _mapper.Map<IEnumerable<UserDto>>(users)
                );
            } catch(Exception e)
            {
                return Problem(title: "error fetching users", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginRes>> Login(LoginReq requestData)
        {
            try
            {
                var token = await _userService.Login(requestData.UserName, requestData.Password);
                return Ok(new LoginRes
                {
                    Token = token
                });
            } catch(NotFoundException e)
            {
                return Problem(title: "error logging in user", detail: e.Message, statusCode: StatusCodes.Status404NotFound);
            } catch(Exception e)
            {
                return Problem(title: "error logging in user", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("register", Name = "Register user")]
        public async Task<ActionResult<RegisterRes>> Register(RegisterReq requestData)
        {
            try
            {
                var user = await _userService.Register(requestData.UserName, requestData.Password);
                return Ok(new RegisterRes
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Role = user.Role
                });

            }

            catch(UserRegistrationException e)
            {
                Console.WriteLine(e.Message);
                return Problem(title: "Error registering user", detail: "error registering user", statusCode: 400);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Problem(title: "Error registering user", detail: "error registering user", statusCode: 500);
            }
        }
    }
}