using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Authorization.Helper
{
    public class Authorization
    {
        
    }

    public class Token : IAuthorizationRequirement
    {
   
    }

    public class UserTokenHandler:AuthorizationHandler<Token>
    {
        private readonly IHttpContextAccessor _contextAccessor = null;

        public UserTokenHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, Token requirement)
        {
            var authorizationFilterContext = context.Resource as AuthorizationFilterContext;

            if(!context.User.HasClaim(x=>x.Type == "X-Token") || !_contextAccessor.HttpContext.Request.Headers.Where(x=>x.Key == "X-Token").Any())
            {          
                context.Fail();
                return Task.CompletedTask;
            }
            else
            {
                string TokenSession = context.User.FindFirst(x=>x.Type == "X-Token").Value;
                string TokenHeader =  _contextAccessor.HttpContext.Request.Headers.Where(x=>x.Key == "X-Token").FirstOrDefault().Value;
                if(TokenSession == TokenHeader)
                {                    
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();              
                }
                return Task.CompletedTask;
            }
        }
    }
}