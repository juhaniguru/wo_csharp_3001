using System.IdentityModel.Tokens.Jwt;
using API.CustomClaims;
using API.Interfaces;
using API.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace API.Policies.Handlers
{
    public class XpAuthorizationHandler(ILogger<XpAuthorizationHandler> _logger, IUserService _userService) : AuthorizationHandler<XpRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, XpRequirement requirement)
        {
            
            var loggedInUser = context.User;
           
            var idClaim = loggedInUser.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);
            // tokenissa ei ole Xp-avainta
            if(idClaim == null)
            {

                return;
            }
            
            var idStr = idClaim.Value;
            

            var success = int.TryParse(idStr, out int id);
            
            if(!success)
            {
                return;    
            }

            var account = await _userService.GetAccont(id);
            if(account == null)
            {
                return;
            }

            
            if(account.Xp > requirement.Xp)
            {
                context.Succeed(requirement);
            }

            return;
        }
    }
}