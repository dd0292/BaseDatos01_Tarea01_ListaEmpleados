using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
