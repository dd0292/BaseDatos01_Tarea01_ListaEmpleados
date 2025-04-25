using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
	public class Employee
	{
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [DisplayName("IdPuesto")]
        public int IdPuesto { get; set; }

        [Required]
        [DisplayName("ValorDocumentoIdentidad")]
        public int ValorDocumentoIdentidad { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("FechaContratacion")]
        public string FechaContratacion { get; set; }

        [Required]
        [DisplayName("SaldoVacaciones")]
        public decimal SaldoVacaciones { get; set; }

        [Required]
        [DisplayName("EsActivo")]
        public bool EsActivo { get; set; }

        [Required]
        [DisplayName("NombrePuesto")]
        public string NombrePuesto { get; set; }


    }
}