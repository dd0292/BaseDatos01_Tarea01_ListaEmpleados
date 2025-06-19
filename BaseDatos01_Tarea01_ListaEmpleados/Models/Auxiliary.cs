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
        public TipoDocumento(int Id, string Nombre)
        {
            this.Id = Id;
            this.Nombre = Nombre;
        }

        public TipoDocumento(int Id)
        {
            this.Id = Id;
        }
        public int Id { get; set; }

        public string Nombre { get; set; }
    }

    public class Departamento
    {
        public Departamento (int Id, string Nombre)
        {
            this.Id = Id;
            this.Nombre = Nombre;
        }

        public Departamento(int Id)
        {
            this.Id = Id;
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class Puesto
    {
        public Puesto(int Id, string Nombre, decimal SalarioXHora) { 
            this.Id = Id;
            this.Nombre = Nombre;
            this.SalarioXHora = SalarioXHora;
        }

        public Puesto(int Id)
        {
            this.Id = Id;
        }
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal SalarioXHora { get; set; }
    }

    public class Usuario 
    {

        public Usuario(int Id, string Nombre, string Contrasena)
        {
            this.Id = Id;
            this.Nombre = Nombre;
            this.Contrasena = Contrasena;
        }

        public Usuario(string Nombre, string Contrasena)
        {
            this.Nombre = Nombre;
            this.Contrasena = Contrasena;
        }

        [DisplayName("Id Usuario")]
        public int Id { get; set; }

        [DisplayName("User Name")]
        public string Nombre { get; set; }

        [DisplayName("Contraseña")]
        public string Contrasena { get; set; }
    }
}