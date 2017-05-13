namespace Loto
{
    partial class Form1
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Wymagana metoda obsługi projektanta — nie należy modyfikować 
        /// zawartość tej metody z edytorem kodu.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.plikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.otwórzToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.działajToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wieleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testujToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skalujToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skalujJasnoścWieluToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wIeleObliczToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testoweToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.domyslnaLinikaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialogLinijek = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(858, 566);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plikToolStripMenuItem,
            this.testoweToolStripMenuItem,
            this.domyslnaLinikaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(882, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // plikToolStripMenuItem
            // 
            this.plikToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.otwórzToolStripMenuItem,
            this.działajToolStripMenuItem,
            this.wieleToolStripMenuItem,
            this.testujToolStripMenuItem,
            this.skalujToolStripMenuItem,
            this.skalujJasnoścWieluToolStripMenuItem,
            this.wIeleObliczToolStripMenuItem});
            this.plikToolStripMenuItem.Name = "plikToolStripMenuItem";
            this.plikToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.plikToolStripMenuItem.Text = "plik";
            // 
            // otwórzToolStripMenuItem
            // 
            this.otwórzToolStripMenuItem.Name = "otwórzToolStripMenuItem";
            this.otwórzToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.otwórzToolStripMenuItem.Text = "Wczytaj";
            this.otwórzToolStripMenuItem.Click += new System.EventHandler(this.otwórzToolStripMenuItem_Click);
            // 
            // działajToolStripMenuItem
            // 
            this.działajToolStripMenuItem.Enabled = false;
            this.działajToolStripMenuItem.Name = "działajToolStripMenuItem";
            this.działajToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.działajToolStripMenuItem.Text = "Uruchom";
            this.działajToolStripMenuItem.Click += new System.EventHandler(this.działajToolStripMenuItem_Click);
            // 
            // wieleToolStripMenuItem
            // 
            this.wieleToolStripMenuItem.Name = "wieleToolStripMenuItem";
            this.wieleToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.wieleToolStripMenuItem.Text = "Eksportuj litery";
            this.wieleToolStripMenuItem.Click += new System.EventHandler(this.wieleToolStripMenuItem_Click);
            // 
            // testujToolStripMenuItem
            // 
            this.testujToolStripMenuItem.Name = "testujToolStripMenuItem";
            this.testujToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.testujToolStripMenuItem.Text = "testuj";
            this.testujToolStripMenuItem.Visible = false;
            this.testujToolStripMenuItem.Click += new System.EventHandler(this.testujToolStripMenuItem_Click);
            // 
            // skalujToolStripMenuItem
            // 
            this.skalujToolStripMenuItem.Name = "skalujToolStripMenuItem";
            this.skalujToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.skalujToolStripMenuItem.Text = "Skaluj";
            this.skalujToolStripMenuItem.Visible = false;
            this.skalujToolStripMenuItem.Click += new System.EventHandler(this.skalujToolStripMenuItem_Click);
            // 
            // skalujJasnoścWieluToolStripMenuItem
            // 
            this.skalujJasnoścWieluToolStripMenuItem.Name = "skalujJasnoścWieluToolStripMenuItem";
            this.skalujJasnoścWieluToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.skalujJasnoścWieluToolStripMenuItem.Text = "skaluj jasnośc wielu";
            this.skalujJasnoścWieluToolStripMenuItem.Click += new System.EventHandler(this.skalujJasnoścWieluToolStripMenuItem_Click);
            // 
            // wIeleObliczToolStripMenuItem
            // 
            this.wIeleObliczToolStripMenuItem.Name = "wIeleObliczToolStripMenuItem";
            this.wIeleObliczToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.wIeleObliczToolStripMenuItem.Text = "WIele Oblicz";
            this.wIeleObliczToolStripMenuItem.Visible = false;
            this.wIeleObliczToolStripMenuItem.Click += new System.EventHandler(this.wIeleObliczToolStripMenuItem_Click);
            // 
            // testoweToolStripMenuItem
            // 
            this.testoweToolStripMenuItem.Name = "testoweToolStripMenuItem";
            this.testoweToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.testoweToolStripMenuItem.Text = "Testowe";
            this.testoweToolStripMenuItem.Visible = false;
            this.testoweToolStripMenuItem.Click += new System.EventHandler(this.testoweToolStripMenuItem_Click);
            // 
            // domyslnaLinikaToolStripMenuItem
            // 
            this.domyslnaLinikaToolStripMenuItem.Name = "domyslnaLinikaToolStripMenuItem";
            this.domyslnaLinikaToolStripMenuItem.Size = new System.Drawing.Size(103, 20);
            this.domyslnaLinikaToolStripMenuItem.Text = "DomyslnaLinika";
            this.domyslnaLinikaToolStripMenuItem.Click += new System.EventHandler(this.domyslnaLinikaToolStripMenuItem_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // openFileDialogLinijek
            // 
            this.openFileDialogLinijek.FileName = "openFileDialog2";
            this.openFileDialogLinijek.Filter = "Liniki |*.linika";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 606);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem plikToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem otwórzToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem działajToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem wieleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testujToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skalujToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem skalujJasnoścWieluToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testoweToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wIeleObliczToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem domyslnaLinikaToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialogLinijek;
    }
}

