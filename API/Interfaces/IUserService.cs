
using API.Models;

namespace API.Interfaces
{
    public interface IUserService
    {
        Task<AppUser> Register(string username, string password);
        Task<AppUser?> GetByUserName(string username);

        Task<string> Login(string username, string password);
        Task<IEnumerable<AppUser>> GetAll();

        Task<AppUser?> GetAccont(int id);
    }
}