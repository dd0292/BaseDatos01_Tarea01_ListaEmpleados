using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL // Data Access Layer
{
	public class _DAL
	{
        System.Web.HttpContext context = System.Web.HttpContext.Current;

        string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();
        string UserName = HttpContext.Current.Session["Username"].ToString();
        string UserId = HttpContext.Current.Session["UserId"].ToString();
        string ClientIp = "192.168.18.7"; 

        public _DAL() 
        {
            ClientIp = (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                   context.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
        }


        public int ActualizarEmpleado(string nombreAntiguo, int docAntiguo, Employee nuevoEmpleado, int idPuesto)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ActualizarEmpleado", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNombre", nombreAntiguo);
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", docAntiguo);
                cmd.Parameters.AddWithValue("@inNuevoNombre", nuevoEmpleado.Nombre);
                cmd.Parameters.AddWithValue("@inNuevoValorDocumentoIdentidad", nuevoEmpleado.ValorDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@inIdPuesto", idPuesto);
                cmd.Parameters.AddWithValue("@inUserName", UserName);
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

        public int EliminarEmpleadoLogicamente(string nombre, int valorDocumento)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("EliminarEmpleadoLogicamente", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNombre", nombre);
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumento);
                cmd.Parameters.AddWithValue("@inUserName", UserName);
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

        public int EliminarEmpleadoCancelar(string nombre, int valorDocumento)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("dbo.EliminarEmpleadoCancelar", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNombre", nombre);
                cmd.Parameters.AddWithValue("@inValorDocumentoIdentidad", valorDocumento);
                cmd.Parameters.AddWithValue("@inUserName", UserName);
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

        public List<Employee> FiltrarEmpleados(string filtro)
        {
            List<Employee> list = new List<Employee>();

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("FiltrarEmpleados", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                int i = 0;
                cmd.Parameters.AddWithValue("@inFiltro", filtro ?? "");
                cmd.Parameters.AddWithValue("@inEsNumero", int.TryParse(filtro, out i));

                cmd.Parameters.AddWithValue("@inUserName", UserName);
                cmd.Parameters.AddWithValue("@inIdPostByUser", UserId);
                cmd.Parameters.AddWithValue("@inPostInIP", ClientIp);

                SqlParameter outParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outParam);

                SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
                DataTable dataTableEmployees = new DataTable();

                connection.Open();
                sqlData.Fill(dataTableEmployees);
                connection.Close();

                foreach (DataRow dr in dataTableEmployees.Rows)
                {
                    list.Add(new Employee
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        ValorDocumentoIdentidad = Convert.ToInt32(dr["ValorDocumentoIdentidad"]),
                        Nombre = dr["Nombre"].ToString(),
                    });
                }

            }

            return list;
        }

        public int InsertarEmpleado(Employee employee, string puestoNombre)
        {
            int resultado;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("InsertarEmpleado", connection);
                cmd.CommandType = CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue("@Nombre", employee.Nombre);
                cmd.Parameters.AddWithValue("@ValorDocumentoIdentidad", employee.ValorDocumentoIdentidad);
                

                int puestoId = ObtenerIdPuestoPorNombre(puestoNombre);
                cmd.Parameters.AddWithValue("@IdPuesto", puestoId);


                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                connection.Open();
                cmd.ExecuteNonQuery(); 
                resultado = Convert.ToInt32(cmd.Parameters["@outResultCode"].Value);
                connection.Close();
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
                            PostTime = Convert.ToDateTime(reader["PostTime"])
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

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    empleado = new Employee
                    {
                        Nombre = reader["Nombre"].ToString(),
                        ValorDocumentoIdentidad = Convert.ToInt32(reader["ValorDocumentoIdentidad"]),
                        SaldoVacaciones = Convert.ToInt32(reader["SaldoVacaciones"]),
                        FechaContratacion = reader["FechaContratacion"].ToString(),
                        NombrePuesto = reader["NombrePuesto"].ToString()
                    };
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
                cmd.Parameters.AddWithValue("@NombrePuesto", nombrePuesto);

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

        public List<string> ObtenerListaPuestos()
        {
            List<string> puestos = new List<string>();

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerListaPuestos", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    puestos.Add(
                        reader["Nombre"].ToString()
                    );
                }

                reader.Close();
                conn.Close();
            }

            return puestos;
        }

    }
}


