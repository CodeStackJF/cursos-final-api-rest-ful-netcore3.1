using System.Threading.Tasks;
using Cursos.Models;
using Cursos.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Cursos.Helper;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Cors;

namespace Cursos.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class LoginController:Controller
    {
        private readonly CursosCTX ctx;
        private readonly IConfiguration config;
        public LoginController(CursosCTX _ctx, IConfiguration _config)
        {
            ctx = _ctx;
            config = _config;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post(LoginVM Login)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }

            Usuarios Usuario = await ctx.Usuarios.Where(x=>x.Usuario == Login.Usuario).FirstOrDefaultAsync();
            if(Usuario == null)
            {
                return NotFound(ErrorHelper.Response(404, "Usuario no encontrado."));
            }

            if(HashHelper.CheckHash(Login.Clave, Usuario.Clave, Usuario.Sal))
            {
                var secretKey = config.GetValue<string>("SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, Login.Usuario));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(tokenDescriptor);
                
                string bearer_token = tokenHandler.WriteToken(createdToken);
                return Ok(bearer_token);
            }
            else
            {
                return Forbid();
            }
        }

        [AllowAnonymous]
        [HttpPost("AuthToken")]
        public async Task<IActionResult> AuthToken([FromHeader(Name = "X-Token")] string Token)
        {
            if(string.IsNullOrWhiteSpace(Token))
            {
                return BadRequest();
            }
            
            Usuarios Usuario = await ctx.Usuarios.Where(x=>x.Token == Token).FirstOrDefaultAsync();
            if(Usuario == null)
            {
                return Forbid();
            }

                var secretKey = config.GetValue<string>("SecretKey");
                var key = Encoding.ASCII.GetBytes(secretKey);

                var claims = new ClaimsIdentity();
                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, Usuario.Usuario));
                claims.AddClaim(new Claim("X-Token", Token));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddHours(4),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var createdToken = tokenHandler.CreateToken(tokenDescriptor);
                
                string bearer_token = tokenHandler.WriteToken(createdToken);
                return Ok(bearer_token);
        }
    }
}