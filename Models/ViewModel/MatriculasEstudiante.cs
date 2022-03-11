using System;
using System.Collections.Generic;

namespace Cursos.Models.ViewModel
{
    public class MatriculasEstudiante
    {
        public int Periodo { get; set; }
        public DateTime Fecha { get; set; }
        public List<CursosMatricula> Cursos {get; set;}
    }

    public class CursosMatricula
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
    }
}