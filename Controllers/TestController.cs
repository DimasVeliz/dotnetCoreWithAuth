using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnetCoreWithJWTAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
       
        private readonly UserManager<IdentityUser> userManager;

        public TestController( UserManager<IdentityUser> userManager)
        {
            
            this.userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<IdentityUser> Get()
        {
           
            return userManager.Users;
        }
        
    }
}