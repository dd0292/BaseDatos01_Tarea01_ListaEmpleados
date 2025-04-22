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
        [DisplayName("ID")]
        public int id { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Salario")]
        public decimal Salario { get; set; }

        //[Key]
        //[DisplayName("Id")]
        //public decimal Id { get; set; }

        //[Required]
        //[DisplayName("IdPuesto")]
        //public decimal IdPuesto { get; set; }

        //[Required]
        //[DisplayName("ValorDocumentoIdentidad")]
        //public decimal ValorDocumentoIdentidad { get; set; }

        //[Required]
        //[DisplayName("Nombre")]
        //public decimal Nombre { get; set; }

        //[Required]
        //[DisplayName("FechaContratacion")]
        //public decimal FechaContratacion { get; set; }

        //[Required]
        //[DisplayName("SaldoVacaciones")]
        //public decimal SaldoVacaciones { get; set; }

        //[Required]
        //[DisplayName("EsActivo")]
        //public decimal EsActivo { get; set; }


    }
}