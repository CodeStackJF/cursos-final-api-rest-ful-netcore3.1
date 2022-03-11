using Cursos.Helper;
using Cursos.Models;
using Cursos.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Cursos.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CursosController:ControllerBase
    {
        private readonly CursosCTX ctx;

        public CursosController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        public async Task<IEnumerable<Curso>> Get()
        {
            return await ctx.Curso.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var Curso = await ctx.Curso.FindAsync(id);
            if(Curso == null)
            return NotFound(ErrorHelper.Response(404, $"Curso {id} no encontrado."));

            return Ok(Curso);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar([FromQuery] string b, [FromQuery] bool? estado)
        {
            if(!string.IsNullOrWhiteSpace(b))
            {
                return Ok(await ctx.Curso
                                .Where(
                                    x=>
                                    (
                                        x.Descripcion.Contains(b)
                                        || 
                                        x.Codigo.Contains(b)
                                    )
                                    &&
                                    x.Estado == (estado == null ? x.Estado:estado.Value)
                                )
                                .ToListAsync());
            }
            else
            {
                 return Ok(await ctx.Curso
                                .Where(
                                    x=>                                    
                                    x.Estado == (estado == null ? x.Estado:estado.Value)
                                )
                                .ToListAsync());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Curso curso)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }

            if(await ctx.Curso.Where(x=>x.Codigo == curso.Codigo).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El código {curso.Codigo} ya existe."));
            }

            curso.Estado = curso.Estado ?? true;
            ctx.Curso.Add(curso);
            await ctx.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = curso.IdCurso}, curso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Curso Curso)
        {
            if(Curso.IdCurso == 0)
            {
                Curso.IdCurso = id;
            }

            if(Curso.IdCurso != id)
            {
                return BadRequest(ErrorHelper.Response(400, "Petición no válida."));
            }

            if(!await ctx.Curso.Where(x=>x.IdCurso == Curso.IdCurso).AsNoTracking().AnyAsync())
            {
                return NotFound(ErrorHelper.Response(404, $"El curso {Curso.IdCurso} no existe."));
            }

            if(await ctx.Curso.Where(x=>x.Codigo == Curso.Codigo && x.IdCurso != Curso.IdCurso).AsNoTracking().AnyAsync())
            {
                 return BadRequest(ErrorHelper.Response(400, $"El código {Curso.Codigo} ya existe."));
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            Curso.Estado = Curso.Estado ?? true;
            
            ctx.Entry(Curso).State = EntityState.Modified;
            await ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Curso = await ctx.Curso.Include(x=>x.InscripcionCurso).Where(x=>x.IdCurso == id).SingleOrDefaultAsync();
            if(Curso == null)
            {
                return NotFound(ErrorHelper.Response(404, $"Curso {id} no encontrado."));
            }
            
            if(Curso.InscripcionCurso.Count > 0)
            {
                return BadRequest(ErrorHelper.Response(400, "No puede eliminar este curso porque existen registros de inscripción."));
            }
            
            ctx.Curso.Remove(Curso);
            await ctx.SaveChangesAsync();
            return NoContent();
        }
    }
}