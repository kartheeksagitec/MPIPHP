using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using MPIPHP.CustomDataObjects;
using Sagitec.DBUtility;
using MPIPHP.DataObjects;
using System.Collections.ObjectModel;

namespace Encryption
{
    public partial class FrmEncrypt : Form
    {
        public FrmEncrypt()
        {
            InitializeComponent();
            Fields pageFields = new Fields();
            pnlEncrypt.Controls.Add(pageFields);
            pageFields.Dock = DockStyle.Fill;

        }

        

        public void EncryptSSN()
        {
            try
            {
                IDbConnection lIDbConnection = null;

                if (lIDbConnection == null)
                {
                    lIDbConnection = DBFunction.GetDBConnection();

                }
                string strQuery = "select * from sgt_person";
                DataTable ldtbResult = DBFunction.DBSelect(strQuery, lIDbConnection);
                if (ldtbResult.Rows.Count > 0)
                {
                    foreach (DataRow ldtr in ldtbResult.Rows)
                    {
                        string strUpdateSql = "UPDATE SGT_PERSON SET SSN = @SSN WHERE PERSON_ID = @ID";
                        int lintPersonId = Convert.ToInt32(ldtr[enmPerson.person_id.ToString()]);
                        string strSSN = Convert.ToString(ldtr[enmPerson.ssn.ToString()]);

                        if (string.IsNullOrEmpty(strSSN) && strSSN.Length != 9)
                            continue;

                        strSSN = HelperFunction.SagitecEncryptAES(strSSN);

                        

                        Collection<IDbDataParameter> larrParameter = new Collection<IDbDataParameter>();
                        IDbDataParameter lobjDbParameter = DBFunction.GetDBParameter();
                        lobjDbParameter.ParameterName = "@SSN";
                        lobjDbParameter.DbType = DbType.String;
                        lobjDbParameter.Value = strSSN;
                        larrParameter.Add(lobjDbParameter);

                        lobjDbParameter = DBFunction.GetDBParameter();
                        lobjDbParameter.ParameterName = "@ID";
                        lobjDbParameter.DbType = DbType.Int32;
                        lobjDbParameter.Value = lintPersonId;
                        larrParameter.Add(lobjDbParameter);
                        DBFunction.DBNonQuery(strUpdateSql, larrParameter, lIDbConnection, null);
                    }
                }

            }
            catch(Exception ex)
            {

            }

        }

        private void btnSSN_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Updating SSN...";
            Application.DoEvents();

            try
            {
                EncryptSSN();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "SSN Updated";
            }
        }

        private void btnDOB_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            lblStatus.Text = "Updating DOB...";
            Application.DoEvents();

            try
            {
                EncryptDOB();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
                lblStatus.Text = "DOB Updated";
            }
        }

        public void EncryptDOB()
        {

            try
            {
                IDbConnection lIDbConnection = null;

                if (lIDbConnection == null)
                {
                    lIDbConnection = DBFunction.GetDBConnection();

                }
                string strQuery = "select * from sgt_person";
                DataTable ldtbResult = DBFunction.DBSelect(strQuery, lIDbConnection);
                if (ldtbResult.Rows.Count > 0)
                {
                    foreach (DataRow ldtr in ldtbResult.Rows)
                    {
                        string strUpdateSql = "UPDATE SGT_PERSON SET DATE_OF_BIRTH = @DOB WHERE PERSON_ID = @ID";
                        int lintPersonId = Convert.ToInt32(ldtr[enmPerson.person_id.ToString()]);
                        string strDOB = Convert.ToString(ldtr[enmPerson.date_of_birth.ToString()]);
                        //Length to check if it is already encrypted.
                        if (string.IsNullOrEmpty(strDOB) && strDOB.Length > 20)
                            continue;

                        strDOB = HelperFunction.SagitecEncryptAES(strDOB);

                        Collection<IDbDataParameter> larrParameter = new Collection<IDbDataParameter>();
                        IDbDataParameter lobjDbParameter = DBFunction.GetDBParameter();
                        lobjDbParameter.ParameterName = "@DOB";
                        lobjDbParameter.DbType = DbType.String;
                        lobjDbParameter.Value = strDOB;
                        larrParameter.Add(lobjDbParameter);

                        lobjDbParameter = DBFunction.GetDBParameter();
                        lobjDbParameter.ParameterName = "@ID";
                        lobjDbParameter.DbType = DbType.Int32;
                        lobjDbParameter.Value = lintPersonId;
                        larrParameter.Add(lobjDbParameter);
                        DBFunction.DBNonQuery(strUpdateSql, larrParameter, lIDbConnection, null);
                    }
                }
                if (lIDbConnection.State == ConnectionState.Open)
                {
                    lIDbConnection.Close();
                }

            }
            catch(Exception ex)
            {

            }

        }

    }
}
