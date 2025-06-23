using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BaseDatos01_Tarea01_ListaEmpleados
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            // Ruta para Planilla Mensual
            routes.MapRoute(
                  name: "Admin",
                  url: "Admin/Index",
                  defaults: new { controller = "Admin", action = "Index" }
              );
            routes.MapRoute(
                name: "MainViewEmployee",
                url: "Employee/MainViewEmployee",
                defaults: new { controller = "Employee", action = "MainViewEmployee" }
            );
            routes.MapRoute(
                name: "PlanillaSemanal",
                url: "Employee/Semanal/{id}/{semanaId}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Semanal",
                    semanaId = UrlParameter.Optional
                }
            );
            routes.MapRoute(
                name: "PlanillaMensual",
                url: "Employee/Monthly/{id}/{mesId}",
                defaults: new
                {
                    controller = "Employee",
                    action = "Monthly",
                    mesId = UrlParameter.Optional  // Hace que el mesId sea opcional
                },
                constraints: new
                {
                    id = @"\d+"  // Solo permite números para el id
                }
            );
            routes.MapRoute(
                name: "PlanillaMensualSoloId",
                url: "Employee/Monthly/{id}",
                defaults: new { controller = "Employee", action = "Monthly", mesId = UrlParameter.Optional }
            );
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}


