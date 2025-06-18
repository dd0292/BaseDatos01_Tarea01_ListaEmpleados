using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class MovementController : Controller
    {
        _DAL _employeeDAL = new _DAL();
        // GET: Movement
        //public ActionResult Index(string Nombre, int ValorDocumentoIdentidad)
        //{
        //    Employee empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);
        //    List<Movement> movimientos = _employeeDAL.ListarMovimientosEmpleado(ValorDocumentoIdentidad);

        //    var viewModel = new EmployeeMovementsViewModel
        //    {
        //        Empleado = empleado,
        //        Movimientos = movimientos
        //    };
        //    return View(viewModel);
        //}

        //[HttpGet]
        //public ActionResult Create(string Nombre, int ValorDocumentoIdentidad)
        //{
        //    var tiposMovimiento = _employeeDAL.ObtenerTiposMovimiento();
        //    ViewBag.TiposMovimiento = new SelectList(tiposMovimiento);

        //    Employee empleado = _employeeDAL.ObtenerEmpleadoPorNombreYDocumento(Nombre, ValorDocumentoIdentidad);
        //    ViewBag.Empleado = empleado;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Create(string Nombre, int ValorDocumentoIdentidad, string TipoMovimiento, Movement move)
        //{
        //    Console.WriteLine("Nombre: " + Nombre);
        //    int outCode;

        //    outCode = _employeeDAL.InsertarMovimiento(
        //        ValorDocumentoIdentidad,
        //        TipoMovimiento,
        //        move.Monto
        //    );

        //    if (outCode == 0)
        //    {
        //        TempData["SuccessMessage"] = "Movimiento registrado correctamente.";
        //    }
        //    else
        //    {
        //        string errorDescription = _employeeDAL.ObtenerDescripcionError(outCode);
        //        TempData["ErrorMessage"] = $"[ERROR {outCode}] {errorDescription}";
        //    }
        //    return RedirectToAction("Index", "Movement", new { Nombre = Nombre, ValorDocumentoIdentidad = ValorDocumentoIdentidad });

        //}
    }
}
