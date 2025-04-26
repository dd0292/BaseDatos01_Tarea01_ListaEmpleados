using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class MovimientoViewModel
    {

        [Required(ErrorMessage = "Debe ingresar un monto")]
        [Range(-1000000, 1000000, ErrorMessage = "Monto debe estar entre -1000 y 1000")]
        public decimal Monto { get; set; }

    }

}