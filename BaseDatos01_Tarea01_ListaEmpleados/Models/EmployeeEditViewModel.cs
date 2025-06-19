using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class EmployeeEditViewModel
    {
        public EmployeeEditViewModel() { }
        public EmployeeEditViewModel(Employee employee, SelectList TiposDocumento, SelectList Puestos, SelectList Departamentos) 
        { 
            Id = employee.Id;
            Nombre = employee.Nombre;
            ValorDocumento = employee.ValorDocumento;
            FechaNacimiento = employee.FechaNacimiento;

            TiposDocumentoOriginal = employee.TipoDocumento.Nombre;
            PuestosOriginal = employee.Puesto.Nombre;
            DepartamentosOriginal = employee.Departamento.Nombre;  

            EstadoOriginalJson = EstadoOriginalJson = JsonConvert.SerializeObject(employee);

            this.TiposDocumento = TiposDocumento;
            this.Puestos = Puestos;
            this.Departamentos = Departamentos;
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public int IdTipoDocumento { get; set; }

        [Required(ErrorMessage = "El valor del documento es requerido")]
        [StringLength(50, ErrorMessage = "El documento no puede exceder 50 caracteres")]
        public string ValorDocumento { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El puesto es requerido")]
        public int IdPuesto { get; set; }

        [Required(ErrorMessage = "El departamento es requerido")]
        public int IdDepartamento { get; set; }

        public string EstadoOriginalJson { get; set; }

        public string TiposDocumentoOriginal { get; set; }
        public string PuestosOriginal { get; set; }
        public string DepartamentosOriginal { get; set; }

        public SelectList TiposDocumento { get; set; }
        public SelectList Puestos { get; set; }
        public SelectList Departamentos { get; set; }
    }
}