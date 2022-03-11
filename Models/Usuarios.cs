using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cursos.Models
{
    public partial class Usuarios
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El usuario no puede estar vacío.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La contraseña no debe estar vacía.")]
        public string Clave { get; set; }

        [Compare("Clave", ErrorMessage = "Las contraseñas no coinciden.")]
        [NotMapped]
        public string ConfirmaClave { get; set; }

        public string Sal { get; set; }
        public string Token { get; set; }
    }
}