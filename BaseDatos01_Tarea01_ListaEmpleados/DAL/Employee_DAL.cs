using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL
{
	public class Employee_DAL
	{
		string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();

		// Get Employees List
		public List<Employee> GetEmployeesList(string filtro) 
		{
			List<Employee> list = new List<Employee>();

			using (SqlConnection connection = new SqlConnection(conString)) 
			{
                SqlCommand cmd = new SqlCommand("FiltrarEmpleados", connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Filtro", filtro ?? "");

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

        // Add Into Employees List
        public int InsertEmployee(Employee employee, string puestoNombre)
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
        
        public string GetErrorFromCode(int code)
        {
            string descripcion;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerDescripcionError", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetro de entrada
                cmd.Parameters.AddWithValue("@Codigo", code);

                // Parámetro de salida
                SqlParameter outputDescripcion = new SqlParameter("@Descripcion", SqlDbType.NVarChar, -1);
                outputDescripcion.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputDescripcion);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();

                descripcion = outputDescripcion.Value?.ToString();
            }

            return descripcion;
        }

        public List<string> GetJobList()
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

        public Employee ObtenerEmpleadoPorNombreYDocumento(string nombre, int valorDocumento)
        {
            Employee empleado = null;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ObtenerEmpleadoPorNombreYDocumento", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@ValorDocumentoIdentidad", valorDocumento);

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


        public int EliminarEmpleadoLogicamente(string nombre, int valorDocumento)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("EliminarEmpleadoLogicamente", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", nombre);
                cmd.Parameters.AddWithValue("@ValorDocumentoIdentidad", valorDocumento);

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


        public int ActualizarEmpleado(string nombreAntiguo, int docAntiguo, Employee nuevoEmpleado, int idPuesto)
        {
            int resultCode;

            using (SqlConnection conn = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ActualizarEmpleado", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Nombre", nombreAntiguo);
                cmd.Parameters.AddWithValue("@ValorDocumentoIdentidad", docAntiguo);
                cmd.Parameters.AddWithValue("@NuevoNombre", nuevoEmpleado.Nombre);
                cmd.Parameters.AddWithValue("@NuevoValorDocumentoIdentidad", nuevoEmpleado.ValorDocumentoIdentidad);
                cmd.Parameters.AddWithValue("@IdPuesto", idPuesto);

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

    }
}


