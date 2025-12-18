using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory; // Paquete Serilog.Sinks.InMemory
using System;
using System.Collections.Generic;
using System.Linq; // Necesario para Skip() y ToList()

namespace Interfaz_BMolecultar_IG
{
    // Clase auxiliar para transferir la información de log al MainForm
    public class LogEntry
    {
        public string Message { get; set; }
        public LogEventLevel Level { get; set; }
    }

    /// <summary>
    /// Gestiona la instancia global de Serilog y proporciona un buffer en memoria
    /// para que el MainForm pueda leer los mensajes nuevos.
    /// </summary>
    public static class AppLogger
    {
        private static readonly InMemorySink _logBuffer;
        public static ILogger Log { get; }

        // Variable para rastrear el número de logs ya leídos (último índice)
        private static int _lastReadIndex = 0;

        static AppLogger()
        {
            // Instancia del sink en memoria
            _logBuffer = new InMemorySink();

            // Configuración del Logger de Serilog
            Log = new LoggerConfiguration()
                .MinimumLevel.Debug()
                // Escribe en nuestro buffer en memoria
                .WriteTo.Sink(_logBuffer)
                .CreateLogger();
        }

        /// <summary>
        /// Obtiene los mensajes de log nuevos (no leídos) desde el buffer.
        /// Este método usa indexación para evitar mostrar logs duplicados.
        /// </summary>
        public static List<LogEntry> GetNewLogMessages()
        {
            List<LogEntry> entries = new List<LogEntry>();

            // Obtener todos los eventos actualmente en el buffer
            var allEvents = _logBuffer.LogEvents;

            // Determinar cuántos mensajes nuevos hay basándose en el índice de lectura
            int newEventsCount = allEvents.Count() - _lastReadIndex;

            if (newEventsCount <= 0)
            {
                return entries; // No hay mensajes nuevos.
            }

            // Iterar solo sobre los mensajes nuevos usando Skip() para saltar los ya procesados.
            // Usamos ToList() para asegurar que la colección no cambie mientras iteramos.
            foreach (LogEvent logEvent in allEvents.Skip(_lastReadIndex).ToList())
            {
                // Formatear el mensaje: [HH:mm:ss] [NIVEL] Mensaje
                string formattedMessage = $"{logEvent.Timestamp.ToLocalTime():HH:mm:ss} [{logEvent.Level}] {logEvent.RenderMessage()}";

                entries.Add(new LogEntry
                {
                    Message = formattedMessage,
                    Level = logEvent.Level
                });
            }

            // ACTUALIZAR el índice al número total de logs leídos.
            _lastReadIndex = allEvents.Count();

            return entries;
        }

        /// <summary>
        /// Registra un mensaje de error con información de excepción.
        /// </summary>
        public static void LogError(Exception ex, string message)
        {
            // Usa el logger estándar de Serilog.
            Log.Error(ex, message);
        }

        /// <summary>
        /// Registra un mensaje de información.
        /// </summary>
        public static void LogInformation(string message)
        {
            Log.Information(message);
        }

        /// <summary>
        /// Registra un mensaje de advertencia.
        /// </summary>
        public static void LogWarning(string message)
        {
            Log.Warning(message);
        }
    }
}