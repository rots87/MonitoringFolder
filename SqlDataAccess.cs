using Microsoft.Data.SqlClient;
using System;
using System.Data;

namespace Interfaz_BMolecultar_IG
{
    /// <summary>
    /// Capa de Acceso a Datos (DAL).
    /// Gestiona la construcción de cadenas de conexión y la validación de conectividad con SQL Server.
    /// </summary>
    public class SqlDataAccess
    {
        /// <summary>
        /// Construye una cadena de conexión segura y estandarizada utilizando SqlConnectionStringBuilder.
        /// </summary>
        public static string BuildConnectionString(string server, string database, string userId, string password)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                UserID = userId,
                Password = password,

                // Configuraciones de seguridad y rendimiento
                Encrypt = true,
                TrustServerCertificate = true,
                ConnectTimeout = 15 // Timeout corto para pruebas de conectividad rápidas
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Valida la conectividad con el servidor de base de datos utilizando las credenciales proporcionadas.
        /// </summary>
        /// <param name="errorMessage">Salida del detalle del error en caso de fallo.</param>
        /// <returns>True si la conexión es exitosa, False si falla.</returns>
        public static bool TestConnection(string server, string database, string userId, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            string connectionString = BuildConnectionString(server, database, userId, password);

            // El bloque 'using' garantiza el cierre y liberación de la conexión (Connection Pooling)
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Registro de auditoría positivo
                    AppLogger.LogInformation("Validación de conectividad SQL: EXITOSA.");
                    return true;
                }
                catch (SqlException ex)
                {
                    // Registro de error específico de base de datos
                    AppLogger.LogError(ex, $"Fallo de conexión SQL (Error {ex.Number}).");
                    errorMessage = $"Error SQL ({ex.Number}): {ex.Message}";
                    return false;
                }
                catch (Exception ex)
                {
                    // Registro de error general de infraestructura
                    AppLogger.LogError(ex, "Fallo de conexión SQL (Error General).");
                    errorMessage = $"Error desconocido: {ex.Message}";
                    return false;
                }
            }
        }
    }
}