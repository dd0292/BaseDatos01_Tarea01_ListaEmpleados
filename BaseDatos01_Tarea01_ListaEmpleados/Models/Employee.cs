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
        public Employee() { }
        public Employee(int Id, string Nombre, 
            int IdTipoDocumento, string TipoDocumento, 
            string ValorDocumento,
            int IdPuesto, string NombrePuesto, decimal SalarioPorHora,
            int IdDepartamento, string Departamento,
            DateTime FechaNacimiento, 
            int IdUsuario, string Username, 
            string Password,
            bool Estado
            ) {
            this.Id = Id;
            this.Nombre = Nombre;
            this.TipoDocumento = new TipoDocumento(Id: IdTipoDocumento, Nombre: TipoDocumento) ;
            this.ValorDocumento = ValorDocumento;
            this.Puesto = new Puesto(Id: IdPuesto , Nombre: NombrePuesto, SalarioXHora: SalarioPorHora);
            this.Departamento = new Departamento(Id: IdDepartamento , Nombre: Departamento);
            this.FechaNacimiento = FechaNacimiento;
            this.Usuario = new Usuario(Id: IdUsuario, Nombre: Username, Contrasena: Password);
            this.Estado = Estado;
        }

        [Key]
        [DisplayName("Id")]
        public int Id { get; set; }

        [Required]
        [DisplayName("Nombre")]
        public string Nombre { get; set; }

        [DisplayName("Tipo Documento")]
        public TipoDocumento TipoDocumento { get; set; }

        [Required]
        [DisplayName("Documento")]
        public string ValorDocumento { get; set; }

        [DisplayName("Departamento")]
        public Departamento Departamento { get; set; }

        [DisplayName("Puesto")]
        public Puesto Puesto { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [DisplayName("Estado")]
        public bool Estado { get; set; }

        [DisplayName("Usuario")]
        public Usuario Usuario { get; set; }

    }
}