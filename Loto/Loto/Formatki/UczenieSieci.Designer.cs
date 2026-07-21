namespace ŚieciNeuronowe
{
    partial class UczenieSieci
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
            button1 = new Button();
            folderBrowserDialog1 = new FolderBrowserDialog();
            listBox1 = new ListBox();
            button2 = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            button3 = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            textBox3 = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            textBox4 = new TextBox();
            label4 = new Label();
            textBox5 = new TextBox();
            label5 = new Label();
            button4 = new Button();
            button5 = new Button();
            textBox6 = new TextBox();
            label6 = new Label();
            checkBox1 = new CheckBox();
            textBox7 = new TextBox();
            textBox8 = new TextBox();
            checkBox2 = new CheckBox();
            label7 = new Label();
            textBox9 = new TextBox();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(22, 42);
            button1.Margin = new Padding(5, 6, 5, 6);
            button1.Name = "button1";
            button1.Size = new Size(207, 44);
            button1.TabIndex = 0;
            button1.Text = "otwórz";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(22, 129);
            listBox1.Margin = new Padding(5, 6, 5, 6);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(392, 329);
            listBox1.TabIndex = 1;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button2.Enabled = false;
            button2.Location = new Point(22, 473);
            button2.Margin = new Padding(5, 6, 5, 6);
            button2.Name = "button2";
            button2.Size = new Size(395, 44);
            button2.TabIndex = 2;
            button2.Text = "Ucz propagacji wstecznej";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox1.Location = new Point(432, 165);
            textBox1.Margin = new Padding(5, 6, 5, 6);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(164, 31);
            textBox1.TabIndex = 3;
            textBox1.Text = "0.7";
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox2.Location = new Point(432, 265);
            textBox2.Margin = new Padding(5, 6, 5, 6);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(164, 31);
            textBox2.TabIndex = 4;
            textBox2.Text = "0.3";
            // 
            // button3
            // 
            button3.Location = new Point(260, 42);
            button3.Margin = new Padding(5, 6, 5, 6);
            button3.Name = "button3";
            button3.Size = new Size(193, 44);
            button3.TabIndex = 5;
            button3.Text = "Wczytaj Siec";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(432, 129);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(177, 25);
            label1.TabIndex = 6;
            label1.Text = "Współczynik Uczenia";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(427, 235);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(213, 25);
            label2.TabIndex = 7;
            label2.Text = "Moment/ uczenie sąsiada";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(427, 329);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(175, 25);
            label3.TabIndex = 8;
            label3.Text = "liczba prób uczących";
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox3.Location = new Point(432, 360);
            textBox3.Margin = new Padding(5, 6, 5, 6);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(164, 31);
            textBox3.TabIndex = 9;
            textBox3.Text = "2000000";
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBox4
            // 
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox4.Location = new Point(432, 458);
            textBox4.Margin = new Padding(5, 6, 5, 6);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(164, 31);
            textBox4.TabIndex = 10;
            textBox4.Text = "5";
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(427, 427);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(93, 25);
            label4.TabIndex = 11;
            label4.Text = "liczba Pętli";
            // 
            // textBox5
            // 
            textBox5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox5.Location = new Point(432, 552);
            textBox5.Margin = new Padding(5, 6, 5, 6);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(164, 31);
            textBox5.TabIndex = 12;
            textBox5.Text = "1.1";
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(432, 521);
            label5.Margin = new Padding(5, 0, 5, 0);
            label5.Name = "label5";
            label5.Size = new Size(177, 25);
            label5.TabIndex = 13;
            label5.Text = "Bias /liczba sąsiadów";
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button4.Enabled = false;
            button4.Location = new Point(22, 529);
            button4.Margin = new Padding(5, 6, 5, 6);
            button4.Name = "button4";
            button4.Size = new Size(395, 44);
            button4.TabIndex = 14;
            button4.Text = "Ucz wzorzec";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button5.Enabled = false;
            button5.Location = new Point(22, 585);
            button5.Margin = new Padding(5, 6, 5, 6);
            button5.Name = "button5";
            button5.Size = new Size(395, 44);
            button5.TabIndex = 15;
            button5.Text = "Ucz rywalizujące";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // textBox6
            // 
            textBox6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox6.Location = new Point(432, 629);
            textBox6.Margin = new Padding(5, 6, 5, 6);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(164, 31);
            textBox6.TabIndex = 16;
            textBox6.Text = "5";
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(432, 602);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(408, 25);
            label6.TabIndex = 17;
            label6.Text = "liczba powtórzeń błąd/liczba porównań wzglednie";
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(432, 679);
            checkBox1.Margin = new Padding(5, 6, 5, 6);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(149, 29);
            checkBox1.TabIndex = 18;
            checkBox1.Text = "Ucz Poprawne";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // textBox7
            // 
            textBox7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox7.Location = new Point(608, 165);
            textBox7.Margin = new Padding(5, 6, 5, 6);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(124, 31);
            textBox7.TabIndex = 19;
            textBox7.Text = "0.5";
            // 
            // textBox8
            // 
            textBox8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox8.Location = new Point(625, 673);
            textBox8.Margin = new Padding(5, 6, 5, 6);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(147, 31);
            textBox8.TabIndex = 20;
            textBox8.Text = "0,5";
            textBox8.TextChanged += textBox8_TextChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(483, 50);
            checkBox2.Margin = new Padding(5, 6, 5, 6);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(142, 29);
            checkBox2.TabIndex = 21;
            checkBox2.Text = "Zapisz Wynik";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Location = new Point(20, 733);
            label7.Margin = new Padding(5, 0, 5, 0);
            label7.Name = "label7";
            label7.Size = new Size(146, 25);
            label7.TabIndex = 22;
            label7.Text = "Skaler szerokości";
            // 
            // textBox9
            // 
            textBox9.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox9.Location = new Point(180, 733);
            textBox9.Margin = new Padding(5, 6, 5, 6);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(164, 31);
            textBox9.TabIndex = 23;
            textBox9.Text = "1";
            // 
            // UczenieSieci
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(862, 794);
            Controls.Add(textBox9);
            Controls.Add(label7);
            Controls.Add(checkBox2);
            Controls.Add(textBox8);
            Controls.Add(textBox7);
            Controls.Add(checkBox1);
            Controls.Add(label6);
            Controls.Add(textBox6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(label5);
            Controls.Add(textBox5);
            Controls.Add(label4);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(button2);
            Controls.Add(listBox1);
            Controls.Add(button1);
            Margin = new Padding(5, 6, 5, 6);
            Name = "UczenieSieci";
            Text = "UczenieSieci";
            Load += UczenieSieci_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBox9;
    }
}