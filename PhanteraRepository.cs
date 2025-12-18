using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace Interfaz_BMolecultar_IG
{
    public class PhanteraRepository
    {
        private readonly AppConfigModel _config;

        public PhanteraRepository(AppConfigModel config)
        {
            _config = config;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(SqlDataAccess.BuildConnectionString(
                _config.ServidorSQL,
                _config.NombreBaseDatos,
                _config.UsuarioSQL,
                _config.ContrasenaSQL
            ));
        }

        /// <summary>
        /// Busca el ID interno de la orden (o_id) basándose en el código de barras (o_numero).
        /// </summary>
        public int? BuscarOrdenPorCodigoBarras(string codigoBarras)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT o_id FROM Ordenes WHERE o_numero = @Barcode";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // VarChar explícito para preservar ceros a la izquierda
                        SqlParameter param = new SqlParameter("@Barcode", SqlDbType.VarChar, 16);
                        param.Value = codigoBarras;
                        cmd.Parameters.Add(param);

                        object result = cmd.ExecuteScalar();

                        if (result != null && int.TryParse(result.ToString(), out int oId))
                        {
                            return oId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogError(ex, $"Error al buscar la orden '{codigoBarras}' en SQL.");
                }
            }
            return null;
        }

        /// <summary>
        /// ACTUALIZA el resultado en la tabla Laboratorios si la prueba ya existe.
        /// </summary>
        /// <returns>True si se actualizó una fila. False si la prueba no existía para esa orden.</returns>
        public bool ActualizarResultado(int ordenId, string resultado)
        {
            using (SqlConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();

                    // LÓGICA UPDATE PURO:
                    // Solo actualizamos si ya existe la fila con ese l_ord_id y l_pru_id.
                    // l_estado = 2 (Validado/Terminado)
                    // l_fecha_mod = GETDATE() (Fecha/Hora actual del servidor SQL)
                    string query = @"
                        UPDATE Laboratorios
                        SET 
                            l_resultado = @Resultado,
                            l_fecha_mod = GETDATE(),
                            l_estado = 2,
                            l_ins_id = @InsId
                        WHERE 
                            l_ord_id = @OrdId 
                            AND l_pru_id = @PruId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Resultado", resultado);
                        cmd.Parameters.AddWithValue("@InsId", _config.IdEquipo);
                        cmd.Parameters.AddWithValue("@OrdId", ordenId);
                        cmd.Parameters.AddWithValue("@PruId", 2219); // Aquí va el 2219 (configurado en el JSON)

                        // ExecuteNonQuery devuelve el número de filas afectadas.
                        int filasAfectadas = cmd.ExecuteNonQuery();

                        // Si es > 0, significa que encontró la prueba y la actualizó.
                        // Si es 0, significa que esa orden NO tenía esa prueba pedida.
                        return filasAfectadas > 0;
                    }
                }
                catch (Exception ex)
                {
                    AppLogger.LogError(ex, $"Error SQL al actualizar resultado para Orden {ordenId}.");
                    return false;
                }
            }
        }
    }
}