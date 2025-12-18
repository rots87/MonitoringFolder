using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text; // Necesario para StringBuilder

namespace Interfaz_BMolecultar_IG
{
    /// <summary>
    /// Formulario de Gestión de Configuración (CI - Configuration Item).
    /// Permite modificar parámetros operativos y credenciales de acceso a datos.
    /// </summary>
    public partial class ConfigForm : Form
    {
        private AppConfigModel _configuracionActual;
        private bool _seGuardoExitosamente = false;

        public ConfigForm(AppConfigModel config)
        {
            InitializeComponent();
            _configuracionActual = config;

            CargarDatosEnInterfaz();

            lblResultadoPrueba.Text = string.Empty;
        }

        private void CargarDatosEnInterfaz()
        {
            txtRutaCsv.Text = _configuracionActual.RutaCarpetaCSV;
            txtServidor.Text = _configuracionActual.ServidorSQL;
            txtDB.Text = _configuracionActual.NombreBaseDatos;
            txtUsuario.Text = _configuracionActual.UsuarioSQL;
            txtPassword.Text = _configuracionActual.ContrasenaSQL;
            // Asumimos que hay un txtTabla para la tabla destino, si no, se puede omitir
            // txtTabla.Text = _configuracionActual.NombreTablaDestino; 
        }

        // ---------------------------------------------------------
        // GESTIÓN DE RUTAS
        // ---------------------------------------------------------
        private void btnExaminarRuta_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Seleccione la carpeta de origen para archivos CSV.";

                if (Directory.Exists(txtRutaCsv.Text))
                {
                    fbd.SelectedPath = txtRutaCsv.Text;
                }

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtRutaCsv.Text = fbd.SelectedPath;
                }
            }
        }

        // ---------------------------------------------------------
        // VALIDACIÓN DE SERVICIO
        // ---------------------------------------------------------
        private void btnProbarConexion_Click(object sender, EventArgs e)
        {
            // Validamos antes de probar para no perder tiempo
            if (!ValidarDatosCriticos(soloConexionSql: true)) return;

            lblResultadoPrueba.Text = "Validando conectividad...";
            lblResultadoPrueba.ForeColor = Color.Black;
            Application.DoEvents();

            bool conexionExitosa = SqlDataAccess.TestConnection(
                txtServidor.Text,
                txtDB.Text,
                txtUsuario.Text,
                txtPassword.Text,
                out string error
            );

            if (conexionExitosa)
            {
                lblResultadoPrueba.Text = "Conexión Establecida Correctamente";
                lblResultadoPrueba.ForeColor = Color.DarkGreen;
            }
            else
            {
                lblResultadoPrueba.Text = "Fallo en la Conexión";
                lblResultadoPrueba.ForeColor = Color.Red;
                MessageBox.Show($"Detalle del error:\n{error}", "Diagnóstico de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ---------------------------------------------------------
        // PERSISTENCIA (Guardar Cambios)
        // ---------------------------------------------------------
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            // 1. VALIDACIÓN (Quality Gate)
            // Si la validación falla, detenemos el proceso de guardado inmediatamente.
            if (!ValidarDatosCriticos(soloConexionSql: false))
            {
                return;
            }

            // 2. Actualizar modelo en memoria
            _configuracionActual.RutaCarpetaCSV = txtRutaCsv.Text;
            _configuracionActual.ServidorSQL = txtServidor.Text;
            _configuracionActual.NombreBaseDatos = txtDB.Text;
            _configuracionActual.UsuarioSQL = txtUsuario.Text;
            _configuracionActual.ContrasenaSQL = txtPassword.Text;

            // 3. Persistir en disco (JSON)
            ConfigManager.SaveConfig(_configuracionActual);

            // 4. Finalizar transacción
            _seGuardoExitosamente = true;
            this.DialogResult = DialogResult.OK;

            MessageBox.Show("La configuración ha sido actualizada.\nEs necesario reiniciar el monitoreo para aplicar los cambios.", "Configuración Guardada", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        // ---------------------------------------------------------
        // MÉTODO AUXILIAR DE VALIDACIÓN
        // ---------------------------------------------------------
        /// <summary>
        /// Verifica que todos los campos requeridos tengan información.
        /// </summary>
        /// <param name="soloConexionSql">Si es true, solo valida campos de SQL (para el botón probar).</param>
        /// <returns>True si todo es válido, False si faltan datos.</returns>
        private bool ValidarDatosCriticos(bool soloConexionSql)
        {
            StringBuilder camposFaltantes = new StringBuilder();

            // Validaciones SQL (Requeridas siempre)
            if (string.IsNullOrWhiteSpace(txtServidor.Text)) camposFaltantes.AppendLine("- Servidor SQL");
            if (string.IsNullOrWhiteSpace(txtDB.Text)) camposFaltantes.AppendLine("- Nombre de Base de Datos");
            if (string.IsNullOrWhiteSpace(txtUsuario.Text)) camposFaltantes.AppendLine("- Usuario SQL");

            // La contraseña podría ser vacía en casos raros, pero generalmente es requerida. 
            // Si tu política permite pass vacíos, comenta esta línea.
            if (string.IsNullOrWhiteSpace(txtPassword.Text)) camposFaltantes.AppendLine("- Contraseña SQL");

            // Validaciones de Archivos (Solo requeridas al Guardar, no al probar conexión SQL)
            if (!soloConexionSql)
            {
                if (string.IsNullOrWhiteSpace(txtRutaCsv.Text)) camposFaltantes.AppendLine("- Ruta de Carpeta CSV");
            }

            // Si hay errores, mostramos mensaje y retornamos false
            if (camposFaltantes.Length > 0)
            {
                MessageBox.Show(
                    "No se puede continuar debido a que faltan datos obligatorios:\n\n" + camposFaltantes.ToString(),
                    "Datos Incompletos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }

            return true;
        }

        // ---------------------------------------------------------
        // CONTROL DE FLUJO Y CIERRE
        // ---------------------------------------------------------
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_seGuardoExitosamente) return;

            var resultado = MessageBox.Show(
                "¿Desea descartar los cambios no guardados?",
                "Confirmar Cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void btnCancelar_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}