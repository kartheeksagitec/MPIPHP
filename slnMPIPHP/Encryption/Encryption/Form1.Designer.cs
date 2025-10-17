namespace Encryption
{
    partial class FrmEncrypt
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
            this.pnlEncrypt = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnDOB = new System.Windows.Forms.Button();
            this.btnSSN = new System.Windows.Forms.Button();
            this.pnlEncrypt.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlEncrypt
            // 
            this.pnlEncrypt.Controls.Add(this.lblStatus);
            this.pnlEncrypt.Controls.Add(this.btnDOB);
            this.pnlEncrypt.Controls.Add(this.btnSSN);
            this.pnlEncrypt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEncrypt.Location = new System.Drawing.Point(0, 0);
            this.pnlEncrypt.Name = "pnlEncrypt";
            this.pnlEncrypt.Size = new System.Drawing.Size(335, 157);
            this.pnlEncrypt.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(50, 135);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 13);
            this.lblStatus.TabIndex = 2;
            // 
            // btnDOB
            // 
            this.btnDOB.Location = new System.Drawing.Point(215, 33);
            this.btnDOB.Name = "btnDOB";
            this.btnDOB.Size = new System.Drawing.Size(98, 50);
            this.btnDOB.TabIndex = 1;
            this.btnDOB.Text = "Encrypt DOB";
            this.btnDOB.UseVisualStyleBackColor = true;
            this.btnDOB.Click += new System.EventHandler(this.btnDOB_Click);
            // 
            // btnSSN
            // 
            this.btnSSN.Location = new System.Drawing.Point(33, 33);
            this.btnSSN.Name = "btnSSN";
            this.btnSSN.Size = new System.Drawing.Size(90, 50);
            this.btnSSN.TabIndex = 0;
            this.btnSSN.Text = "Encrypt SSN";
            this.btnSSN.UseVisualStyleBackColor = true;
            this.btnSSN.Click += new System.EventHandler(this.btnSSN_Click);
            // 
            // FrmEncrypt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 157);
            this.Controls.Add(this.pnlEncrypt);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmEncrypt";
            this.Text = "EncryptFields";
            this.pnlEncrypt.ResumeLayout(false);
            this.pnlEncrypt.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlEncrypt;
        private System.Windows.Forms.Button btnDOB;
        private System.Windows.Forms.Button btnSSN;
        private System.Windows.Forms.Label lblStatus;
    }
}

