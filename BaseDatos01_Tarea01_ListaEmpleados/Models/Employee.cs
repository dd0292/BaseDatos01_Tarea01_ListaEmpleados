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
    }
}