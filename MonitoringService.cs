using System;
using System.Collections.Generic; // Para Dictionary
using System.IO;
using System.Linq; // Para Select y Skip
using System.Threading;
using System.Globalization; // Para leer decimales

namespace Interfaz_BMolecultar_IG
{
    public class MonitoringService
    {
        private FileSystemWatcher _watcher;
        private readonly AppConfigModel _config;
        private bool _isRunning = false;

        // Evento para notificar a la UI
        public event Action<string> OnProcesando;

        public MonitoringService(AppConfigModel config)
        {
            _config = config;
        }

        public bool IsRunning => _isRunning;

        public void StartMonitoring()
        {
            if (_isRunning) return;
            if (string.IsNullOrWhiteSpace(_config.RutaCarpetaCSV) || !Directory.Exists(_config.RutaCarpetaCSV))
            {
                AppLogger.LogError(null, $"Ruta inválida: {_config.RutaCarpetaCSV}");
                return;
            }

            try
            {
                // Barrido Inicial (Archivos .tsv)
                foreach (string archivo in Directory.GetFiles(_config.RutaCarpetaCSV, "*.tsv"))
                {
                    ProcesarArchivo(archivo);
                }

                // Vigilancia Tiempo Real (.tsv)
                _watcher = new FileSystemWatcher(_config.RutaCarpetaCSV, "*.tsv");
                _watcher.Created += (s, e) => ProcesarArchivo(e.FullPath);
                _watcher.EnableRaisingEvents = true;
                _isRunning = true;

                AppLogger.LogInformation($"Servicio ACTIVO. Vigilando archivos TSV en: {_config.RutaCarpetaCSV}");
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex, "Error al iniciar servicio.");
                StopMonitoring();
            }
        }

