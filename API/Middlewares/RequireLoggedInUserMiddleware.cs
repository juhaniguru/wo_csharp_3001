
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Middlewares
{
    public class RequireLoggedInUserMiddleware(IUserService _userService) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            if(endpoint == null)
            {
                await next(context);
                return;
            }

            var authAttribute = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
            if(authAttribute == null)
            {
                await next(context);
                return;
            }
            
            var claims = context.User.Claims;
            var idStr = claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            if(idStr == null)
            {
                await next(context);
                return;
            }

            var success = int.TryParse(idStr.Value, out int id);
            if(!success)
            {
                await next(context);
                return;
            }

            var loggedInUser = await _userService.GetAccont(id);
            context.Items["loggedInUser"] = loggedInUser;
            await next(context);


        }
    }
}