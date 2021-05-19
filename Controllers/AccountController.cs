using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using dotnetCoreWithJWTAuth.Configuration;
using dotnetCoreWithJWTAuth.Data;
using dotnetCoreWithJWTAuth.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace dotnetCoreWithJWTAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;

        public AccountController(ApplicationDbContext _context, UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            this._context = _context;
            this._userManager = userManager;
            this._jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpGet]
        public IEnumerable<IdentityUser> Get()
        {
            return _userManager.Users;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto newUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Errors = new List<string>(){
                        "Invalid payload"
                    },
                    Success = false
                });
            }

            var existingUSer = await _userManager.FindByEmailAsync(newUser.Email);
            if (existingUSer != null)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Errors = new List<string>(){
                        "Provided Email is been already used"
                    },
                    Success = false
                });
            }

            var identityUser = new IdentityUser()
            {
                Email = newUser.Email,
                UserName = newUser.UserName,
                

            };
            var result = await _userManager.CreateAsync(identityUser, newUser.Password);
            if (!result.Succeeded)
            {

                return BadRequest(new RegistrationResponse()
                {
                    Errors = result.Errors.Select(err => err.Description).ToList(),
                    Success = false
                });

            }
            else
            {
                return new ObjectResult(GenerateJwtToken(identityUser));
            }
        }

        private dynamic GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);


            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new {ur.UserId,ur.RoleId,r.Name};

            var claims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())

            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role.Name));
            }

            var tokenDescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claims),
                SigningCredentials= new SigningCredentials(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);

            var jwtToken =jwtTokenHandler.WriteToken(token);


            return new {
                UserName=user.UserName,
                Access_Token = jwtToken
            };
        }
    }



}