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

        [Key]
        [DisplayName("Id")]
        public decimal Id { get; set; }

        [Required]
        [DisplayName("IdEmpleado ")]
        public decimal IdEmpleado { get; set; }

        [Required]
        [DisplayName("IdTipoMovimiento")]
        public decimal IdTipoMovimiento { get; set; }

        [Required]
        [DisplayName("Fecha")]
        public decimal Fecha { get; set; }

        [Required]
        [DisplayName("Monto")]
        public decimal Monto { get; set; }

        [Required]
        [DisplayName("NuevoSaldo")]
        public decimal NuevoSaldo { get; set; }

        [Required]
        [DisplayName("IdPostByUser")]
        public decimal IdPostByUser { get; set; }

        [Required]
        [DisplayName("PostInIP")]
        public decimal PostInIP { get; set; }

        [Required]
        [DisplayName("PostTime")]
        public decimal PostTime { get; set; }

    }
}