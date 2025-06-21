using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class AdminController : Controller
    {
        _DAL _employeeDAL = new _DAL();
        int outResultCode = 0;
        string outResultDescription = string.Empty;

        public ActionResult Index(string filtro = "")
        {

            var employeeList = _employeeDAL.FiltrarEmpleados(filtro, ref outResultCode, ref outResultDescription);

            if (outResultCode != 0)
            {
                TempData["ErrorMessage"] = $"[ERROR {outResultCode}] : {outResultDescription}";
            }
            else if (employeeList.Count == 0)
            {
                TempData["InfoMessage"] = "No hay empleados registrados actualmente en la Base de Datos con dicha informacion...";
            }


            return View(employeeList);
        }

        // Create
        [HttpGet]
        public ActionResult Create()
        {
            List<TipoDocumento> listaTipoDocu = _employeeDAL.ObtenerListaTipoDocumentos(ref outResultCode, ref outResultDescription);
            List<Puesto> listaPuestos = _employeeDAL.ObtenerListaPuestos(ref outResultCode, ref outResultDescription);
            List<Departamento> listaDepartamentos = _employeeDAL.ObtenerListaDepartamentos(ref outResultCode, ref outResultDescription);

            if (outResultCode != 0)
            {
                TempData["ErrorMessage"] = $"[ERROR {outResultCode}] : {outResultDescription}";
            }

            ViewBag.TiposDocumento = new SelectList(listaTipoDocu, "Id", "Nombre");
            ViewBag.Puestos = new SelectList(listaPuestos, "Id", "Nombre");
            ViewBag.Departamentos = new SelectList(listaDepartamentos, "Id", "Nombre");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee employee, int tipoDocumentoId, int departamentoId, int puestoId, string username, string password)
        {

            try
            {
                employee.Usuario = new Usuario(Nombre: username, Contrasena: password);
                employee.TipoDocumento = new TipoDocumento(Id: tipoDocumentoId);
                employee.Departamento = new Departamento(Id: departamentoId);
                employee.Puesto = new Puesto(Id: puestoId);

                _employeeDAL.InsertarEmpleado(employee, ref outResultCode, ref outResultDescription);

                if (outResultCode == 0)
                {
                    TempData["SuccessMessage"] = "Inserción exitosa !!!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"[ERROR {outResultCode}] : {outResultDescription}";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al insertar empleado: " + ex.Message;

                var listaTiposDoc = _employeeDAL.ObtenerListaTipoDocumentos(ref outResultCode, ref outResultDescription);
                var listaPuestos = _employeeDAL.ObtenerListaPuestos(ref outResultCode, ref outResultDescription);
                var listaDepartamentos = _employeeDAL.ObtenerListaDepartamentos(ref outResultCode, ref outResultDescription);

                if (outResultCode != 0)
                {
                    TempData["ErrorMessage"] = $"[ERROR {outResultCode}] : {outResultDescription}";
                }

                ViewBag.TiposDocumento = new SelectList(listaTiposDoc, "Id", "Nombre", tipoDocumentoId);
                ViewBag.Puestos = new SelectList(listaPuestos, "Id", "Nombre", puestoId);
                ViewBag.Departamentos = new SelectList(listaDepartamentos, "Id", "Nombre", departamentoId);

                return View(employee);
            }

        }

        // Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Employee employee = _employeeDAL.ObtenerEmpleadoPorId(id, ref outResultCode, ref outResultDescription);

            var listaTiposDoc = _employeeDAL.ObtenerListaTipoDocumentos(ref outResultCode, ref outResultDescription);
            var listaPuestos = _employeeDAL.ObtenerListaPuestos(ref outResultCode, ref outResultDescription);
            var listaDepartamentos = _employeeDAL.ObtenerListaDepartamentos(ref outResultCode, ref outResultDescription);

            if (outResultCode != 0 || employee == null)
            {
                TempData["ErrorMessage"] = $"[Error {outResultCode}] : {outResultDescription}";
                return RedirectToAction("Index");
            }

            var model = new EmployeeEditViewModel(
                employee: employee,
                TiposDocumento: new SelectList(listaTiposDoc, "Id", "Nombre", employee.TipoDocumento.Id),
                Puestos: new SelectList(listaPuestos, "Id", "Nombre", employee.Puesto.Id),
                Departamentos: new SelectList(listaDepartamentos, "Id", "Nombre", employee.Departamento.Id)
                );

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeEditViewModel model)
        {
            _employeeDAL.ActualizarEmpleado(model, ref outResultCode, ref outResultDescription);

            if (outResultCode == 0)
            {
                TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = $"[ERROR {outResultCode}] {outResultDescription}";
            }
            return RedirectToAction("Index");
        }

        // Delete 
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Employee employee = _employeeDAL.ObtenerEmpleadoPorId(id, ref outResultCode, ref outResultDescription);

            if (outResultCode != 0 || employee == null)
            {
                TempData["ErrorMessage"] = $"[Error {outResultCode}] : {outResultDescription}";
                return RedirectToAction("Index");
            }

            return View(employee);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int Id)
        {
            _employeeDAL.EliminarEmpleadoLogicamente(Id, ref outResultCode, ref outResultDescription);

            if (outResultCode == 0)
                TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
            else
                TempData["ErrorMessage"] = $"[ERROR {outResultCode}] {outResultDescription}";

            return RedirectToAction("Index");
        }

        // XML_Fuctions
        [HttpGet]
        public ActionResult XML_Fuctions()
        {
            return View();
        }

        [HttpPost]
        public ActionResult XML_Fuctions(HttpPostedFileBase xmlCatalogos, HttpPostedFileBase xmlOperaciones)
        {
            if (xmlCatalogos == null)
            {
                ViewBag.Mensaje = "Debe seleccionar un catalogo XML.";
                return View();
            }

            try
            {
                _employeeDAL.EliminarBaseDeDatos(ref outResultCode, ref outResultDescription);


                string xmlCatalogoContent;
                using (var reader = new StreamReader(xmlCatalogos.InputStream))
                {
                    xmlCatalogoContent = reader.ReadToEnd();
                }
                _employeeDAL.CargarCatalogoXML(xmlCatalogoContent, ref outResultCode, ref outResultDescription);
                if (outResultCode != 0)
                {
                    TempData["ErrorMessage"] = $"[ERROR CATALOGO {outResultCode}] {outResultDescription}";
                    return View();
                }

                if (xmlOperaciones != null)
                {
                    string xmlOperacionContent;
                    using (var reader = new StreamReader(xmlOperaciones.InputStream))
                    {
                        xmlOperacionContent = reader.ReadToEnd();
                    }
                    _employeeDAL.CargarOperacionesXML(xmlOperacionContent, ref outResultCode, ref outResultDescription);
                    if (outResultCode != 0)
                    {
                        TempData["ErrorMessage"] = $"[ERROR OPERACIONES {outResultCode}] {outResultDescription}";
                        return View();
                    }
                }

                TempData["SuccessMessage"] = "Datos cargados correctamente desde ambos XML.";
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al procesar archivos: {ex.Message}";
            }

            return View();
        }

        // Impersonate Emplyee
        [HttpGet]
        public ActionResult impersonateEmplyee(int Id)
        {
            Session["CurrentEmployeeId"] = Id;
            _employeeDAL.RegistrarEvento(12, $"empleadoId : {Id}, ", ref outResultCode, ref outResultDescription);
            return RedirectToAction("MainViewEmployee", "Employee");
        }
    }
}