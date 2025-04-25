using System;
using System.Collections.Generic;
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

        protected void Login1_Authenticate(object sender, AuthenticateEventArgs e)
        {
            string username = Login1.UserName;
            string password = Login1.Password;

            string connectionString = "Server=mssql-196050-0.cloudclusters.net,10264;Database=Empleado;User Id=Fabricio;Password=Tareabd2;TrustServerCertificate=true;";
            string query = "SELECT Id, Username FROM Usuarios WHERE Username = @Username AND Password = @Password";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // ¡En un caso real usa hashing!

                    try
                    {
                        con.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            e.Authenticated = true;

                            // Guardamos datos importantes en Session
                            Session["UserId"] = reader["Id"];
                            Session["Username"] = username;
                           // Session["NombreCompleto"] = reader["Nombre"];
                           // Session["Rol"] = reader["Rol"];

                            // Ejemplo adicional: hora de login
                            Session["LastLogin"] = DateTime.Now;
                            Response.Redirect("~/Home/Index");
                        }
                        else
                        {
                            e.Authenticated = false;
                        }
                    }
                    catch (SqlException ex)
                    {

                        Console.WriteLine("Error SQL: " + ex.Message);
                    }
                }
            }
        }
    }
}
    
