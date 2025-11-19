using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController(IBlogService _blogService, IMapper _mapper) : ControllerBase
    {

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetById(int id) {
            try
            {
                // täällä taas kannattaa palauttaa erillinen dto-objekti, koska mukana on käyttäjätietoja
                var blog = await _blogService.GetById(id);
                return Ok(
                    _mapper.Map<BlogDto>(blog)
                );
            }
            catch (NotFoundException)
            {
                // HUOM! Koska NotFound ottaa parametrikseen object?-tietotyypin, sen voi jättää pois tai luoda oman lennosta
                return NotFound(new
                {
                    Title = "blog not found",
                    Detail = $"blog with the id of {id} not found",
                    
                });
            }
            catch (Exception ex)
            {
                return Problem(title: "error fetching blog", detail: ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogDto>> CreateBlog(CreateBlogReq requestData)
        {
            try
            {
                
                var loggedInUser = HttpContext.Items["loggedInUser"] as AppUser;
                var blog = await _blogService.Create(requestData, loggedInUser!.Id);
                //return Ok(
                  //  _mapper.Map<BlogDto>(blog)
                //);

                // JsonSerializer yrittää populoida automaattiseti relatiivisen datan (AppUser)

                // Blog -> AppUser -> Blog -> AppUser -> Blog -> AppUser ...

                //return Ok(blog);

                // BlogDto -> UserDto -> Loppu

                return Ok(
                    _mapper.Map<BlogDto>(blog)
                );



            } catch(Exception e)
            {
                return Problem(title: "error fetching blogs", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetAll()
        {
            try
            {
                var blogs = await _blogService.GetAll();
                return Ok(
                    _mapper.Map<IEnumerable<BlogDto>>(blogs)
                );
            } catch(Exception e)
            {
                return Problem(title: "error fetching blogs", detail: e.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}