using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaz_BMolecultar_IG
{
    public class AppConfigModel
    {
        // ----------------------------------------------------
        // SECCIÓN 1: CONFIGURACIÓN DE ARCHIVOS CSV (Phantera)
        // ----------------------------------------------------

        // [JsonPropertyName("RutaCSV")] // Se puede usar si se quiere cambiar el nombre en el JSON
        public string RutaCarpetaCSV { get; set; } = string.Empty;


        // ----------------------------------------------------
        // SECCIÓN 2: CONEXIÓN SQL SERVER
        // ----------------------------------------------------

        public string ServidorSQL { get; set; } = string.Empty;
        public string NombreBaseDatos { get; set; } = string.Empty;
        public string UsuarioSQL { get; set; } = string.Empty;
        public string ContrasenaSQL { get; set; } = string.Empty;
        public string NombreTablaDestino { get; set; } = string.Empty;
        // El ID numérico de la prueba en tu base de datos (l_pru_id)
        public int IdPruebaDefault { get; set; } = 0;
        // El ID del instrumento Phantera en tu base de datos (l_ins_id)
        public int IdEquipo { get; set; } = 1;
        // ----------------------------------------------------
        // SECCIÓN 3: PARÁMETROS DE EJECUCIÓN (Ejemplo futuro)
        // ----------------------------------------------------

        // Aunque usaremos FileSystemWatcher, es bueno tener un valor por defecto.
        // El tiempo está en milisegundos. 30000ms = 30 segundos
        public int IntervaloChequeoMs { get; set; } = 30000;
    }
}
