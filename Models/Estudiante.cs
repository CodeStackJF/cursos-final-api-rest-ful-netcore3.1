using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cursos.Models
{
    public partial class Estudiante
    {
        [Key]
        public int IdEstudiante { get; set; }

        /*[StringLength(10)]
        [Required(ErrorMessage = "El código es obligatorio.")]
        [MinLength(10, ErrorMessage = "El código debe ser mínimo de 10 caracteres.")]
        [MaxLength(10, ErrorMessage = "El código debe ser máximo de 10 caracteres.")]*/
        public string Codigo { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(3, ErrorMessage = "El nombre debe ser mínimo de 3 caracteres.")]
        [MaxLength(50, ErrorMessage = "El nombre debe ser máximo de 50 caracteres.")]
        public string Nombre { get; set; }

        [StringLength(50)]
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MinLength(3, ErrorMessage = "El apellido debe ser mínimo de 3 caracteres.")]
        [MaxLength(50, ErrorMessage = "El apellido debe ser máximo de 50 caracteres.")]
        public string Apellido { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public string NombreApellido { get; set; }


        [Column(TypeName = "date")]
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
        [DataType(DataType.Date, ErrorMessage = "La fecha no es válida.")]
        public DateTime? FechaNacimiento { get; set; }
    }
}