        public void StopMonitoring()
        {
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
            string nombreArchivo = Path.GetFileName(rutaArchivo);
            OnProcesando?.Invoke(nombreArchivo); // Notificar UI

            Thread.Sleep(500);
            bool moverAProcesados = false;

            int totalLeidos = 0;
            int totalActualizados = 0;
            int totalErrores = 0;

            try
            {
                PhanteraRepository repo = new PhanteraRepository(_config);
                string[] lineas = File.ReadAllLines(rutaArchivo);

                if (lineas.Length > 1) // Encabezado + Datos
                {
                    // ---------------------------------------------------------
                    // 1. MAPEO DINÁMICO DE COLUMNAS
                    // ---------------------------------------------------------
                    // Leemos encabezados y limpiamos comillas
                    string[] headers = lineas[0].Split('\t').Select(h => h.Trim('"').Trim()).ToArray();

                    Dictionary<string, int> mapaColumnas = new Dictionary<string, int>();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        if (!mapaColumnas.ContainsKey(headers[i]))
                            mapaColumnas.Add(headers[i], i);
                    }

                    // Validamos COLUMNAS REQUERIDAS (Agregamos SampleType)
                    if (!mapaColumnas.ContainsKey("SampleID") ||
                        !mapaColumnas.ContainsKey("Channel") ||
                        !mapaColumnas.ContainsKey("SampleType") ||  // <--- NUEVA VALIDACIÓN
                        !mapaColumnas.ContainsKey("LR_Ct_NonNormalized"))
                    {
                        AppLogger.LogError(null, $"El archivo {nombreArchivo} no tiene las columnas requeridas (SampleID, Channel, SampleType, LR_Ct_NonNormalized).");
                        moverAProcesados = false;
                    }
                    else
                    {
                        // Indices detectados
                        int idxSample = mapaColumnas["SampleID"];
                        int idxChannel = mapaColumnas["Channel"];
                        int idxSampleType = mapaColumnas["SampleType"]; // <--- NUEVO ÍNDICE
                        int idxResult = mapaColumnas["LR_Ct_NonNormalized"];

                        // ---------------------------------------------------------
                        // 2. PROCESAMIENTO DE FILAS
                        // ---------------------------------------------------------
                        for (int i = 1; i < lineas.Length; i++)
                        {
                            string linea = lineas[i];
                            if (string.IsNullOrWhiteSpace(linea)) continue;

                            string[] celdas = linea.Split('\t');
                            if (celdas.Length < headers.Length) continue;

                            // LIMPIEZA DE DATOS
                            string sampleId = celdas[idxSample].Trim('"').Trim();
                            string channel = celdas[idxChannel].Trim('"').Trim();
                            string sampleType = celdas[idxSampleType].Trim('"').Trim(); // <--- LEER TIPO
                            string rawResult = celdas[idxResult].Trim('"').Trim();

                            // --- FILTROS DE NEGOCIO ---

                            // 1. Solo procesar si es PACIENTE REAL (Specimen)
                            // Usamos ToUpper para evitar problemas si viene "specimen" o "Specimen"
                            if (sampleType.ToUpper() != "SPECIMEN")
                            {
                                continue; // Ignoramos Controles, Blancos, etc.
                            }

                            // 2. Solo procesar canal FAM
                            if (channel.ToUpper() != "FAM")
                            {
                                continue; // Ignoramos otros canales
                            }

                            totalLeidos++;

                            // --- LÓGICA DE RESULTADO (GBS) ---
                            string resultadoFinal = "INDETERMINADO";

                            if (rawResult.ToLower() == "nc")
                            {
                                resultadoFinal = "NEGATIVO";
                            }
                            else
                            {
                                if (double.TryParse(rawResult, NumberStyles.Any, CultureInfo.InvariantCulture, out double ctValue))
                                {
                                    if (ctValue < 40.0)
                                        resultadoFinal = "POSITIVO";
                                    else
                                        resultadoFinal = "NEGATIVO";
                                }
                                else
                                {
                                    AppLogger.LogWarning($"[DATO INVÁLIDO] Muestra {sampleId}: Valor '{rawResult}' no numérico.");
                                    totalErrores++;
                                    continue;
                                }
                            }

                            // --- INTERACCIÓN SQL ---
                            int? ordenId = repo.BuscarOrdenPorCodigoBarras(sampleId);

                            if (ordenId.HasValue)
                            {
                                bool actualizado = repo.ActualizarResultado(ordenId.Value, resultadoFinal);

                                if (actualizado)
                                {
                                    totalActualizados++;
                                    moverAProcesados = true;
                                }
                                else
                                {
                                    AppLogger.LogWarning($"[OMITIDO] Muestra {sampleId}: Prueba GBS no solicitada.");
                                    totalErrores++;
                                    moverAProcesados = true;
                                }
                            }
                            else
                            {
                                AppLogger.LogWarning($"[NO ENCONTRADA] Muestra {sampleId}: No existe orden en SQL.");
                                totalErrores++;
                            }
                        }

                        // REPORTE RESUMEN
                        if (totalActualizados > 0)
                        {
                            string msg = totalErrores == 0
                                ? $"[CARGA EXITOSA] {nombreArchivo}: {totalActualizados} pacientes actualizados."
                                : $"[CARGA PARCIAL] {nombreArchivo}: {totalActualizados} OK, {totalErrores} Alertas.";

                            if (totalErrores == 0) AppLogger.LogInformation(msg); else AppLogger.LogWarning(msg);
                        }
                        else if (totalErrores > 0)
                        {
                            AppLogger.LogError(null, $"[CARGA FALLIDA] {nombreArchivo}: 0 cargados, {totalErrores} errores.");
                        }
                        else if (totalLeidos == 0)
                        {
                            // Si leímos el archivo pero no encontramos ningún "FAM" + "Specimen"
                            AppLogger.LogInformation($"[INFO] Archivo {nombreArchivo} procesado (Solo contenía Controles o canales no válidos).");
                            moverAProcesados = true;
                        }
                    }
                }
                else
                {
                    AppLogger.LogWarning($"Archivo vacío o sin encabezados: {nombreArchivo}");
                    moverAProcesados = true;
                }
            }
            catch (Exception ex)
            {
                AppLogger.LogError(ex, $"Error crítico procesando {nombreArchivo}");
                moverAProcesados = false;
            }
            finally
            {
                OnProcesando?.Invoke(null);
                MoverArchivo(rutaArchivo, moverAProcesados);
            }
        }

        private void MoverArchivo(string rutaOriginal, bool exito)
        {
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