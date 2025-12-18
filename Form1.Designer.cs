namespace Interfaz_BMolecultar_IG
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            configuracionToolStripMenuItem = new ToolStripMenuItem();
            tsmiAbrirConfiguracion = new ToolStripMenuItem();
            tsmiSalir = new ToolStripMenuItem();
            rtxtOutput = new RichTextBox();
            statusBar = new StatusStrip();
            tsslEstadoPrincipal = new ToolStripStatusLabel();
            tsslArchivoActual = new ToolStripStatusLabel();
            logTimer = new System.Windows.Forms.Timer(components);
            menuStrip1.SuspendLayout();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { configuracionToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(464, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // configuracionToolStripMenuItem
            // 
            configuracionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { tsmiAbrirConfiguracion, tsmiSalir });
            configuracionToolStripMenuItem.Name = "configuracionToolStripMenuItem";
            configuracionToolStripMenuItem.Size = new Size(95, 20);
            configuracionToolStripMenuItem.Text = "Configuracion";
            // 
            // tsmiAbrirConfiguracion
            // 
            tsmiAbrirConfiguracion.Name = "tsmiAbrirConfiguracion";
            tsmiAbrirConfiguracion.Size = new Size(179, 22);
            tsmiAbrirConfiguracion.Text = "Abrir Configuración";
            tsmiAbrirConfiguracion.Click += tsmiAbrirConfiguracion_Click;
            // 
            // tsmiSalir
            // 
            tsmiSalir.Name = "tsmiSalir";
            tsmiSalir.Size = new Size(179, 22);
            tsmiSalir.Text = "Salir";
            tsmiSalir.Click += tsmiSalir_Click;
            // 
            // rtxtOutput
            // 
            rtxtOutput.Location = new Point(12, 27);
            rtxtOutput.Name = "rtxtOutput";
            rtxtOutput.Size = new Size(440, 216);
            rtxtOutput.TabIndex = 1;
            rtxtOutput.Text = "";
            // 
            // statusBar
            // 
            statusBar.Items.AddRange(new ToolStripItem[] { tsslEstadoPrincipal, tsslArchivoActual });
            statusBar.Location = new Point(0, 271);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(464, 22);
            statusBar.TabIndex = 2;
            statusBar.Text = "statusStrip1";
            // 
            // tsslEstadoPrincipal
            // 
            tsslEstadoPrincipal.Name = "tsslEstadoPrincipal";
            tsslEstadoPrincipal.Size = new Size(32, 17);
            tsslEstadoPrincipal.Text = "Listo";
            // 
            // tsslArchivoActual
            // 
            tsslArchivoActual.Name = "tsslArchivoActual";
            tsslArchivoActual.Size = new Size(99, 17);
            tsslArchivoActual.Text = "tsslArchivoActual";
            // 
            // logTimer
            // 
            logTimer.Enabled = true;
            logTimer.Interval = 500;
            logTimer.Tick += logTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(464, 293);
            Controls.Add(statusBar);
            Controls.Add(rtxtOutput);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MaximumSize = new Size(480, 332);
            MinimumSize = new Size(480, 332);
            Name = "MainForm";
            Text = "Interfaz de Comunicación Phantera";
            FormClosing += MainForm_FormClosing;
            Load += Form1_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem configuracionToolStripMenuItem;
        private ToolStripMenuItem tsmiAbrirConfiguracion;
        private ToolStripMenuItem tsmiSalir;
        private RichTextBox rtxtOutput;
        private StatusStrip statusBar;
        private ToolStripStatusLabel tsslEstadoPrincipal;
        private ToolStripStatusLabel tsslArchivoActual;
        private System.Windows.Forms.Timer logTimer;
    }
}
