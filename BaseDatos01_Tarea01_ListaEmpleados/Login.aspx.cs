using BaseDatos01_Tarea01_ListaEmpleados.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BaseDatos01_Tarea01_ListaEmpleados
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void IncrementFailedAttempts(string username)
        {
            string key = $"FailedAttempts_{username}";

            if (Application[key] == null)
            {
                Application[key] = 1;
            }
            else
            {
                int currentAttempts = (int)Application[key];
                Application[key] = currentAttempts + 1;
            }

            Application[$"LastAttempt_{username}"] = DateTime.Now;
        }

        private int GetFailedAttempts(string username)
        {
            string key = $"FailedAttempts_{username}";
            return Application[key] == null ? 0 : (int)Application[key];
        }

        private void ResetFailedAttempts(string username)
        {
            string key = $"FailedAttempts_{username}";
            Application[key] = 0;
        }

        private bool IsUserLockedOut(string username)
        {
            int attempts = GetFailedAttempts(username);
            if (attempts >= 3)
            {
                object lastAttempt = Application[$"LastAttempt_{username}"]; 
                if (lastAttempt != null && lastAttempt is DateTime)
                {
                    DateTime lastAttemptTime = (DateTime)lastAttempt;
                    if ((DateTime.Now - lastAttemptTime).TotalMinutes < 5) // Bloqueo de 5 minutos
                    {
                        return true;
                    }
                    else
                    {
                        ResetFailedAttempts(username);
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            DALBeforeLogIn dal = new DALBeforeLogIn();
            string username = Login1.UserName;
            string password = Login1.Password;

            if (IsUserLockedOut(username))
            {
                Login1.FailureText = "Cuenta temporalmente bloqueada por demasiados intentos fallidos. Intente más tarde.";
                e.Authenticated = false;
                return;
            }
            string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();

            using (SqlConnection con = new SqlConnection(conString))
            {
                SqlCommand cmd = new SqlCommand("ValidarUsuario", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@inUsername", username);
                cmd.Parameters.AddWithValue("@inPassword", password);

                SqlParameter outputParam = new SqlParameter("@outResultCode", SqlDbType.Int);
                outputParam.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(outputParam);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        ResetFailedAttempts(username);
                        e.Authenticated = true;

                        // Guarda datos importantes en Session
                        Session["UserId"] = reader["Id"];
                        Session["Username"] = username;
                        Session["LastLogin"] = DateTime.Now;

                        dal.LoginExitoso(username, Convert.ToInt32(reader["Id"]));

                        Response.Redirect("~/Home/Index", false);
                        Context.ApplicationInstance.CompleteRequest(); 
                        return;
                    }
                    else
                    {
                        IncrementFailedAttempts(username);

                        int attempts = GetFailedAttempts(username);
                        int remainingAttempts = 3 - attempts;

                        if (remainingAttempts > 0)
                        {
                            Login1.FailureText = $"Usuario o contraseña incorrectos. Le quedan {remainingAttempts} intentos.";
                            dal.PasswordNoExiste(attempts, username);
                        }
                        else
                        {
                            Login1.FailureText = "Cuenta bloqueada por demasiados intentos fallidos. Intente más tarde.";
                            dal.LoginDeshabilitado(username);
                        }
                        e.Authenticated = false;
                    }
                }
                catch (SqlException ex)
                {

                    Console.WriteLine("Error SQL: " + ex.Message);
                    e.Authenticated = false;
                }
                
            }
        }
    }
}
