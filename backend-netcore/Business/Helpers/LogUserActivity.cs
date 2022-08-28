using Business.Abstract;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private readonly IUserDal _userDal;

        public LogUserActivity(IUserDal userDal)
        {
            _userDal = userDal;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            
            

            var userName = resultContext.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var user = await _userDal.GetAsnc(x=>x.UserName==userName);

            user.LastActive = DateTime.Now;

            _userDal.Update(user);

            



        }
    }
}
