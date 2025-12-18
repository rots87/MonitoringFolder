using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Serilog.Events;

namespace Interfaz_BMolecultar_IG
{
    /// <summary>
    /// Formulario Principal (Dashboard).
    /// Responsable de la orquestación del servicio, visualización de logs y estado operativo.
    /// </summary>
    public partial class MainForm : Form
    {
        // Configuración Global (Singleton-like para el ciclo de vida del Form)
        private AppConfigModel _configuracionGlobal = ConfigManager.LoadConfig();

        // Servicio de Monitoreo (Business Logic)
        private MonitoringService _monitoringService;

        public MainForm()
        {
            InitializeComponent();

            // Preparamos la instancia del servicio y suscribimos eventos
            InicializarServicio();
        }

        /// <summary>
        /// Inicializa o Reinicia la instancia del servicio de monitoreo.
        /// Gestiona la suscripción a eventos para evitar fugas de memoria.
        /// </summary>
        private void InicializarServicio()
        {
            // 1. Limpieza previa: Desuscribir evento si ya existía una instancia
            if (_monitoringService != null)
            {
                _monitoringService.OnProcesando -= ActualizarEstadoArchivo;
            }

            // 2. Nueva Instancia: Inyectamos la configuración actual
            _monitoringService = new MonitoringService(_configuracionGlobal);

            // 3. Suscripción: Escuchamos cuando el servicio detecta un archivo
            _monitoringService.OnProcesando += ActualizarEstadoArchivo;
        }

        // -------------------------------------------------------------------------
        // GESTIÓN DE ESTADO VISUAL (Thread-Safe)
        // -------------------------------------------------------------------------

        /// <summary>
        /// Actualiza la barra de estado con el nombre del archivo que se está procesando.
        /// Maneja la invocación entre hilos (Cross-thread operation).
        /// </summary>
        private void ActualizarEstadoArchivo(string nombreArchivo)
        {
            // Si la llamada viene del hilo del FileSystemWatcher, la enviamos al hilo de la UI
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ActualizarEstadoArchivo), nombreArchivo);
                return;
            }

            // Lógica de UI (ya en el hilo principal)
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                tsslArchivoActual.Text = "Esperando archivos...";
            }
            else
            {
                tsslArchivoActual.Text = $"Subiendo resultados del archivo: {nombreArchivo}...";
            }
        }

        // -------------------------------------------------------------------------
        // CICLO DE VIDA DEL FORMULARIO
        // -------------------------------------------------------------------------

        private void Form1_Load(object sender, EventArgs e)
        {
            tsslArchivoActual.Text = "Inactivo";

            // Validación de arranque: ¿Tenemos lo mínimo para operar?
            if (string.IsNullOrWhiteSpace(_configuracionGlobal.ServidorSQL) ||
                string.IsNullOrWhiteSpace(_configuracionGlobal.RutaCarpetaCSV))
            {
                AppLogger.LogWarning("Inicio: Configuración incompleta. El servicio permanecerá en pausa.");

                tsslEstadoPrincipal.Text = "Configuración Pendiente";
                tsslEstadoPrincipal.ForeColor = Color.Orange;
            }
            else
            {
                AppLogger.LogInformation("Inicio: Configuración válida detectada. Iniciando servicio...");

                // Intento de arranque automático
                _monitoringService.StartMonitoring();

                if (_monitoringService.IsRunning)
                {
                    tsslEstadoPrincipal.Text = "Monitoreando Activo";
                    tsslEstadoPrincipal.ForeColor = Color.DarkGreen;
                    tsslArchivoActual.Text = "Esperando archivos...";
                }
                else
                {
                    // El log interno ya habrá registrado la causa del fallo
                    tsslEstadoPrincipal.Text = "Error al Iniciar";
                    tsslEstadoPrincipal.ForeColor = Color.Red;
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Confirmación solo si es cierre manual por usuario
            if (e.CloseReason == CloseReason.UserClosing)
            {
                var resultado = MessageBox.Show(
                    "¿Está seguro que desea salir?\n\nAl cerrar la aplicación, se detendrá la importación automática de resultados.",
                    "Confirmar Salida",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (resultado == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Apagado limpio del servicio
            if (_monitoringService != null)
            {
                _monitoringService.StopMonitoring();
            }
        }

        // -------------------------------------------------------------------------
        // MENÚ Y NAVEGACIÓN
        // -------------------------------------------------------------------------

        private void tsmiAbrirConfiguracion_Click(object sender, EventArgs e)
        {
            ConfigForm configForm = new ConfigForm(_configuracionGlobal);

            // Si el usuario guarda cambios (DialogResult.OK), reiniciamos el servicio
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                AppLogger.LogInformation("Configuración modificada por usuario. Reiniciando servicios...");

                // 1. Detener
                _monitoringService.StopMonitoring();

                // 2. Reconstruir (para tomar nuevos valores)
                InicializarServicio();

                // 3. Iniciar
                _monitoringService.StartMonitoring();

                // 4. Actualizar UI
                if (_monitoringService.IsRunning)
                {
                    tsslEstadoPrincipal.Text = "Monitoreando Activo";
                    tsslEstadoPrincipal.ForeColor = Color.DarkGreen;
                    tsslArchivoActual.Text = "Esperando archivos...";
                    AppLogger.LogInformation("Servicio reiniciado exitosamente.");
                }
                else
                {
                    tsslEstadoPrincipal.Text = "Detenido (Error)";
                    tsslEstadoPrincipal.ForeColor = Color.Red;
                }
            }
        }

        private void tsmiSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // -------------------------------------------------------------------------
        // VISUALIZACIÓN DE LOGS (RichTextBox)
        // -------------------------------------------------------------------------

        private void logTimer_Tick(object sender, EventArgs e)
        {
            // Consumimos el buffer de logs en memoria
            List<LogEntry> newEntries = AppLogger.GetNewLogMessages();

            if (newEntries.Count > 0)
            {
                rtxtOutput.SuspendLayout();

                foreach (LogEntry entry in newEntries)
                {
                    Color textColor = GetColorForLogLevel(entry.Level);

                    rtxtOutput.SelectionStart = rtxtOutput.TextLength;
                    rtxtOutput.SelectionLength = 0;
                    rtxtOutput.SelectionColor = textColor;
                    rtxtOutput.AppendText(entry.Message + Environment.NewLine);
                }

                // Restaurar scroll y color base
                rtxtOutput.SelectionColor = rtxtOutput.ForeColor;
                rtxtOutput.ScrollToCaret();
                rtxtOutput.ResumeLayout();
            }
        }

        /// <summary>
        /// Mapeo de niveles de log a colores de interfaz.
        /// </summary>
        private Color GetColorForLogLevel(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Error:
                case LogEventLevel.Fatal: return Color.Red;
                case LogEventLevel.Warning: return Color.Orange;
                case LogEventLevel.Information: return Color.DarkGreen;
                default: return Color.Gray;
            }
        }
    }
}