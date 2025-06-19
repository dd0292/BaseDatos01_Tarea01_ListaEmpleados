using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataTable ExecuteStoredProcedure(
            string procedureName,
            SqlParameter[] inputParameters,
            ref int outResultCode,
            ref string outResultDescription,
            CommandBehavior commandBehavior = CommandBehavior.CloseConnection)
        {
            DataTable resultTable = new DataTable();
            SqlConnection connection = null;
            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                outResultCode = -1;
                outResultDescription = "Error no especificado";

                connection = new SqlConnection(_connectionString);
                command = new SqlCommand(procedureName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (inputParameters != null)
                {
                    command.Parameters.AddRange(inputParameters);
                }
                command.Parameters.Add(new SqlParameter("@outResultCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                });
                command.Parameters.Add(new SqlParameter("@outResultDescription", SqlDbType.NVarChar, 529)
                {
                    Direction = ParameterDirection.Output
                });

                connection.Open();
                reader = command.ExecuteReader(CommandBehavior.CloseConnection);


                resultTable.Load(reader);
                reader.Close();

                outResultCode = command.Parameters["@outResultCode"].Value != DBNull.Value ?
                    Convert.ToInt32(command.Parameters["@outResultCode"].Value) : -1;

                outResultDescription = command.Parameters["@outResultDescription"].Value != DBNull.Value ?
                    Convert.ToString(command.Parameters["@outResultDescription"].Value) : "Error desconocido";

                return resultTable;
            }
            catch (SqlException sqlEx)
            {
                outResultCode = sqlEx.Number;
                outResultDescription = $"SQL Error [{sqlEx.Number}]: {sqlEx.Message}";

                CleanupResources(reader, command, connection);
                throw new ApplicationException(outResultDescription, sqlEx);
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                outResultDescription = $"General Error: {ex.Message}";

                CleanupResources(reader, command, connection);
                throw new ApplicationException(outResultDescription, ex);
            }
        }

        public int ExecuteNonQueryStoredProcedure(
            string procedureName,
            SqlParameter[] parameters,
            ref int outResultCode,
            ref string outResultDescription)
        {
            try { 
                outResultCode = -1;
                outResultDescription = "Error no especificado";

                using (SqlConnection connection = new SqlConnection(_connectionString))
                using (SqlCommand command = new SqlCommand(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    command.Parameters.Add(new SqlParameter("@outResultCode", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    });

                    command.Parameters.Add(new SqlParameter("@outResultDescription", SqlDbType.NVarChar, 529)
                    {
                        Direction = ParameterDirection.Output
                    });

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();

                    outResultCode = command.Parameters["@outResultCode"].Value != DBNull.Value ?
                        Convert.ToInt32(command.Parameters["@outResultCode"].Value) : -1;

                    outResultDescription = command.Parameters["@outResultDescription"].Value != DBNull.Value ?
                        Convert.ToString(command.Parameters["@outResultDescription"].Value) : "Error desconocido";
                    
                    return rowsAffected;
                }
            }
            catch (SqlException sqlEx)
            {
                outResultCode = sqlEx.Number;
                outResultDescription = $"SQL Error [{sqlEx.Number}]: {sqlEx.Message}";
                throw new ApplicationException(outResultDescription, sqlEx);
            }
            catch (Exception ex)
            {
                outResultCode = -1;
                outResultDescription = $"General Error: {ex.Message}";
                throw new ApplicationException(outResultDescription, ex);
            }
        }

        private void CleanupResources(SqlDataReader reader, SqlCommand command, SqlConnection connection)
        {
            try
            {
                reader?.Dispose();
                command?.Dispose();
                if (reader == null || reader.IsClosed)
                {
                    connection?.Close();
                    connection?.Dispose();
                }
            }
            catch
            {

            }
        }

    }
}