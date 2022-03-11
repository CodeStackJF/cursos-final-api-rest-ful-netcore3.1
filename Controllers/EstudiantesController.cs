using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cursos.Models;
using Cursos.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Cursos.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;

namespace Cursos.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EstudiantesController:ControllerBase
    {
        private readonly CursosCTX ctx;

        public EstudiantesController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        [HttpGet]
        public async Task<IEnumerable<Estudiante>> Get()
        {
            return await ctx.Estudiante.ToListAsync();
        }

        [HttpGet("{id}", Name="GetEstudiante")]
        public async Task<IActionResult> Get(int id, string codigo)
        {
            var estudiante = await ctx.Estudiante.FindAsync(id);
            if(estudiante == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(estudiante);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Estudiante Estudiante)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            else
            {
                /*if(await ctx.Estudiante.Where(x=>x.Codigo == Estudiante.Codigo).AnyAsync())
                {
                    return BadRequest(ErrorHelper.Response(400, $"El código {Estudiante.Codigo} ya existe."));
                }*/

                Estudiante.IdEstudiante = 0;
                ctx.Estudiante.Add(Estudiante);
                await ctx.SaveChangesAsync();
                Estudiante.Codigo = "EST" + Estudiante.IdEstudiante.ToString().PadLeft(7, '0');
                await ctx.SaveChangesAsync();

                return CreatedAtRoute("GetEstudiante", new { id = Estudiante.IdEstudiante, Codigo = Estudiante.Codigo }, Estudiante);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Estudiante Estudiante)
        {
            if(Estudiante.IdEstudiante == 0)
            {
                Estudiante.IdEstudiante = id;
            }

            if(Estudiante.IdEstudiante != id)
            {
                return BadRequest(ErrorHelper.Response(400, "Petición no válida."));
            }
            
            if(!await ctx.Estudiante.Where(x=>x.IdEstudiante == id).AsNoTracking().AnyAsync())
            {
                return NotFound();
            }

            /*if(await ctx.Estudiante.Where(x=>x.Codigo == Estudiante.Codigo && x.IdEstudiante != Estudiante.IdEstudiante).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El código {Estudiante.Codigo} ya existe."));
            }*/

            ctx.Entry(Estudiante).State = EntityState.Modified;
            if(!TryValidateModel(Estudiante, nameof(Estudiante)))
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            
            await ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/CambiarCodigo")]
        public async Task<IActionResult> CambiarCodigo(int id, [FromQuery] string codigo)
        {

            if(string.IsNullOrWhiteSpace(codigo))
            {
                return BadRequest(ErrorHelper.Response(400, "El código está vacío."));
            }

            var Estudiante = await ctx.Estudiante.FindAsync(id);
            if(Estudiante == null)
            {
                return NotFound();
            }

            if(await ctx.Estudiante.Where(x=>x.Codigo == codigo && x.IdEstudiante != id).AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El código {codigo} ya existe."));
            }

            Estudiante.Codigo = codigo;

            if(!TryValidateModel(Estudiante, nameof(Estudiante)))
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }

            await ctx.SaveChangesAsync();
            return StatusCode(200, Estudiante);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Estudiante = await ctx.Estudiante.FindAsync(id);
            if(Estudiante == null)
            {
                return NotFound();
            }

            ctx.Estudiante.Remove(Estudiante);
            await ctx.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, JsonPatchDocument<Estudiante> _Estudiante)
        {
            var Estudiante = await ctx.Estudiante.FindAsync(id);
            if(Estudiante == null)
            {
                return NotFound();
            }

            _Estudiante.ApplyTo(Estudiante, ModelState);
            if(!TryValidateModel(Estudiante, "Estudiante"))
            {
                return BadRequest(ErrorHelper.GetModelStateErrors(ModelState));
            }
            await ctx.SaveChangesAsync();
            return Ok(Estudiante);
        }
    }
}