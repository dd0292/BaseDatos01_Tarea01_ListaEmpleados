using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class EmployeeController : Controller
    {

        Employee_DAL _employeeDAL = new Employee_DAL();

        public ActionResult Index(string filtro = "")
        {
            var employeeList = _employeeDAL.GetEmployeesList(filtro);

            if (employeeList.Count == 0)
            {
                TempData["InfoMessage"] = "No hay empleados registrados actualmente en la base de Datos...";
            }

            return View(employeeList);
        }

        // GET: Employee/Create
        [HttpGet]
        public ActionResult Create()
        {
            var listaPuestos = _employeeDAL.GetJobList();
            ViewBag.Puestos = new SelectList(listaPuestos);
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee employee, string puestoNombre)
        {
            int outCode = 0;
            try
            {

                if (true) {
                    outCode = _employeeDAL.InsertEmployee(employee, puestoNombre);

                    if (outCode == 0) 
                    {
                        TempData["SuccessMessage"] = "Inserción exitosa !!!";
                    } 
                    else
                    {
                        string errorDescription = _employeeDAL.GetErrorFromCode(outCode);
                        TempData["ErrorMessage"] = $"[ERROR0 {outCode}] {errorDescription}";
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

        // GET: Emplyee/Read
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

            ViewBag.Puestos = new SelectList(_employeeDAL.GetJobList(), empleado.NombrePuesto);
            return View(empleado);
        }

        [HttpPost]
        public ActionResult Edit(string nombreAntiguo, int docAntiguo, Employee empleadoEditado, string puestoNombre)
        {
            int idPuesto = _employeeDAL.ObtenerIdPuestoPorNombre(puestoNombre);

            int resultado = _employeeDAL.ActualizarEmpleado(nombreAntiguo, docAntiguo, empleadoEditado, idPuesto);

            if (resultado == 0)
                TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
            else
                TempData["ErrorMessage"] = "Ya existe un empleado con ese nombre o documento.";

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
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(string Nombre, int ValorDocumentoIdentidad)
        {
            int resultado = _employeeDAL.EliminarEmpleadoLogicamente(Nombre, ValorDocumentoIdentidad);

            if (resultado == 0)
                TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
            else
                TempData["ErrorMessage"] = "Error al eliminar el empleado.";

            return RedirectToAction("Index");
        }

    }
}
