using System.ComponentModel.DataAnnotations;

namespace Cursos.Models.ViewModel
{
    public class LoginVM
    {
        [Required(ErrorMessage="El usuario es obligatorio.")]
        public string Usuario { get; set; }
        [Required(ErrorMessage="La clave es obligatoria.")]
        public string Clave { get; set; }
    }
}