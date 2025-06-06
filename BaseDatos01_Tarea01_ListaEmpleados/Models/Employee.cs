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
        public Employee(int Id, string Nombre, int TipoDocumento, 
            string ValorDocumento, string FechaNacimiento, string Departamento, 
            string Puesto, int IdUsuario) {
            this.Id = Id;
            this.Nombre = Nombre;
            this.TipoDocumento = TipoDocumento;
            this.ValorDocumento = ValorDocumento;
            this.Puesto = Puesto;
            this.IdUsuario = IdUsuario;
            this.Departamento = Departamento;
            this.FechaNacimiento = FechaNacimiento;
            Activo = true;
        }   

        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [Required]
        [DisplayName("Tipo Documento")]
        public int  TipoDocumento { get; set; }

        [Required]
        [DisplayName("Documento")]
        public string ValorDocumento { get; set; }

        [Required]
        [DisplayName("Puesto")]
        public string Puesto { get; set; }

        [Required]
        [DisplayName("Departamento")]
        public string Departamento { get; set; }

        [Required]
        [DisplayName("FechaNacimiento")]
        public string FechaNacimiento { get; set; }

        [Required]
        [DisplayName("Activo")]
        public bool Activo { get; set; }

        [Required]
        [DisplayName("IdUsuario")]
        public int IdUsuario { get; set; }


    }
}