using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.CustomClaims;
using API.Interfaces;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class SymmetricTokenTool(IConfiguration _config) : ITokenTool
    {
        public string CreateToken(AppUser user)
        {
            var key = _config["TokenKey"];
            if (key == null)
            {
                throw new Exception("token key is required");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

            var claims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(XpClaim.XP, user.Xp.ToString())

            };

            var _token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(_token);
        }
    }
}