namespace BudgetImmigration
{
    partial class Tutorial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tutorial));
            this.label2 = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.picBoxTutorial = new System.Windows.Forms.PictureBox();
            this.btnPreview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picBoxTutorial)).BeginInit();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(161, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(676, 38);
            this.label2.TabIndex = 101;
            this.label2.Text = "Uma breve descrição de como funciona o sistema.";
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(902, 372);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(80, 27);
            this.btnNext.TabIndex = 102;
            this.btnNext.Text = "Próximo";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnSkip
            // 
            this.btnSkip.Location = new System.Drawing.Point(19, 372);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(80, 27);
            this.btnSkip.TabIndex = 103;
            this.btnSkip.Text = "Pular Tutorial";
            this.btnSkip.UseVisualStyleBackColor = true;
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            // 
            // picBoxTutorial
            // 
            this.picBoxTutorial.Image = global::BudgetImmigration.Properties.Resources.Tutorial1;
            this.picBoxTutorial.Location = new System.Drawing.Point(19, 50);
            this.picBoxTutorial.Name = "picBoxTutorial";
            this.picBoxTutorial.Size = new System.Drawing.Size(963, 316);
            this.picBoxTutorial.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picBoxTutorial.TabIndex = 104;
            this.picBoxTutorial.TabStop = false;
            // 
            // btnPreview
            // 
            this.btnPreview.Enabled = false;
            this.btnPreview.Location = new System.Drawing.Point(816, 372);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(80, 27);
            this.btnPreview.TabIndex = 105;
            this.btnPreview.Text = "Anterior";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // Tutorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::BudgetImmigration.Properties.Resources.Fundo_Azul;
            this.ClientSize = new System.Drawing.Size(994, 410);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.picBoxTutorial);
            this.Controls.Add(this.btnSkip);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.label2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Tutorial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.picBoxTutorial)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.PictureBox picBoxTutorial;
        private System.Windows.Forms.Button btnPreview;
    }
}