namespace Interfaz_BMolecultar_IG
{
    partial class ConfigForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            grpArchivos = new GroupBox();
            btnExaminarRuta = new Button();
            lblRutaCsv = new Label();
            txtRutaCsv = new TextBox();
            folderBrowserDialog1 = new FolderBrowserDialog();
            grpDatabase = new GroupBox();
            btnProbarConexion = new Button();
            lblResultadoPrueba = new Label();
            txtPassword = new TextBox();
            txtUsuario = new TextBox();
            txtDB = new TextBox();
            txtServidor = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            btnCancelar = new Button();
            btnGuardar = new Button();
            grpArchivos.SuspendLayout();
            grpDatabase.SuspendLayout();
            SuspendLayout();
            // 
            // grpArchivos
            // 
            grpArchivos.Controls.Add(btnExaminarRuta);
            grpArchivos.Controls.Add(lblRutaCsv);
            grpArchivos.Controls.Add(txtRutaCsv);
            grpArchivos.Location = new Point(12, 17);
            grpArchivos.Name = "grpArchivos";
            grpArchivos.Size = new Size(427, 75);
            grpArchivos.TabIndex = 0;
            grpArchivos.TabStop = false;
            grpArchivos.Text = "Ubicación de Archivos CSV (Phantera)";
            // 
            // btnExaminarRuta
            // 
            btnExaminarRuta.Location = new Point(395, 32);
            btnExaminarRuta.Name = "btnExaminarRuta";
            btnExaminarRuta.Size = new Size(26, 25);
            btnExaminarRuta.TabIndex = 2;
            btnExaminarRuta.Text = "...";
            btnExaminarRuta.UseVisualStyleBackColor = true;
            btnExaminarRuta.Click += btnExaminarRuta_Click;
            // 
            // lblRutaCsv
            // 
            lblRutaCsv.AutoSize = true;
            lblRutaCsv.Location = new Point(6, 35);
            lblRutaCsv.Name = "lblRutaCsv";
            lblRutaCsv.Size = new Size(160, 15);
            lblRutaCsv.TabIndex = 1;
            lblRutaCsv.Text = "Ruta de Carpeta Compartida:";
            // 
            // txtRutaCsv
            // 
            txtRutaCsv.Location = new Point(172, 32);
            txtRutaCsv.Name = "txtRutaCsv";
            txtRutaCsv.Size = new Size(224, 23);
            txtRutaCsv.TabIndex = 0;
            // 
            // grpDatabase
            // 
            grpDatabase.Controls.Add(btnProbarConexion);
            grpDatabase.Controls.Add(lblResultadoPrueba);
            grpDatabase.Controls.Add(txtPassword);
            grpDatabase.Controls.Add(txtUsuario);
            grpDatabase.Controls.Add(txtDB);
            grpDatabase.Controls.Add(txtServidor);
            grpDatabase.Controls.Add(label4);
            grpDatabase.Controls.Add(label3);
            grpDatabase.Controls.Add(label2);
            grpDatabase.Controls.Add(label1);
            grpDatabase.Location = new Point(12, 114);
            grpDatabase.Name = "grpDatabase";
            grpDatabase.Size = new Size(427, 215);
            grpDatabase.TabIndex = 1;
            grpDatabase.TabStop = false;
            grpDatabase.Text = "Conexión a Base de Datos SQL Server";
            // 
            // btnProbarConexion
            // 
            btnProbarConexion.Location = new Point(119, 170);
            btnProbarConexion.Name = "btnProbarConexion";
            btnProbarConexion.Size = new Size(106, 23);
            btnProbarConexion.TabIndex = 9;
            btnProbarConexion.Text = "Probar Conexion";
            btnProbarConexion.UseVisualStyleBackColor = true;
            btnProbarConexion.Click += btnProbarConexion_Click;
            // 
            // lblResultadoPrueba
            // 
            lblResultadoPrueba.AutoSize = true;
            lblResultadoPrueba.Location = new Point(280, 174);
            lblResultadoPrueba.Name = "lblResultadoPrueba";
            lblResultadoPrueba.Size = new Size(38, 15);
            lblResultadoPrueba.TabIndex = 8;
            lblResultadoPrueba.Text = "label5";
            lblResultadoPrueba.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(172, 128);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(224, 23);
            txtPassword.TabIndex = 7;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(172, 94);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(224, 23);
            txtUsuario.TabIndex = 6;
            // 
            // txtDB
            // 
            txtDB.Location = new Point(172, 60);
            txtDB.Name = "txtDB";
            txtDB.Size = new Size(224, 23);
            txtDB.TabIndex = 5;
            // 
            // txtServidor
            // 
            txtServidor.Location = new Point(172, 26);
            txtServidor.Name = "txtServidor";
            txtServidor.Size = new Size(224, 23);
            txtServidor.TabIndex = 4;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(11, 131);
            label4.Name = "label4";
            label4.Size = new Size(91, 15);
            label4.TabIndex = 3;
            label4.Text = "Contraseña SQL";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 97);
            label3.Name = "label3";
            label3.Size = new Size(71, 15);
            label3.TabIndex = 2;
            label3.Text = "Usuario SQL";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 63);
            label2.Name = "label2";
            label2.Size = new Size(143, 15);
            label2.TabIndex = 1;
            label2.Text = "Nombre de Base de Datos";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 29);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 0;
            label1.Text = "Servidor/IP";
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(364, 355);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 2;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click_1;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(265, 355);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 3;
            btnGuardar.Text = "Guardar Configuración";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // ConfigForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(450, 395);
            Controls.Add(btnGuardar);
            Controls.Add(btnCancelar);
            Controls.Add(grpDatabase);
            Controls.Add(grpArchivos);
            MaximizeBox = false;
            MaximumSize = new Size(466, 434);
            MinimumSize = new Size(466, 434);
            Name = "ConfigForm";
            Text = "Configuración de la Interfaz de Comunicación Phantera";
            FormClosing += ConfigForm_FormClosing;
            grpArchivos.ResumeLayout(false);
            grpArchivos.PerformLayout();
            grpDatabase.ResumeLayout(false);
            grpDatabase.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox grpArchivos;
        private TextBox txtRutaCsv;
        private Label lblRutaCsv;
        private Button btnExaminarRuta;
        private FolderBrowserDialog folderBrowserDialog1;
        private GroupBox grpDatabase;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox txtDB;
        private TextBox txtServidor;
        private TextBox txtPassword;
        private TextBox txtUsuario;
        private Label lblResultadoPrueba;
        private Button btnProbarConexion;
        private Button btnCancelar;
        private Button btnGuardar;
    }
}