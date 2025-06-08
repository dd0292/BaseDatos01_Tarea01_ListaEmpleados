using BaseDatos01_Tarea01_ListaEmpleados.Models;
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

        private static int? ExtractIntFromBrackets(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            var pattern = @"\[(\d+)\]";
            var match = Regex.Match(input, pattern);

            if (match.Success && match.Groups.Count > 1)
            {

                if (int.TryParse(match.Groups[1].Value, out int result))
                {
                    return result;
                }
            }

            return null;
        }

        public int EliminarEmpleadoCancelar(string nombre, int valorDocumento)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("dbo.EliminarEmpleadoCancelar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNombre", nombre);
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumento);
                cmd.Parameters.AddWithValue("@inIdPostByUser", UserId);
                cmd.Parameters.AddWithValue("@inPostInIP", ClientIp);

                SqlParameter outParam = new SqlParameter("@outResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                resultCode = Convert.ToInt32(outParam.Value);
                conn.Close();
            }

            return resultCode;
        }

        public int InsertarMovimiento(int valorDocumentoIdentidad, string idTipoMovimiento, decimal monto)
        {
            int resultado = -1;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("InsertarMovimiento", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@inIdTipoMovimiento", ExtractIntFromBrackets(idTipoMovimiento));
                cmd.Parameters.AddWithValue("@inMonto", monto);
                cmd.Parameters.AddWithValue("@inIdPostByUser", UserId);
                cmd.Parameters.AddWithValue("@inPostInIP", ClientIp);

                SqlParameter outParam = new SqlParameter("@outResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                conn.Open();
                cmd.ExecuteNonQuery();

                resultado = (int)cmd.Parameters["@outResultCode"].Value;
            }

            return resultado;
        }

        public List<Movement> ListarMovimientosEmpleado(int valorDocumentoIdentidad)
        {
            List<Movement> lista = new List<Movement>();

            using (SqlConnection conn = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("ListarMovimientosEmpleado", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumentoIdentidad);

                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Movement mov = new Movement
                        {
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            NombreTipo = reader["NombreTipo"].ToString(),
                            Monto = Convert.ToDecimal(reader["Monto"]),
                            NuevoSaldo = Convert.ToDecimal(reader["NuevoSaldo"]),
                            NombreUsuario = reader["NombreUsuario"].ToString(),
                            IdPostByUser = reader["IdPostByUser"].ToString(),
                            PostTime = Convert.ToDateTime(reader["PostTime"]),
                            PostInIP = reader["PostInIP"].ToString()
                        };
                        lista.Add(mov);
                    }
                }
            }

            return lista;
        }

        public string ObtenerDescripcionError(int code)
        {
            string descripcion;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerDescripcionError", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inCodigoError", code);

                SqlParameter outputDescripcion = new SqlParameter("@outDescripcion", SqlDbType.NVarChar, -1);
                outputDescripcion.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputDescripcion);

                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                descripcion = outputDescripcion.Value?.ToString();
            }

            return descripcion;
        }

        public Employee ObtenerEmpleadoPorNombreYDocumento(string nombre, int valorDocumento)
        {
            Employee empleado = null;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerEmpleadoPorNombreYDocumento", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNombre", nombre);
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumento);

                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    //empleado = new Employee();
                }

                reader.Close();
                conn.Close();
            }

            return empleado;
        }

        public int ObtenerIdPuestoPorNombre(string nombrePuesto)
        {
            int idPuesto = -1;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerIdPuestoPorNombre", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@inNombrePuesto", nombrePuesto);

                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                conn.Open();
                object result = cmd.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int id))
                {
                    idPuesto = id;
                }

                conn.Close();
            }

            return idPuesto;
        }

        public List<string> ObtenerTiposMovimiento()
        {
            List<string> tiposMovimiento = new List<string>();

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerTiposMovimiento", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter outParam = new SqlParameter("@outResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outParam);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    tiposMovimiento.Add($"[{reader["Id"].ToString()}] {reader["Nombre"].ToString()}");
                }
            }

            return tiposMovimiento;
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
                            Estado: row["Estado"].ToString()
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
    }
}


