using System;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models.ViewModels
{
    public class SemanaPlanillaResumen
    {
        public int NumeroSemana { get; set; }       // Para mostrar "Semana 1"
        public DateTime FechaInicio { get; set; }   // Formato: dd/MM
        public DateTime FechaFin { get; set; }      // Formato: dd/MM
        public int DiasTrabajados { get; set; }     // Días laborados
        public decimal HorasNormales { get; set; }  // Horas regulares
        public decimal HorasExtras { get; set; }    // Horas extras
        public decimal SalarioBruto { get; set; }   // Bruto semanal
        public decimal SalarioNeto { get; set; }    // Neto semanal
    }
}