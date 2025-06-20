using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL
{
	public class DALBeforeLogIn
	{
        System.Web.HttpContext context = System.Web.HttpContext.Current;
        string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();
        string ClientIp = "592.178.18.7";

        public DALBeforeLogIn()
        {
            ClientIp = (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                   context.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
        }

       

        public int PasswordNoExiste(int intentos, string username)
        {
            int resultado;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("PasswordNoExiste", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inNumeroIntentos", intentos);
                cmd.Parameters.AddWithValue("@inUserName", username);
                cmd.Parameters.AddWithValue("@inPostInIP", ClientIp);

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

        public int LoginDeshabilitado(string username)
        {
            int resultado;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("LoginDeshabilitado", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inUserName", username);
                cmd.Parameters.AddWithValue("@inPostInIP", ClientIp);

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
        public DataTable ValidateUser(string username, string password)
        {
            DataTable resultTable = new DataTable();

            using (SqlConnection con = new SqlConnection(conString))
            using (SqlCommand cmd = new SqlCommand("ValidarUsuario", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                cmd.Parameters.AddWithValue("@inUsername", username);
                cmd.Parameters.AddWithValue("@inPassword", password);

                // Parámetros de salida (completos)
                cmd.Parameters.Add(new SqlParameter("@outUserId", SqlDbType.Int) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@outUserType", SqlDbType.Int) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@outEmployeeId", SqlDbType.Int) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@outNombreEmpleado", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
                cmd.Parameters.Add(new SqlParameter("@outResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output });

                con.Open();
                cmd.ExecuteNonQuery();

                // Crear tabla de resultados
                resultTable.Columns.Add("UserId", typeof(int));
                resultTable.Columns.Add("UserType", typeof(int));
                resultTable.Columns.Add("EmployeeId", typeof(int));
                resultTable.Columns.Add("NombreEmpleado", typeof(string));
                resultTable.Columns.Add("ResultCode", typeof(int));

                resultTable.Rows.Add(
                    cmd.Parameters["@outUserId"].Value,
                    cmd.Parameters["@outUserType"].Value,
                    cmd.Parameters["@outEmployeeId"].Value,
                    cmd.Parameters["@outNombreEmpleado"].Value,
                    cmd.Parameters["@outResultCode"].Value
                );

                return resultTable;
            }
        }

    }
}