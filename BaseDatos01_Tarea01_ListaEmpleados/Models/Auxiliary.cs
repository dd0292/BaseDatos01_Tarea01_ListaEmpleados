using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.Models
{
    public class TipoDocumento
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
    }

    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class Puesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal SalarioXHora { get; set; }
    }

    public class Usuario 
    {
        [DisplayName("Id Usuario")]
        public int Id { get; set; }

        [DisplayName("User Name")]
        public string Nombre { get; set; }

        [DisplayName("Contraseña")]
        public string Contrasena { get; set; }
    }
}