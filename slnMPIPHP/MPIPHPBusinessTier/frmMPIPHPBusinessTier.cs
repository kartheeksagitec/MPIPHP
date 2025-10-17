#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using MPIPHP.Interface;
using Sagitec.BusinessTier;
using Sagitec.Common;
using System.Collections;
using Sagitec.MetaDataCache;
using Sagitec.DBCache;
using Sagitec.BusinessObjects;
using Sagitec.Rules;
using System.ServiceModel;
using System.Collections.ObjectModel;
using Sagitec.Interface;
using System.Configuration;
using NeoSpin.BusinessTier;

#endregion

namespace MPIPHP.BusinessTier
{
	class frmMPIPHPBusinessTier: System.Windows.Forms.Form
	{
		public frmMPIPHPBusinessTier()
		{
			InitializeComponent();
        }

        private TabControl tbcSagitecServer;
        private TabPage tbpMetaData;
        private TabPage tbpDbCache;
        private TabPage tbpBusinessTier;
        private TextBox txbError;
        private Label lblErrorDetails;
        private Label lblBTMessage;
        private Label lblMessageCaption;
        private ListView lsvMetaCacheInfo;
        private ColumnHeader clmName;
        private ColumnHeader clmReplaced;
        private ColumnHeader clmType;
        private ColumnHeader clmDirectory;
        private Label lblMDC;
        private Label label2;
        private Button btnRefreshMetaData;
        private ListView lsvDbCache;
        private ColumnHeader columnHeader1;
        private ColumnHeader clmRowCount;
        private ColumnHeader clmQuery;
        private Label lblDbCacheMessage;
        private Label label4;
        private Button btnRefreshDB;
        private StatusStrip stsDetails;
        private ToolStripStatusLabel lblStatusBar;
        private ColumnHeader clmStatus;
        private busMQRequestHandler iobjRequestHandler = null;
        private Button btnRefreshRules;
        //FM upgrade: 6.0.0.30 changes
        private ICollection<ServiceHost> icolServiceHosts;

