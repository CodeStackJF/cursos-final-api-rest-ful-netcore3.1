using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursos.Helper;
using Cursos.Models;
using Cursos.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuariosController:Controller
    {
        CursosCTX ctx; 
        
        public UsuariosController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        public async Task<IActionResult> Get()
        {
            List<UsuarioVM> Usuarios = await ctx.Usuarios.Select(x=>new UsuarioVM(){
                IdUsuario = x.IdUsuario,
                Usuario = x.Usuario
            }).ToListAsync();
            return Ok(Usuarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            UsuarioVM Usuarios = await ctx.Usuarios.Where(x=>x.IdUsuario == id).Select(x=>new UsuarioVM(){
                IdUsuario = x.IdUsuario,
                Usuario = x.Usuario
            }).SingleOrDefaultAsync();
            return Ok(Usuarios);
        }

        [HttpPost]

        public async Task<IActionResult> Post(Usuarios Usuario)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }

            if(await ctx.Usuarios.Where(x=>x.Usuario == Usuario.Usuario).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El usuario {Usuario.Usuario} ya existe."));
            }

            HashedPassword Password = HashHelper.Hash(Usuario.Clave);
            Usuario.Clave = Password.Password;
            Usuario.Sal = Password.Salt;
            Usuario.Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            ctx.Usuarios.Add(Usuario);
            await ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new {id=Usuario.IdUsuario}, new UsuarioVM(){
                IdUsuario = Usuario.IdUsuario,
                Usuario = Usuario.Usuario,
                Token = Usuario.Token
            });
        }
    }
}