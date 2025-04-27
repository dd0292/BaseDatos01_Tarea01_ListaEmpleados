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
        [DisplayName("Nuevo Saldo")]
        public decimal NuevoSaldo { get; set; }

        [Required]
        [DisplayName("Id Post By User")]
        public string IdPostByUser { get; set; }

        [Required]
        [DisplayName("Post Time")]
        public DateTime PostTime { get; set; }

        [Required]
        [DisplayName("Nombre Tipo")]
        public string NombreTipo { get; set; }

        [Required]
        [DisplayName("Nombre Usuario")]
        public string NombreUsuario { get; set; }

        [Required]
        [DisplayName("IP")]
        public string PostInIP { get; set; }

    }
}