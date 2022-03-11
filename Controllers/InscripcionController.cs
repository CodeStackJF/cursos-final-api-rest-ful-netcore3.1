using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cursos.Helper;
using Cursos.Models;
using Cursos.Models.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cursos.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class InscripcionController:Controller
    {
        CursosCTX ctx;

        public InscripcionController(CursosCTX _ctx)
        {
            ctx = _ctx;
        }

        public async Task<List<InscripcionVM>> Get()
        {
            return await ctx.InscripcionCurso
                    .Include(x=>x.Curso).Include(x=>x.Periodo)
                    .Select(x=>new InscripcionVM(){
                        IdEstudiante = x.IdEstudiante,
                        Codigo = x.Estudiante.Codigo,
                        Curso = x.Curso,
                        Periodo = x.Periodo
                    })
                    .ToListAsync();
        }

        [HttpGet("{periodo}/{estudiante}")]
        public async Task<IActionResult> CursosEstudiante(int periodo, int estudiante)
        {
            var Inscripciones = await ctx.InscripcionCurso
            .Where(x=>x.IdPeriodo == periodo && x.IdEstudiante == estudiante)
            .Select(x=>new CursoVM(){                
                Codigo = x.Curso.Codigo,
                Nombre = x.Curso.Descripcion,
                FechaInscripcion = x.Fecha.Value
            }).ToListAsync();
            return Ok(Inscripciones);
        }

        [HttpGet("{periodo}/{estudiante}/{curso}")]
        public async Task<IActionResult> Get(int periodo, int estudiante, string curso)
        {
            var Inscripcion = await ctx.InscripcionCurso
            .Include(x=>x.Curso).Include(x=>x.Periodo)
            .Where(x=>x.IdEstudiante == estudiante && x.Curso.Codigo == curso && x.IdPeriodo == periodo)
            .Select(x=>
            new InscripcionVM(){
                IdEstudiante = x.IdEstudiante,
                Codigo = x.Estudiante.Codigo,
                Curso = x.Curso,
                Periodo = x.Periodo
            })
            .SingleOrDefaultAsync();

            if(Inscripcion == null)
            {
                return NotFound(ErrorHelper.Response(404, $"El curso {curso} no se encuentra inscrito."));
            }

            return Ok(Inscripcion);
        }

        [HttpPost("{periodo}/{estudiante}/{curso}")]
        public async Task<IActionResult> Post(int periodo, int estudiante, string curso)
        {
            if(!await ctx.Periodo.Where(x=>x.IdPeriodo == periodo).AsNoTracking().AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El periodo {periodo} se encuentra cerrado o no existe."));
            }

            var Estudiante = await ctx.Estudiante.Where(x=>x.IdEstudiante == estudiante).AsNoTracking().SingleOrDefaultAsync();
            if(Estudiante == null)
            {
                return NotFound(ErrorHelper.Response(404, $"El estudiante {estudiante} no existe."));
            }

            var Curso = await ctx.Curso.Where(x=>x.Codigo == curso).AsNoTracking().SingleOrDefaultAsync();
            if(Curso == null)
            {
                return NotFound(ErrorHelper.Response(404, $@"El curso {curso} no existe."));
            }
            
            if(await ctx.InscripcionCurso.Where(x=>x.IdEstudiante == estudiante && x.IdCurso == Curso.IdCurso && x.IdPeriodo == periodo).AsNoTracking().AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, $"El curso {curso} ya se encuentra inscrito."));
            }

            ctx.InscripcionCurso.Add(new InscripcionCurso(){
                IdEstudiante = estudiante,
                IdCurso = Curso.IdCurso,
                IdPeriodo = periodo,
                Fecha = DateTime.Now
            });
            await ctx.SaveChangesAsync();

            var Inscripcion = new InscripcionVM(){
                IdEstudiante = estudiante,
                Codigo = Estudiante.Codigo,
                Curso = Curso,
                Periodo = await ctx.Periodo.Where(x=>x.IdPeriodo == periodo).AsNoTracking().SingleOrDefaultAsync()
            };
            return CreatedAtAction(nameof(Get), new {periodo = periodo, curso = curso, estudiante = estudiante}, Inscripcion);
        }

        [HttpPost("{periodo}/{estudiante}")]
        public async Task<IActionResult> InscribirCursos(int periodo, int estudiante, [FromBody] List<int> cursos)
        {
            List<InscripcionCurso> Inscripciones = cursos.Select(IdCurso=> new InscripcionCurso(){
                IdEstudiante = estudiante,
                IdCurso = IdCurso,
                IdPeriodo = periodo,
                Fecha = DateTime.Now
            }).ToList();
            await ctx.InscripcionCurso.AddRangeAsync(Inscripciones);
            await ctx.SaveChangesAsync();
            return Ok(Inscripciones);
        }

        [HttpDelete("{periodo}/{estudiante}/{curso}")]
        public async Task<IActionResult> Delete(int periodo, int estudiante, string curso)
        {
            if(!await ctx.Periodo.Where(x=>x.IdPeriodo == periodo && x.Estado == true).AsNoTracking().AnyAsync())
            {
                return BadRequest(ErrorHelper.Response(400, "El periodo se encuentra cerrado o no existe."));
            }
            
            int IdCurso = await ctx.Curso.Where(x=>x.Codigo == curso).AsNoTracking().Select(x=>x.IdCurso).FirstOrDefaultAsync();

            var Inscripcion = await ctx.InscripcionCurso.FindAsync(estudiante, periodo, IdCurso);
            if(Inscripcion == null)
            {
                return NotFound(ErrorHelper.Response(404, "El curso no se encuentra inscrito."));
            }

            ctx.InscripcionCurso.Remove(Inscripcion);
            await ctx.SaveChangesAsync();
            return NoContent();
        }        

        [HttpGet("{periodo}/{curso}/estudiantes")]
        public async Task<IActionResult> InscripcionesEstudiante(int periodo, string curso)
        {
            List<EstudiantesCursoVM> Estudiantes = await ctx.InscripcionCurso
                                                        .Include(x=>x.Curso)
                                                        .Include(x=>x.Estudiante)
                                                        .Where(x=>x.IdPeriodo == periodo && x.Curso.Codigo == curso)
                                                        .Select(x=>new EstudiantesCursoVM(){
                                                            IdEstudiante = x.IdEstudiante,
                                                            Codigo = x.Estudiante.Codigo,
                                                            Nombres = x.Estudiante.Nombre,
                                                            Apellidos = x.Estudiante.Apellido,
                                                            NombreCompleto = x.Estudiante.NombreApellido
                                                        })
                                                        .ToListAsync();
            return Ok(Estudiantes);
        }
    }
}