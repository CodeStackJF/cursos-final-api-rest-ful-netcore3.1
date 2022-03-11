using System.Collections.Generic;

namespace Cursos.Models.ViewModel
{
    public class MatriculaEstudiantesVM
    {
        public int IdPeriodo { get; set; }
        public int Periodo { get; set; }
        public List<EstudiantesCursoVM> Estudiantes {get; set;}
    }
}