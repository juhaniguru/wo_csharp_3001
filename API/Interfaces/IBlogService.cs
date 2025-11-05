
using API.Dtos;
using API.Models;

namespace API.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<Blog>> GetAll();
        // tätä tarvitaan myöhemmin
        Task<Blog> GetById(int id);
        
        Task<Blog> Create(CreateBlogReq requestData, int loggedInUser);

    }
}