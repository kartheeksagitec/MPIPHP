using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using MPIPHP.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.Common;
using Sagitec.MetaDataCache;
using Sagitec.DBCache;
using Sagitec.Rules;

namespace MPIPHP.MPIPHPJobService
{
    public partial class MPIPHPJobService : Form
    {
        //static string istrUserId = busConstant.BATCH_USER;
        private bool lblnServiceStarted = false;
        private bool iblnBatchStartedServer = false;
        //public utlPassInfo iobjPassInfo, iobjPassInfoLog;
        public MPIPHPJobService()
        {
            InitializeComponent();
            FormInitialize();

        }

        private JobMaster lobjWorker;

        private void StartServers()
        {
            //FM upgrade: 6.0.0.30 changes - Remove ServiceHelper.Initialize wherever used in the project
            //ServiceHelper.Initialize(utlServiceType.Local);
            srvMetaDataCache.LoadXMLCache();
            bool lblnSuccess = srvDBCache.LoadCacheInfo();
            if (!lblnSuccess)
            {
                MessageBox.Show(srvDBCache.istrResult);
                Close();
            }

            utlPassInfo.iobjPassInfo = new utlPassInfo();

            RulesEngine.Initialize(utlExecutionMode.Application);
            ParsingResult lobjResult = RulesEngine.LoadRulesAndExpressions(utlPassInfo.iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);

            if (lobjResult.ilstErrors.Count > 0)
            {
                StringBuilder lstrbRuleErrors = new StringBuilder();
                lstrbRuleErrors.AppendLine("Rules Errors: ");
                foreach (utlRuleMessage lobjMessage in lobjResult.ilstErrors)
                {
                    lstrbRuleErrors.AppendLine(string.Format("Object ID: {0}; Rule: {1}; Message: {2}",
                        lobjMessage.istrObjectID, lobjMessage.istrRuleID, lobjMessage.istrMessage));
                }
                MessageBox.Show(lstrbRuleErrors.ToString());
            }
        }

        private void FormInitialize()
        {
            StartServers();
            try
            {
                lobjWorker = (JobMaster)WorkerFactory.GetWorker(WorkerType.Master);
                lobjWorker.Start();
                lblnServiceStarted = true;
                lblInformation.Text = "Job Service Started : " + DateTime.Now;
            }
            catch (Exception ex)
            {
                lblInformation.Text = ex.Message;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (lblnServiceStarted)
                {
                    lobjWorker.Stop();
                    lblInformation.Text = "Job Service Stopped : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                lblInformation.Text = ex.Message;
            }
        }

        private void MPIPHPJobService_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void MPIPHPJobService_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lblnServiceStarted)
            {
                lobjWorker.Stop();
            }
        }
    }
}
