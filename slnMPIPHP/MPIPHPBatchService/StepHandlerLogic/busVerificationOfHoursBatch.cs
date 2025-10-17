using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using Sagitec.DataObjects;
using System.Linq;
using Sagitec.CustomDataObjects;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.ExceptionPub;
using MPIPHP.BusinessObjects;
using System.Threading.Tasks;
using Sagitec.Interface;

namespace MPIPHPJobService
{
    public class busVerificationOfHoursBatch : busBatchHandler
    {
        #region Properties
        private object iobjLock = null;
        public Collection<busPayeeAccount> iclbPayeeAccount { get; set; }
        public int PAYEE_ACCOUNT_ID { get; set; }
        public busPaymentSchedule ibusPaymentSchedule { get; set; }
        public DateTime idtLastBenefitPaymentDate { get; set; }
        public DateTime idtNextBenefitPaymentDate { get; set; }
        string lstrReportPrefixPaymentScheduleID = string.Empty;
        #endregion

        #region VERIFICATION_Of_HOURS_BATCH
        public void VerificationOfHoursBatch()
        {
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            busBase lobjBase = new busBase();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
            iobjLock = new object();

            DataTable ldtbFinalTableForReport = new DataTable();
            idtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(busConstant.IAP_PLAN_ID);
           
            idtNextBenefitPaymentDate = busGlobalFunctions.GetPaymentDayForIAP(idtLastBenefitPaymentDate.AddDays(7));
            
            if (idtNextBenefitPaymentDate != DateTime.MinValue)
            {
                DataTable ldtPayeeAccountData = busBase.Select("cdoPayeeAccount.GetPayeeAccountsForVerificationOfHoursBatch", new object[1] { idtNextBenefitPaymentDate });

                if (ldtPayeeAccountData.Rows.Count > 0)
                {
                    ArrayList aarrResultReport = new ArrayList();
                    DataTable ldtbPayeeAccountsForReport = new DataTable();
                    ldtbPayeeAccountsForReport = ldtPayeeAccountData.Clone();
                    
                    iclbPayeeAccount = lobjBase.GetCollection<busPayeeAccount>(ldtPayeeAccountData, "icdoPayeeAccount");                  
                    //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                    ParallelOptions po = new ParallelOptions();
                    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                    Parallel.ForEach(iclbPayeeAccount, po, (lbusPayeeAccount, loopState) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "VerificationOfHoursBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        ArrayList aarrResult = new ArrayList();
                        Hashtable ahtbQueryBkmarks = new Hashtable();
                        DateTime ldtFromDate = DateTime.MinValue;
                        DateTime ldtToDate = DateTime.MinValue;

                        if (lbusPayeeAccount.ibusParticipant == null)
                        {
                            lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                            lbusPayeeAccount.ibusParticipant.FindPerson(lbusPayeeAccount.icdoPayeeAccount.person_id);
                            if (lbusPayeeAccount.ibusParticipant != null)
                            {
                                lbusPayeeAccount.LoadBenefitDetails();
                                lbusPayeeAccount.LoadDRODetails();
                            }
                        }

                        utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                        string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                        SqlParameter[] parameters = new SqlParameter[3];
                        SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                        SqlParameter param2 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);
                        SqlParameter param3 = new SqlParameter("@TO_DATE", DbType.DateTime);

                        param1.Value = lbusPayeeAccount.ibusParticipant.icdoPerson.istrSSNNonEncrypted;
                        parameters[0] = param1;
                        // Kunal : MPI Wanted to do it this way.[else everyehere it should be a day after end of the week of retirement.
                        param2.Value = busGlobalFunctions.GetFirstDayOfWeek(lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate);
                        parameters[1] = param2;

                        param3.Value = busGlobalFunctions.GetLastDayOfWeek(lbusPayeeAccount.icdoPayeeAccount.benefit_begin_date);
                        parameters[2] = param3;

                        DataTable ldtbIAPInfo = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataAfterRetirement", astrLegacyDBConnetion, null, parameters);

                        if (ldtbIAPInfo.Rows.Count > 0)
                        {
                            var distinctRows = (from DataRow dRow in ldtbIAPInfo.Rows
                                                where dRow["OldEmployerNum"].IsNotNull()
                                                select new
                                                {
                                                    OldEmployerNum = dRow["OldEmployerNum"],
                                                    EmployerName = dRow["EmployerName"],
                                                    Address1 = dRow["Address1"],
                                                    City = dRow["City"],
                                                    Address2 = dRow["Address2"],
                                                    State = dRow["State"],
                                                    Contact1 = dRow["Contact1"],
                                                    PostalCode = dRow["PostalCode"],
                                                    Contact2 = dRow["Contact2"],
                                                    Street = dRow["Street"],
                                                }).Distinct();

                            foreach (var ldtRow in distinctRows)
                            {
                                decimal ldecIAPHours = (from DataRow drow in ldtbIAPInfo.Rows
                                                        where Convert.ToString(drow["OldEmployerNum"]) == Convert.ToString(ldtRow.OldEmployerNum)
                                                        select
                                                           Convert.ToDecimal(drow["IAPHours"])
                                                     ).Sum();

                                if (ldecIAPHours > 0)
                                {
                                    lbusPayeeAccount.idecIAPHours4QtrAlloc = ldecIAPHours;
                                }

                                ldtFromDate = (from DataRow drow in ldtbIAPInfo.Rows
                                               where drow["OldEmployerNum"].ToString().ToLower().Trim() == ldtRow.OldEmployerNum.ToString().ToLower().Trim()
                                               select Convert.ToDateTime(drow["FromDate"])
                                                       ).OrderBy(t => t.Date).First();

                                ldtToDate = (from DataRow drow in ldtbIAPInfo.Rows
                                             where drow["OldEmployerNum"].ToString().ToLower().Trim() == ldtRow.OldEmployerNum.ToString().ToLower().Trim()
                                             select Convert.ToDateTime(drow["ToDate"])
                                                     ).OrderByDescending(t => t.Date).First();

                                if (ldtFromDate != DateTime.MinValue)
                                {
                                    lbusPayeeAccount.idtFromDate = ldtFromDate;
                                    lbusPayeeAccount.istrFromDate = lbusPayeeAccount.idtFromDate.ToString("MMMM") + " " + lbusPayeeAccount.idtFromDate.Year;
                                }

                                var colIAPHours = (from DataRow drow in ldtbIAPInfo.Rows
                                                   where Convert.ToString(drow["OldEmployerNum"]) == Convert.ToString(ldtRow.OldEmployerNum)
                                                   select new
                                                   {
                                                       IAPHours = Convert.ToDecimal(drow["IAPHours"]),
                                                       FromDate = Convert.ToDateTime(drow["FromDate"])
                                                   }
                                                    );

                                lbusPayeeAccount.iclbPayeeAccountForTable = new Collection<busPayeeAccount>();
                                foreach (var ldtIAPRow in colIAPHours)
                                {
                                    busPayeeAccount lbusPayeeAccountForTable = new busPayeeAccount();
                                    lbusPayeeAccountForTable.idtFromDtCorr16 = Convert.ToDateTime(ldtIAPRow.FromDate);
                                    lbusPayeeAccountForTable.istrFormattedDate = Convert.ToString(Convert.ToDateTime(ldtIAPRow.FromDate).ToString("d"));
                                    lbusPayeeAccount.istrFromDtCorr16 = lbusPayeeAccountForTable.idtFromDtCorr16.ToString("MMMM") + " " + lbusPayeeAccountForTable.idtFromDtCorr16.Year;

                                    lbusPayeeAccountForTable.istr1 = "Were any of the " + Convert.ToDecimal(ldtIAPRow.IAPHours) + " hours reported p/e " + lbusPayeeAccountForTable.istrFormattedDate +
                                                                     " worked in " + lbusPayeeAccount.istrFromDtCorr16 + "?  " + busConstant.YES_CAPS + "  " + busConstant.NO_CAPS;

                                    lbusPayeeAccount.iclbPayeeAccountForTable.Add(lbusPayeeAccountForTable);
                                }

                                lbusPayeeAccount.idtVerificationHoursEffectiveDate = lbusPayeeAccount.icdoPayeeAccount.idtRetireMentDate;
                                lbusPayeeAccount.istrVerificationHoursEffectiveDate = lbusPayeeAccount.idtVerificationHoursEffectiveDate.ToString("MMMM") + " " + lbusPayeeAccount.idtVerificationHoursEffectiveDate.Year;

                                if (ldtToDate != DateTime.MinValue)
                                {
                                    lbusPayeeAccount.idtToDate = ldtToDate;
                                    lbusPayeeAccount.istrToDate = String.Format("{0:MMMM dd,yyyy}", lbusPayeeAccount.idtToDate);
                                }

                                if (ldtRow.EmployerName.IsNotNull())
                                {
                                    lbusPayeeAccount.istrEmployerName = Convert.ToString(ldtRow.EmployerName);
                                }
                                if (ldtRow.Address1.IsNotNull())
                                {
                                    lbusPayeeAccount.istrAddress1 = Convert.ToString(ldtRow.Address1);
                                }
                                if (ldtRow.City.IsNotNull())
                                {
                                    lbusPayeeAccount.istrCity1 = Convert.ToString(ldtRow.City);
                                }
                                if (ldtRow.Address2.IsNotNull())
                                {
                                    lbusPayeeAccount.istrAddress2 = Convert.ToString(ldtRow.Address2);
                                }
                                if (ldtRow.State.IsNotNull())
                                {
                                    lbusPayeeAccount.istrState = Convert.ToString(ldtRow.State);
                                }
                                if (ldtRow.Contact1.IsNotNull())
                                {
                                    lbusPayeeAccount.istrContact1 = Convert.ToString(ldtRow.Contact1);
                                    if (lbusPayeeAccount.istrContact1 != "")
                                    {
                                        lbusPayeeAccount.istrContactTRUE = busConstant.FLAG_YES;
                                    }
                                }
                                if (ldtRow.PostalCode.IsNotNull())
                                {
                                    lbusPayeeAccount.istrPostalCode = Convert.ToString(ldtRow.PostalCode);
                                }
                                if (ldtRow.Contact2.IsNotNull())
                                {
                                    lbusPayeeAccount.istrContact2 = Convert.ToString(ldtRow.Contact2);
                                }
                                if (ldtRow.Street.IsNotNull())
                                {
                                    lbusPayeeAccount.istrStreet = Convert.ToString(ldtRow.Street);
                                }

                                lbusPayeeAccount.idecIAPHoursSUM = lbusPayeeAccount.idecIAPHoursSUM + lbusPayeeAccount.idecIAPHours4QtrAlloc;

                                if (lbusPayeeAccount.idecIAPHoursSUM > 0)
                                {
                                    //Create Correspondence.                            
                                    aarrResult.Add(lbusPayeeAccount);
                                    this.CreateCorrespondence(busConstant.NOTIFICATION_OF_HOURS_LETTER_TO_PAYEE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                                    this.CreateCorrespondence(busConstant.VERIFICATION_OF_HOURS_LETTER_TO_EMPLOYER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks);
                                    aarrResult.Clear();
                                }
                            }

                            if (lbusPayeeAccount.idecIAPHours4QtrAlloc > 0)
                            {
                                //Payee Accounts changed to ‘Review’ status.
                                lbusPayeeAccount.CreateReviewPayeeAccountStatus();

                                //Create Workflow.
                                busWorkflowHelper.InitializeWorkflow(busConstant.UPDATE_PAYEE_ACCOUNT, lbusPayeeAccount.icdoPayeeAccount.person_id, 0, lbusPayeeAccount.icdoPayeeAccount.payee_account_id, ahtbQueryBkmarks);
                                PAYEE_ACCOUNT_ID = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;

                                //Create Report
                                if (!(aarrResultReport.Contains(PAYEE_ACCOUNT_ID)))
                                {
                                    aarrResultReport.Add(PAYEE_ACCOUNT_ID);
                                }
                            }
                        }

                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }

                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;

                    });
                    if (aarrResultReport != null)
                    {
                        foreach (int ldrPayeeAccountID in aarrResultReport)
                        {
                            foreach (DataRow ldrPayeeAccount in ldtPayeeAccountData.Rows)
                            {
                                if (ldrPayeeAccountID == Convert.ToInt32(ldrPayeeAccount["PAYEE_ACCOUNT_ID"]))
                                {
                                    DataRow ldrPayeeAccountRow = ldtbPayeeAccountsForReport.NewRow();

                                    ldrPayeeAccountRow["MPI_PERSON_ID"] = ldrPayeeAccount["MPI_PERSON_ID"];
                                    ldrPayeeAccountRow["PAYEE_NAME"] = ldrPayeeAccount["PAYEE_NAME"];
                                    ldrPayeeAccountRow["Account_Relationship"] = ldrPayeeAccount["Account_Relationship"];
                                    ldrPayeeAccountRow["Benefit_Type"] = ldrPayeeAccount["Benefit_Type"];
                                    ldrPayeeAccountRow["RETIREMENT_DATE"] = ldrPayeeAccount["RETIREMENT_DATE"];
                                    ldrPayeeAccountRow["Retirement_Type"] = ldrPayeeAccount["Retirement_Type"];
                                    ldrPayeeAccountRow["Benefit_Option"] = ldrPayeeAccount["Benefit_Option"];
                                    ldrPayeeAccountRow["Fund_Type"] = ldrPayeeAccount["Fund_Type"];
                                    ldrPayeeAccountRow["PAYMENT_DATE_REPORT"] = ldrPayeeAccount["PAYMENT_DATE_REPORT"];
                                    ldrPayeeAccountRow["IAP_HOURS"] = (from obj in iclbPayeeAccount
                                                                       where obj.icdoPayeeAccount.payee_account_id == ldrPayeeAccountID
                                                                       select obj.idecIAPHoursSUM).Sum();
                                 
                                    ldtbPayeeAccountsForReport.Rows.Add(ldrPayeeAccountRow);
                                }
                            }
                        }                      
                        ExecuteFinalReports(ldtbPayeeAccountsForReport, lstrReportPrefixPaymentScheduleID);
                    }
                }
            }
            utlPassInfo.iobjPassInfo = lobjMainPassInfo;
        }
        #endregion      

        private int ExecuteFinalReports(DataTable adtPayeeAccountData, string astrReportPrefixPaymentScheduleID)
        {
            int lintrtn = 0;
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            List<string> llstGeneratedReports = new List<string>();
            string lstrReportPrefixPaymentScheduleID = string.Empty;

            try
            {
                idlgUpdateProcessLog("Verification of Hours Report", "INFO", istrProcessName);
                adtPayeeAccountData.TableName = "rptVerificationofHoursReport";
                if (adtPayeeAccountData.Rows.Count > 0)
                {
                    lstrReportPath = CreatePDFReport(adtPayeeAccountData, "rptVerificationofHoursReport", astrReportPrefixPaymentScheduleID);
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Verification of Hours Report generated succesfully", "INFO", istrProcessName);
                    lintrtn = 1;
                }
                else
                {
                    idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
                }
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                idlgUpdateProcessLog("Verification of Hours Report Failed.", "INFO", istrProcessName);
                return -1;
            }

            return lintrtn;
        }

    }
}

