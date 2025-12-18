using System;
using System.IO;
using System.Threading;

namespace Interfaz_BMolecultar_IG
{
    public class MonitoringService
    {
        // ... (Variables existentes) ...
        private FileSystemWatcher _watcher;
        private readonly AppConfigModel _config;
        private bool _isRunning = false;

        // 1. NUEVO EVENTO: Para comunicar al MainForm qué archivo se está tocando
        // El string será el nombre del archivo.
        public event Action<string> OnProcesando;

        public MonitoringService(AppConfigModel config)
        {
            _config = config;
        }

        // ... (Métodos StartMonitoring y StopMonitoring quedan IGUALES) ...
        // ... (Solo asegúrate de que copias el resto del código que ya tenías) ...

        // Aquí pongo las partes que no cambian resumidas para no llenar la pantalla, 
        // PERO el cambio importante está en ProcesarArchivo:

        public bool IsRunning => _isRunning;

        public void StartMonitoring()
        {
            // ... (Tu código de StartMonitoring igual que antes) ...
            if (_isRunning) return;
            if (string.IsNullOrWhiteSpace(_config.RutaCarpetaCSV) || !Directory.Exists(_config.RutaCarpetaCSV))
            {
                AppLogger.LogError(null, $"Ruta inválida: {_config.RutaCarpetaCSV}");
                return;
            }

            try
            {
                // Barrido Inicial
                foreach (string archivo in Directory.GetFiles(_config.RutaCarpetaCSV, "*.csv"))
                {
                    ProcesarArchivo(archivo);
                }

                _watcher = new FileSystemWatcher(_config.RutaCarpetaCSV, "*.csv");
                _watcher.Created += (s, e) => ProcesarArchivo(e.FullPath);
                _watcher.EnableRaisingEvents = true;
                _isRunning = true;

                AppLogger.LogInformation($"Servicio ACTIVO. Vigilando: {_config.RutaCarpetaCSV}");
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex, "Error al iniciar servicio.");
                StopMonitoring();
            }
        }

        public void StopMonitoring()
        {
            // ... (Tu código de StopMonitoring igual que antes) ...
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                _watcher.Dispose();
                _watcher = null;
            }
            _isRunning = false;
            AppLogger.LogInformation("Servicio DETENIDO.");
        }

        private void ProcesarArchivo(string rutaArchivo)
        {
            // 2. DISPARAR EVENTO (INICIO)
            // Avisamos al MainForm que empezamos.
            string nombreArchivo = Path.GetFileName(rutaArchivo);
            OnProcesando?.Invoke(nombreArchivo);

            Thread.Sleep(500);
            bool moverAProcesados = false;

            int totalLeidos = 0;
            int totalActualizados = 0;
            int totalErrores = 0;

            try
            {
                PhanteraRepository repo = new PhanteraRepository(_config);
                string[] lineas = File.ReadAllLines(rutaArchivo);

                if (lineas.Length > 0)
                {
                    foreach (string linea in lineas)
                    {
                        // ... (Toda tu lógica de procesamiento que hicimos en el paso anterior) ...
                        // ... (Copiar y pegar la lógica del foreach, split, repo.ActualizarResultado, etc.) ...

                        if (string.IsNullOrWhiteSpace(linea)) continue;
                        totalLeidos++;
                        string[] partes = linea.Split(',');
                        if (partes.Length < 2) continue;
                        string codigoBarras = partes[0].Trim();
                        string resultado = partes[1].Trim();
                        int? ordenId = repo.BuscarOrdenPorCodigoBarras(codigoBarras);
                        if (ordenId.HasValue)
                        {
                            bool actualizado = repo.ActualizarResultado(ordenId.Value, resultado);
                            if (actualizado) { totalActualizados++; moverAProcesados = true; }
                            else { AppLogger.LogWarning($"[OMITIDO] {codigoBarras}: Prueba no pedida."); totalErrores++; moverAProcesados = true; }
                        }
                        else { AppLogger.LogWarning($"[NO ENCONTRADA] {codigoBarras}: No existe orden."); totalErrores++; }
                    }

                    // Reporte Resumen
                    if (totalActualizados > 0)
                    {
                        if (totalErrores == 0) AppLogger.LogInformation($"[CARGA EXITOSA] {nombreArchivo}: {totalActualizados} actualizados.");
                        else AppLogger.LogWarning($"[CARGA PARCIAL] {nombreArchivo}: {totalActualizados} OK, {totalErrores} Alertas.");
                    }
                    else if (totalErrores > 0) AppLogger.LogError(null, $"[CARGA FALLIDA] {nombreArchivo}: 0 OK, {totalErrores} errores.");
                }
                else
                {
                    AppLogger.LogWarning($"Archivo vacío: {nombreArchivo}");
                    moverAProcesados = true;
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex, $"Error procesando {nombreArchivo}");
                moverAProcesados = false;
            }
            finally
            {
                MoverArchivo(rutaArchivo, moverAProcesados);

                // 3. DISPARAR EVENTO (FIN)
                // Enviamos null para indicar que ya terminamos y estamos libres.
                OnProcesando?.Invoke(null);
            }
        }

        private void MoverArchivo(string rutaOriginal, bool exito)
        {
            // ... (Tu código de MoverArchivo igual que antes) ...
            try
            {
                FileInfo fi = new FileInfo(rutaOriginal);
                string carpeta = Path.Combine(fi.DirectoryName, exito ? "Procesados" : "Errores");
                if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);
                string destino = Path.Combine(carpeta, fi.Name);
                if (File.Exists(destino)) File.Delete(destino);
                File.Move(rutaOriginal, destino);
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex, $"No se pudo mover {Path.GetFileName(rutaOriginal)}");
            }
        }
    }
}