        #region Windows forms designer code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.tbcSagitecServer = new System.Windows.Forms.TabControl();
            this.tbpMetaData = new System.Windows.Forms.TabPage();
            this.lsvMetaCacheInfo = new System.Windows.Forms.ListView();
            this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmReplaced = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmDirectory = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmStatus = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblMDC = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRefreshMetaData = new System.Windows.Forms.Button();
            this.tbpDbCache = new System.Windows.Forms.TabPage();
            this.lsvDbCache = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmRowCount = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.clmQuery = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblDbCacheMessage = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnRefreshDB = new System.Windows.Forms.Button();
            this.tbpBusinessTier = new System.Windows.Forms.TabPage();
            this.txbError = new System.Windows.Forms.TextBox();
            this.lblErrorDetails = new System.Windows.Forms.Label();
            this.lblBTMessage = new System.Windows.Forms.Label();
            this.lblMessageCaption = new System.Windows.Forms.Label();
            this.stsDetails = new System.Windows.Forms.StatusStrip();
            this.lblStatusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnRefreshRules = new System.Windows.Forms.Button();
            this.tbcSagitecServer.SuspendLayout();
            this.tbpMetaData.SuspendLayout();
            this.tbpDbCache.SuspendLayout();
            this.tbpBusinessTier.SuspendLayout();
            this.stsDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbcSagitecServer
            // 
            this.tbcSagitecServer.Controls.Add(this.tbpMetaData);
            this.tbcSagitecServer.Controls.Add(this.tbpDbCache);
            this.tbcSagitecServer.Controls.Add(this.tbpBusinessTier);
            this.tbcSagitecServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcSagitecServer.Location = new System.Drawing.Point(0, 0);
            this.tbcSagitecServer.Name = "tbcSagitecServer";
            this.tbcSagitecServer.SelectedIndex = 0;
            this.tbcSagitecServer.Size = new System.Drawing.Size(1051, 532);
            this.tbcSagitecServer.TabIndex = 6;
            // 
            // tbpMetaData
            // 
            this.tbpMetaData.Controls.Add(this.lsvMetaCacheInfo);
            this.tbpMetaData.Controls.Add(this.lblMDC);
            this.tbpMetaData.Controls.Add(this.label2);
            this.tbpMetaData.Controls.Add(this.btnRefreshMetaData);
            this.tbpMetaData.Location = new System.Drawing.Point(4, 22);
            this.tbpMetaData.Name = "tbpMetaData";
            this.tbpMetaData.Padding = new System.Windows.Forms.Padding(3);
            this.tbpMetaData.Size = new System.Drawing.Size(1043, 506);
            this.tbpMetaData.TabIndex = 0;
            this.tbpMetaData.Text = "MetaData";
            this.tbpMetaData.UseVisualStyleBackColor = true;
            // 
            // lsvMetaCacheInfo
            // 
            this.lsvMetaCacheInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvMetaCacheInfo.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmReplaced,
            this.clmType,
            this.clmDirectory,
            this.clmStatus});
            this.lsvMetaCacheInfo.Location = new System.Drawing.Point(11, 73);
            this.lsvMetaCacheInfo.Name = "lsvMetaCacheInfo";
            this.lsvMetaCacheInfo.Size = new System.Drawing.Size(1024, 412);
            this.lsvMetaCacheInfo.TabIndex = 14;
            this.lsvMetaCacheInfo.UseCompatibleStateImageBehavior = false;
            this.lsvMetaCacheInfo.View = System.Windows.Forms.View.Details;
            this.lsvMetaCacheInfo.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lsvMetaCacheInfo_ColumnClick);
            // 
            // clmName
            // 
            this.clmName.Text = "Name";
            this.clmName.Width = 200;
            // 
            // clmReplaced
            // 
            this.clmReplaced.Text = "Replaced";
            this.clmReplaced.Width = 99;
            // 
            // clmType
            // 
            this.clmType.Text = "Type";
            this.clmType.Width = 100;
            // 
            // clmDirectory
            // 
            this.clmDirectory.Text = "Directory";
            this.clmDirectory.Width = 382;
            // 
            // clmStatus
            // 
            this.clmStatus.Text = "Status";
            this.clmStatus.Width = 250;
            // 
            // lblMDC
            // 
            this.lblMDC.AutoSize = true;
            this.lblMDC.Location = new System.Drawing.Point(73, 10);
            this.lblMDC.Name = "lblMDC";
            this.lblMDC.Size = new System.Drawing.Size(33, 13);
            this.lblMDC.TabIndex = 12;
            this.lblMDC.Text = "None";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Message : ";
            // 
            // btnRefreshMetaData
            // 
            this.btnRefreshMetaData.Location = new System.Drawing.Point(11, 35);
            this.btnRefreshMetaData.Name = "btnRefreshMetaData";
            this.btnRefreshMetaData.Size = new System.Drawing.Size(127, 28);
            this.btnRefreshMetaData.TabIndex = 10;
            this.btnRefreshMetaData.Text = "Refresh MetaData";
            this.btnRefreshMetaData.Click += new System.EventHandler(this.btnRefreshMetaData_Click);
            // 
            // tbpDbCache
            // 
            this.tbpDbCache.Controls.Add(this.lsvDbCache);
            this.tbpDbCache.Controls.Add(this.lblDbCacheMessage);
            this.tbpDbCache.Controls.Add(this.label4);
            this.tbpDbCache.Controls.Add(this.btnRefreshDB);
            this.tbpDbCache.Location = new System.Drawing.Point(4, 22);
            this.tbpDbCache.Name = "tbpDbCache";
            this.tbpDbCache.Padding = new System.Windows.Forms.Padding(3);
            this.tbpDbCache.Size = new System.Drawing.Size(1043, 506);
            this.tbpDbCache.TabIndex = 1;
            this.tbpDbCache.Text = "DB Cache";
            this.tbpDbCache.UseVisualStyleBackColor = true;
            // 
            // lsvDbCache
            // 
            this.lsvDbCache.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lsvDbCache.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.clmRowCount,
            this.clmQuery});
            this.lsvDbCache.FullRowSelect = true;
            this.lsvDbCache.Location = new System.Drawing.Point(11, 68);
            this.lsvDbCache.Name = "lsvDbCache";
            this.lsvDbCache.Size = new System.Drawing.Size(1024, 420);
            this.lsvDbCache.TabIndex = 14;
            this.lsvDbCache.UseCompatibleStateImageBehavior = false;
            this.lsvDbCache.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 123;
            // 
            // clmRowCount
            // 
            this.clmRowCount.Text = "No. Of Rows";
            this.clmRowCount.Width = 80;
            // 
            // clmQuery
            // 
            this.clmQuery.Text = "Query Details";
            this.clmQuery.Width = 836;
            // 
            // lblDbCacheMessage
            // 
            this.lblDbCacheMessage.AutoSize = true;
            this.lblDbCacheMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDbCacheMessage.Location = new System.Drawing.Point(73, 12);
            this.lblDbCacheMessage.Name = "lblDbCacheMessage";
            this.lblDbCacheMessage.Size = new System.Drawing.Size(10, 13);
            this.lblDbCacheMessage.TabIndex = 12;
            this.lblDbCacheMessage.Text = "l";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Message : ";
            // 
            // btnRefreshDB
            // 
            this.btnRefreshDB.Location = new System.Drawing.Point(11, 28);
            this.btnRefreshDB.Name = "btnRefreshDB";
            this.btnRefreshDB.Size = new System.Drawing.Size(151, 34);
            this.btnRefreshDB.TabIndex = 10;
            this.btnRefreshDB.Text = "Refresh Database Cache";
            this.btnRefreshDB.Click += new System.EventHandler(this.btnRefreshDB_Click);
            // 
            // tbpBusinessTier
            // 
            this.tbpBusinessTier.Controls.Add(this.btnRefreshRules);
            this.tbpBusinessTier.Controls.Add(this.txbError);
            this.tbpBusinessTier.Controls.Add(this.lblErrorDetails);
            this.tbpBusinessTier.Controls.Add(this.lblBTMessage);
            this.tbpBusinessTier.Controls.Add(this.lblMessageCaption);
            this.tbpBusinessTier.Location = new System.Drawing.Point(4, 22);
            this.tbpBusinessTier.Name = "tbpBusinessTier";
            this.tbpBusinessTier.Padding = new System.Windows.Forms.Padding(3);
            this.tbpBusinessTier.Size = new System.Drawing.Size(1043, 506);
            this.tbpBusinessTier.TabIndex = 2;
            this.tbpBusinessTier.Text = "Business Tier";
            this.tbpBusinessTier.UseVisualStyleBackColor = true;
            // 
            // txbError
            // 
            this.txbError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbError.Location = new System.Drawing.Point(8, 82);
            this.txbError.Multiline = true;
            this.txbError.Name = "txbError";
            this.txbError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txbError.Size = new System.Drawing.Size(1032, 421);
            this.txbError.TabIndex = 15;
            // 
            // lblErrorDetails
            // 
            this.lblErrorDetails.AutoSize = true;
            this.lblErrorDetails.Location = new System.Drawing.Point(17, 45);
            this.lblErrorDetails.Name = "lblErrorDetails";
            this.lblErrorDetails.Size = new System.Drawing.Size(70, 13);
            this.lblErrorDetails.TabIndex = 14;
            this.lblErrorDetails.Text = "Error Details :";
            // 
            // lblBTMessage
            // 
            this.lblBTMessage.AutoSize = true;
            this.lblBTMessage.Location = new System.Drawing.Point(97, 18);
            this.lblBTMessage.Name = "lblBTMessage";
            this.lblBTMessage.Size = new System.Drawing.Size(33, 13);
            this.lblBTMessage.TabIndex = 12;
            this.lblBTMessage.Text = "None";
            // 
            // lblMessageCaption
            // 
            this.lblMessageCaption.AutoSize = true;
            this.lblMessageCaption.Location = new System.Drawing.Point(17, 18);
            this.lblMessageCaption.Name = "lblMessageCaption";
            this.lblMessageCaption.Size = new System.Drawing.Size(59, 13);
            this.lblMessageCaption.TabIndex = 11;
            this.lblMessageCaption.Text = "Message : ";
            // 
            // stsDetails
            // 
            this.stsDetails.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusBar});
            this.stsDetails.Location = new System.Drawing.Point(0, 510);
            this.stsDetails.Name = "stsDetails";
            this.stsDetails.Size = new System.Drawing.Size(1051, 22);
            this.stsDetails.TabIndex = 16;
            this.stsDetails.Text = "statusStrip1";
            // 
            // lblStatusBar
            // 
            this.lblStatusBar.Name = "lblStatusBar";
            this.lblStatusBar.Size = new System.Drawing.Size(118, 17);
            this.lblStatusBar.Text = "toolStripStatusLabel1";
            // 
            // btnRefreshRules
            // 
            this.btnRefreshRules.Location = new System.Drawing.Point(907, 0);
            this.btnRefreshRules.Name = "btnRefreshRules";
            this.btnRefreshRules.Size = new System.Drawing.Size(136, 40);
            this.btnRefreshRules.TabIndex = 21;
            this.btnRefreshRules.Text = "Refresh Rules";
            this.btnRefreshRules.UseVisualStyleBackColor = true;
            this.btnRefreshRules.Click += new System.EventHandler(this.btnRefreshRules_Click);
            // 
            // frmMPIPHPBusinessTier
            // 
            this.ClientSize = new System.Drawing.Size(1051, 532);
            this.Controls.Add(this.stsDetails);
            this.Controls.Add(this.tbcSagitecServer);
            this.Name = "frmMPIPHPBusinessTier";
            this.Text = "MPIPHP Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMPIPHPBusinessTier_FormClosing);
            this.Load += new System.EventHandler(this.frmMPIPHPBusinessTier_Load);
            this.Shown += new System.EventHandler(this.frmMPIPHPBusinessTier_Shown);
            this.tbcSagitecServer.ResumeLayout(false);
            this.tbpMetaData.ResumeLayout(false);
            this.tbpMetaData.PerformLayout();
            this.tbpDbCache.ResumeLayout(false);
            this.tbpDbCache.PerformLayout();
            this.tbpBusinessTier.ResumeLayout(false);
            this.tbpBusinessTier.PerformLayout();
            this.stsDetails.ResumeLayout(false);
            this.stsDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        void frmMPIPHPBusinessTier_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (iobjRequestHandler != null)
                iobjRequestHandler.StopProcessing();
        }

	#endregion

		private void frmMPIPHPBusinessTier_Load(object sender, EventArgs e)
		{
            // srvMetaDataCache.idlgAfterRefresh = new srvMetaDataCache.AfterRefresh(AfterRefresh);
            icolServiceHosts = new Collection<ServiceHost>();
        }

        private void AfterRefresh()
        {
            lblMDC.Text = "MetaData cache successfully refreshed at " + DateTime.Now.ToString();
        }

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

        private void btnRefreshMetaData_Click(object sender, EventArgs e)
        {
            srvMPIPHPMetaDataCache.LoadXMLCache();
            lblMDC.Text = "MetaData cache successfully refreshed at " + DateTime.Now.ToString();
        }

        private void lsvMetaCacheInfo_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            lsvMetaCacheInfo.ListViewItemSorter = new ListViewItemComparer(e.Column);
            lsvMetaCacheInfo.Sort();
        }

        // Implements the manual sorting of items by columns.
        class ListViewItemComparer : IComparer
        {
            private int col;
            public ListViewItemComparer()
            {
                col = 0;
            }
            public ListViewItemComparer(int column)
            {
                col = column;
            }
            public int Compare(object x, object y)
            {
                return String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
            }
        }

        private void btnRefreshDB_Click(object sender, EventArgs e)
        {
            //FM upgrade: 6.0.0.30 changes
            //srvMPIPHPDBCache.idstDBCache.Clear();
            srvMPIPHPDBCache.idctDBCache.Clear();
            srvMPIPHPDBCache.LoadCacheInfo();

            lblDbCacheMessage.Text = "All queries successfully refreshed and cached at " + DateTime.Now.ToString();
        }

        private void frmMPIPHPBusinessTier_Shown(object sender, EventArgs e)
        {
            //FM upgrade: 6.0.0.30 changes
            try
            {
                System.ServiceModel.Channels.Binding lntbBinding = ServiceHelper.GetNetTcpBinding(true);
                lntbBinding.ReceiveTimeout = TimeSpan.Parse(ConfigurationManager.AppSettings["WCFRECEIVETIMEOUT"]);
                string lstrBaseUrl = MPIPHP.Common.ApplicationSettings.Instance.BusinessTierUrl;

                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IMetaDataCache>(typeof(srvMPIPHPMetaDataCache), string.Format(lstrBaseUrl, utlConstants.istrMetaDataCache), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IDBCache>(typeof(srvMPIPHPDBCache), string.Format(lstrBaseUrl, utlConstants.istrDBCache), lntbBinding));
                //icolServiceHosts.Add(ServiceHelper.GetServiceHost <IsrvSystemManagement> (typeof(srvSystemManagement), string.Format(lstrBaseUrl, "srvSystemManagement"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCommon), string.Format(lstrBaseUrl, "srvCommon"), lntbBinding));

                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvAdmin), string.Format(lstrBaseUrl, "srvAdmin"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvAudit), string.Format(lstrBaseUrl, "srvAudit"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCalculation), string.Format(lstrBaseUrl, "srvCalculation"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvCorrespondence), string.Format(lstrBaseUrl, "srvCorrespondence"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvEligibilityRules), string.Format(lstrBaseUrl, "srvEligibilityRules"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvIAPAllocation), string.Format(lstrBaseUrl, "srvIAPAllocation"), lntbBinding));
                //icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvMPIPHP), string.Format(lstrBaseUrl, "srvMPIPHP"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvMPIPHPMSS), string.Format(lstrBaseUrl, "srvMPIPHPMSS"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvOrganization), string.Format(lstrBaseUrl, "srvOrganization"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvPayeeAccount), string.Format(lstrBaseUrl, "srvPayeeAccount"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvPerson), string.Format(lstrBaseUrl, "srvPerson"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvReports), string.Format(lstrBaseUrl, "srvReports"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvScout), string.Format(lstrBaseUrl, "srvScout"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvUserActivity), string.Format(lstrBaseUrl, "srvUserActivity"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvWorkflow), string.Format(lstrBaseUrl, "srvWorkflow"), lntbBinding));
                icolServiceHosts.Add(ServiceHelper.GetServiceHost<IBusinessTier>(typeof(srvBPMN), string.Format(lstrBaseUrl, "srvBPMN"), lntbBinding));  
            }
            catch (Exception E)
            {
                lblBTMessage.Text = "Business Tier might already be running";
                txbError.Text = E.Message;
                return;
            }

            lblStatusBar.Text = "Loading MetaData Cache....";
            Application.DoEvents();
            srvMPIPHPMetaDataCache.LoadXMLCache();
            foreach (stcCacheInfo lstcCacheInfo in srvMPIPHPMetaDataCache.icolCacheInfo)
            {
                ListViewItem lvi = new ListViewItem(lstcCacheInfo.istrName);
                lvi.SubItems.Add(lstcCacheInfo.istrReplaced);
                lvi.SubItems.Add(lstcCacheInfo.istrType);
                lvi.SubItems.Add(lstcCacheInfo.istrDirectory);
                lvi.SubItems.Add(lstcCacheInfo.istrStatus);
                lvi.ToolTipText = lstcCacheInfo.istrStatus;
                lsvMetaCacheInfo.Items.Add(lvi);
            }
            lblMDC.Text = "All XML files successfully loaded and cached at " + DateTime.Now.ToString();
            lblStatusBar.Text = "MetaData Cache Loaded";
            Application.DoEvents();
            System.Threading.Thread.Sleep(2000);

            lblStatusBar.Text = "Loading Database Cache....";
            tbcSagitecServer.SelectedTab = tbpDbCache;
            Application.DoEvents();
            bool lblnSuccess = srvMPIPHPDBCache.LoadCacheInfo();
            if (lblnSuccess)
            {
                int lintCounter = 0;
                foreach (KeyValuePair<string, string> query in srvMPIPHPDBCache.iarrDBCacheInfo)
                {
                    ListViewItem lvi = new ListViewItem(query.Key);
                    lvi.SubItems.Add((srvMPIPHPDBCache.iarrRowCount[lintCounter]).ToString().PadLeft(12));
                    lvi.SubItems.Add(query.Value);
                    lsvDbCache.Items.Add(lvi);
                    lintCounter++;
                }

                lblDbCacheMessage.Text = "All queries successfully executed and cached at " + DateTime.Now.ToString();
                Application.DoEvents();
                System.Threading.Thread.Sleep(2000);
            }
            else
            {
                MessageBox.Show(srvMPIPHPDBCache.istrResult);
                Close();
            }

            lblStatusBar.Text = "DB Cache Loaded";
            System.Threading.Thread.Sleep(2000);

            tbcSagitecServer.SelectedTab = tbpBusinessTier;
            lblStatusBar.Text = "Completed Intialization";
            Application.DoEvents();
            try
            {
                RemotingConfiguration.Configure("MPIPHPBusinessTier.exe.config", false);
            }
            catch (Exception E)
            {
                lblBTMessage.Text = "Business Tier might already be running";
                txbError.Text = E.Message;
                return;
            }

            try
            {
                iobjRequestHandler = new busMQRequestHandler(utlConstants.SystemQueue);
                iobjRequestHandler.StartProcessing();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Following Error occured while instantiating AuditLogQueue processer : " + ex.Message);
            }
            CompileRules();
            lblStatusBar.Text = "Ready";
            //FM upgrade: 6.0.0.30 changes
            srvMainDB.iblnBTReady = true;
        }

        /// <summary>
        /// Compile Rules
        /// </summary>
        private static void CompileRules()
        {
            try
            {
                ParsingResult lobjLoadResult = srvMainDB.LoadRulesAndExpressions();
                StringBuilder lstrbRulesError = new StringBuilder();
                RulesEngine.CompileRules(true);
 
                foreach (utlRuleMessage lobjMessage in lobjLoadResult.ilstErrors)
                {
                    string lstrError = string.Empty;
                    lstrbRulesError.AppendLine(String.Format("Object : {0} Rule : {1} : Message : {2}", lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }

                if (lstrbRulesError.ToString().Length > 0)
                    MessageBox.Show(lstrbRulesError.ToString());
            }
            catch (Exception er)
            {
                MessageBox.Show("error occured in loading business rules " + er.Message + "\n" + er.StackTrace.ToString());
            }
        }

        private void btnRefreshRules_Click(object sender, EventArgs e)
        {
            try
            {
                btnRefreshMetaData_Click(sender, e);
                CompileRules();
            }
            catch (Exception er)
            {
                MessageBox.Show("error occured in loading business rules " + er.Message + "\n" + er.StackTrace.ToString());
            }
        }
    }
}
