using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class AdminController : Controller
    {

        _DAL _employeeDAL = new _DAL();

        public ActionResult Index(string filtro = "")
        {
            var employeeList = _employeeDAL.FiltrarEmpleados(filtro);

            if (employeeList.Count == 0)
            {
                TempData["InfoMessage"] = "No hay empleados registrados actualmente en la Base de Datos con dicha informacion...";
            }

            return View(employeeList);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var listaPuestos = _employeeDAL.ObtenerListaPuestos();
            ViewBag.Puestos = new SelectList(listaPuestos);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee employee, string puestoNombre)
        {
            int outCode = 0;
            try
            {

                if (true) {
                    outCode = _employeeDAL.InsertarEmpleado(employee, puestoNombre);

                    if (outCode == 0) 
                    {
                        TempData["SuccessMessage"] = "Inserción exitosa !!!";
                    } 
                    else
                    {
                        string errorDescription = _employeeDAL.ObtenerDescripcionError(outCode);
                        TempData["ErrorMessage"] = $"[ERROR {outCode}] {errorDescription}";
                    }
                }

                return RedirectToAction("Index");
            }
            catch(Exception ex) 
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Read(string Nombre, int ValorDocumentoIdentidad)
        {
            var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            if (empleado == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction("Index");
            }

            return View(empleado);
        }

        [HttpGet]
        public ActionResult Edit(string Nombre, int ValorDocumentoIdentidad)
        {
            var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            if (empleado == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction("Index");
            }

            ViewBag.Puestos = new SelectList(_employeeDAL.ObtenerListaPuestos(), empleado.NombrePuesto);
            return View(empleado);
        }

        [HttpPost]
        public ActionResult Edit(string nombreAntiguo, int docAntiguo, Employee empleadoEditado, string puestoNombre)
        {
            int idPuesto = _employeeDAL.ObtenerIdPuestoPorNombre(puestoNombre);

            int resultado = _employeeDAL.ActualizarEmpleado(nombreAntiguo, docAntiguo, empleadoEditado, idPuesto);

            if (resultado != 0)
            {
                string errorDescription = _employeeDAL.ObtenerDescripcionError(resultado);
                TempData["ErrorMessage"] = $"[ERROR {resultado}] {errorDescription}";
            }
            else
            {
                TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
                
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Delete(string Nombre, int ValorDocumentoIdentidad)
        {
            var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            if (empleado == null)
            {
                TempData["ErrorMessage"] = "Empleado no encontrado.";
                return RedirectToAction("Index");
            }

            return View(empleado);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(string Nombre, int ValorDocumentoIdentidad)
        {
            int resultado = _employeeDAL.EliminarEmpleadoLogicamente(Nombre, ValorDocumentoIdentidad);

            if (resultado == 0)
                TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
            else
                TempData["ErrorMessage"] = "Error al eliminar el empleado.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult DeleteCancel(string Nombre, int ValorDocumentoIdentidad)
        {
            int resultado = _employeeDAL.EliminarEmpleadoCancelar(Nombre, ValorDocumentoIdentidad);

            if (resultado != 0)
                TempData["ErrorMessage"] = "Error al eliminar el empleado.";

            return RedirectToAction("Index");
        }

    }
}
