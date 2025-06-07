using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class EmployeeEditViewModel
    {
        public EmployeeEditViewModel(Employee employee, SelectList TiposDocumento, SelectList Puestos, SelectList Departamentos) 
        { 
            Id = employee.Id;
            Nombre = employee.Nombre;
            ValorDocumento = employee.ValorDocumento;
            FechaNacimiento = employee.FechaNacimiento;

            EstadoOriginalJson = EstadoOriginalJson = JsonConvert.SerializeObject(employee);

            this.TiposDocumento = TiposDocumento;
            this.Puestos = Puestos;
            this.Departamentos = Departamentos;
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ValorDocumento { get; set; }
        public string FechaNacimiento { get; set; }

        public string EstadoOriginalJson { get; set; }

        public SelectList TiposDocumento { get; set; }
        public SelectList Puestos { get; set; }
        public SelectList Departamentos { get; set; }
    }
}