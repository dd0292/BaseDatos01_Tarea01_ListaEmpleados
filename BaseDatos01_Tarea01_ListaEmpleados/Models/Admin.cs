using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
	public class Admin
	{
        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Id Puesto")]
        public int IdPuesto { get; set; }

        [Required]
        [DisplayName("Documento Identidad")]
        public int ValorDocumentoIdentidad { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Fecha Contratacion")]
        public string FechaContratacion { get; set; }

        [Required]
        [DisplayName("Saldo Vacaciones")]
        public decimal SaldoVacaciones { get; set; }

        [Required]
        [DisplayName("Es Activo")]
        public bool EsActivo { get; set; }

        [Required]
        [DisplayName("Nombre Puesto")]
        public string NombrePuesto { get; set; }


    }
}