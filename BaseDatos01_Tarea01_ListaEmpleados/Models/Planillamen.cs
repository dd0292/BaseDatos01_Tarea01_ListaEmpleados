using BaseDatos01_Tarea01_ListaEmpleados.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class PlanillaMensualViewModel
    {
        // Información del empleado
        public string NombreCompleto { get; set; }
        public string Puesto { get; set; }
        public decimal SalarioBase { get; set; }

        // Información del mes actual
        public MesPlanilla MesActual { get; set; }

        // Semanas que componen este mes
        public List<SemanaPlanillaResumen> Semanas { get; set; }

        // Deducciones aplicadas
        public List<Deduccion> Deducciones { get; set; }
    }

    public class MesPlanilla
    {
        public string NombreMes { get; set; }
        public int Anio { get; set; }
        public DateTime FechaInicio { get; set; } // Primer jueves del periodo
        public DateTime FechaFin { get; set; }    // Último jueves del periodo
        public bool Cerrado { get; set; }
        public decimal SalarioBruto { get; set; }
        public decimal TotalDeducciones { get; set; }
        public decimal SalarioNeto { get; set; }
        public decimal TotalHorasNormales { get; set; }
        public decimal TotalHorasExtras { get; set; }
    }

    public class Deduccion
    {
        public string Tipo { get; set; }          // Ej: "Seguro Social", "Impuesto Renta"
        public string Descripcion { get; set; }   // Descripción detallada
        public decimal Monto { get; set; }
        public bool Obligatorio { get; set; }     // Si es deducción obligatoria o voluntaria
    }
}