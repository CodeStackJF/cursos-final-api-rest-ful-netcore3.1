using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cursos.Models
{
    public partial class Periodo
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int IdPeriodo { get; set; }

        [Required(ErrorMessage = "El año es obligatorio.")]
        [Range(2000, 2999, ErrorMessage = "El año debe estar comprendido entre el 2,000 y el 2,999")]
        public int Anio { get; set; }
        
        public bool? Estado { get; set; }
    }
}