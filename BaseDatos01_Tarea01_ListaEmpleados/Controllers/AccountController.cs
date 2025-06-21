using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BaseDatos01_Tarea01_ListaEmpleados.DAL;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class AccountController : Controller
    {
        private readonly DALBeforeLogIn _dal = new DALBeforeLogIn(); // Tu DAL existente

        // GET: /Account/Login (Vista de login)
        public ActionResult Login()
        {
            return View(); // Vista Login.cshtml
        }

        // POST: /Account/Login (Procesar credenciales)
        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            DataTable userData = _dal.ValidateUser(username, password);

            if (userData.Rows.Count > 0 && Convert.ToInt32(userData.Rows[0]["ResultCode"]) == 1)
            {

                // Guardar datos en sesión (como en tu WebForm)
                Session["CurrentUserId"] = userData.Rows[0]["UserId"];
                Session["CurrentEmployeeId"] = userData.Rows[0]["EmployeeId"];
                Session["UserType"] = userData.Rows[0]["UserType"];

                // Redirigir según UserType
                int userType = Convert.ToInt32(userData.Rows[0]["UserType"]);
                if (userType == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    return RedirectToAction("MainViewEmployee", "Employee");
                }
            }

            ViewBag.Error = "Credenciales inválidas"; // Mensaje de error
            return View();
        }

        // Cerrar sesión
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        } // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
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

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
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
