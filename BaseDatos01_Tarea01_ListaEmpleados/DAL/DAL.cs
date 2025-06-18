using BaseDatos01_Tarea01_ListaEmpleados.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL // Data Access Layer
{
    public class _DAL
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;

        string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();
        DatabaseHelper dbHelper;

        // string UserName = HttpContext.Current.Session["Username"].ToString();
        // string UserId = HttpContext.Current.Session["UserId"].ToString();

        int UserId = 1;
        string ClientIp = "192.168.18.7";

        public _DAL()
        {
            ClientIp = (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? context.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
            dbHelper = new DatabaseHelper(conString);
        }

        //-----------------------------------------------------------------------------------------

        public List<Employee> FiltrarEmpleados(string filtro, ref int outResultCode, ref string outResultDescription)
        {
            var list = new List<Employee>();

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@inFiltro", SqlDbType.VarChar, 100) { Value = filtro ?? "" },
                new SqlParameter("@inIdUsuario", SqlDbType.Int) { Value = UserId },
                new SqlParameter("@inUserIP", SqlDbType.VarChar, 64) { Value = ClientIp }
            };

            try
            {

                DataTable resultTable = dbHelper.ExecuteStoredProcedure(
                    "sp_FiltrarEmpleados",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0) 
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        list.Add(new Employee(
                            Id: Convert.ToInt32(row["IdEmpleado"]),
                            Nombre: row["NombreEmpleado"].ToString(),

                            IdTipoDocumento: Convert.ToInt32(row["IdTipoDocumento"]),
                            TipoDocumento: row["TipoDocumento"].ToString(),

                            ValorDocumento: row["DocumentoEmpleado"].ToString(),

                            IdPuesto: Convert.ToInt32(row["IdPuesto"]),
                            NombrePuesto: row["NombrePuesto"].ToString(),
                            SalarioPorHora: Convert.ToDecimal(row["SalarioPorHora"]),

                            IdDepartamento: Convert.ToInt32(row["IdDepartamento"]),
                            Departamento: row["Departamento"].ToString(),

                            FechaNacimiento: Convert.ToDateTime(row["FechaNacimiento"]),

                            IdUsuario: Convert.ToInt32(row["IdUsuario"]),
                            Username: row["Usuario"].ToString(),
                            Password: row["Passphrase"].ToString(),
                            Estado: Convert.ToBoolean(row["Estado"])
                        ));
                    }
                    Console.WriteLine($"Éxito [FiltrarEmpleados]: {outResultDescription}");
                }
                else
                {
                    Console.WriteLine($"Error [FiltrarEmpleados]: Código {outResultCode} - {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                Console.WriteLine($"Exception [FiltrarEmpleados]: {ex.Message}");
            }

            return list;
        }

        public Employee ObtenerEmpleadoPorId(int id, ref int outResultCode, ref string outResultDescription)
        {
            Employee empleado = null;

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@inId", SqlDbType.Int) { Value = id },
            };

            try
            {

                DataTable resultTable = dbHelper.ExecuteStoredProcedure(
                    "sp_ObtenerEmpleadoPorId",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        empleado  = new Employee(
                            Id: Convert.ToInt32(row["Id"]),
                            Nombre: row["Nombre"].ToString(),

                            IdTipoDocumento: Convert.ToInt32(row["IdTipoDocumento"]),
                            TipoDocumento: row["TipoDocumento"].ToString(),

                            ValorDocumento: row["ValorDocumento"].ToString(),

                            IdPuesto: Convert.ToInt32(row["IdPuesto"]),
                            NombrePuesto: row["NombrePuesto"].ToString(),
                            SalarioPorHora: Convert.ToDecimal(row["SalarioPorHora"]),

                            IdDepartamento: Convert.ToInt32(row["IdDepartamento"]),
                            Departamento: row["Departamento"].ToString(),

                            FechaNacimiento: Convert.ToDateTime(row["FechaNacimiento"]),

                            IdUsuario: Convert.ToInt32(row["IdUsuario"]),
                            Username: row["Username"].ToString(),
                            Password: row["Passphrase"].ToString(),
                            Estado: Convert.ToBoolean(row["Estado"])
                        );
                    }
                    Console.WriteLine($"Éxito [ObtenerEmpleadoPorId]: {outResultDescription}");
                }
                else
                {
                    Console.WriteLine($"Error [ObtenerEmpleadoPorId]: Código {outResultCode} - {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                Console.WriteLine($"Exception [ObtenerEmpleadoPorId]: {ex.Message}");
            }

            return empleado;
        }
        
        public List<TipoDocumento> ObtenerListaTipoDocumentos(ref int outResultCode, ref string outResultDescription)
        {
            var list = new List<TipoDocumento>();
            var parameters = new SqlParameter[] { };

            try
            {
                DataTable resultTable = dbHelper.ExecuteStoredProcedure(
                    "sp_ObtenerListaTipoDocumentos",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        list.Add(new TipoDocumento(
                            Id: Convert.ToInt32(row["Id"]),
                            Nombre: row["Nombre"].ToString()
                            )
                        );
                    }
                    Console.WriteLine($"Éxito [ObtenerListaTipoDocumentos]: {outResultDescription}");
                }
                else
                {
                    Console.WriteLine($"Error [ObtenerListaTipoDocumentos]: Código {outResultCode} - {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                Console.WriteLine($"Exception [ObtenerListaTipoDocumentos]: {ex.Message}");
            }

            return list;
        }

        public List<Puesto> ObtenerListaPuestos(ref int outResultCode, ref string outResultDescription)
        {
            var list = new List<Puesto>();
            var parameters = new SqlParameter[] { };

            try
            {
                DataTable resultTable = dbHelper.ExecuteStoredProcedure(
                    "sp_ObtenerListaPuestos",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        list.Add(new Puesto(
                            Id: Convert.ToInt32(row["Id"]),
                            Nombre: row["Nombre"].ToString(),
                            SalarioXHora: Convert.ToDecimal(row["SalarioXHora"])
                            )
                        );
                    }
                    Console.WriteLine($"Éxito [ObtenerListaPuestos]: {outResultDescription}");
                }
                else
                {
                    Console.WriteLine($"Error [ObtenerListaPuestos]: Código {outResultCode} - {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                Console.WriteLine($"Exception [ObtenerListaPuestos]: {ex.Message}");
            }

            return list;
        }

        public List<Departamento> ObtenerListaDepartamentos(ref int outResultCode, ref string outResultDescription)
        {
            var list = new List<Departamento>();
            var parameters = new SqlParameter[] { };

            try
            {
                DataTable resultTable = dbHelper.ExecuteStoredProcedure(
                    "sp_ObtenerListaDepartamentos",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    foreach (DataRow row in resultTable.Rows)
                    {
                        list.Add(new Departamento(
                            Id: Convert.ToInt32(row["Id"]),
                            Nombre: row["Nombre"].ToString()
                            )
                        );
                    }
                    Console.WriteLine($"Éxito [ObtenerListaTipoDocumentos]: {outResultDescription}");
                }
                else
                {
                    Console.WriteLine($"Error [ObtenerListaTipoDocumentos]: Código {outResultCode} - {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                Console.WriteLine($"Exception [ObtenerListaDepartamentos]: {ex.Message}");
            }

            return list;
        }

        public int InsertarEmpleado(Employee model, ref int outResultCode, ref string outResultDescription)
        {
            int employeeId = -1;
            int userId = -1;

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@inNombre", model.Nombre),
                new SqlParameter("@inIdTipoDocumento", model.TipoDocumento.Id),
                new SqlParameter("@inValorDocumento", model.ValorDocumento),
                new SqlParameter("@inFechaNacimiento", model.FechaNacimiento),
                new SqlParameter("@inIdPuesto", model.Puesto.Id),
                new SqlParameter("@inIdDepartamento", model.Departamento.Id),
                new SqlParameter("@inUsername", model.Usuario.Nombre),
                new SqlParameter("@inPassword", model.Usuario.Contrasena),
                new SqlParameter("@inIdUsuarioEjecutor", UserId),
                new SqlParameter("@inUserIP", ClientIp),

                new SqlParameter("@outResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@outResultMessage", SqlDbType.VarChar, 529) { Direction = ParameterDirection.Output },
                new SqlParameter("@outIdEmpleado", SqlDbType.Int) { Direction = ParameterDirection.Output },
                new SqlParameter("@outIdUsuario", SqlDbType.Int) { Direction = ParameterDirection.Output }
            };

            try
            {
                dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_CrearEmpleadoYUsuario",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                outResultCode = parameters.First(p => p.ParameterName == "@outResultCode").Value != DBNull.Value ?
                    (int)parameters.First(p => p.ParameterName == "@outResultCode").Value : -1;

                outResultDescription = parameters.First(p => p.ParameterName == "@outResultMessage").Value != DBNull.Value ?
                    parameters.First(p => p.ParameterName == "@outResultMessage").Value.ToString() : "Error desconocido";

                employeeId = parameters.First(p => p.ParameterName == "@outIdEmpleado").Value != DBNull.Value ?
                    (int)parameters.First(p => p.ParameterName == "@outIdEmpleado").Value : -1;

                userId = parameters.First(p => p.ParameterName == "@outIdUsuario").Value != DBNull.Value ?
                    (int)parameters.First(p => p.ParameterName == "@outIdUsuario").Value : -1;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar empleado: {outResultDescription} -- {ex.ToString()}");
            }

            return outResultCode;
        }

        public int ActualizarEmpleado(EmployeeEditViewModel newEmpleado, ref int outResultCode, ref string outResultDescription)
        {

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@inIdEmpleado", SqlDbType.Int) { Value = newEmpleado.Id },
                new SqlParameter("@inNombre", SqlDbType.VarChar, 100) { Value = newEmpleado.Nombre },
                new SqlParameter("@inIdTipoDocumento", SqlDbType.Int) { Value = newEmpleado.IdTipoDocumento },
                new SqlParameter("@inValorDocumento", SqlDbType.VarChar, 64) { Value = newEmpleado.ValorDocumento },
                new SqlParameter("@inFechaNacimiento", SqlDbType.Date) { Value = newEmpleado.FechaNacimiento },
                new SqlParameter("@inIdPuesto", SqlDbType.Int) { Value = newEmpleado.IdPuesto },
                new SqlParameter("@inIdDepartamento", SqlDbType.Int) { Value = newEmpleado.IdDepartamento },

                new SqlParameter("@inIdUsuario", SqlDbType.Int) { Value = UserId },
                new SqlParameter("@inUserIP", SqlDbType.VarChar, 64) { Value = ClientIp }
            };

            try
            {
                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_EditarEmpleado",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($"Empleado editado correctamente. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al editado: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al editado empleado: {ex.Message}");

            }

            return outResultCode;
        }

        public int EliminarEmpleadoLogicamente(int employeeId, ref int outResultCode, ref string outResultDescription)
        {

            var parameters = new SqlParameter[]
            {
                new SqlParameter("@inIdEmpleado", SqlDbType.Int) { Value = employeeId },
                new SqlParameter("@inIdUsuario", SqlDbType.Int) { Value = UserId },
                new SqlParameter("@inUserIP", SqlDbType.VarChar, 64) { Value = ClientIp }
            };

            try
            {
                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_EliminarEmpleado",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($"Empleado eliminado correctamente. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al eliminar: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al eliminar empleado: {ex.Message}");

            }

            return outResultCode;
        }

        public int EliminarBaseDeDatos(ref int outResultCode, ref string outResultDescription)
        {

            try
            {
                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_BorrarYReiniciarBaseDatos",
                    new SqlParameter[] { },
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($"Base de Datos Reiniciada. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al Reiniciada: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al Reiniciada DB: {ex.Message}");

            }

            return outResultCode;
        }

        public int CargarCatalogoXML(string xml, ref int outResultCode, ref string outResultDescription)
        {

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@inXmlCat", SqlDbType.Xml) {Value = xml, Direction = ParameterDirection.Input},
                };

                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_CargarCatalogoXML",
                    new SqlParameter[] { },
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($"Empleado eliminado correctamente. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al eliminar: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al eliminar empleado: {ex.Message}");

            }

            return outResultCode;
        }

        public int CargarOperacionesXML(string xml, ref int outResultCode, ref string outResultDescription)
        {

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@XmlOperacion", SqlDbType.Xml) {Value = xml, Direction = ParameterDirection.Input},
                    new SqlParameter("@inIdUsuario", SqlDbType.Int) { Value = UserId,  Direction = ParameterDirection.Input },
                    new SqlParameter("@inUserIP", SqlDbType.VarChar, 64) { Value = ClientIp,  Direction = ParameterDirection.Input }
                };

                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_CargarOperacionesXML",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($" CargarOperacionesXM correctamente. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al CargarOperacionesXM: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al CargarOperacionesXM: {ex.Message}");

            }

            return outResultCode;
        }

        public int RegistrarEvento(int IdTipoEvento, string info, ref int outResultCode, ref string outResultDescription)
        {

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@inIdUsuario", SqlDbType.Int) {Value = UserId, Direction = ParameterDirection.Input},
                    new SqlParameter("@inIdTipoEvento", SqlDbType.Int) {Value = IdTipoEvento, Direction = ParameterDirection.Input},
                    new SqlParameter("@inIPUser", SqlDbType.Int) {Value = ClientIp, Direction = ParameterDirection.Input},
                    new SqlParameter("@inDescripcion", SqlDbType.Int) {Value = info, Direction = ParameterDirection.Input},
                };

                int rowsAffected = dbHelper.ExecuteNonQueryStoredProcedure(
                    "sp_RegistrarEvento",
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($" Registrar Evento correctamente");
                }
                else
                {
                    Console.WriteLine($"Error al Registrar Evento: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al Registrar Evento: {ex.Message}");

            }

            return outResultCode;
        }
    }
}


