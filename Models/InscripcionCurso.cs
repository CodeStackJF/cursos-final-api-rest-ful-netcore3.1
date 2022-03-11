using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cursos.Models
{
    public partial class InscripcionCurso
    {
        [Key]
        public int IdEstudiante { get; set; }
        [Key]
        public int IdPeriodo { get; set; }
        [Key]
        public int IdCurso { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? Fecha { get; set; }

        
        [ForeignKey("IdCurso")]
        public virtual Curso Curso { get; set; }

        [ForeignKey("IdPeriodo")]
        public virtual Periodo Periodo {get; set;}

        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante {get; set;}
    }
}
