using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.CustomExceptions;
using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class UserService(DataContext _repository, ITokenTool _tokenTool) : IUserService
    {
        public async Task<IEnumerable<AppUser>> GetAll()
        {
            var users = await _repository.Users.AsNoTracking().ToListAsync();
            return users;
        }

        public async Task<AppUser?> GetByUserName(string username)
        {                                                                          //'Juhani' => 'juhani'
                                                                                   // resumè => resume 

            // SELECT * FROM Users WHERE LOWER(Users.Username) = LOWER('Juhani') LIMIT 1;
            // SWEDISH_LATIN_CI
            var user = await _repository.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == username.ToLower());
            return user;
        }

        public async Task<string> Login(string username, string password)
        {
            var user = await GetByUserName(username);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            var hmac = new HMACSHA512(user.PasswordSalt);
            var computedPassword = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            bool passwordsMatch = true;

            for (int i = 0; i < computedPassword.Length; i++)
            {
                // jos salasanat eivät täsmää, palautetaan null
                if (computedPassword[i] != user.HashedPassword[i])
                {
                    // ns. early exitin vuoksi hyökkääjä voi käytännössä laskea suoritusajoista,
                    // kuinka monta bytea on mennyt oikein ja arvata oikean salasanan tavu tavulta
                    passwordsMatch = false;
                }
            }

            if (!passwordsMatch)
            {
                throw new NotFoundException("user not found");
            }

            return _tokenTool.CreateToken(user);

            
        }   

        public async Task<AppUser> Register(string username, string password)
        {
            /*
            var existingUser = GetByUserName(username);
            if(existingUser != null)
            {
                throw new UserRegistrationException("username must be unique");
            }
            */

            var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                Role = "user",
                // hmac.Key on randomi salt, jonka loit automaattisesti
                // kun teit hmac-instanssin
                PasswordSalt = hmac.Key,
                // ComputeHash tekee hashin selkokielisestä salasanasta
                HashedPassword = hmac.ComputeHash(
                    // Encoding.UTF8.GetBytes?
                    // req.Password on string-tyyypiä, mutta ComputeHash
                    // haluaa parametring byte[]-arrayna
                    // GetBytes siis palauttaa merkkijonosta byte[] arrayn.

                    Encoding.UTF8.GetBytes(password)
                )

            };

            await _repository.Users.AddAsync(user);
            await _repository.SaveChangesAsync();
            return user;
        }
    }
}