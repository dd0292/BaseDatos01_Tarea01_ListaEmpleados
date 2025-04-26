using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class Movement
    {

        [Required]
        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [DisplayName("Monto")]
        public decimal Monto { get; set; }

        [Required]
        [DisplayName("NuevoSaldo")]
        public decimal NuevoSaldo { get; set; }

        [Required]
        [DisplayName("IdPostByUser")]
        public string IdPostByUser { get; set; }

        [Required]
        [DisplayName("PostTime")]
        public DateTime PostTime { get; set; }

        [Required]
        [DisplayName("NombreTipo")]
        public string NombreTipo { get; set; }

        [Required]
        [DisplayName("NombreUsuario")]
        public string NombreUsuario { get; set; }

    }
}