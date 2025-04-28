using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

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

        public int LoginExitoso(string username, int id)
        {
            int resultado;

            using (SqlConnection connection = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("LoginExitoso", connection);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inUserName", username);
                cmd.Parameters.AddWithValue("@inIdPostByUser", id);
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

    }
}