namespace NeoBPMN.Service
{
    partial class frmBPMNService
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="ablnDisposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool ablnDisposing)
        {
            if (ablnDisposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(ablnDisposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnClose = new System.Windows.Forms.Button();
            this.txbError = new System.Windows.Forms.TextBox();
            this.lblErrorDetails = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefreshMetaDataCache = new System.Windows.Forms.Button();
            this.btnRefreshDBCache = new System.Windows.Forms.Button();
            this.btnRefreshRules = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(546, 198);
            this.btnClose.Name = "btnClose";
            this.btnClose.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txbError
            // 
            this.txbError.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txbError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txbError.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.txbError.Location = new System.Drawing.Point(88, 36);
            this.txbError.Multiline = true;
            this.txbError.Name = "txbError";
            this.txbError.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbError.Size = new System.Drawing.Size(533, 156);
            this.txbError.TabIndex = 9;
            // 
            // lblErrorDetails
            // 
            this.lblErrorDetails.AutoSize = true;
            this.lblErrorDetails.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblErrorDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblErrorDetails.Location = new System.Drawing.Point(12, 39);
            this.lblErrorDetails.Name = "lblErrorDetails";
            this.lblErrorDetails.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblErrorDetails.Size = new System.Drawing.Size(70, 13);
            this.lblErrorDetails.TabIndex = 8;
            this.lblErrorDetails.Text = "Error Details :";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(81, 203);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(35, 13);
            this.lblStatus.TabIndex = 11;
            this.lblStatus.Text = "label1";
            // 
            // btnRefreshMetaDataCache
            // 
            this.btnRefreshMetaDataCache.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnRefreshMetaDataCache.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshMetaDataCache.Location = new System.Drawing.Point(317, 7);
            this.btnRefreshMetaDataCache.Name = "btnRefreshMetaDataCache";
            this.btnRefreshMetaDataCache.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnRefreshMetaDataCache.Size = new System.Drawing.Size(103, 23);
            this.btnRefreshMetaDataCache.TabIndex = 12;
            this.btnRefreshMetaDataCache.Text = "Refresh Metadata";
            this.btnRefreshMetaDataCache.Click += new System.EventHandler(this.btnRefreshMetaDataCache_Click);
            // 
            // btnRefreshDBCache
            // 
            this.btnRefreshDBCache.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnRefreshDBCache.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshDBCache.Location = new System.Drawing.Point(426, 7);
            this.btnRefreshDBCache.Name = "btnRefreshDBCache";
            this.btnRefreshDBCache.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnRefreshDBCache.Size = new System.Drawing.Size(105, 23);
            this.btnRefreshDBCache.TabIndex = 13;
            this.btnRefreshDBCache.Text = "Refresh DB Cache";
            this.btnRefreshDBCache.Click += new System.EventHandler(this.btnRefreshDBCache_Click);
            // 
            // btnRefreshRules
            // 
            this.btnRefreshRules.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnRefreshRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshRules.Location = new System.Drawing.Point(537, 7);
            this.btnRefreshRules.Name = "btnRefreshRules";
            this.btnRefreshRules.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnRefreshRules.Size = new System.Drawing.Size(84, 23);
            this.btnRefreshRules.TabIndex = 14;
            this.btnRefreshRules.Text = "Refresh Rules";
            this.btnRefreshRules.Click += new System.EventHandler(this.btnRefreshRules_Click);
            // 
            // frmBPMNService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 227);
            this.Controls.Add(this.btnRefreshRules);
            this.Controls.Add(this.btnRefreshDBCache);
            this.Controls.Add(this.btnRefreshMetaDataCache);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txbError);
            this.Controls.Add(this.lblErrorDetails);
            this.Name = "frmBPMNService";
            this.Text = "NeoSpin BPM Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmNeoFlowService_FormClosing);
            this.Shown += new System.EventHandler(this.frmBPMNService_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txbError;
        private System.Windows.Forms.Label lblErrorDetails;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRefreshMetaDataCache;
        private System.Windows.Forms.Button btnRefreshDBCache;
        private System.Windows.Forms.Button btnRefreshRules;
    }
}

