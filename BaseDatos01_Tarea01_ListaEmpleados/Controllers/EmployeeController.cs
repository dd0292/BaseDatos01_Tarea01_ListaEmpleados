using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class EmployeeController : Controller
    {

        _DAL _employeeDAL = new _DAL();
        string sss;

        public ActionResult MainViewEmployee()
        {
            // 1. Verificación segura de sesión
            if (Session["CurrentUserId"] == null || Session["CurrentEmployeeId"] == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Account");
            }

            // 2. Conversión segura de IDs
            int userId, employeeId;
            try
            {
                userId = Convert.ToInt32(Session["CurrentUserId"]);
                employeeId = Convert.ToInt32(Session["CurrentEmployeeId"]);
            }
            catch
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Account");
            }

            // 3. Obtener datos del empleado
            int resultCode = 0;
            string resultDescription = "";
            Employee empleado = _employeeDAL.ObtenerEmpleadoPorId(employeeId, ref resultCode, ref resultDescription);

            // 4. Verificación de pertenencia
            if (empleado == null || empleado.Usuario == null || empleado.Usuario.Id != userId)
            {
                System.Diagnostics.Debug.WriteLine($"Inconsistencia: UserId={userId} vs EmpleadoUserId={empleado?.Usuario?.Id}");
                FormsAuthentication.SignOut();
                return RedirectToAction("Login", "Account");
            }

            // 5. Pasar datos a la vista
            return View(empleado);
        }


        [Authorize]
            [HttpGet]
        public ActionResult Semanal(int id, int? semanaId)
        {
            // Validación básica
            if (id <= 0)
            {
                Debug.WriteLine("[ERROR] ID de empleado inválido: " + id);
                TempData["Error"] = "ID de empleado no válido";
                return RedirectToAction("Error");
            }

            Debug.WriteLine($"[DEBUG] Consultando planilla - EmpleadoID: {id}, SemanaID: {semanaId}");

            int outCode = 0;
            string outMessage = "";
            var planilla = _employeeDAL.ObtenerPlanillaSemanal(id, semanaId, ref outCode, ref outMessage);

            // Manejo de errores
            if (outCode != 0)
            {
                Debug.WriteLine($"[ERROR] Código: {outCode}, Mensaje: {outMessage}");
                TempData["Error"] = outMessage;
                return RedirectToAction("Error", new { errorCode = outCode });
            }

            // Validación de estructura según tu modelo
            if (planilla?.Semana == null || planilla.Semana.MovimientosPorDia.Any(d => d == null))
            {
                Debug.WriteLine("[WARNING] Estructura de planilla inválida");
                return View("EmptyPlanilla", new
                {
                    EmpleadoId = id,
                    SemanaId = semanaId,
                    Message = "Estructura de datos incompleta"
                });
            }

            // Cálculo adicional de horas por día (opcional)
            CalcularHorasPorDia(planilla.Semana);

            return View(planilla);
        }

        private void CalcularHorasPorDia(SemanaPlanilla semana)
        {
            for (int i = 0; i < 7; i++)
            {
                semana.HorasPorDia[i] = semana.MovimientosPorDia[i].Sum(m => m.Horas);
            }
            semana.HorasTotales = semana.HorasPorDia.Sum();
        }

        public ActionResult Error(int errorCode)
        {
            ViewBag.ErrorCode = errorCode;
            ViewBag.ErrorMessage = TempData["Error"]?.ToString() ?? "Error desconocido";
            return View();
        }
        /* [Authorize]
     /*    public ActionResult Monthly(int id) // Recibe el ID del empleado
         {
             // Lógica para cargar datos mensuales
             int outCode = 0;
             string sss = "";
             var planillaMensual = _employeeDAL.ObtenerPlanillaMensual(id, ref outCode, ref sss);

             if (planillaMensual == null)
             {
                 TempData["Error"] = "No se encontró la planilla mensual";
                 return RedirectToAction("Error");
             }

             return View(planillaMensual); // Pasa los datos a Monthly.cshtml
         }
         public ActionResult Index(string filtro = "")
         {
             int outCode = 0;
             /*
             var employeeList = _employeeDAL.FiltrarEmpleados(filtro, ref outCode, ref sss);

             if (employeeList.Count == 0)
             {
                 TempData["InfoMessage"] = "No hay empleados registrados actualmente en la Base de Datos con dicha informacion...";
             }


             return View();
         }*/
        [HttpGet]
        public ActionResult Create()
        {
            //var listaPuestos = _employeeDAL.ObtenerListaPuestos(ref outCode, ref sss);
            //ViewBag.Puestos = new SelectList(listaPuestos);
            return View();
        }

        [HttpPost]
        public ActionResult Create(Employee employee, string puestoNombre)
        {
            int outCode = 0;
            try
            {

                //if (true) {
                //   // outCode = _employeeDAL.InsertarEmpleado(employee, puestoNombre);

                //    if (outCode == 0) 
                //    {
                //        TempData["SuccessMessage"] = "Inserción exitosa !!!";
                //    } 
                //    else
                //    {
                //        string errorDescription = _employeeDAL.ObtenerDescripcionError(outCode);
                //        TempData["ErrorMessage"] = $"[ERROR {outCode}] {errorDescription}";
                //    }
                //}

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
            //var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            //if (empleado == null)
            //{
            //    TempData["ErrorMessage"] = "Empleado no encontrado.";
            //    return RedirectToAction("Index");
            //}

            return View();
        }

        [HttpGet]
        public ActionResult Edit(string Nombre, int ValorDocumentoIdentidad)
        {
            //var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            //if (empleado == null)
            //{
            //    TempData["ErrorMessage"] = "Empleado no encontrado.";
            //    return RedirectToAction("Index");
            //}

            //ViewBag.Puestos = new SelectList(_employeeDAL.ObtenerListaPuestos(ref outCode, ''), empleado.Puesto);
            return View();
        }

        [HttpPost]
       // public ActionResult Edit(string nombreAntiguo, int docAntiguo, Employee empleadoEditado, string puestoNombre)
      //  {
        //    int idPuesto = _employeeDAL.ObtenerIdPuestoPorNombre(puestoNombre);

            //int resultado = _employeeDAL.ActualizarEmpleado(nombreAntiguo, docAntiguo, empleadoEditado, idPuesto);

       //     if (resultado != 0)
       //     {
       //         string errorDescription = _employeeDAL.ObtenerDescripcionError(resultado);
      //          TempData["ErrorMessage"] = $"[ERROR {resultado}] {errorDescription}";
       //     }
       //     else
       //     {
       //         TempData["SuccessMessage"] = "Empleado actualizado correctamente.";
      //          
       //     }
       //     return RedirectToAction("Index");
      //  }

        [HttpGet]
        public ActionResult Delete(string Nombre, int ValorDocumentoIdentidad)
        {
            //var empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);

            //if (empleado == null)
            //{
            //    TempData["ErrorMessage"] = "Empleado no encontrado.";
            //    return RedirectToAction("Index");
            //}

            return View();
        }

        [HttpPost]
       // public ActionResult DeleteConfirmed(string Nombre, int ValorDocumentoIdentidad)
       // {
           // int resultado = _employeeDAL.EliminarEmpleadoLogicamente(Nombre, ValorDocumentoIdentidad);

        //    if (resultado == 0)
          //      TempData["SuccessMessage"] = "Empleado eliminado correctamente.";
          //  else
           //     TempData["ErrorMessage"] = "Error al eliminar el empleado.";

           // return RedirectToAction("Index");
      //  }

        [HttpGet]
        public ActionResult DeleteCancel(string Nombre, int ValorDocumentoIdentidad)
        {
            //int resultado = _employeeDAL.EliminarEmpleadoCancelar(Nombre, ValorDocumentoIdentidad);

            //if (resultado != 0)
            //    TempData["ErrorMessage"] = "Error al eliminar el empleado.";

            return RedirectToAction("Index");
        }


    }
}
