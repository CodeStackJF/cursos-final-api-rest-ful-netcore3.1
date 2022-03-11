using System;
using System.Collections.Generic;

namespace Cursos.Models.ViewModel
{
    public class EstudianteMatriculaInscripcionesVM
    {
        public int IdEstudiante { get; set; }
        public string Codigo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string NombreCompleto { get; set; }

        public List<MatriculasVM> Matriculas {get; set; }
        public List<InscripcionesVM> Inscripciones {get; set; }
    }

     public class MatriculasVM
    {
        public int IdPeriodo { get; set; }
        public DateTime Fecha { get; set; }
        
    }

    public class InscripcionesVM
    {
        public int IdEstudiante { get; set; }
        public int Anio { get; set; }
        public string Codigo { get; set; }
        public string Descripcion { get; set; }
    }

}