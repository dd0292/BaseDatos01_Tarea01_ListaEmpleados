using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
	public class EmployeeMovementsViewModel
	{
        public Employee Empleado { get; set; }
        public List<Movement> Movimientos { get; set; }
    }
}