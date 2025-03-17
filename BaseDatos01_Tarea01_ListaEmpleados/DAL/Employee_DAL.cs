using BaseDatos01_Tarea01_ListaEmpleados.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL
{
	public class Employee_DAL
	{
		string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();

		// Get Employees List
		public List<Employee> GetEmployeesList() 
		{
			List<Employee> list = new List<Employee>();

			using (SqlConnection connection = new SqlConnection(conString)) 
			{
				SqlCommand cmd = connection.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ObtenerEmpleados_Alfabético";
				SqlDataAdapter sqlData = new SqlDataAdapter(cmd);
				DataTable dataTableEmployees = new DataTable();

				connection.Open();
				sqlData.Fill(dataTableEmployees);
				connection.Close();

				foreach (DataRow dr in dataTableEmployees.Rows) 
				{
					list.Add(new Employee
					{
						id = Convert.ToInt32(dr["id"]),
						Nombre = dr["Nombre"].ToString(),
						Salario = Convert.ToDecimal(dr["Salario"])
					});
				}

            }

			return list;
		}


        // Add Into Employees List
        public bool InsertEmployee(Employee employee)
        {
            int resultado;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("InsertarEmpleado", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@NombreVar", employee.Nombre);
                cmd.Parameters.AddWithValue("@SalarioVar", employee.Salario);

                SqlParameter outputParam = new SqlParameter("@Resultado", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                connection.Open();
                cmd.ExecuteNonQuery(); 
                resultado = Convert.ToInt32(cmd.Parameters["@Resultado"].Value);
                connection.Close();
            }

            return resultado == 0;
        }
    }
}


