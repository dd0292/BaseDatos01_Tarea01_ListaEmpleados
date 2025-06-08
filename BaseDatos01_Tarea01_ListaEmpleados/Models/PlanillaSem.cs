using System;
using System.Collections.Generic;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class PlanillaEmpleado
    {
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; }
        public string Puesto { get; set; }
        public decimal SalarioHora { get; set; }

        public List<RegistroSemanal> Semanas { get; set; }

        public class RegistroSemanal
        {
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public decimal[] HorasPorDia { get; set; } = new decimal[7];
            public decimal HorasTotales { get; set; }
            public decimal SalarioBruto { get; set; }
            public decimal Descuentos { get; set; }
            public string MotivoDescuento { get; set; }
            public decimal SalarioNeto { get; set; }
        }
    }
}