using System;
using System.Collections.Generic;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class PlanillaSem
    {
        // Información básica del empleado
        public int EmpleadoId { get; set; }
        public string NombreCompleto { get; set; }
        public string Puesto { get; set; }
        public decimal SalarioHora { get; set; }

        // Lista de semanas disponibles
        public List<SemanaPlanilla> Semanas { get; set; } = new List<SemanaPlanilla>();

        public class SemanaPlanilla
        {
            public int SemanaId { get; set; }
            public DateTime FechaInicio { get; set; }
            public DateTime FechaFin { get; set; }
            public bool Cerrada { get; set; }

            // Horas por día (Lunes=0, Domingo=6)
            public decimal[] HorasPorDia { get; set; } = new decimal[7];

            // Movimientos por día (para el botón de detalles)
            public List<MovimientoDia>[] MovimientosPorDia { get; set; } = new List<MovimientoDia>[7];

            // Totales de la semana
            public decimal HorasTotales { get; set; }
            public decimal SalarioBruto { get; set; }
            public decimal Descuentos { get; set; }
            public string MotivoDescuento { get; set; }
            public decimal SalarioNeto { get; set; }

            public SemanaPlanilla()
            {
                for (int i = 0; i < 7; i++)
                {
                    MovimientosPorDia[i] = new List<MovimientoDia>();
                }
            }
        }

        public class MovimientoDia
        {
            public DateTime Fecha { get; set; }
            public string TipoMovimiento { get; set; }
            public string Categoria { get; set; }
            public decimal Monto { get; set; }
            public decimal Horas { get; set; }
            public string Descripcion { get; set; }
        }
    }
}