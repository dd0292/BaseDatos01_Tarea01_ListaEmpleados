using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class EmployeeController : Controller
    {

        Employee_DAL _employeeDAL = new Employee_DAL();

        // GET: Employee
        public ActionResult Index()
        {
            var employeeList = _employeeDAL.GetEmployeesList();

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
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee employee)
        {
            bool IsInserted = false;
            try
            {

                if (ModelState.IsValid) { 
                    IsInserted = _employeeDAL.InsertEmployee(employee);

                    if (IsInserted) 
                    {
                        TempData["SuccessMessage"] = "Inserción exitosa !!!";
                    } 
                    else
                    {
                        TempData["ErrorMessage"] = "[ERROR 50001]Nombre de Empleado ya existe";
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
        public ActionResult Read()
        {
            return View();
        }

        // POST: Employee/Read
        [HttpPost]
        public ActionResult Read(Employee employee)
        {
            bool IsInserted = false;
            try
            {

                if (ModelState.IsValid)
                {
                    IsInserted = _employeeDAL.InsertEmployee(employee);

                    if (IsInserted)
                    {
                        TempData["SuccessMessage"] = "Inserción exitosa !!!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "[ERROR 50001]Nombre de Empleado ya existe";
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View();
            }
        }


        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
