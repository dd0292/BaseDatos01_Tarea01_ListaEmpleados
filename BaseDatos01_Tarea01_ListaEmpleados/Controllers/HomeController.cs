using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)  // Esto podría saltarse si hay una cookie vieja
            //{
            return View();
            //}
            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public ActionResult CargarXml()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CargarXml(HttpPostedFileBase xmlCatalogos, HttpPostedFileBase xmlOperaciones)
        {
            if (xmlCatalogos == null || xmlOperaciones == null)
            {
                ViewBag.Mensaje = "Debe seleccionar ambos archivos XML.";
                return View();
            }

            try
            {
                string contenidoCatalogos, contenidoOperaciones;

                using (var reader1 = new StreamReader(xmlCatalogos.InputStream))
                    contenidoCatalogos = await reader1.ReadToEndAsync();

                using (var reader2 = new StreamReader(xmlOperaciones.InputStream))
                    contenidoOperaciones = await reader2.ReadToEndAsync();

                EjecutarSpXml("sp_CargarCatalogosDesdeXml", contenidoCatalogos);
                EjecutarSpXml("sp_EjecutarOperacionesDesdeXml", contenidoOperaciones);

                ViewBag.Mensaje = "Archivos procesados correctamente.";
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = $"Error al procesar archivos: {ex.Message}";
            }

            return View();
        }

        private void EjecutarSpXml(string nombreSp, string xml)
        {
            using (SqlConnection conn = new SqlConnection("TU_CADENA_CONEXION"))
            {
                SqlCommand cmd = new SqlCommand(nombreSp, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@XmlData", xml);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}