using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnetCoreWithJWTAuth.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnetCoreWithJWTAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {

        private readonly UserManager<IdentityUser> userManager;

        public AccountController(UserManager<IdentityUser> userManager)
        {

            this.userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<IdentityUser> Get()
        {
            return userManager.Users;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserModel newUser)
        {
            var identityUser = new IdentityUser()
            {
                Email = newUser.Email,
                UserName = newUser.UserName,

            };
            var result = await userManager.CreateAsync(identityUser, "HolaDimas@2");
            if (result.Succeeded)
            {

                return Ok(result);
            }
            else
            {
                return new StatusCodeResult(500);
            }
        }


    }
}