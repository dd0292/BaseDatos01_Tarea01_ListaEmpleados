using BaseDatos01_Tarea01_ListaEmpleados.Models;
using BaseDatos01_Tarea01_ListaEmpleados.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BaseDatos01_Tarea01_ListaEmpleados.DAL // Data Access Layer
{
    public class _DAL
    {
        System.Web.HttpContext context = System.Web.HttpContext.Current;

        string conString = ConfigurationManager.ConnectionStrings["adoConnectionString"].ToString();
        DatabaseHelper dbHelper;

        int UserId;
        string ClientIp = "192.168.18.7";

        public _DAL()
        {
            ClientIp = (context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? context.Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();
            UserId = Convert.ToInt32(HttpContext.Current.Session["CurrentUserId"]);
            dbHelper = new DatabaseHelper(conString);
        }

        public int getUseId() { 
            return this.UserId;
        }
        //-----------------------------------------------------------------------------------------

        // Modifica solo el método ObtenerPlanillaSemanal así:
        public PlanillaSem ObtenerPlanillaSemanal(int idEmpleado, int? idSemana, ref int outResultCode, ref string outMessage)
        {
            var planilla = new PlanillaSem();
            outResultCode = 0;
            outMessage = "OK";

            try
            {
                using (var connection = new SqlConnection(conString))
                using (var command = new SqlCommand("sp_ObtenerPlanillaSemanalEmpleado", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros
                    command.Parameters.Add("@IdEmpleado", SqlDbType.Int).Value = idEmpleado;
                    command.Parameters.Add("@IdSemana", SqlDbType.Int).Value = idSemana ?? (object)DBNull.Value;

                    // Parámetros de salida
                    command.Parameters.Add(new SqlParameter("@CodigoResultado", SqlDbType.Int) { Direction = ParameterDirection.Output });
                    command.Parameters.Add(new SqlParameter("@MensajeResultado", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        // 1. Datos básicos del empleado
                        if (reader.Read())
                        {
                            planilla.EmpleadoId = reader.GetInt32(reader.GetOrdinal("EmpleadoId"));
                            planilla.NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto"));
                            planilla.Puesto = reader.GetString(reader.GetOrdinal("Puesto"));
                            planilla.SalarioHora = reader.GetDecimal(reader.GetOrdinal("SalarioHora"));
                        }

                        // 2. Datos de la semana
                        if (reader.NextResult() && reader.Read())
                        {
                            planilla.Semana.SemanaId = reader.GetInt32(reader.GetOrdinal("SemanaId"));
                            planilla.Semana.FechaInicio = reader.GetDateTime(reader.GetOrdinal("FechaInicio"));
                            planilla.Semana.FechaFin = reader.GetDateTime(reader.GetOrdinal("FechaFin"));
                            planilla.Semana.Cerrada = reader.GetBoolean(reader.GetOrdinal("Cerrada"));
                            planilla.Semana.SalarioBruto = reader.GetDecimal(reader.GetOrdinal("SalarioBruto"));
                            planilla.Semana.Descuentos = reader.GetDecimal(reader.GetOrdinal("Descuentos"));
                            planilla.Semana.MotivoDescuento = reader.IsDBNull(reader.GetOrdinal("MotivoDescuento")) ? null : reader.GetString(reader.GetOrdinal("MotivoDescuento"));
                            planilla.Semana.SalarioNeto = reader.GetDecimal(reader.GetOrdinal("SalarioNeto"));
                        }

                        // 3. Movimientos por día (ajustado a tu modelo)
                        if (reader.NextResult())
                        {
                            while (reader.Read())
                            {
                                var movimiento = new MovimientoDia
                                {
                                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                                    TipoMovimiento = reader.GetString(reader.GetOrdinal("TipoMovimiento")),
                                    Categoria = reader.GetString(reader.GetOrdinal("Categoria")),
                                    Horas = reader.GetDecimal(reader.GetOrdinal("Horas")),
                                    Monto = reader.GetDecimal(reader.GetOrdinal("Monto")),
                                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? string.Empty : reader.GetString(reader.GetOrdinal("Descripcion"))
                                };

                                // Asignación optimizada a tu array de 7 días
                                int diaSemana = (int)movimiento.Fecha.DayOfWeek;
                                int indexDia = (diaSemana + 6) % 7; // Ajuste para Lunes=0, Domingo=6
                                planilla.Semana = new SemanaPlanilla()
                                {
                                    MovimientosPorDia = new List<MovimientoDia>[7]
                                };
                                if (indexDia >= 0 && indexDia < 7)
                                {
                                    planilla.Semana.MovimientosPorDia[indexDia].Add(movimiento);
                                }
                                else
                                {
                                    Debug.WriteLine($"[WARNING] Índice de día inválido: {indexDia} para fecha {movimiento.Fecha}");
                                }
                            }
                        }
                    }

                    // Obtener resultados del SP
                    outResultCode = Convert.ToInt32(command.Parameters["@CodigoResultado"].Value);
                    outMessage = command.Parameters["@MensajeResultado"].Value?.ToString() ?? "OK";
                }
            }
            catch (SqlException ex)
            {
                outResultCode = ex.Number;
                outMessage = $"Error de base de datos: {ex.Message}";
                Debug.WriteLine($"[SQL ERROR] {ex}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"StackTrace completo: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Debug.WriteLine($"InnerException: {ex.InnerException}");
                }
            }

            return outResultCode == 0 ? planilla : null;
        }

        public PlanillaMensualViewModel ObtenerPlanillaMensual(int idEmpleado, int? idMes)
        {
            var model = new PlanillaMensualViewModel();
            int outCode = 0;
            string outMessage = "";

            using (var connection = new SqlConnection(conString))
            using (var command = new SqlCommand("sp_ObtenerPlanillaMensualEmpleado", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IdEmpleado", idEmpleado);
                command.Parameters.AddWithValue("@IdMes", idMes ?? (object)DBNull.Value);
                command.Parameters.Add(new SqlParameter("@CodigoResultado", SqlDbType.Int) { Direction = ParameterDirection.Output });
                command.Parameters.Add(new SqlParameter("@MensajeResultado", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output });

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    // 1. Datos del empleado
                    if (reader.Read())
                    {
                        model.NombreCompleto = reader["NombreCompleto"].ToString();
                        model.Puesto = reader["Puesto"].ToString();
                        model.SalarioBase = Convert.ToDecimal(reader["SalarioBase"]);
                    }

                    // 2. Datos del mes
                    if (reader.NextResult() && reader.Read())
                    {
                        model.MesActual = new MesPlanilla
                        {
                            NombreMes = reader["NombreMes"].ToString(),
                            Anio = Convert.ToInt32(reader["Anio"]),
                            FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                            FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                            Cerrado = Convert.ToBoolean(reader["Cerrado"]),
                            SalarioBruto = Convert.ToDecimal(reader["SalarioBruto"]),
                            TotalDeducciones = Convert.ToDecimal(reader["TotalDeducciones"]),
                            SalarioNeto = Convert.ToDecimal(reader["SalarioNeto"]),
                            TotalHorasNormales = Convert.ToDecimal(reader["TotalHorasNormales"]),
                            TotalHorasExtras = Convert.ToDecimal(reader["TotalHorasExtras"])
                        };
                    }

                    // 3. Semanas (mapeo a SemanaPlanillaResumen)
                    if (reader.NextResult())
                    {
                        model.Semanas = new List<SemanaPlanillaResumen>();
                        while (reader.Read())
                        {
                            model.Semanas.Add(new SemanaPlanillaResumen
                            {
                                NumeroSemana = Convert.ToInt32(reader["NumeroSemana"]),
                                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
                                DiasTrabajados = Convert.ToInt32(reader["DiasTrabajados"]),
                                HorasNormales = Convert.ToDecimal(reader["HorasNormales"]),
                                HorasExtras = Convert.ToDecimal(reader["HorasExtras"]),
                                SalarioBruto = Convert.ToDecimal(reader["SalarioBruto"]),
                                SalarioNeto = Convert.ToDecimal(reader["SalarioNeto"])
                            });
                        }
                    }

                    // 4. Deducciones
                    if (reader.NextResult())
                    {
                        model.Deducciones = new List<Deduccion>();
                        while (reader.Read())
                        {
                            model.Deducciones.Add(new Deduccion
                            {
                                Tipo = reader["Tipo"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Monto = Convert.ToDecimal(reader["Monto"]),
                                Obligatorio = Convert.ToBoolean(reader["Obligatorio"])
                            });
                        }
                    }
                }

                outCode = Convert.ToInt32(command.Parameters["@CodigoResultado"].Value);
                outMessage = command.Parameters["@MensajeResultado"].Value?.ToString();

                if (outCode != 0)
                {
                    throw new Exception(outMessage);
                }
            }

            return model;
        }

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
                    parameters,
                    ref outResultCode,
                    ref outResultDescription);

                if (outResultCode == 0)
                {
                    Console.WriteLine($"Tablas catalogo insertadas correctamente. Filas afectadas: {rowsAffected}");
                }
                else
                {
                    Console.WriteLine($"Error al insertar tablas catalogo: {outResultDescription}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al insertar tablas catalogo: {ex.Message}");

            }

            return outResultCode;
        }

        public async Task<(int ResultCode, string ResultDescription)> CargarOperacionesXMLAsync(string xml)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@XmlOperacion", SqlDbType.Xml) { Value = xml },
                    new SqlParameter("@inIdUsuario", SqlDbType.Int) { Value = UserId },
                    new SqlParameter("@inUserIP", SqlDbType.VarChar, 64) { Value = ClientIp },

                    new SqlParameter("@outResultCode", SqlDbType.Int) { Direction = ParameterDirection.Output },
                    new SqlParameter("@outResultDescription", SqlDbType.VarChar, 529) { Direction = ParameterDirection.Output }
                };

                var result = await dbHelper.ExecuteNonQueryStoredProcedureAsync(
                    "sp_CargarOperacionesXML",
                    parameters,
                    180); 

                if (result.outResultCode == 0)
                {
                    Console.WriteLine($"CargarOperacionesXML completado");
                }
                else
                {
                    Console.WriteLine($"Error al cargar operaciones: {result.outResultDescription}");
                }

                return (result.outResultCode, result.outResultDescription);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción en CargarOperacionesXML: {ex.Message}");
                return (-1, ex.Message);
            }
        }

        public int RegistrarEvento(int IdTipoEvento, string info, ref int outResultCode, ref string outResultDescription)
        {

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@inIdUsuario", SqlDbType.Int) {Value = UserId, Direction = ParameterDirection.Input},
                    new SqlParameter("@inIdTipoEvento", SqlDbType.Int) {Value = IdTipoEvento, Direction = ParameterDirection.Input},
                    new SqlParameter("@inIPUser", SqlDbType.VarChar,64) {Value = ClientIp, Direction = ParameterDirection.Input},
                    new SqlParameter("@inDescripcion", SqlDbType.VarChar) {Value = info, Direction = ParameterDirection.Input},
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


