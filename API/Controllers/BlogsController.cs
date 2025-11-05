using System.Collections;
using System.IdentityModel.Tokens.Jwt;
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
                var idClaim = User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub);
                var id = idClaim.Value;
                if (id == null)
                {
                    return NotFound();
                }
                var idInt = int.Parse(id);

                var blog = await _blogService.Create(requestData, idInt);
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