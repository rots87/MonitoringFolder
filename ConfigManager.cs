using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Interfaz_BMolecultar_IG
{
    public static class ConfigManager
    {
        // Nombre del archivo de configuración (será creado en la carpeta de la aplicación)
        private static readonly string ConfigFileName = "appsettings.json";

        // -----------------------------------------------------------
        // 1. GUARDAR CONFIGURACIÓN (Serialización: Objeto C# -> JSON)
        // -----------------------------------------------------------
        /// <summary>
        /// Guarda el objeto de configuración en un archivo JSON en la carpeta de la aplicación.
        /// </summary>
        public static void SaveConfig(AppConfigModel config)
        {
            try
            {
                // Usamos JsonSerializer.Serialize para convertir el objeto a una cadena JSON.
                // Usamos opciones para formatear el JSON de manera legible (con indentación).
                var options = new JsonSerializerOptions { WriteIndented = true };
                string jsonString = JsonSerializer.Serialize(config, options);

                // Escribimos la cadena en el archivo.
                File.WriteAllText(ConfigFileName, jsonString);
            }
            catch (Exception ex)
            {
                // En un sistema real, aquí usaríamos nuestra clase de logging (Serilog).
                MessageBox.Show($"Error al guardar la configuración: {ex.Message}", "Error de Guardado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Si la aplicación no puede guardar la configuración, esto es un error crítico.
            }
        }

        // -----------------------------------------------------------
        // 2. CARGAR CONFIGURACIÓN (Deserialización: JSON -> Objeto C#)
        // -----------------------------------------------------------
        /// <summary>
        /// Carga la configuración desde el archivo JSON. Si no existe, devuelve una nueva instancia.
        /// </summary>
        public static AppConfigModel LoadConfig()
        {
            if (File.Exists(ConfigFileName))
            {
                try
                {
                    // Leemos todo el contenido del archivo.
                    string jsonString = File.ReadAllText(ConfigFileName);

                    // Deserializamos el JSON de vuelta al objeto AppConfigModel.
                    // Si la deserialización falla (ej. formato corrupto), devolveremos un modelo nuevo.
                    AppConfigModel config = JsonSerializer.Deserialize<AppConfigModel>(jsonString);

                    // Si 'config' es null (la deserialización falla), devolvemos un nuevo modelo.
                    return config ?? new AppConfigModel();
                }
                catch (Exception ex)
                {
                    // Error: El archivo existe pero está corrupto. Creamos uno nuevo.
                    MessageBox.Show($"El archivo de configuración JSON está corrupto. Se cargará una configuración nueva. Error: {ex.Message}", "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return new AppConfigModel();
                }
            }
            else
            {
                // El archivo no existe, devolvemos un modelo con los valores por defecto.
                return new AppConfigModel();
            }
        }
    }
}