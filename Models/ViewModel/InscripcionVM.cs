namespace Cursos.Models.ViewModel
{
    public class InscripcionVM
    {
        public int IdEstudiante { get; set; }
        public string Codigo { get; set; }
        public Periodo Periodo { get; set; }
        public Curso Curso { get; set; }
    }
}