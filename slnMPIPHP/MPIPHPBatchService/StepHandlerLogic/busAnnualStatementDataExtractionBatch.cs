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
using System.IO;
using MPIPHP.BusinessTier;


namespace MPIPHPJobService
{

    public class busAnnualStatementDataExtractionBatch : busBatchHandler
    {
        #region Properties

        private object iobjLock = null;

        DataTable ldtbPersonAccount = new DataTable();
        busMainBase lobjMainbase = new busMainBase();

        public busYearEndProcessRequest ibusYearEndProcessRequest { get; set; }
        bool lblnDataInsertedInHeader = false;
        int lintDataExtractionHeaderId = 0;
        int iintYear = 0;
        
        //ChangeID: 57284
        decimal ldecBenefitRateupto10QY;
        decimal ldecBenefitRateafter10QY;
        
        Collection<busPersonAccountRetirementContribution> iclbPersonAccountRetirementContribution { get; set; }
        Collection<busPersonAccount> iclbPersonAccount { get; set; }

        string istrTempTable { get; set; } //PIR 1052

        int iintRecordCount { get; set; } 
        #endregion

        //PIR 1052
        private void RetrieveBatchParameters()
        {
            if (ibusJobHeader != null)
            {
                if (ibusJobHeader.iclbJobDetail == null)
                    ibusJobHeader.LoadJobDetail(true);

                foreach (busJobDetail lobjDetail in ibusJobHeader.iclbJobDetail)
                {
                    foreach (busJobParameters lobjParam in lobjDetail.iclbJobParameters)
                    {
                        switch (lobjParam.icdoJobParameters.param_name)
                        {
                            case "RunForSpecificParticipants":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty()
                                    && Convert.ToString(lobjParam.icdoJobParameters.param_value).Trim() == busConstant.FLAG_YES)
                                {
                                    istrTempTable = busConstant.FLAG_YES;
                                }
                                else
                                {
                                    istrTempTable = busConstant.FLAG_NO;
                                }
                                break;
                            case "TotalRecords":
                                if (Convert.ToString(lobjParam.icdoJobParameters.param_value).IsNotNullOrEmpty())
                                {
                                    iintRecordCount = Convert.ToInt32(lobjParam.icdoJobParameters.param_value);
                                }
                                else
                                {
                                    iintRecordCount = 20000;
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void ProcessAnnualStatementDataExtraction()
        {
            int lintCount = 0;
            int lintTotalCount = 0;
            int lintIncreaseYear = 2015; //ChangeID: 57284

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

            RetrieveBatchParameters();//PIR 1052

            if (ibusYearEndProcessRequest == null)
            {
                //if (istrTempTable == busConstant.FLAG_YES)
                //    LoadAnnualStatementRequest(busConstant.BatchRequest1099rStatusComplete);
                //else
                //    LoadAnnualStatementRequest(busConstant.BatchRequest1099rStatusPending);
                //RID 80207
                LoadAnnualStatementRequest(busConstant.BatchRequest1099rStatusPending);
                if (ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id == 0)
                {
                    LoadAnnualStatementRequest(busConstant.BatchRequest1099rStatusComplete);
                }
            }

            //RID 80207
            bool blnCurrentPlanYear = false;
            busSystemManagement lobjSystemManagement = new busSystemManagement();
            lobjSystemManagement.FindSystemManagement();
            if (ibusYearEndProcessRequest.icdoYearEndProcessRequest.year == (lobjSystemManagement.icdoSystemManagement.batch_date.Year - 1))
            {
                blnCurrentPlanYear = true;
            }

            if (ibusYearEndProcessRequest.icdoYearEndProcessRequest.year_end_process_request_id > 0 && blnCurrentPlanYear)
            {
                iintYear = ibusYearEndProcessRequest.icdoYearEndProcessRequest.year;

                int lintReordCount =Convert.ToInt32( DBFunction.DBExecuteScalar("cdoDataExtractionBatchInfo.CheckForYearEndDataExtractionInfo",
                                  new object[1] { iintYear }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework));
                if(lintReordCount==0)
                {
                    utlConnection utlCoreDBConnection = HelperFunction.GetDBConnectionProperties("core");
                    string astrCoreDBConnection = utlCoreDBConnection.istrConnectionString;

                    SqlParameter[] lPara = new SqlParameter[2];
                    SqlParameter lParameters1 = new SqlParameter("@EXECUTIONYEAR", DbType.Int32);
                    SqlParameter lParameters2 = new SqlParameter("@TEMPTABLE", DbType.String);
                    lParameters1.Value = iintYear;
                    if (istrTempTable.IsNotNullOrEmpty())
                        lParameters2.Value = istrTempTable;
                    else
                        lParameters2.Value = busConstant.FLAG_NO;

                    lPara[0] = lParameters1;
                    lPara[1] = lParameters2;

                    busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetYearEndDataExtractionInfo",
                                                                  astrCoreDBConnection, null, lPara);//PIR 1052

                    string lstrMsg = "cdoDataExtractionBatchInfo.information for Everyone from OPUS - Done";
                    PostInfoMessage(lstrMsg);

                    utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("Legacy");
                    string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;

                    //PIR 1052
                    SqlParameter[] lParameters = new SqlParameter[1];
                    SqlParameter param1 = new SqlParameter("@TEMPTABLE", DbType.String);
                    if (istrTempTable.IsNotNullOrEmpty())
                        param1.Value = istrTempTable;
                    else
                        param1.Value = busConstant.FLAG_NO;
                    lParameters[0] = param1;

                    busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GETYEARENDEXTRACTIONDATAYEARLY",
                                                                  astrLegacyDBConnection, null, lParameters);//PIR 1052

                    //lintReordCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoDataExtractionBatchInfo.CheckForYearEndDataExtractionInfo",
                    //              new object[1] { iintYear }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework));
                    //lstrMsg = "cdoDataExtractionBatchInfo.information for Everyone from EADB for" + lintReordCount.ToString() + " people - Done";

                    //People to be processed 
                    int lintUNPRReordCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoDataExtractionBatchInfo.CountUNPRYearEndDataExtractionInfo",
                    new object[1] { iintYear }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework));
                    lstrMsg = "Total people to be processed " + lintUNPRReordCount.ToString() ;

                    PostInfoMessage(lstrMsg);

                }

                //DataTable ldtbParticipantBenInformation = busBase.Select("cdoDataExtractionBatchInfo.GetYearEndDataExtractionInfo",
                //                                new object[2] { iintYear, istrTempTable });//PIR 1052

                //add top Count to get the records

                //int lintASDECount = Convert.ToInt32(busGlobalFunctions.GetCodeValueDescriptionByValue(52,"ASDE").data1);

                DataTable ldtbParticipantBenInformation = busBase.Select("cdoDataExtractionBatchInfo.GetUNPRYearEndDataExtractionInfo",
                                                new object[2] { iintYear, iintRecordCount });

                DataTable ldtbRetiremenContribution = busBase.Select("cdoDataExtractionBatchInfo.GetRetirementContributionForDataExtBatch",
                                                        new object[2] { iintYear, istrTempTable });//PIR 1052
                
                ldtbPersonAccount = busBase.Select("cdoDataExtractionBatchInfo.GetAllPersonAccount",
                                                       new object[1] { istrTempTable }); //PIR 1052
               
                DataTable ldtbPlanBenefitRate = busBase.Select("cdoPlanBenefitRate.Lookup", new object[] { });
                // ChangeID: 57284
                Collection<cdoPlanBenefitRate> lclbcdoPlanBenefitRate = cdoDummyWorkData.GetCollection<cdoPlanBenefitRate>(ldtbPlanBenefitRate);

                ldecBenefitRateupto10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == lintIncreaseYear && t.qualified_year_limit_value == "10").FirstOrDefault().rate;
                ldecBenefitRateafter10QY = lclbcdoPlanBenefitRate.Where(t => t.plan_year == lintIncreaseYear && t.qualified_year_limit_value == "20").FirstOrDefault().rate;

                DataTable ldtbListOfAllBeneficiaries = busBase.Select("cdoDataExtractionBatchInfo.GetListOfBeneficiaries", new object[] { });

                DataTable ldtbParticipantsReceivedAmt = new DataTable();
                ldtbParticipantsReceivedAmt = busBase.Select("cdoDataExtractionBatchInfo.GetPaidAmountTillLastComputaionalYear", new object[] { });
                
                //string lstrMsg = "cdoDataExtractionBatchInfo.information for Everyone from OPUS - Done";
                //PostInfoMessage(lstrMsg);

                //utlConnection utlLegacyDBConnection = HelperFunction.GetDBConnectionProperties("Legacy");
                //string astrLegacyDBConnection = utlLegacyDBConnection.istrConnectionString;
                
                ////PIR 1052
                //SqlParameter[] lParameters = new SqlParameter[1];
                //SqlParameter param1 = new SqlParameter("@TEMPTABLE", DbType.String);
                //if (istrTempTable.IsNotNullOrEmpty())
                //    param1.Value = istrTempTable;
                //else
                //    param1.Value = busConstant.FLAG_NO;
                //lParameters[0] = param1;
                
                //busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GETYEARENDEXTRACTIONDATAYEARLY",
                //                                              astrLegacyDBConnection, null, lParameters);//PIR 1052

                //lstrMsg = "cdoDataExtractionBatchInfo.information for Everyone from EADB for" + ldtbParticipantBenInformation.Rows.Count.ToString() + " people - Done";
                //PostInfoMessage(lstrMsg);


                if (ldtbParticipantBenInformation.Rows.Count > 0)
                {

                    #region Check IF Annual Statement generated flag is not 'Y'

                    DataTable ldtbAnnualStatementGenerated = busBase.Select<cdoYearEndDataExtractionHeader>(
                     new string[1] { enmYearEndDataExtractionHeader.year.ToString() },
                     new object[1] { iintYear }, null, null);

                    //RID 80207
                    if (ldtbAnnualStatementGenerated.Rows.Count > 0)
                    {
                        busYearEndDataExtractionHeader lbusYearEndDataExtractionHeader = new busYearEndDataExtractionHeader { icdoYearEndDataExtractionHeader = new cdoYearEndDataExtractionHeader() };
                        lbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.LoadData(ldtbAnnualStatementGenerated.Rows[0]);
                        lintDataExtractionHeaderId = lbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.year_end_data_extraction_header_id;
                        lblnDataInsertedInHeader = true;
                    }

                    if (ldtbAnnualStatementGenerated.Rows.Count > 0)
                    {
                        if (istrTempTable != busConstant.FLAG_YES)
                        {
                            //DBFunction.DBNonQuery("cdoDataExtractionBatchHourInfo.DeleteRecords",
                            //      new object[1] { Convert.ToInt32(
                            //      ldtbAnnualStatementGenerated.Rows[0][enmYearEndDataExtractionHeader.year_end_data_extraction_header_id.ToString()])},
                            //          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                            //DBFunction.DBNonQuery("cdoDataExtractionBatchInfo.DeleteRecords",
                            //      new object[1] { Convert.ToInt32(
                            //      ldtbAnnualStatementGenerated.Rows[0][enmYearEndDataExtractionHeader.year_end_data_extraction_header_id.ToString()])},
                            //          iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                            //DBFunction.DBNonQuery("cdoYearEndDataExtractionHeader.DeleteRecord",
                            //    new object[1]{Convert.ToInt32(
                            //    ldtbAnnualStatementGenerated.Rows[0][enmYearEndDataExtractionHeader.year_end_data_extraction_header_id.ToString()])},
                            //        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        }
                        else
                        {
                            DBFunction.DBNonQuery("cdoDataExtractionBatchHourInfo.DeleteSelectedParticipants",
                                  new object[2] { lintDataExtractionHeaderId , istrTempTable },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                            DBFunction.DBNonQuery("cdoDataExtractionBatchInfo.DeleteSelectedParticipants",
                                  new object[2] { lintDataExtractionHeaderId , istrTempTable },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        }
                    }

                    #endregion

                    #region Header Insertion
                    if (!lblnDataInsertedInHeader)
                    {
                        busYearEndDataExtractionHeader lbusYearEndDataExtractionHeader = new busYearEndDataExtractionHeader();
                        lbusYearEndDataExtractionHeader = lbusYearEndDataExtractionHeader.InsertValuesInDataExtractionHeader(iintYear, busConstant.FLAG_NO);
                        lintDataExtractionHeaderId = lbusYearEndDataExtractionHeader.icdoYearEndDataExtractionHeader.year_end_data_extraction_header_id;
                        lblnDataInsertedInHeader = true;
                    }
                    #endregion

                    ParallelOptions po = new ParallelOptions();
                    po.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 3;
                    //po.MaxDegreeOfParallelism = 1; //FOR TESTING RUNNING IN SINGLE THREAD

                    Parallel.ForEach(ldtbParticipantBenInformation.AsEnumerable(), po, (acdoPerson, loopState) =>
                    {

                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "YearEndDataExtractionBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;
                        
                        string lstrSSNDecrypted = string.Empty;
                        busParticipantInformationDataExtraction lbusParticipantInformationDataExtraction = new busParticipantInformationDataExtraction();
                        lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction = new cdoParticipantInformationDataExtraction();
                        lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.LoadData(acdoPerson);

                        lstrSSNDecrypted = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);
                        
                        DataTable ldtWorkInfo = busBase.Select("cdoDataExtractionBatchInfo.WorkHistoryforDataExtractionParticipant", new object[1] { lstrSSNDecrypted });

                        //PIR 787
                        if (ldtWorkInfo != null && ldtWorkInfo.Rows.Count > 0) //f/w upgrade
                        {
                            DataRow[] ldrWorkInfo = ldtWorkInfo.Select(); //f/w upgrade

                            GetEligibleParticipantsDetails(acdoPerson, ldtbParticipantBenInformation, ldrWorkInfo,lobjPassInfo, lintCount, lintTotalCount,
                                                                ldtbRetiremenContribution, ldtbPlanBenefitRate, ldtbListOfAllBeneficiaries, ldtbParticipantsReceivedAmt, lbusParticipantInformationDataExtraction);
                        }
                        else  //RID 106895
                        {
                            lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.process_status_value = "PRCD";
                            lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.Update();
                        }


                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }
                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;

                    });


                    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                    lobjMainPassInfo.idictParams = ldictParams;
                    if (lobjMainPassInfo.iconFramework.State == ConnectionState.Closed)
                        lobjMainPassInfo.iconFramework.Open();
                    utlPassInfo.iobjPassInfo = lobjMainPassInfo;

                    //check for remaining records and return 
                    int lintUNPRReordCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoDataExtractionBatchInfo.CountUNPRYearEndDataExtractionInfo",
                  new object[1] { iintYear }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework));

                    if(lintUNPRReordCount!=0)
                    {
                        PostInfoMessage("Total Unprocess Reords are " + lintUNPRReordCount);
                        return;
                    }


                    #region Sequential Code for Locals
                    PostInfoMessage("Started Processing Local records");
                    DataTable ldtbParticipantBenLocalsInformation = busBase.Select("cdoDataExtractionBatchInfo.GetYearEndDataExtractionInfoLocals", new object[2] { iintYear, istrTempTable });

                    if (ldtbParticipantBenLocalsInformation.IsNotNull() && ldtbParticipantBenLocalsInformation.Rows.Count > 0)
                    {

                        foreach (DataRow acdoPerson in ldtbParticipantBenLocalsInformation.AsEnumerable())
                        {
                            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                                GetEligibleParticipantsLocalsDetails(acdoPerson, ldtbParticipantsReceivedAmt, lobjMainPassInfo);
                        }

                    }
                    PostInfoMessage("Completed Local records");
                    #endregion

                    #region Execute update queries to update Date fields

                    if (lobjMainPassInfo.iconFramework.State == ConnectionState.Closed)
                        lobjMainPassInfo.iconFramework.Open();
                    DBFunction.DBNonQuery("cdoDataExtractionBatchInfo.UpdateID",
                                  new object[1] { lintDataExtractionHeaderId }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework);
                    
                    //PIR 1052
                    DBFunction.DBNonQuery("cdoDataExtractionBatchInfo.UpdateCorrectSSNAndMPIDBasedOnPersonId",
                                  new object[1] { iintYear }, lobjMainPassInfo.iconFramework, lobjMainPassInfo.itrnFramework);

                    #endregion

                    UpdateBatchRequest();

                    if (lobjMainPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjMainPassInfo.iconFramework.Close();
                    }

                    lobjMainPassInfo.iconFramework.Dispose();
                    lobjMainPassInfo.iconFramework = null;



                }

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }
        }

        private void GetEligibleParticipantsLocalsDetails(DataRow acdoPerson, DataTable ldtbParticipantsReceivedNonTaxableAmt, utlPassInfo lobjMainPassInfo)
        {
            lobjMainPassInfo.BeginTransaction();

            try
            {
                cdoDataExtractionBatchInfo lcdoDataExtractionBatchInfo = new cdoDataExtractionBatchInfo();
                Collection<cdoDataExtractionBatchHourInfo> lclDataExtractionBatchHourInfo = new Collection<cdoDataExtractionBatchHourInfo>();
                String lstrSSNDecrypted = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);
                
                LoadDataExtractionIcdo(ref lcdoDataExtractionBatchInfo, acdoPerson, true);
                if (lcdoDataExtractionBatchInfo.person_id.IsNotNull())
                {
                    if ((lcdoDataExtractionBatchInfo.local_161_pension_credits.IsNull() || lcdoDataExtractionBatchInfo.local_161_pension_credits <= Decimal.Zero)
                        && (lcdoDataExtractionBatchInfo.local_161_premerger_benefit.IsNull() || lcdoDataExtractionBatchInfo.local_161_premerger_benefit <= Decimal.Zero)

                        && (lcdoDataExtractionBatchInfo.local_52_pension_credits.IsNull() || lcdoDataExtractionBatchInfo.local_52_pension_credits <= Decimal.Zero)
                        && (lcdoDataExtractionBatchInfo.local_52_premerger_benefit.IsNull() || lcdoDataExtractionBatchInfo.local_52_premerger_benefit <= Decimal.Zero)

                        && (lcdoDataExtractionBatchInfo.local_600_pension_credits.IsNull() || lcdoDataExtractionBatchInfo.local_600_pension_credits <= Decimal.Zero)
                        && (lcdoDataExtractionBatchInfo.local_600_premerger_benefit.IsNull() || lcdoDataExtractionBatchInfo.local_600_premerger_benefit <= Decimal.Zero)

                        && (lcdoDataExtractionBatchInfo.local_666_pension_credits.IsNull() || lcdoDataExtractionBatchInfo.local_666_pension_credits <= Decimal.Zero)
                        && (lcdoDataExtractionBatchInfo.local_666_premerger_benefit.IsNull() || lcdoDataExtractionBatchInfo.local_666_premerger_benefit <= Decimal.Zero)

                        && (lcdoDataExtractionBatchInfo.local_700_pension_credits.IsNull() || lcdoDataExtractionBatchInfo.local_700_pension_credits <= Decimal.Zero)
                        && (lcdoDataExtractionBatchInfo.local_700_premerger_benefit.IsNull() || lcdoDataExtractionBatchInfo.local_700_premerger_benefit <= Decimal.Zero)

                        && (lcdoDataExtractionBatchInfo.monthly_benefit_amt.IsNull() || lcdoDataExtractionBatchInfo.monthly_benefit_amt <= Decimal.Zero)
                        && (Convert.ToString(acdoPerson["AMOUNT_PAID"]).IsNotNullOrEmpty() && Convert.ToDecimal(Convert.ToString(acdoPerson["AMOUNT_PAID"])) <= Decimal.Zero))
                    {
                    }
                    else
                        lcdoDataExtractionBatchInfo.Insert();
                }
                
                lobjMainPassInfo.Commit();

            }

            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                String lstrMsg = "Error Occured for MPID : " + Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.mpi_person_id.ToString()]);
                PostErrorMessage(lstrMsg);
                lobjMainPassInfo.Rollback();
            }


        }


        private void LoadDataExtractionIcdo(ref cdoDataExtractionBatchInfo lcdoDataExtractionBatchInfo, DataRow acdoPerson, bool ablnLoadFullObject)
        {
            if (ablnLoadFullObject)
            {
                //RID 71411 adding new columns
                string strIsDisabilityConversion = string.Empty;
                string strIsConvertedFromPopup = string.Empty;
                string strDROModel = string.Empty;
                if (Convert.ToString((acdoPerson["IS_DISABILITY_CONVERSION"])).IsNotNullOrEmpty())
                {
                    strIsDisabilityConversion = Convert.ToString(acdoPerson["IS_DISABILITY_CONVERSION"]);
                }
                if (Convert.ToString((acdoPerson["IS_CONVERTED_FROM_POPUP"])).IsNotNullOrEmpty())
                {
                    strIsConvertedFromPopup = Convert.ToString(acdoPerson["IS_CONVERTED_FROM_POPUP"]);
                }
                if (Convert.ToString((acdoPerson["DRO_MODEL"])).IsNotNullOrEmpty())
                {
                    strDROModel = Convert.ToString(acdoPerson["DRO_MODEL"]);
                }
                lcdoDataExtractionBatchInfo.is_disability_conversion = strIsDisabilityConversion;
                lcdoDataExtractionBatchInfo.is_converted_from_popup = strIsConvertedFromPopup;
                lcdoDataExtractionBatchInfo.dro_model = strDROModel;

                //PIR 787
                if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.plan_id = Convert.ToInt32(acdoPerson["PLAN_ID"]);
                }

                lcdoDataExtractionBatchInfo.year_end_data_extraction_header_id = lintDataExtractionHeaderId;
                lcdoDataExtractionBatchInfo.person_id = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.person_id.ToString()]);
                lcdoDataExtractionBatchInfo.person_name = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_name.ToString()]);

                if (Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]).IsNotNullOrEmpty() &&
                      Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                {
                    lcdoDataExtractionBatchInfo.md_flag = busConstant.FLAG_YES;
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.person_ssn = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);

                lcdoDataExtractionBatchInfo.person_gender_id = 6014;

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.person_gender_value = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.person_dob = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.participant_date_of_death = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]);
                else
                    lcdoDataExtractionBatchInfo.participant_date_of_death = DateTime.MinValue;

                lcdoDataExtractionBatchInfo.participant_state_id = 150;

                if (Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.participant_state_value = Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]);

                #region set benficiary Info

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_flag = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_id = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_name.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_name = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_name.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_ssn = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]);

                lcdoDataExtractionBatchInfo.beneficiary_gender_id = 6014;

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_gender_value = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_gender_value.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_dob = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.beneficiary_date_of_death = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]);
                #endregion

                bool lblnIsParticipantVested = false;

                if (Convert.ToString(acdoPerson[enmPersonAccountEligibility.vested_date.ToString()]).IsNotNullOrEmpty())
                {
                    if (Convert.ToDateTime(acdoPerson[enmPersonAccountEligibility.vested_date.ToString()]) != DateTime.MinValue)
                    {
                        lblnIsParticipantVested = true;
                    }
                }

                #region set status code

                lcdoDataExtractionBatchInfo.status_code_id = 7052;
                lcdoDataExtractionBatchInfo.status_code_value = SetStatusCodeForParticipant(Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]),
                                   Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]),
                                   Convert.ToString(acdoPerson["beneficiary_flag"]), lblnIsParticipantVested, Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]),
                                   Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]), lcdoDataExtractionBatchInfo.participant_date_of_death,
                                   Convert.ToInt32(acdoPerson["person_id"]), DateTime.MinValue, DateTime.MinValue, Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]));

                #endregion

                #region set benefit option code

                lcdoDataExtractionBatchInfo.benefit_option_code_id = 7056;
                lcdoDataExtractionBatchInfo.benefit_option_code_value = SetBenefitOptionCode(Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.benefit_option_code_value.ToString()]));

                #endregion

                #region set determination date

                if ((Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.determination_date = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                }
                else if (!Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]).IsNullOrEmpty() && (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.beneficiary_first_payment_receive_date = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                }

                if (!Convert.ToString(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]).IsNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.pension_stop_date = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]);
                }

                #endregion

                #region set retirement type

                lcdoDataExtractionBatchInfo.retirement_type_id = 7053;
                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]).IsNotNullOrEmpty())
                {
                    if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_NORMAL)
                    {
                        lcdoDataExtractionBatchInfo.retirement_type_value = busConstant.DATA_EXT_RET_TYPE_REGULAR_PESION;
                    }
                    else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_REDUCED_EARLY)
                    {
                        lcdoDataExtractionBatchInfo.retirement_type_value = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                    }
                    else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                    {
                        lcdoDataExtractionBatchInfo.retirement_type_value = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                    }
                }

                if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_DISABILITY)
                {
                    lcdoDataExtractionBatchInfo.retirement_type_value = busConstant.DATA_EXT_RET_TYPE_DISABILITY;
                }

                #endregion


                //PIR 787
                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]).IsNullOrEmpty() && (acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() != "B" && acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() != "Q"))
                {
                    lcdoDataExtractionBatchInfo.beneficiary_flag = "M";
                }
                else
                {
                    if (acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() != "Q")
                        acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()] = "B";
                }

            }

            #region Local 161
            if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty() && Convert.ToInt32(acdoPerson["PLAN_ID"]) == busConstant.LOCAL_161_PLAN_ID)
            {
                lcdoDataExtractionBatchInfo.local_161_flag = "Y";

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_benefit.ToString()]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.local_161_premerger_benefit = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_benefit.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                       (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                    {
                        lcdoDataExtractionBatchInfo.monthly_benefit_amt = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_benefit.ToString()]);
                    }
                    else if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
                    {
                        lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_benefit.ToString()]);
                    }
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_161_credited_hours.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_161_credited_hours = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_credited_hours.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_161_pension_credits.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_161_pension_credits = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_pension_credits.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_total_qualified_years.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_161_premerger_total_qualified_years = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_161_premerger_total_qualified_years.ToString()]);
                //PIR 787
                if (Convert.ToString(acdoPerson["LOCAL_161_NON_ELIGIBLE_BENEFIT"]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.non_eligible_benefit = Convert.ToDecimal(acdoPerson["LOCAL_161_NON_ELIGIBLE_BENEFIT"]);
            }
            #endregion

            #region Local 52
            if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty() && Convert.ToInt32(acdoPerson["PLAN_ID"]) == busConstant.LOCAL_52_PLAN_ID)
            {
                lcdoDataExtractionBatchInfo.local_52_flag = "Y";

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_benefit.ToString()]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.local_52_premerger_benefit = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_benefit.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                       (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                       Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                    {
                        lcdoDataExtractionBatchInfo.monthly_benefit_amt = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_benefit.ToString()]);
                    }
                    else if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
                    {
                        lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_benefit.ToString()]);
                    }
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_52_credited_hours.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_52_credited_hours = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_credited_hours.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_52_pension_credits.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_52_pension_credits = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_pension_credits.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_total_qualified_years.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_52_premerger_total_qualified_years = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_52_premerger_total_qualified_years.ToString()]);
                //PIR 787
                if (Convert.ToString(acdoPerson["LOCAL_52_NON_ELIGIBLE_BENEFIT"]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.non_eligible_benefit = Convert.ToDecimal(acdoPerson["LOCAL_52_NON_ELIGIBLE_BENEFIT"]);
            }

            #endregion

            #region Local 600
            if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty() && Convert.ToInt32(acdoPerson["PLAN_ID"]) == busConstant.LOCAL_600_PLAN_ID)
            {
                lcdoDataExtractionBatchInfo.local_600_flag = "Y";

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_benefit.ToString()]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.local_600_premerger_benefit = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_benefit.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                       (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                       Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                    {
                        lcdoDataExtractionBatchInfo.monthly_benefit_amt = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_benefit.ToString()]);
                    }
                    else if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
                    {
                        lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_benefit.ToString()]);
                    }
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_600_credited_hours.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_600_credited_hours = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_credited_hours.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_600_pension_credits.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_600_pension_credits = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_pension_credits.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_total_qualified_years.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_600_premerger_total_qualified_years = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_600_premerger_total_qualified_years.ToString()]);
                //PIR 787
                if (Convert.ToString(acdoPerson["LOCAL_600_NON_ELIGIBLE_BENEFIT"]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.non_eligible_benefit = Convert.ToDecimal(acdoPerson["LOCAL_600_NON_ELIGIBLE_BENEFIT"]);
            }
            #endregion

            #region Local 666
            if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty() && Convert.ToInt32(acdoPerson["PLAN_ID"]) == busConstant.LOCAL_666_PLAN_ID)
            {
                lcdoDataExtractionBatchInfo.local_666_flag = "Y";

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_benefit.ToString()]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.local_666_premerger_benefit = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_benefit.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                      (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                      Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                    {
                        lcdoDataExtractionBatchInfo.monthly_benefit_amt = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_benefit.ToString()]);
                    }
                    else if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
                    {
                        lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_benefit.ToString()]);
                    }
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_666_credited_hours.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_666_credited_hours = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_credited_hours.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_666_pension_credits.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_666_pension_credits = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_pension_credits.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_total_qualified_years.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_666_premerger_total_qualified_years = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_666_premerger_total_qualified_years.ToString()]);
                //PIR 787
                if (Convert.ToString(acdoPerson["LOCAL_666_NON_ELIGIBLE_BENEFIT"]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.non_eligible_benefit = Convert.ToDecimal(acdoPerson["LOCAL_666_NON_ELIGIBLE_BENEFIT"]);
            }
            #endregion

            #region Local 700
            if (Convert.ToString(acdoPerson["PLAN_ID"]).IsNotNullOrEmpty() && Convert.ToInt32(acdoPerson["PLAN_ID"]) == busConstant.LOCAL_700_PLAN_ID)
            {
                lcdoDataExtractionBatchInfo.local_700_flag = "Y";

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_benefit.ToString()]).IsNotNullOrEmpty())
                {
                    lcdoDataExtractionBatchInfo.local_700_premerger_benefit = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_benefit.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                    (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                    Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                    {
                        lcdoDataExtractionBatchInfo.monthly_benefit_amt = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_benefit.ToString()]);
                    }
                    else if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
                    {
                        lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_benefit.ToString()]);
                    }
                }

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_700_credited_hours.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_700_credited_hours = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_credited_hours.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_700_pension_credits.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_700_pension_credits = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_pension_credits.ToString()]);

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_total_qualified_years.ToString()]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.local_700_premerger_total_qualified_years = Convert.ToDecimal(acdoPerson[enmDataExtractionBatchInfo.local_700_premerger_total_qualified_years.ToString()]);
                //PIR 787
                if (Convert.ToString(acdoPerson["LOCAL_700_NON_ELIGIBLE_BENEFIT"]).IsNotNullOrEmpty())
                    lcdoDataExtractionBatchInfo.non_eligible_benefit = Convert.ToDecimal(acdoPerson["LOCAL_700_NON_ELIGIBLE_BENEFIT"]);
            }
            #endregion

            //Ticket 106895 item 3
            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]).IsNullOrEmpty() && (acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() != "B" && acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() != "Q"))
            {
                if ( (lcdoDataExtractionBatchInfo.status_code_value == busConstant.RETIRED_PART_BENEFICIARY || lcdoDataExtractionBatchInfo.status_code_value == busConstant.DISABILITY_BENEFITS) 
                     &&   
                     ( Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() 
                       &&( ( Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY 
                          || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY
                          || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY )
                          )
                      )
                      &&
                      ( Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty()
                      )
                    )
                {


                    if (lcdoDataExtractionBatchInfo.life_annuity_amt == 0)
                    {
                        DataTable ldtbEstimatedLifeAnnuity = busBase.Select("cdoPayeeBenefitAccount.GetEstimateLifeAnnuityAmount", new object[1] { Convert.ToInt32(acdoPerson["PAYEE_ACCOUNT_ID"]) });
                        if (ldtbEstimatedLifeAnnuity != null && ldtbEstimatedLifeAnnuity.Rows.Count > 0)
                        {
                            lcdoDataExtractionBatchInfo.life_annuity_amt = Convert.ToDecimal(ldtbEstimatedLifeAnnuity.Rows[0][0]);
                        }
                    }

                    if (lcdoDataExtractionBatchInfo.life_annuity_amt == 0)
                    {
                        busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement();
                        srvMPIPHPMSS lobjMSS = new srvMPIPHPMSS();
                        string lstrMPID = string.Empty;
                        string lstrEncryptedSSN = Convert.ToString(acdoPerson["PERSON_SSN"]);
                        DataTable ldtbMPID = busBase.Select("cdoPerson.GetMPIDFromSSN", new object[1] { lstrEncryptedSSN });
                        if (ldtbMPID.Rows.Count > 0)
                        {
                            lstrMPID = Convert.ToString(ldtbMPID.Rows[0][0]);
                        }


                        lbusBenefitCalculationRetirement = lobjMSS.NewRetirementCalculation(lstrMPID, Convert.ToInt32(acdoPerson["PLAN_ID"]), Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]), null);
                        Collection<busBenefitCalculationOptions> lclbbusBenefitCalculationOptions = null;
                        
                        if (lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail != null && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                            lclbbusBenefitCalculationOptions = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(acdoPerson["PLAN_ID"])).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                        busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                        int lintBenefitId = 0;
                        //lintPlanBenefitId = lbusPlanBenefitXr.GetPlanBenefitId(Convert.ToInt32(acdoPerson["PLAN_ID"]), busConstant.LIFE_ANNUTIY);

                        object lobjPlanBenefitId;
                        lobjPlanBenefitId = DBFunction.DBExecuteScalar("cdoPlanBenefitXr.GetPlanBenefitId", new object[2] { Convert.ToInt32(acdoPerson["PLAN_ID"]), busConstant.LIFE_ANNUTIY }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (lobjPlanBenefitId.IsNotNull())
                            lintBenefitId = (int)lobjPlanBenefitId;
                        else
                            lintBenefitId = busConstant.ZERO_INT;

                        if (lclbbusBenefitCalculationOptions != null && lclbbusBenefitCalculationOptions.Count > 0)
                        {
                            busBenefitCalculationOptions lbusBeneOption = null;
                            if (lclbbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintBenefitId).Count() > 0)
                                lbusBeneOption = lclbbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintBenefitId).FirstOrDefault();

                            if (lbusBeneOption != null && lbusBeneOption.icdoBenefitCalculationOptions != null)
                                lcdoDataExtractionBatchInfo.life_annuity_amt = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                            else
                                lcdoDataExtractionBatchInfo.life_annuity_amt = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(acdoPerson["PLAN_ID"])).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;
                        }
                    }

                    if (lcdoDataExtractionBatchInfo.life_annuity_amt == 0)
                        lcdoDataExtractionBatchInfo.life_annuity_amt = lcdoDataExtractionBatchInfo.monthly_benefit_amt;


                }

            }


        }

        /// <summary>
        ///  Get Eligible Participants Details
        /// </summary>
        /// <param name="acdoPerson"></param>
        /// <param name="adtParticipantBenInfo"></param>
        /// <param name="adtParticipantworkInfoForAllParticipants"></param>
        /// <param name="adtTempTable"></param>
        /// <param name="autlPassInfo"></param>
        /// <param name="aintCount"></param>
        /// <param name="aintTotalCount"></param>
        private void GetEligibleParticipantsDetails(DataRow acdoPerson, DataTable adtParticipantBenInfo, DataRow[] ldrWorkInfo, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount,
                                                    DataTable adtbAllRetirementContibution, DataTable adtbPlanBenefitRate, DataTable adtbListOfAllBeneficiaries,
                                                    DataTable ldtbParticipantsReceivedAmt, busParticipantInformationDataExtraction lbusParticipantInformationDataExtraction)
        {
            
            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

            busBase lbusBase = new busBase();
            int lintPersonId = 0;
            string lstrMPID = string.Empty;

            //PIR 787
            int lintPlanId = 0;
            int lintTotalVestedYears = 0;
            decimal ldecVestedHoursForLastCompYear = 0M;
            decimal ldecMpiLateHoursInLastCompYearForPriorYears = 0M;
            
            cdoDataExtractionBatchHourInfo lcdoDataExtractionBatchHourInfo = new cdoDataExtractionBatchHourInfo();
            Collection<cdoDataExtractionBatchHourInfo> lclDataExtractionBatchHourInfo = new Collection<cdoDataExtractionBatchHourInfo>();
            
                  
            try
            {

                autlPassInfo.BeginTransaction();

                lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.process_status_value = "PRCD";
                lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.Update();
                #region Initialize values

                int lintQualifiedYrTillPreviousYr = 0, lintQFYearBeforeBIS = 0, lintlatestBISYearBeforeLastYear = 0, lintMPIPersonAccountId = 0, lintIAPPersonAccountId = 0, lintTotalQFYearsAtRet = 0, lintTotalQFTillLastPlanYear = 0;
                string lstr5500StatusCode = null;

                decimal ldecGrossMonthlyAmt = 0.0M, ldecAge = 0.0M, ldecTotalAccruedBenefit = 0.0M, ldecAccruedBenefitForPreviousCompYr = 0.0M,
                        ldecTotalHoursTillLastCompYear = 0.0M, ldecHoursForLastCompYear = 0.0M, ldecTotalHoursTillPriorCompYear = 0.0M,
                        ldecEEContribution = 0.0M, ldecEEInterest = 0.0M, ldecUVHPContribution = 0.0M, ldecUVHPInterest = 0.0M,
                        ldecNonTaxableBegningBalance = 0.0M,
                        ldecMinimumGuranteeAmount = 0.0M, ldecGrossAmountPaid = 0.0M, ldecRemainingMinimumGuranteeAmount = 0.0M, // Added for Remianing Minimum Gurantee Amount.  -- Wasim.
                        ldecNonTaxableBeginingBalanceLeft = 0.0M, ldecPriorYearIAPAccountBalance = 0.0M, ldecIAPAllocation1Amt = 0.0M, ldecIAPAllocation2Amt = 0.0M,
                        ldecIAPAllocation4Amt = 0.0M, ldecIAPAlloc2InvestmentAmt = 0.0M, ldecIAPAlloc4InvestmentAmt = 0.0M,
                        ldecIAPAlloc2ForfeitureAmt = 0.0M, ldecIAPAlloc4ForfeitureAmt = 0.0M, ldecQDROOffset = 0.0M, ldecEECurrentYear = 0.0M, ldecEEIntCurrentYear = 0.0M,
                        ldecUVHPCurrentYear = 0.0M, ldecNonTaxableAmtPaid = 0.0M,
                        ldecLateEEContributionAmt = 0.0M, ldecLateEEIntAmt = 0.0M, ldecLateIAPAdjustmentAmt = 0.0M, ldecCurrentYearIAPPaymentAmt = 0.0M,
                        ldecPriorYrLocal161AccBal = 0.0M, ldecPriorYrLocal52AccBal = 0.0M, ldecLocal161LateAdjustmentAmt = 0.0M, ldecLocal52LateAdjustmentAmt = 0.0M,
                        ldecLocal161Alloc1Amt = 0.0M, ldecLocal161Alloc2Amt = 0.0M, ldecLocal161Alloc4Amt = 0.0M, ldecLocal52Alloc1Amt = 0.0M, ldecLocal52Alloc2Amt = 0.0M,
                        ldecLocal52Alloc4Amt = 0.0M, ldecHoursForYearBeforeLastCompYear = 0.0M, ldecTotalQFHoursAtRet = 0.0M, ldecAccruedBenefitTillPReviousComputationYr = 0.0M,
                        ldecPAAccruedBenefitTillYrBeforeLastYear = 0.0M, ldecDiffInAccruedBenefitForLateHours = 0.0M, ldecNonEligibleBenefits = 0M;

                string lstrMDFlag = string.Empty;
                string lstrStatusCode = string.Empty;
                string lstrBenefitCode = string.Empty;
                string lstrRetirementType = string.Empty;
                DateTime ldtDeterminationDate = new DateTime();
                DateTime ldtBeneficiaryFirstPaymentDate = new DateTime();
                DateTime ldtPensionStopDate = new DateTime();
                bool lblnQDROExists = false, lblnPreRetDeathExists = false, lblnDeathPostRet = false, lblnWithdrawalExists = false, lblnBeneficiaryExists = false;
                decimal ldecUVHPContributionOffset = 0, ldecUVHPInterestOffset = 0;
                bool lblnIsParticipantVested = false;
                bool lblnIsEligibleForActiveIncrease = false; //ChangeID: 57284
                string lstrEligibleActiveIncr = Convert.ToString('N'); //ChangeID: 57284

                decimal ldecLateHours = 0;

                //RID 71411 adding new columns
                string strIsDisabilityConversion = string.Empty;
                string strIsConvertedFromPopup = string.Empty;
                string strDROModel = string.Empty;
                if (Convert.ToString((acdoPerson["IS_DISABILITY_CONVERSION"])).IsNotNullOrEmpty())
                {
                    strIsDisabilityConversion = Convert.ToString(acdoPerson["IS_DISABILITY_CONVERSION"]);
                }
                if (Convert.ToString((acdoPerson["IS_CONVERTED_FROM_POPUP"])).IsNotNullOrEmpty())
                {
                    strIsConvertedFromPopup = Convert.ToString(acdoPerson["IS_CONVERTED_FROM_POPUP"]);
                }
                if (Convert.ToString((acdoPerson["DRO_MODEL"])).IsNotNullOrEmpty())
                {
                    strDROModel = Convert.ToString(acdoPerson["DRO_MODEL"]);
                }

                #endregion

                busBenefitApplication lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lbusBenefitApplication.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };

                lintPersonId = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.person_id.ToString()]);
                lstrMPID = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.mpi_person_id.ToString()]);
                lbusBenefitApplication.ibusPerson.icdoPerson.person_id = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.person_id.ToString()]);
                lbusBenefitApplication.ibusPerson.icdoPerson.ssn = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);
                lbusBenefitApplication.ibusPerson.icdoPerson.istrSSNNonEncrypted = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);
                
                if (ldtbPersonAccount.AsEnumerable().Where(item => Convert.ToInt32(item["Person_ID"]) == lintPersonId && Convert.ToInt32(item[enmPlan.plan_id.ToString().ToUpper()]) != busConstant.LIFE_PLAN_ID).Count() > 0)
                    lbusBenefitApplication.ibusPerson.iclbPersonAccount = 
                        lobjMainbase.GetCollection<busPersonAccount>(ldtbPersonAccount.AsEnumerable().Where(item => Convert.ToInt32(item["Person_ID"]) == lintPersonId && Convert.ToInt32(item[enmPlan.plan_id.ToString().ToUpper()]) != busConstant.LIFE_PLAN_ID).AsDataTable(), "icdoPersonAccount");
                
                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]).IsNotNullOrEmpty())
                {
                    lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth =
                        Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]);
                    lbusBenefitApplication.ibusPerson.icdoPerson.date_of_birth =
                       lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth;
                }
              
                #region check for Plan Info

                DateTime ldtForfeitureDate = new DateTime();
                DateTime ldtVestedDate = new DateTime();

                if (Convert.ToInt32(acdoPerson[enmPlan.plan_id.ToString()]) == 2)
                {
                    lintMPIPersonAccountId = Convert.ToInt32(acdoPerson[enmPersonAccount.person_account_id.ToString()]);

                    if (Convert.ToString(acdoPerson[enmPersonAccountEligibility.forfeiture_date.ToString()]).IsNotNullOrEmpty())
                    {
                        ldtForfeitureDate = Convert.ToDateTime(acdoPerson[enmPersonAccountEligibility.forfeiture_date.ToString()]);
                    }

                    if (Convert.ToString(acdoPerson[enmPersonAccountEligibility.vested_date.ToString()]).IsNotNullOrEmpty())
                    {
                        ldtVestedDate = Convert.ToDateTime(acdoPerson[enmPersonAccountEligibility.vested_date.ToString()]);
                    }
                    
                }

                if (ldtVestedDate.IsNotNull() && ldtVestedDate != DateTime.MinValue)
                {
                    lblnIsParticipantVested = true;
                }

                #endregion

                #region Check for TRUE participant Info

                DateTime ldtPartciantDateOfDeath = new DateTime();

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]).IsNotNullOrEmpty())
                {
                    ldtPartciantDateOfDeath = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]);
                }


                #region Add Local Plan Info and Prior year Info


                string lstrLocal600Flag = string.Empty, lstrLocal666Flag = string.Empty, lstrLocal700Flag = string.Empty,
                       lstrLocal52Flag = string.Empty, lstrLocal161Flag = string.Empty;
                decimal ldec600PreMergerBenefit = 0.0M, ldec666PreMergerBenefit = 0.0M, ldec700PreMergerBenefit = 0.0M,
                        ldec52PreMergerBenefit = 0.0M, ldec161PreMergerBenefit = 0.0M, ldecEEContTillPriorYear = 0.0M, ldecEEIntTillPriorYear = 0.0M, ldecUVHPContTillPriorYear = 0.0M,
                        ldec52PensionCredits = 0.0M, ldec52CreditedHours = 0.0M, ldec600PensionCredits = 0.0M, ldec600CreditedHours = 0.0M,
                        ldec666PensionCredits = 0.0M, ldec666CreditedHours = 0.0M, ldec700PensionCredits = 0.0M, ldec700CreditedHours = 0.0M,
                        ldec161PensionCredits = 0.0M, ldec161CreditedHours = 0.0M;
                decimal ldec600QualifiedYears = 0.0M, ldec666QualifiedYears = 0.0M, ldec700QualifiedYears = 0.0M,
                                         ldec52QualifiedYears = 0.0M, ldec161QualifiedYears = 0.0M, ldecTotalQFYrBeginingOfPreviousCompYear = 0.0M;
                
                #endregion


                ArrayList larrYears = new ArrayList();
                DataTable ldtbWorkInfoForAnnualStatement = new DataTable();
                DataTable ldtbWorkInfoForPensionActuary = new DataTable();
                ldtbWorkInfoForAnnualStatement = CreateTempTableToStoreWorkInfo(ldtbWorkInfoForAnnualStatement);
                ldtbWorkInfoForPensionActuary = CreateTempTableToStoreWorkInfo(ldtbWorkInfoForPensionActuary);
                int lintCompYear;
                if (!ldrWorkInfo.IsNullOrEmpty())
                {
                    foreach (DataRow ldr in ldrWorkInfo)
                    {
                        lintCompYear = 0;
                        
                        decimal ldecPensionHours = 0;
                        DateTime ldtPlanstartDate = new DateTime();
                        DateTime ldtFirstHourReported = new DateTime();

                        if (Convert.ToString(ldr["TotalLateHours"]).IsNotNullOrEmpty())
                        {
                            ldecLateHours += Convert.ToDecimal(ldr["TotalLateHours"]);
                        }

                        if (Convert.ToString(ldr["TotalPensionHours"]).IsNotNullOrEmpty())
                        {
                            ldecPensionHours = Convert.ToDecimal(ldr["TotalPensionHours"]);
                        }
                        if (Convert.ToString(ldr["computationyear"]).IsNotNullOrEmpty())
                        {
                            lintCompYear = Convert.ToInt32(ldr["computationyear"]);
                        }

                        if (Convert.ToString(ldr["PlanStartDate"]).IsNotNullOrEmpty())
                        {
                            ldtPlanstartDate = Convert.ToDateTime(ldr["PlanStartDate"]);
                        }

                        if (Convert.ToString(ldr["firstHourReported"]).IsNotNullOrEmpty())
                        {
                            ldtFirstHourReported = Convert.ToDateTime(ldr["firstHourReported"]);
                        }
          
                        if (!larrYears.Contains(lintCompYear))
                        {
                            larrYears.Add(lintCompYear);
                            var a = ldrWorkInfo.AsEnumerable().Where(
                                         item => item.Field<int>("computationyear") == lintCompYear && item.Field<decimal?>("TotalPensionHours").IsNotNull());
                            decimal ldecHours = a.Sum(item => item.Field<decimal>("TotalPensionHours"));

                            #region Process Annual Statement work Info

                            DataRow ldrWorkInfoForAnnualStatement = ldtbWorkInfoForAnnualStatement.NewRow();
                            ldrWorkInfoForAnnualStatement["year"] = lintCompYear;
                            ldrWorkInfoForAnnualStatement["qualified_hours"] = ldecHours;
                            ldrWorkInfoForAnnualStatement["vested_hours"] = ldecHours;
                            //PIR 787
                            ldrWorkInfoForAnnualStatement["PlanStartDate"] = ldtPlanstartDate;
                            ldrWorkInfoForAnnualStatement["firstHourReported"] = ldtFirstHourReported;

                            ldtbWorkInfoForAnnualStatement.Rows.Add(ldrWorkInfoForAnnualStatement);
                            #endregion

                            #region  Process Pension Actuary Data
                            
                            #endregion
                        }
                    }
                }

                //PIR 787
                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.plan_id.ToString()]).IsNotNullOrEmpty())
                {
                    lintPlanId = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.plan_id.ToString()]);
                }

                if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]).IsNotNullOrEmpty())
                {
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date =
                        Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);

                    if (lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death.IsNotNull() && lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue && lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death < lbusBenefitApplication.icdoBenefitApplication.retirement_date)
                        lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death;
                }
                else if (lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death.IsNotNull() && lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = lbusBenefitApplication.ibusPerson.icdoPerson.date_of_death;
                else
                    lbusBenefitApplication.icdoBenefitApplication.retirement_date = busGlobalFunctions.GetLastDayofMonth(DateTime.Now).AddDays(1);

                //Sid Jain 06082013
                ldecAge = busGlobalFunctions.CalculatePersonAge(lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, lbusBenefitApplication.icdoBenefitApplication.retirement_date);


                if (Convert.ToString((acdoPerson["GROSS_AMOUNT"])).IsNotNullOrEmpty())
                {
                    ldecGrossMonthlyAmt = Convert.ToDecimal(acdoPerson["GROSS_AMOUNT"]);
                }

                int lintTotalLocalQFYearsCount = Convert.ToInt32(ldec600QualifiedYears + ldec161QualifiedYears + ldec666QualifiedYears + ldec700QualifiedYears + ldec52QualifiedYears);
                busCalculation lbusCalculation = new busCalculation();

                #region load work history, Contributions and IAP Amounts for Annual Statement

                busPersonAccount lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccount.icdoPersonAccount.plan_id = busConstant.MPIPP_PLAN_ID;
                lbusPersonAccount.icdoPersonAccount.person_account_id = lintMPIPersonAccountId;

                if (lintMPIPersonAccountId > 0)
                {
                    #region EE UVHP contributions

                    if (lintMPIPersonAccountId > 0)
                    {
                        DataRow[] ldrMPIRetirementContribution = adtbAllRetirementContibution.FilterTable(utlDataType.Numeric, enmPersonAccount.person_account_id.ToString(), lintMPIPersonAccountId);
                        lbusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
                        busMainBase lbusMain = new busMainBase();
                        lbusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution = lbusMain.GetCollection<busPersonAccountRetirementContribution>(
                                                                ldrMPIRetirementContribution, "icdoPersonAccountRetirementContribution");
                        if (ldrMPIRetirementContribution.Count() > 0)
                        {
                            if (Convert.ToString(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]).IsNotNullOrEmpty())
                            {
                                ldecEEContribution = Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_contribution_amount.ToString()]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString()]).IsNotNullOrEmpty())
                            {
                                ldecEEInterest = Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString()]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.ee_int_amount.ToString()]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_amount.ToString()]).IsNotNullOrEmpty())
                            {
                                ldecUVHPContribution = Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_amount.ToString()]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_amount.ToString()]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]).IsNotNullOrEmpty())
                            {
                                ldecUVHPInterest = Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0][enmPersonAccountRetirementContribution.uvhp_int_amount.ToString()]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["EE_CURRENT_YEAR"]).IsNotNullOrEmpty())
                            {
                                ldecEECurrentYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["EE_CURRENT_YEAR"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["EE_CURRENT_YEAR"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["ee_int_current_year"]).IsNotNullOrEmpty())
                            {
                                ldecEEIntCurrentYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_int_current_year"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_int_current_year"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["UVHP_CURRENT_YEAR"]).IsNotNullOrEmpty())
                            {
                                ldecUVHPCurrentYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["UVHP_CURRENT_YEAR"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["UVHP_CURRENT_YEAR"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["late_ee_contribution_amount"]).IsNotNullOrEmpty())
                            {
                                ldecLateEEContributionAmt = Convert.ToDecimal(ldrMPIRetirementContribution[0]["late_ee_contribution_amount"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["late_ee_contribution_amount"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["late_ee_int_amount"]).IsNotNullOrEmpty())
                            {
                                ldecLateEEIntAmt = Convert.ToDecimal(ldrMPIRetirementContribution[0]["late_ee_int_amount"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["late_ee_int_amount"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["ee_amt_till_prior_year"]).IsNotNullOrEmpty())
                            {
                                ldecEEContTillPriorYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_amt_till_prior_year"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_amt_till_prior_year"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["ee_int_till_prior_year"]).IsNotNullOrEmpty())
                            {
                                ldecEEIntTillPriorYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_int_till_prior_year"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["ee_int_till_prior_year"]);
                            }
                            if (Convert.ToString(ldrMPIRetirementContribution[0]["uvhp_amt_till_prior_year"]).IsNotNullOrEmpty())
                            {
                                ldecUVHPContTillPriorYear = Convert.ToDecimal(ldrMPIRetirementContribution[0]["uvhp_amt_till_prior_year"]) < 0 ? 0
                                    : Convert.ToDecimal(ldrMPIRetirementContribution[0]["uvhp_amt_till_prior_year"]);
                            }
                        }
                    }

                    #endregion
                }

                
                if (ldtbWorkInfoForAnnualStatement.Rows.Count > 0 && lintMPIPersonAccountId > 0)
                {
                    
                    lbusBenefitApplication.aclbPersonWorkHistory_MPI = new Collection<cdoDummyWorkData>();
                    lbusBenefitApplication.aclbPersonWorkHistory_MPI = cdoDummyWorkData.GetCollection<cdoDummyWorkData>(ldtbWorkInfoForAnnualStatement);

                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNotNull() && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count() > 0)
                    {
                        lbusBenefitApplication.idecAge = busGlobalFunctions.CalculatePersonAge(lbusBenefitApplication.ibusPerson.icdoPerson.idtDateofBirth, DateTime.Now);
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderBy(item => item.year).ToList().ToCollection();
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI = lbusBenefitApplication.PaddingForBridgingService(lbusBenefitApplication.aclbPersonWorkHistory_MPI);
                        lbusBenefitApplication.ProcessWorkHistoryPadding(lbusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP); 
                        lbusBenefitApplication.ProcessWorkHistoryforBISandForfieture(lbusBenefitApplication.aclbPersonWorkHistory_MPI, busConstant.MPIPP);
                        //RID 80207
                        ////PIR 787
                        if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]).IsNullOrEmpty() && (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNullOrEmpty())
                        {
                            lbusBenefitApplication.DetermineVesting(ablnBISBatch: true);
                        }

                        busPersonAccountEligibility lbusLocalPersonAccountEligibility = new busPersonAccountEligibility { icdoPersonAccountEligibility = new cdoPersonAccountEligibility() };
                        lbusLocalPersonAccountEligibility = lbusLocalPersonAccountEligibility.LoadPersonAccEligibilityByPersonAccountId(lbusBenefitApplication.ibusPerson.iclbPersonAccount.Where(account => account.icdoPersonAccount.istrPlanCode == busConstant.MPIPP).First().icdoPersonAccount.person_account_id);
                        if (lbusLocalPersonAccountEligibility != null)
                        {
                            ldtForfeitureDate = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.forfeiture_date;
                            ldtVestedDate = lbusLocalPersonAccountEligibility.icdoPersonAccountEligibility.vested_date;
                        }
                        
                        ldecLateHours = 0;
                        if (!ldrWorkInfo.IsNullOrEmpty())
                        {
                            foreach (DataRow ldr in ldrWorkInfo)
                            {
                                if (Convert.ToString(ldr["TotalLateHours"]).IsNotNullOrEmpty())
                                {
                                    if (ldtForfeitureDate == DateTime.MinValue ||
                                        (ldtForfeitureDate != DateTime.MinValue && Convert.ToString(ldr["computationyear"]).IsNotNullOrEmpty() && Convert.ToInt32(ldr["computationyear"]) > ldtForfeitureDate.Year))
                                    {
                                        ldecLateHours += Convert.ToDecimal(ldr["TotalLateHours"]);
                                    }
                                }
                            }
                        }


                        //PIR 1052
                        if (ldtVestedDate.IsNotNull() && ldtVestedDate != DateTime.MinValue)
                        {
                            lblnIsParticipantVested = true;
                        }
                        else
                        {
                            lblnIsParticipantVested = false;
                        }
                    }

                    if (Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]) == "CNCL")
                    {
                        acdoPerson["PAYEE_ACCOUNT_STATUS"] = DBNull.Value;

                        if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
                            acdoPerson[enmPayeeAccount.payee_account_id.ToString()] = DBNull.Value;

                        if (Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]).IsNotNullOrEmpty())
                            acdoPerson[enmPersonAccount.status_value.ToString()] = "ACTV";

                    }

                    decimal lintHours = 0;
                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).Any() && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).FirstOrDefault().IsNotNull())
                        lintHours = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).FirstOrDefault().qualified_hours; //This will be same for both                          

                    if ((lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() || (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count < 1 && ldtForfeitureDate.Year != iobjSystemManagement.icdoSystemManagement.batch_date.Year - 1
                        && lintHours <= 0 && (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNullOrEmpty()))
                        || (ldtVestedDate == DateTime.MinValue && acdoPerson[enmPersonAccount.status_value.ToString()].ToString() == "DCSD") ||
                        (ldtVestedDate != DateTime.MinValue && acdoPerson[enmPersonAccount.status_value.ToString()].ToString() == "DCSD" &&
                            Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNullOrEmpty() && ldtPartciantDateOfDeath != DateTime.MinValue //PIR 787
                            && ldtPartciantDateOfDeath.Year < iintYear))
                        && (ldecEEContribution) <= 1 && (ldecUVHPContribution) <= 1
                        && (ldtPartciantDateOfDeath.Year < iintYear || ldtPartciantDateOfDeath == DateTime.MinValue)
                        && (ldecIAPAllocation1Amt + ldecIAPAllocation2Amt + ldecIAPAllocation4Amt + ldecIAPAlloc2ForfeitureAmt + ldecIAPAlloc2InvestmentAmt
                           + ldecIAPAlloc4ForfeitureAmt + ldecIAPAlloc4InvestmentAmt + ldecLocal161Alloc1Amt + ldecLocal161Alloc2Amt + ldecLocal161Alloc4Amt + ldecLocal161LateAdjustmentAmt
                           + ldecLocal52Alloc1Amt + ldecLocal52Alloc2Amt + ldecLocal52Alloc4Amt + ldecLocal52LateAdjustmentAmt) <= 0
                        )
                    {
                        autlPassInfo.Commit();
                        return;
                    }



                    if (Convert.ToString(acdoPerson["CAT_TYPE"]) == "ACTIVE_DCSD" ||
                      (Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_AND_ACTIVE" && (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNullOrEmpty()
                      || (acdoPerson[enmPayeeAccount.payee_account_id.ToString()].ToString().IsNotNullOrEmpty() && Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]).IsNotNullOrEmpty()
                        && Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]).Year == iobjSystemManagement.icdoSystemManagement.batch_date.Year)))
                        ||
                        (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]).IsNotNullOrEmpty()
                         && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() && ((Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY
                         || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY))
                         )
                        ||
                        Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]) == busConstant.FLAG_YES
                       )
                    {

                        if ((!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty() && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Count > 0 
                            ))
                        {

                            DataTable ldtbWorkHistoryWeekly = new DataTable();

                            Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();
                            busBenefitCalculationHeader lbusBenefitCalculationHeader = new busBenefitCalculationHeader { icdoBenefitCalculationHeader = new cdoBenefitCalculationHeader() };
                            lbusBenefitCalculationHeader.ibusPerson = lbusBenefitApplication.ibusPerson;
                            lbusBenefitCalculationHeader.LoadAllRetirementContributions(null);

                            decimal ldecUnreducedBenefitAmount = lbusCalculation.CalculateUnReducedBenefitAmtForPension(lbusBenefitApplication.ibusPerson, ldecAge,
                                                             busGlobalFunctions.GetLastDateOfComputationYear(iintYear),
                                                             lbusPersonAccount, lbusBenefitApplication, false, null, null, lbusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                             null, null, adtbPlanBenefitRate, false, lintTotalLocalQFYearsCount, ldtbWorkHistoryWeekly.Rows.Count > 0 ? ldtbWorkHistoryWeekly : null);
                            if (ldtForfeitureDate != DateTime.MinValue)
                            {

                                ldecUnreducedBenefitAmount = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year >
                                                             ldtForfeitureDate.Year).Sum(item => item.idecBenefitAmount);

                            }

                            //Fields Total Uptil End of 2012
                            DateTime ldtWithdrawalDate = new DateTime();

                            DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                            if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                            {

                                ldtWithdrawalDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                     where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                     orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                     select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();
                            }

                            if (ldtWithdrawalDate > ldtForfeitureDate)
                            {
                                ldecTotalAccruedBenefit = lbusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal("", ldtVestedDate,
                                                                            ldecUnreducedBenefitAmount, lbusBenefitApplication.ibusPerson, lbusBenefitApplication.ibusPerson.iclbPersonAccount,
                                                                            lbusBenefitApplication.icdoBenefitApplication.retirement_date, lbusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                                            lbusBenefitCalculationHeader.iclbPersonAccountRetirementContribution, ldtForfeitureDate.Year, ref lclbRetCont);
                            }
                            else
                            {
                                ldecTotalAccruedBenefit = ldecUnreducedBenefitAmount;
                            }
                        }
                    }
                    else
                    {
                        ldecTotalAccruedBenefit = Decimal.Zero;
                    }

                }


                if (ldtPartciantDateOfDeath != DateTime.MinValue && ldtPartciantDateOfDeath.Year < iintYear &&
                    Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == busConstant.PERSON_ACCOUNT_STATUS_DECEASED && (Convert.ToString(acdoPerson["beneficiary_flag"]).IsNullOrEmpty())
                && Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]).IsNullOrEmpty())
                {
                    if ((ldecEEContribution + ldecEEInterest) <= 0 && (ldecUVHPContribution + ldecUVHPInterest) <= 0)
                    {
                        autlPassInfo.Commit();
                        return;
                    }
                }

                if (ldtPartciantDateOfDeath != DateTime.MinValue && ldtPartciantDateOfDeath.Year < iintYear && ((Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNotNullOrEmpty() && Convert.ToDateTime(acdoPerson["BENEFICIARY_DATE_OF_DEATH"].ToString()).Year < iintYear) || Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNullOrEmpty()))
                {
                    if ((Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_AND_ACTIVE" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                        && Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == "DCSD" && ldtPartciantDateOfDeath != DateTime.MinValue)
                        ||
                        (Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_BENE_DEATH" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                        && Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == "DCSD" && ldtPartciantDateOfDeath != DateTime.MinValue)
                        )
                    {
                        if (ldecRemainingMinimumGuranteeAmount <= Decimal.Zero && (ldecUVHPContribution + ldecUVHPInterest) <= Decimal.Zero)
                        {
                            autlPassInfo.Commit();
                            return;
                        }
                    }
                }


                #endregion

                #region Set data from Work history for annual Statement

                if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    
                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear && item.year > ldtForfeitureDate.Year).Count() > 0 && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear && item.year > ldtForfeitureDate.Year).FirstOrDefault().idecBenefitAmount.IsNotNull())
                        ldecAccruedBenefitForPreviousCompYr = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear && item.year > ldtForfeitureDate.Year).FirstOrDefault().idecBenefitAmount;


                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).Count() > 0 && lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).FirstOrDefault().IsNotNull())
                        ldecHoursForLastCompYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).FirstOrDefault().qualified_hours; //This will be same for both                          

                    
                    //Fields Total Uptil End of 2012
                    DateTime ldtWithdrawalDate = new DateTime();
                    DateTime ldtEffectiveDate = new DateTime();
                    string lstrContributionSubTypeValue = string.Empty;

                    DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                    if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                    {

                        ldtWithdrawalDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                             where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                             orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                             select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                    }
                    ldtEffectiveDate = ldtWithdrawalDate > ldtForfeitureDate ? ldtWithdrawalDate : ldtForfeitureDate;

                    if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(o => o.year <= iintYear && o.year > ldtEffectiveDate.Year).Count() > 0)
                    {
                        ldecTotalHoursTillPriorCompYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(o => o.year <= iintYear && o.year > ldtEffectiveDate.Year).Sum(o => o.qualified_hours);
                    }


                    if (ldtWithdrawalDate > ldtForfeitureDate)
                    {
                        decimal ldecHoursAfterWithdrawal = 0M;
                        if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == ldtWithdrawalDate.Year).IsNullOrEmpty())
                        {
                            ldecHoursAfterWithdrawal = lbusCalculation.CalculateBenefitAmountForWithdrawalYear(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, busConstant.MPIPP_PLAN_ID,
                             ldtWithdrawalDate, lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == ldtWithdrawalDate.Year).FirstOrDefault());
                            ldecTotalHoursTillPriorCompYear += lbusCalculation.ldecHoursAfterWithdrawal;
                        }
                    }


                    if (ldecLateHours != 0)
                        ldecTotalHoursTillPriorCompYear = ldecTotalHoursTillPriorCompYear - ldecLateHours;




                    if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).IsNullOrEmpty() && ldtForfeitureDate.Year == iintYear)
                    {
                        lintQualifiedYrTillPreviousYr = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).FirstOrDefault().qualified_years_count;
                    }
                    else
                    {
                        lintQualifiedYrTillPreviousYr = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                    }


                    if (lintTotalLocalQFYearsCount > 0)
                    {
                        if (lintQualifiedYrTillPreviousYr - lintTotalLocalQFYearsCount >= 0)
                        {
                            lintQualifiedYrTillPreviousYr = lintQualifiedYrTillPreviousYr - lintTotalLocalQFYearsCount;
                        }

                    }

                    //PIR 787
                    if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).IsNullOrEmpty() && ldtForfeitureDate.Year == iintYear)
                    {
                        lintTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).FirstOrDefault().vested_years_count;
                    }
                    else
                    {
                        lintTotalVestedYears = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().vested_years_count;
                    }

                    //PIR 787
                    if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).IsNullOrEmpty() && ldtForfeitureDate.Year != iintYear)
                    {
                        ldecVestedHoursForLastCompYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).FirstOrDefault().vested_hours;
                    }

                    //PIR 787
                    ldecMpiLateHoursInLastCompYearForPriorYears = ldecLateHours;

                    if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).IsNullOrEmpty())
                        ldecHoursForYearBeforeLastCompYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear - 1).FirstOrDefault().qualified_hours;
                }

                #endregion

                #region Set Data from Work History for Pension Actuary

                if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.IsNullOrEmpty())
                {
                    if (ldtForfeitureDate.Year == iintYear)
                        ldecPAAccruedBenefitTillYrBeforeLastYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(o => o.year <= iintYear - 1).Sum(o => o.idecBenefitAmount);

                    if (Convert.ToString(acdoPerson["CAT_TYPE"]) == "ACTIVE_DCSD" || (Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_AND_ACTIVE" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNullOrEmpty())
                        || Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]) == busConstant.FLAG_YES)
                    {

                        ldecAccruedBenefitTillPReviousComputationYr = ldecTotalAccruedBenefit;
                        
                        //PIR 787
                        ldecTotalHoursTillLastCompYear = ldecTotalHoursTillPriorCompYear;

                        //PIR 1052
                        if (ldecLateHours != 0)
                            ldecTotalHoursTillLastCompYear += ldecLateHours;

                        ldecDiffInAccruedBenefitForLateHours = ldecPAAccruedBenefitTillYrBeforeLastYear - ldecAccruedBenefitForPreviousCompYr;
                    }

                    if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == iintYear).IsNullOrEmpty())
                    {
                        lintTotalQFTillLastPlanYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                                        item => item.year == iintYear).FirstOrDefault().qualified_years_count;
                    }
                    else
                    {
                        lintTotalQFTillLastPlanYear = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Last().qualified_years_count;
                    }

                    //}

                    //PIR 787
                    if (lbusBenefitApplication.icdoBenefitApplication.retirement_date != DateTime.MinValue &&
                        lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                            o => o.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year && o.year > ldtForfeitureDate.Year).Count() > 0
                        && Convert.ToString(acdoPerson["beneficiary_flag"]).IsNullOrEmpty())
                    {

                        decimal ldecHoursAfterRetirementdate = 0M;
                        utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
                        string lstrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;

                        SqlParameter[] lsqlParameters = new SqlParameter[2];
                        SqlParameter param1 = new SqlParameter("@SSN", DbType.String);
                        SqlParameter param2 = new SqlParameter("@RETIREMENT_DATE", DbType.DateTime);

                        param1.Value = lbusBenefitApplication.ibusPerson.icdoPerson.ssn;
                        lsqlParameters[0] = param1;

                        param2.Value = lbusBenefitApplication.icdoBenefitApplication.retirement_date;
                        lsqlParameters[1] = param2;

                        DataTable ldtGetHoursAfterRetirementDate = busGlobalFunctions.ExecuteSPtoGetDataTable("usp_GetWorkDataAfterRetirement", lstrLegacyDBConnetion, null, lsqlParameters);
                        if (ldtGetHoursAfterRetirementDate.Rows.Count > 0 && Convert.ToString(ldtGetHoursAfterRetirementDate.Rows[0]["ComputationYear"]).IsNotNullOrEmpty()
                            && Convert.ToInt32(ldtGetHoursAfterRetirementDate.Rows[0]["ComputationYear"]) == lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year)
                        {

                            ldecHoursAfterRetirementdate = ldtGetHoursAfterRetirementDate.AsEnumerable().Where(item => item.Field<Int16?>("ComputationYear") == Convert.ToInt16(lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year)).Sum(item => item.Field<decimal>("PensionHours"));
                        }

                        //PIR 1052
                        DateTime ldtWithdrawalDate = new DateTime();
                        DateTime ldtEffectiveDate = new DateTime();

                        DataTable ldtbCheckPersonHasWithdrawal = busBase.Select("cdoBenefitApplication.CheckPersonHasWithdrawal", new object[1] { lbusBenefitApplication.ibusPerson.icdoPerson.person_id });
                        if (ldtbCheckPersonHasWithdrawal != null && ldtbCheckPersonHasWithdrawal.Rows.Count > 0)
                        {

                            ldtWithdrawalDate = (from item in ldtbCheckPersonHasWithdrawal.AsEnumerable()
                                                 where item.Field<string>("CONTRIBUTION_TYPE_VALUE") == "EE"
                                                 orderby item.Field<DateTime>("WITHDRAWAL_DATE") descending
                                                 select item.Field<DateTime>("WITHDRAWAL_DATE")).FirstOrDefault();

                        }
                        ldtEffectiveDate = ldtWithdrawalDate > ldtForfeitureDate ? ldtWithdrawalDate : ldtForfeitureDate;

                        if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                           o => o.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year && o.year > ldtEffectiveDate.Year).Count() > 0)
                        {
                            ldecTotalQFHoursAtRet = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                           o => o.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year && o.year > ldtEffectiveDate.Year).Sum(o => o.qualified_hours);

                        }
                        else if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                    o => o.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year && o.year > ldtForfeitureDate.Year).Count() > 0)
                        {
                            ldecTotalQFHoursAtRet = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                    o => o.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year && o.year > ldtForfeitureDate.Year).Sum(o => o.qualified_hours);
                        }


                        if (ldtWithdrawalDate > ldtForfeitureDate && ldtWithdrawalDate.Year < lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year)
                        {
                            decimal ldecHoursAfterWithdrawal = 0M;
                            if (!lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == ldtWithdrawalDate.Year).IsNullOrEmpty())
                            {
                                ldecHoursAfterWithdrawal = lbusCalculation.CalculateBenefitAmountForWithdrawalYear(lbusBenefitApplication.ibusPerson.icdoPerson.ssn, busConstant.MPIPP_PLAN_ID,
                                 ldtWithdrawalDate, lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year == ldtWithdrawalDate.Year).FirstOrDefault());
                                ldecTotalQFHoursAtRet += lbusCalculation.ldecHoursAfterWithdrawal;
                            }
                        }

                        if (ldecTotalQFHoursAtRet > ldecHoursAfterRetirementdate)
                        {
                            ldecTotalQFHoursAtRet = ldecTotalQFHoursAtRet - ldecHoursAfterRetirementdate;
                        }

                        lintTotalQFYearsAtRet = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                           item => item.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year).LastOrDefault().qualified_years_count;
                    }

                    
                    foreach (cdoDummyWorkData lcdoDummyWorkData in lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year))
                    {
                        if (lcdoDummyWorkData.bis_years_count == 2)
                        {
                            lintQFYearBeforeBIS = lcdoDummyWorkData.year;
                            break;
                        }
                    }


                    if (lintQFYearBeforeBIS == iintYear)
                    {
                        //New Logic for 5500 Report State
                        foreach (cdoDummyWorkData lcdoDummyWorkData in lbusBenefitApplication.aclbPersonWorkHistory_MPI.OrderByDescending(item => item.year))
                        {
                            if (lcdoDummyWorkData.year < iintYear && ldtVestedDate != DateTime.MinValue && lcdoDummyWorkData.year > ldtVestedDate.Year
                                && lcdoDummyWorkData.bis_years_count == 2)
                            {
                                lintlatestBISYearBeforeLastYear = lcdoDummyWorkData.year;
                                break;
                            }
                        }

                        if (lintlatestBISYearBeforeLastYear > 0 && lintlatestBISYearBeforeLastYear != lintQFYearBeforeBIS)
                        {
                            lstr5500StatusCode = "B";
                        }
                        else
                        {
                            lstr5500StatusCode = "A";
                        }

                    }


                    if (lintQFYearBeforeBIS > 0)
                    {
                        if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                                    o => o.year <= lintQFYearBeforeBIS && o.year > ldtForfeitureDate.Year).Count() > 0)
                            ldecNonEligibleBenefits = lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(
                                                    o => o.year <= lintQFYearBeforeBIS && o.year > ldtForfeitureDate.Year).Sum(o => o.idecBenefitAmount);
                    }

                    #region Check For Active Increase Flag
                    //ChangeID: 57284
                    lblnIsEligibleForActiveIncrease = IsEligibleForActiveIncrease(lbusBenefitApplication);
                    if (lblnIsEligibleForActiveIncrease)
                    {
                        lstrEligibleActiveIncr = Convert.ToString('Y');
                    }
                    else
                    {
                        lstrEligibleActiveIncr = Convert.ToString('N');
                    }
                    #endregion
                }

                #endregion



                #region Add new Rows to temp table

                if ((Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]) != "B" && Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]) != "Q")
                    ||
                    (ldtPartciantDateOfDeath != DateTime.MinValue && Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]) == "D"
                       && Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNullOrEmpty()))
                {

                    if ((Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_BENE_DEATH" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                         && Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == "DCSD" && ldtPartciantDateOfDeath != DateTime.MinValue && ldtPartciantDateOfDeath.Year < iintYear &&
                         ((Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNotNullOrEmpty() && Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])).Year < iintYear) || Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNullOrEmpty()))
                       )
                    {
                        if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_TERM_CERTAIN
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                            )
                        {
                            if (((ldecRemainingMinimumGuranteeAmount <= Decimal.Zero && (ldecUVHPContribution + ldecUVHPInterest) <= Decimal.Zero)) || (Convert.ToString(acdoPerson["TERM_CERTAIN_END_DATE"]).IsNotNullOrEmpty() &&
                                ((Convert.ToDateTime(acdoPerson["TERM_CERTAIN_END_DATE"]) < ldtPartciantDateOfDeath)
                                  || (Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNotNullOrEmpty() && Convert.ToDateTime(acdoPerson["TERM_CERTAIN_END_DATE"]) < Convert.ToDateTime(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])))
                                ))
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                        else
                        {
                            // Remaining Minimum Gurantee Amount Change
                            if (ldecRemainingMinimumGuranteeAmount <= Decimal.Zero && (ldecUVHPContribution + ldecUVHPInterest) <= Decimal.Zero)
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                    }

                    #region set status code

                    lstrStatusCode = SetStatusCodeForParticipant(Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]),
                                       Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]),
                                       Convert.ToString(acdoPerson["beneficiary_flag"]), lblnIsParticipantVested, Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]), Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]), ldtPartciantDateOfDeath,
                                       Convert.ToInt32(acdoPerson["person_id"]), ldtVestedDate, ldtForfeitureDate, Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]), lbusBenefitApplication.aclbPersonWorkHistory_MPI);

                    #endregion

                    #region set benefit option code

                    lstrBenefitCode = SetBenefitOptionCode(Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.benefit_option_code_value.ToString()]));

                    #endregion

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    {
                        lstrMDFlag = busConstant.FLAG_YES;
                    }

                    #region Set Values in Life Annuity field -- PIR 827 fix
                    decimal ldecReducedBenefitAmount = 0;

                    if ( //(lstrStatusCode == busConstant.RETIRED_PART_BENEFICIARY || lstrStatusCode == busConstant.DISABILITY_BENEFITS) &&   //Ticket 106895 item 3
                        ((lstrBenefitCode == busConstant.DATA_EXT_JS_50_POP_UP ||
                        lstrBenefitCode == busConstant.DATA_EXT_JS_75_POP_UP || lstrBenefitCode == busConstant.DATA_EXT_JS_100_POP_UP)
                        || (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() && ((Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_50_PERCENT_POPUP_ANNUITY || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY
                         || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY))))
                        )
                    {



                        decimal ldecLateAdjustmentamount = 0;

                        lbusBenefitApplication.ibusPerson.iclbPersonAccount = new Collection<busPersonAccount>();
                        if (ldtbPersonAccount.AsEnumerable().Where(item => Convert.ToInt32(item["Person_ID"]) == lintPersonId).Count() > 0)
                            lbusBenefitApplication.ibusPerson.iclbPersonAccount = lobjMainbase.GetCollection<busPersonAccount>(ldtbPersonAccount.AsEnumerable().Where(item => Convert.ToInt32(item["Person_ID"]) == lintPersonId).AsDataTable(), "icdoPersonAccount");

                        string lstrMemberRetirementType = Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]);
                        Collection<cdoPersonAccountRetirementContribution> lclbRetCont = new Collection<cdoPersonAccountRetirementContribution>();

                        if (lstrMemberRetirementType != busConstant.RETIREMENT_TYPE_LATE && lstrMemberRetirementType != busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                        {
                            ldecReducedBenefitAmount = lbusCalculation.CalculateAccruedBenefitForPersonWithWithdrawal(string.Empty, ldtVestedDate,
                                                             ldecTotalAccruedBenefit, lbusBenefitApplication.ibusPerson, lbusBenefitApplication.ibusPerson.iclbPersonAccount,
                                                             lbusBenefitApplication.icdoBenefitApplication.retirement_date, lbusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                             lbusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution, ldtForfeitureDate.Year, ref lclbRetCont);
                        }

                        ldecReducedBenefitAmount = lbusCalculation.CalculateReducedBenefit(lbusBenefitApplication.ibusPerson, Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]),
                                                    ldecAge, lbusBenefitApplication.icdoBenefitApplication.retirement_date, ldtVestedDate, lbusPersonAccount, lbusBenefitApplication, false, null, null,
                                                    lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(item => item.year <= lbusBenefitApplication.icdoBenefitApplication.retirement_date.Year).FirstOrDefault().qualified_years_count,
                                                    ldecReducedBenefitAmount, Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]), true, lbusBenefitApplication.aclbPersonWorkHistory_MPI,
                                                    lbusBenefitApplication.ibusPerson.iclbPersonAccountRetirementContribution, ref ldecLateAdjustmentamount, "", 0, false, true);

                        ////Ticket 106895 item 3
                        //if (ldecReducedBenefitAmount == 0)
                        //    ldecReducedBenefitAmount = lbusCalculation.CalculateUnReducedBenefitAmtForPension( lbusBenefitApplication.ibusPerson, 
                        //                            ldecAge, lbusBenefitApplication.icdoBenefitApplication.retirement_date 
                        //                            , lbusPersonAccount, lbusBenefitApplication, false, null, null,
                        //                            lbusBenefitApplication.aclbPersonWorkHistory_MPI, 
                        //                            Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]));

                        //Ticket 106895 item 3
                        if (ldecReducedBenefitAmount == 0)
                        {
                            DataTable ldtbEstimatedLifeAnnuity = busBase.Select("cdoPayeeBenefitAccount.GetEstimateLifeAnnuityAmount", new object[1] { Convert.ToInt32(acdoPerson["PAYEE_ACCOUNT_ID"]) });
                            if (ldtbEstimatedLifeAnnuity != null && ldtbEstimatedLifeAnnuity.Rows.Count > 0)
                            {
                                ldecReducedBenefitAmount = Convert.ToDecimal(ldtbEstimatedLifeAnnuity.Rows[0][0]);
                            }
                        }

                        //Ticket 106895 item 3
                        if (ldecReducedBenefitAmount == 0)
                        {
                            busMssBenefitCalculationRetirement lbusBenefitCalculationRetirement = new busMssBenefitCalculationRetirement();
                            srvMPIPHPMSS lobjMSS = new srvMPIPHPMSS();
                            //string lstrMPID = string.Empty;
                            string lstrEncryptedSSN = Convert.ToString(acdoPerson["PERSON_SSN"]);
                            DataTable ldtbMPID = busBase.Select("cdoPerson.GetMPIDFromSSN", new object[1] { lstrEncryptedSSN });
                            if (ldtbMPID.Rows.Count > 0)
                            {
                                lstrMPID = Convert.ToString(ldtbMPID.Rows[0][0]);
                            }


                            lbusBenefitCalculationRetirement = lobjMSS.NewRetirementCalculation(lstrMPID, Convert.ToInt32(acdoPerson["PLAN_ID"]), Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]), null);
                            Collection<busBenefitCalculationOptions> lclbbusBenefitCalculationOptions = null;
                            if(lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail != null && lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Count > 0)
                                lclbbusBenefitCalculationOptions = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(acdoPerson["PLAN_ID"])).FirstOrDefault().iclbBenefitCalculationOptions.ToList().ToCollection();

                            busPlanBenefitXr lbusPlanBenefitXr = new busPlanBenefitXr();
                            int lintBenefitId;
                            //lintPlanBenefitId = lbusPlanBenefitXr.GetPlanBenefitId(Convert.ToInt32(acdoPerson["PLAN_ID"]), busConstant.LIFE_ANNUTIY);

                            object lobjPlanBenefitId;
                            lobjPlanBenefitId = DBFunction.DBExecuteScalar("cdoPlanBenefitXr.GetPlanBenefitId", new object[2] { Convert.ToInt32(acdoPerson["PLAN_ID"]), busConstant.LIFE_ANNUTIY }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                            if (lobjPlanBenefitId.IsNotNull())
                                lintBenefitId = (int)lobjPlanBenefitId;
                            else
                                lintBenefitId =  busConstant.ZERO_INT;

                            if (lclbbusBenefitCalculationOptions != null && lclbbusBenefitCalculationOptions.Count > 0)
                            {
                                busBenefitCalculationOptions lbusBeneOption = null;
                                if (lclbbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintBenefitId).Count() > 0)
                                    lbusBeneOption = lclbbusBenefitCalculationOptions.Where(option => option.icdoBenefitCalculationOptions.plan_benefit_id == lintBenefitId).FirstOrDefault();

                                if (lbusBeneOption != null && lbusBeneOption.icdoBenefitCalculationOptions != null)
                                    ldecReducedBenefitAmount = lbusBeneOption.icdoBenefitCalculationOptions.benefit_amount;
                                else
                                    ldecReducedBenefitAmount = lbusBenefitCalculationRetirement.iclbBenefitCalculationDetail.Where(item => item.icdoBenefitCalculationDetail.plan_id == Convert.ToInt32(acdoPerson["PLAN_ID"])).FirstOrDefault().icdoBenefitCalculationDetail.early_reduced_benefit_amount;
                            }
                        }

                    }
                    #endregion

                    #region set determination date
                    //PIR 787
                    if ((Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNotNullOrEmpty())
                    {
                        ldtDeterminationDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                    }
                    else if (!Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]).IsNullOrEmpty() && (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNotNullOrEmpty()
                        && !Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNullOrEmpty())
                    {
                        ldtBeneficiaryFirstPaymentDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                    }

                    if (!Convert.ToString(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]).IsNullOrEmpty())
                    {
                        ldtPensionStopDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]);
                    }
                    
                    if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]).IsNotNullOrEmpty())
                    {
                        #region Minimum Gurantee Amount Change
                        if (!Convert.ToString(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]).IsNullOrEmpty())
                            ldecMinimumGuranteeAmount = Convert.ToDecimal(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]);

                        if (ldtbParticipantsReceivedAmt != null && ldtbParticipantsReceivedAmt.Rows.Count > 0 &&
                            Convert.ToInt32(acdoPerson[enmPersonAccount.plan_id.ToString()]) == busConstant.MPIPP_PLAN_ID)
                        {
                            if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
                            {
                                DataRow[] ldrAmountPaid = ldtbParticipantsReceivedAmt.FilterTable(utlDataType.Numeric, enmPayeeAccount.payee_account_id.ToString(),
                                                                Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]));
                                if (ldrAmountPaid.Count() > 0 && Convert.ToString(ldrAmountPaid[0]["PAID_GROSS_AMOUNT"]).IsNotNullOrEmpty())
                                {
                                    ldecGrossAmountPaid = Convert.ToDecimal(ldrAmountPaid[0]["PAID_GROSS_AMOUNT"]);
                                }
                            }
                        }
                        ldecRemainingMinimumGuranteeAmount = Math.Round(ldecMinimumGuranteeAmount - ldecGrossAmountPaid, 2);
                        if (ldecRemainingMinimumGuranteeAmount < 0)
                            ldecRemainingMinimumGuranteeAmount = 0.0M;
                        #endregion
                    }

                    #endregion

                    #region set retirement type

                    if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_NORMAL)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REGULAR_PESION;
                        }
                        else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_REDUCED_EARLY)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                        }
                        else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                        }
                    }

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_DISABILITY;
                    }



                    #endregion

                    acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()] = "M";

                    //PIR 1052
                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                        && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM && lstrStatusCode == busConstant.LUMP)
                    {
                        DataTable ldtLumpSumAmtPaid = busBase.Select("cdoDataExtractionBatchInfo.GetAmountPaidInLastCompYear", new object[2] { Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]), lintPlanId });
                        if (ldtLumpSumAmtPaid.Rows.Count > 0 && Convert.ToString(ldtLumpSumAmtPaid.Rows[0]["PAID_GROSS_AMOUNT"]).IsNotNullOrEmpty())
                        {
                            ldecGrossMonthlyAmt = Convert.ToDecimal(ldtLumpSumAmtPaid.Rows[0]["PAID_GROSS_AMOUNT"]);
                        }
                        else
                        {
                            autlPassInfo.Commit();
                            return;
                        }
                    }

                        AddRowsInDatabase(lintDataExtractionHeaderId, lintPersonId, acdoPerson, lstrStatusCode, lintQualifiedYrTillPreviousYr, lintQFYearBeforeBIS,
                       ldecAccruedBenefitForPreviousCompYr, ldecTotalAccruedBenefit - ldecQDROOffset, ldecEEContribution, ldecEEInterest, ldecUVHPContribution,
                       ldecUVHPInterest, ldecHoursForLastCompYear, ldecTotalHoursTillLastCompYear, ldecTotalHoursTillPriorCompYear, ldecGrossMonthlyAmt,
                       ldtDeterminationDate, ldtBeneficiaryFirstPaymentDate, ldtPensionStopDate, lstrRetirementType,
                       lstrLocal600Flag, ldec600QualifiedYears, ldec600PreMergerBenefit, lstrLocal666Flag, ldec666QualifiedYears, ldec666PreMergerBenefit,
                       lstrLocal700Flag, ldec700QualifiedYears, ldec700PreMergerBenefit, lstrLocal52Flag, ldec52QualifiedYears, ldec52PreMergerBenefit,
                       lstrLocal161Flag, ldec161QualifiedYears, ldec161PreMergerBenefit, ldecPriorYearIAPAccountBalance,
                       ldecIAPAllocation1Amt, ldecIAPAllocation2Amt, ldecIAPAllocation4Amt, ldecIAPAlloc2ForfeitureAmt, ldecIAPAlloc4ForfeitureAmt,
                       ldecIAPAlloc2InvestmentAmt, ldecIAPAlloc4InvestmentAmt, ldecEEContTillPriorYear + ldecEEIntTillPriorYear, ldecUVHPContTillPriorYear, ldecTotalQFYrBeginingOfPreviousCompYear,
                       ldecEECurrentYear + ldecEEIntCurrentYear, ldecUVHPCurrentYear, ldec52PensionCredits, ldec52CreditedHours,
                       ldec600PensionCredits, ldec600CreditedHours, ldec666PensionCredits, ldec666CreditedHours, ldec700PensionCredits, ldec700CreditedHours,
                       ldec161PensionCredits, ldec161CreditedHours, ldecLateIAPAdjustmentAmt, ldecCurrentYearIAPPaymentAmt, ldecPriorYrLocal52AccBal, ldecLocal52Alloc1Amt,
                       ldecLocal52Alloc2Amt, ldecLocal52Alloc4Amt, ldecPriorYrLocal161AccBal, ldecLocal161Alloc1Amt, ldecLocal161Alloc2Amt, ldecLocal161Alloc4Amt,
                       ldecLocal52LateAdjustmentAmt, ldecLocal161LateAdjustmentAmt, ldecHoursForYearBeforeLastCompYear, ldecTotalQFHoursAtRet, lintTotalQFYearsAtRet,
                       lstrBenefitCode, ldecAccruedBenefitTillPReviousComputationYr, adtbListOfAllBeneficiaries, lintTotalQFTillLastPlanYear,
                       ldecDiffInAccruedBenefitForLateHours, ldecLateEEContributionAmt + ldecLateEEIntAmt, ldecRemainingMinimumGuranteeAmount, ldecReducedBenefitAmount, ldecNonEligibleBenefits, lstrMDFlag,
                       lintPlanId, lintTotalVestedYears, ldecVestedHoursForLastCompYear, ldecMpiLateHoursInLastCompYearForPriorYears, lstr5500StatusCode,lstrEligibleActiveIncr
                       , strIsDisabilityConversion, strIsConvertedFromPopup, strDROModel  //RID 71411                
                       ); //ChangeID: 57284
                    
                }
                #endregion


                #endregion

                #region Check if BENEFICIARY INFO

                if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNotNullOrEmpty() &&
                    ((acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() == "B" ||
                            acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() == "Q") ||
                   (acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()].ToString() == "D" &&
                    (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_id.ToString()]) != Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()])))))
                {
                                       
                    if ((Convert.ToString(acdoPerson["CAT_TYPE"]) == "RETIREE_BENE_DEATH" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                         && Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == "DCSD" && Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNotNullOrEmpty() && Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])) != DateTime.MinValue
                         && Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])).Year < iintYear && ldtPartciantDateOfDeath.Year < iintYear)
                        )
                    {
                        if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_TERM_CERTAIN
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                            )
                        {
                            if ((ldecRemainingMinimumGuranteeAmount <= Decimal.Zero && (ldecUVHPContribution + ldecUVHPInterest) <= Decimal.Zero) || (Convert.ToString(acdoPerson["TERM_CERTAIN_END_DATE"]).IsNotNullOrEmpty() && Convert.ToDateTime(Convert.ToString(acdoPerson["TERM_CERTAIN_END_DATE"])) < Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]))))
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                        else
                        {
                            // Remaining Minimum Gurantee Amount Change
                            if (ldecRemainingMinimumGuranteeAmount <= Decimal.Zero && (ldecUVHPContribution + ldecUVHPInterest) <= Decimal.Zero)
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                    }

                    if ((Convert.ToString(acdoPerson["CAT_TYPE"]) == "QDRO" && Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                         && Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]) == "DCSD" && Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]).IsNotNullOrEmpty() && Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])) != DateTime.MinValue
                         && Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"])).Year < iintYear && ldtPartciantDateOfDeath.Year < iintYear)
                        )
                    {
                        if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TEN_YEARS_TERM_CERTAIN
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY
                           || Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY
                            )
                        {
                            if (ldecRemainingMinimumGuranteeAmount <= Decimal.Zero || (Convert.ToString(acdoPerson["TERM_CERTAIN_END_DATE"]).IsNotNullOrEmpty() && Convert.ToDateTime(Convert.ToString(acdoPerson["TERM_CERTAIN_END_DATE"])) < Convert.ToDateTime(Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]))))
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                        else
                        {
                            // Remaining Minimum Gurantee Amount Change
                            if (ldecRemainingMinimumGuranteeAmount <= Decimal.Zero)
                            {
                                autlPassInfo.Commit();
                                return;
                            }
                        }
                    }

                    DateTime ldtBenefitBeginDate = new DateTime();
                    DateTime ldtBenefitEndDate = new DateTime();
                    decimal ldecNonTaxBeginingBal = 0, lintTotalQFYrBeginingOfPreviousCompYear = 0.0M, ldecMGA = 0;

                    int lintBeneficiaryPersonId = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()].ToString());

                    #region set status and benefit code

                    lstrStatusCode = SetStatusCodeForParticipant(Convert.ToString(acdoPerson[enmPersonAccount.status_value.ToString()]),
                                       Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]),
                                       Convert.ToString(acdoPerson["beneficiary_flag"]), lblnIsParticipantVested, Convert.ToString(acdoPerson["PAYEE_ACCOUNT_STATUS"]),
                                       Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]), ldtPartciantDateOfDeath,
                                       Convert.ToInt32(acdoPerson["person_id"]), ldtVestedDate, ldtForfeitureDate, Convert.ToString(acdoPerson["BENEFICIARY_DATE_OF_DEATH"]), lbusBenefitApplication.aclbPersonWorkHistory_MPI);

                    lstrBenefitCode = SetBenefitOptionCode(Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.benefit_option_code_value.ToString()]));

                    #endregion


                    #region set other columns and  dates

                    if (Convert.ToString((acdoPerson["GROSS_AMOUNT"])).IsNotNullOrEmpty())
                    {
                        ldecGrossMonthlyAmt = Convert.ToDecimal(acdoPerson["GROSS_AMOUNT"]);
                    }

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]).IsNotNullOrEmpty() &&
                      Convert.ToString(acdoPerson[enmPayeeAccount.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_MINIMUM_DISTRIBUTION)
                    {
                        lstrMDFlag = busConstant.FLAG_YES;
                    }
                    //PIR 787
                    if ((Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()])).IsNotNullOrEmpty())
                    {
                        ldtDeterminationDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                    }
                    else if (!Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]).IsNullOrEmpty() && (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()])).IsNotNullOrEmpty())
                    {
                        ldtBeneficiaryFirstPaymentDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                    }

                    if (!Convert.ToString(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]).IsNullOrEmpty())
                    {
                        ldtPensionStopDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]);
                    }


                    if (Convert.ToString(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]).IsNotNullOrEmpty())
                    {
                        ldecMGA = Convert.ToDecimal(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]);
                    }

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]).IsNotNullOrEmpty())
                    {
                        ldtBenefitBeginDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_begin_date.ToString()]);
                    }

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]).IsNotNullOrEmpty())
                    {
                        ldtBenefitEndDate = Convert.ToDateTime(acdoPerson[enmPayeeAccount.benefit_end_date.ToString()]);
                    }

                    #region Minimum Gurantee Amount Change
                    if (!Convert.ToString(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]).IsNullOrEmpty())
                        ldecMinimumGuranteeAmount = Convert.ToDecimal(acdoPerson[enmPayeeAccount.minimum_guarantee_amount.ToString()]);

                    if (ldtbParticipantsReceivedAmt != null && ldtbParticipantsReceivedAmt.Rows.Count > 0 &&
                        Convert.ToInt32(acdoPerson[enmPersonAccount.plan_id.ToString()]) == busConstant.MPIPP_PLAN_ID)
                    {
                        if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty())
                        {
                            DataRow[] ldrAmountPaid = ldtbParticipantsReceivedAmt.FilterTable(utlDataType.Numeric, enmPayeeAccount.payee_account_id.ToString(),
                                                            Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]));
                            if (ldrAmountPaid.Count() > 0 && Convert.ToString(ldrAmountPaid[0]["PAID_GROSS_AMOUNT"]).IsNotNullOrEmpty())
                            {
                                ldecGrossAmountPaid = Convert.ToDecimal(ldrAmountPaid[0]["PAID_GROSS_AMOUNT"]);
                            }
                        }
                    }
                    ldecRemainingMinimumGuranteeAmount = Math.Round(ldecMinimumGuranteeAmount - ldecGrossAmountPaid, 2);
                    if (ldecRemainingMinimumGuranteeAmount < 0)
                        ldecRemainingMinimumGuranteeAmount = 0.0M;
                    #endregion

                 

                    #endregion


                    #region set retirement type

                    if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]).IsNotNullOrEmpty())
                    {
                        if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_NORMAL)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REGULAR_PESION;
                        }
                        else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_REDUCED_EARLY)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                        }
                        else if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.retirement_type_value.ToString()]) == busConstant.RETIREMENT_TYPE_SPL_REDUCED_EARLY)
                        {
                            lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_REDUCED_EARLY;
                        }
                    }

                    if (Convert.ToString(acdoPerson[enmPayeeAccount.benefit_account_type_value.ToString()]) == busConstant.BENEFIT_TYPE_DISABILITY)
                    {
                        lstrRetirementType = busConstant.DATA_EXT_RET_TYPE_DISABILITY;
                    }

                    #endregion

                    if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]) != "Q")
                        acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()] = "B";

                    //PIR 1052
                    if (Convert.ToString(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()
                        && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM && lstrStatusCode == busConstant.LUMP)
                    {
                        DataTable ldtLumpSumAmtPaid = busBase.Select("cdoDataExtractionBatchInfo.GetAmountPaidInLastCompYear", new object[2] { Convert.ToInt32(acdoPerson[enmPayeeAccount.payee_account_id.ToString()]), lintPlanId });
                        if (ldtLumpSumAmtPaid.Rows.Count > 0 && Convert.ToString(ldtLumpSumAmtPaid.Rows[0]["PAID_GROSS_AMOUNT"]).IsNotNullOrEmpty())
                        {
                            ldecGrossMonthlyAmt = Convert.ToDecimal(ldtLumpSumAmtPaid.Rows[0]["PAID_GROSS_AMOUNT"]);
                        }
                        else
                        {
                            autlPassInfo.Commit();
                            return;
                        }

                    }


                    if (Convert.ToInt32(acdoPerson[enmPersonAccount.plan_id.ToString()]) == busConstant.MPIPP_PLAN_ID &&
                        (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_id.ToString()]) != Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()])))
                    {
                            AddRowsInDatabase(lintDataExtractionHeaderId, lintPersonId, acdoPerson, lstrStatusCode, lintQualifiedYrTillPreviousYr, lintQFYearBeforeBIS,
                            ldecAccruedBenefitForPreviousCompYr, ldecTotalAccruedBenefit - ldecQDROOffset, ldecEEContribution, ldecEEInterest, ldecUVHPContribution,
                            ldecUVHPInterest, ldecHoursForLastCompYear, ldecTotalHoursTillLastCompYear, ldecTotalHoursTillPriorCompYear, ldecGrossMonthlyAmt, ldtBenefitBeginDate, ldtBenefitBeginDate, ldtBenefitEndDate,
                            lstrRetirementType, lstrLocal600Flag, ldec600QualifiedYears, ldec600PreMergerBenefit, lstrLocal666Flag, ldec666QualifiedYears, ldec666PreMergerBenefit,
                            lstrLocal700Flag, ldec700QualifiedYears, ldec700PreMergerBenefit, lstrLocal52Flag, ldec52QualifiedYears, ldec52PreMergerBenefit,
                            lstrLocal161Flag, ldec161QualifiedYears, ldec161PreMergerBenefit, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0, 0.0M,
                            0.0M, ldec52PensionCredits, ldec52CreditedHours, ldec600PensionCredits, ldec600CreditedHours, ldec666PensionCredits, ldec666CreditedHours,
                            ldec700PensionCredits, ldec700CreditedHours, ldec161PensionCredits, ldec161CreditedHours, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M,
                            0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0.0M, 0,
                            lstrBenefitCode, 0.0M, null, lintTotalQFTillLastPlanYear, 0, 0, ldecRemainingMinimumGuranteeAmount, 0, 0, lstrMDFlag,
                            lintPlanId, lintTotalVestedYears, ldecVestedHoursForLastCompYear, ldecMpiLateHoursInLastCompYearForPriorYears, lstr5500StatusCode,lstrEligibleActiveIncr
                            , strIsDisabilityConversion, strIsConvertedFromPopup, strDROModel  //RID 71411                
                            );//PIR 787 //ChangeID: 57284
                    }

                }



                #endregion
                
                foreach (cdoDataExtractionBatchHourInfo item in lclDataExtractionBatchHourInfo)
                {
                    item.Insert();
                }
               
                autlPassInfo.Commit();

            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lstrMPID.ToString() + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                    PostErrorMessage("Error for :" + lbusParticipantInformationDataExtraction.icdoParticipantInformationDataExtraction.participant_information_data_extraction_id);
                }
                autlPassInfo.Rollback();

            }
        }

        /// <summary>
        /// create temp table
        /// </summary>
        /// <param name="ldtCompleteParticipantInfo"></param>
        /// <returns></returns>
        private DataTable CreateTempTabletoStoreCompleteInfo(DataTable ldtCompleteParticipantInfo)
        {
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.data_extraction_batch_info_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.year_end_data_extraction_header_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_name.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_ssn.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_gender_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_gender_value.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.person_dob.ToString(), typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.participant_date_of_death.ToString(), typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_flag.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_name.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_ssn.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_gender_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_gender_value.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_dob.ToString(), typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("status_code_id", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("status_code_value", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("total_qualified_years", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("participant_state_id", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("participant_state_value", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("last_qf_yr_before_bis", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("non_eligible_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("accrued_benefit_for_prior_year", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("accrued_benefit_till_last_comp_year", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("total_ee_contribution_amt", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("total_uvhp_amt", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("total_ee_interest_amt", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("total_uvhp_interest_amt", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("ytd_hours_for_last_comp_year", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("total_hours", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("ytd_hours_before_last_comp_year", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_600_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("local_600_premerger_total_qualified_years", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_600_premerger_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_666_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("local_666_premerger_total_qualified_years", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_666_premerger_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_700_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("local_700_premerger_total_qualified_years", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_700_premerger_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_52_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("local_52_premerger_total_qualified_years", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_52_premerger_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_161_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("local_161_premerger_total_qualified_years", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("local_161_premerger_benefit", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("monthly_benefit_amt", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("non_taxable_amt_left", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("return_to_work_flag", typeof(String));
            ldtCompleteParticipantInfo.Columns.Add("determination_date", typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("beneficiary_first_payment_receive_date", typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("pension_stop_date", typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("total_qualified_years_at_ret", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("total_qualified_hours_at_ret", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString(), typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("lump_amt_taken_in_last_comp_yr", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("retirement_type_id", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("retirement_type_value", typeof(string));

            ldtCompleteParticipantInfo.Columns.Add("ee_amt_prior_year", typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add("uvhp_amt_prior_year", typeof(decimal));

            //PIR 787
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.plan_id.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.total_vested_years.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.vested_hours_for_last_comp_year.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.mpi_late_hours_in_last_comp_year_for_prior_years.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.mpi_5500_status_code.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.eligible_active_incr_flag.ToString(), typeof(string)); //ChangeID: 57284


           
            ldtCompleteParticipantInfo.Columns.Add("created_by", typeof(string));
            ldtCompleteParticipantInfo.Columns.Add("created_date", typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("modified_by", typeof(string));
            ldtCompleteParticipantInfo.Columns.Add("modified_date", typeof(DateTime));
            ldtCompleteParticipantInfo.Columns.Add("update_seq", typeof(int));

            ldtCompleteParticipantInfo.Columns.Add("benefit_option_code_id", typeof(int));
            ldtCompleteParticipantInfo.Columns.Add("benefit_option_code_value", typeof(String));
            
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.ee_contribution_amt.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.uvhp_contribution_amt.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.mpi_person_id.ToString(), typeof(String));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_52_pension_credits.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_52_credited_hours.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_600_pension_credits.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_600_credited_hours.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_666_pension_credits.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_666_credited_hours.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_700_pension_credits.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_700_credited_hours.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_161_pension_credits.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.local_161_credited_hours.ToString(), typeof(decimal));
            
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.ytd_hours_for_year_before_last_comp_year.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.accrued_benefit_till_previous_year.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.total_qf_yr_end_of_last_comp_year.ToString(), typeof(int));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.diff_accrued_benfit_for_late_hour.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.late_ee_contribution.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.life_annuity_amt.ToString(), typeof(decimal));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.md_flag.ToString(), typeof(string));

            //RID 71411
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.is_disability_conversion.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.is_converted_from_popup.ToString(), typeof(string));
            ldtCompleteParticipantInfo.Columns.Add(enmDataExtractionBatchInfo.dro_model.ToString(), typeof(string));

            return ldtCompleteParticipantInfo;
        }

        /// <summary>
        /// Create temp table to store work infor for particular person
        /// </summary>
        /// <param name="adtWorkInfoTempTable"></param>
        /// <returns></returns>
        private DataTable CreateTempTableToStoreWorkInfo(DataTable adtWorkInfoTempTable)
        {
            adtWorkInfoTempTable.Columns.Add("year", typeof(int));
            adtWorkInfoTempTable.Columns.Add("qualified_hours", typeof(decimal));
            adtWorkInfoTempTable.Columns.Add("vested_hours", typeof(decimal));
            //PIR 787
            adtWorkInfoTempTable.Columns.Add("PlanStartDate", typeof(DateTime));
            adtWorkInfoTempTable.Columns.Add("firstHourReported", typeof(DateTime));

            return adtWorkInfoTempTable;
        }

        private DataTable CreateTempTabletoStoreCompleteworkInfo(DataTable ldtCompleteWorkInfo)
        {
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.data_extraction_batch_hour_info_id.ToString(), typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.data_extraction_batch_info_id.ToString(), typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.person_id.ToString(), typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.employer_no.ToString(), typeof(string));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.employer_name.ToString(), typeof(string));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.computation_year.ToString(), typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.hours_reported.ToString(), typeof(decimal));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.late_hour_reported.ToString(), typeof(decimal));
            ldtCompleteWorkInfo.Columns.Add("created_by", typeof(string));
            ldtCompleteWorkInfo.Columns.Add("created_date", typeof(DateTime));
            ldtCompleteWorkInfo.Columns.Add("modified_by", typeof(string));
            ldtCompleteWorkInfo.Columns.Add("modified_date", typeof(DateTime));
            ldtCompleteWorkInfo.Columns.Add("update_seq", typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.union_code.ToString(), typeof(string));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.negative_qualified_years.ToString(), typeof(int));
            ldtCompleteWorkInfo.Columns.Add(enmDataExtractionBatchHourInfo.year_end_data_extraction_header_id.ToString(), typeof(int));

            return ldtCompleteWorkInfo;
        }

        /// <summary>
        /// set status code
        /// </summary>
        /// <param name="acdoDummyworkData"></param>
        /// <param name="astrBenefitAccountTypeValue"></param>
        /// <param name="ablnPreDeathExists"></param>
        /// <param name="ablnPostDeathExists"></param>
        /// <param name="ablnIsParticipantVested"></param>
        /// <param name="adtParticipantBenInfo"></param>
        /// //New code Added
        private string SetStatusCodeForParticipant(string astrPersonAccountStatus, string astrBenefitAccountTypeValue, string astrBeneficiaryFlag, bool ablnIsParticipantVested, string astrPayeeAccountStatus, string astrBenefitOption, DateTime adtParticipantDateofDeath
            , int aintParticipantId, DateTime adtVestedDate, DateTime adtForfietureDate, string astrBeneficiaryDOD, Collection<cdoDummyWorkData> aclbPersonWorkHistory_MPI = null)
        {
            string lstrStatusCode = string.Empty;
            bool IsInActive = false;

            //PIR 1052
            bool IsDeceased = false;

            if (adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year < DateTime.Now.Year)
            {
                IsDeceased = true;
            }

            if (aclbPersonWorkHistory_MPI != null && aclbPersonWorkHistory_MPI.Count() > 0)
            {
                int lintYear = aclbPersonWorkHistory_MPI.Max(t => t.year);
                if (aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear).Count() > 0 &&
                    aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear).FirstOrDefault().istrBisParticipantFlag == busConstant.FLAG_YES)
                {
                    IsInActive = true;
                }

                if (!IsInActive)
                {
                    if (aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear).Count() > 0 && aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear - 1).Count() > 0)
                    {
                        if (aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear).FirstOrDefault().qualified_hours < 200 &&
                            aclbPersonWorkHistory_MPI.Where(t => t.year == lintYear - 1).FirstOrDefault().qualified_hours < 200)
                        {
                            IsInActive = true;
                        }
                    }
                }
            }
            else
            {
                if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE)
                {
                    IsInActive = false;
                }
                else if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)
                {
                    IsInActive = true;
                }
            }


            #region Previous Code

            cdoPayeeAccount lParticipantsPayeeAccount = null;

            //New code Added
            if (astrBeneficiaryFlag.IsNotNullOrEmpty())
            {

                DataTable ldtPartcipantsPayeeAccount = busBase.Select<cdoPayeeAccount>
                    (new string[2] { enmPayeeAccount.person_id.ToString(), enmPayeeAccount.retiree_incr_flag.ToString() },
                    new object[2] { aintParticipantId, "N" },
                    null, null);
                if (ldtPartcipantsPayeeAccount.Rows.Count > 0)
                {
                    lParticipantsPayeeAccount = new cdoPayeeAccount();
                    lParticipantsPayeeAccount.LoadData(ldtPartcipantsPayeeAccount.Rows[0]);
                }
            }

            if (astrBenefitOption.IsNotNullOrEmpty() && astrBenefitOption == busConstant.LUMP_SUM) // WE DO NOT CARE IF DEAD OR NOT DEAD. IF LUMPSUM TAKEN WE CATEGORIZE AS LUMP
                lstrStatusCode = busConstant.LUMP;

            else if (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DISABILITY && (adtParticipantDateofDeath.IsNull() ||
                adtParticipantDateofDeath == DateTime.MinValue)
                && astrPayeeAccountStatus.IsNotNullOrEmpty() &&
                astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED
                     && astrPersonAccountStatus != busConstant.PERSON_ACCOUNT_STATUS_DECEASED)
            {
                lstrStatusCode = busConstant.DISABILITY_BENEFITS;
            }
            //New code Added
            else if (lParticipantsPayeeAccount.IsNotNull() && lParticipantsPayeeAccount.benefit_account_type_value == busConstant.BENEFIT_TYPE_DISABILITY &&
               (adtParticipantDateofDeath.IsNotNull() && adtParticipantDateofDeath != DateTime.MinValue) &&
                astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED
                && astrBenefitAccountTypeValue != busConstant.BENEFIT_TYPE_QDRO)
            {
                lstrStatusCode = busConstant.DISABILITY_BENEFITS;
            }
            //PIR 787
            else if (lParticipantsPayeeAccount.IsNull() && astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DISABILITY
                && astrPayeeAccountStatus.IsNotNullOrEmpty() && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED
                //&& astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED
                && astrBeneficiaryFlag.IsNullOrEmpty() && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)
            {
                lstrStatusCode = busConstant.DISABILITY_BENEFITS;
            }

            //PIR 787  //PIR 1052  
            else if ((astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_DECEASED || IsDeceased) && (astrBeneficiaryFlag.IsNullOrEmpty())//PIR 1052
                && (astrPayeeAccountStatus.IsNullOrEmpty() || astrBenefitAccountTypeValue.IsNullOrEmpty()))
            {
                lstrStatusCode = busConstant.PRE_RETIREMENT_DEATH;

                if (!IsInActive && ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
                {
                    lstrStatusCode = busConstant.VESTED_ACTIVE_PARTICIPANT;
                }
                else if (!IsInActive && !ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
                {
                    lstrStatusCode = busConstant.NON_VESTED_ACTIVE_PARTICIPANT;
                }
                else if (IsInActive && !ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
                {
                    lstrStatusCode = busConstant.NON_VESTED_INACTIVE;
                }
                else if (IsInActive && ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
                {
                    lstrStatusCode = busConstant.VESTED_INACTIVE;
                }
            }

            else if ((((astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_DECEASED ||
                    (adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year < DateTime.Now.Year)) && (astrBeneficiaryFlag == "D" //PIR 1052
                || astrPayeeAccountStatus == busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED)) ||
                (astrBeneficiaryFlag == "Q" && astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_QDRO && astrBeneficiaryDOD.IsNotNullOrEmpty() &&
                    Convert.ToDateTime(astrBeneficiaryDOD) != DateTime.MinValue)) &&
                ((adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year < DateTime.Now.Year) ||
                    (astrBeneficiaryDOD.IsNotNullOrEmpty() && Convert.ToDateTime(astrBeneficiaryDOD).Year < DateTime.Now.Year)))
            {
                lstrStatusCode = busConstant.POST_RETIREMENT_DEATH;
            }
            //PIR 787 04/29/2015
            else if (lParticipantsPayeeAccount.IsNull() && astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DISABILITY
                && astrPayeeAccountStatus.IsNotNullOrEmpty() && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED
                && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED
                && astrBeneficiaryFlag.IsNullOrEmpty() && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year < DateTime.Now.Year)
            {
                lstrStatusCode = busConstant.POST_RETIREMENT_DEATH;
            }

            else if (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_RETIREMENT || astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_QDRO
                     || astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_POST_RETIREMENT || astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT
                     || (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_WITHDRAWAL && astrBenefitOption == busConstant.LIFE_ANNUTIY)) //PIR 1052
            {
                lstrStatusCode = busConstant.RETIRED_PART_BENEFICIARY;
            }

            //PIR 1052
            else if (!IsInActive && ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
            {
                lstrStatusCode = busConstant.VESTED_ACTIVE_PARTICIPANT;
            }
            else if (!IsInActive && !ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
            {
                lstrStatusCode = busConstant.NON_VESTED_ACTIVE_PARTICIPANT;
            }
            else if (IsInActive && !ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
            {
                lstrStatusCode = busConstant.NON_VESTED_INACTIVE;
            }
            else if (IsInActive && ablnIsParticipantVested && ((!IsDeceased && (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE || astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE)) || (lstrStatusCode == busConstant.PRE_RETIREMENT_DEATH && adtParticipantDateofDeath != DateTime.MinValue && adtParticipantDateofDeath.Year >= DateTime.Now.Year)))
            {
                lstrStatusCode = busConstant.VESTED_INACTIVE;
            }
            #endregion

            #region Please Dont delete.. need to discuss this code with rohan once

            //if (astrBenefitOption.IsNotNullOrEmpty() && astrBenefitOption == busConstant.LUMP_SUM)
            //    lstrStatusCode = "H"; //Cash-out/Lumpsum

            //else if (astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DISABILITY && (adtParticipantDateofDeath.IsNull() || adtParticipantDateofDeath == DateTime.MinValue) 
            //         && (astrPayeeAccountStatus.IsNotNullOrEmpty() && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED))
            //    lstrStatusCode = "G"; //Disabled - Participants currently receiving benefits on Disability Retirement

            //else if (astrBenefitAccountTypeValue != busConstant.BENEFIT_TYPE_DISABILITY
            //         && (adtParticipantDateofDeath.IsNull() || adtParticipantDateofDeath == DateTime.MinValue) &&
            //         (astrPayeeAccountStatus.IsNotNullOrEmpty() && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED))
            //    lstrStatusCode = "F"; //Retiree/Beneficiary - Participants who are receiving benefits on Retirement(excluding Disability) or Beneficiaries/Alternate 
            //                          //Payee receiving benefits in prior year.

            //else if ((astrBenefitAccountTypeValue == busConstant.BENEFIT_TYPE_DEATH_PRE_RETIREMENT)
            //    || (adtParticipantDateofDeath.IsNotNull() && adtParticipantDateofDeath != DateTime.MinValue && astrPayeeAccountStatus.IsNullOrEmpty()))
            //    lstrStatusCode = "D"; //Pre-Retirement Death - Participant died prior to retirement.

            //else if ((adtParticipantDateofDeath.IsNotNull() && adtParticipantDateofDeath != DateTime.MinValue) &&
            //     (astrPayeeAccountStatus.IsNotNullOrEmpty() && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_COMPLETED && astrPayeeAccountStatus != busConstant.PAYEE_ACCOUNT_STATUS_CANCELLED))
            //    lstrStatusCode = "E"; //Retired/Disability/Bene Death - Participant who were receiving Retirement or Disability benefits died 
            //                            // in prior year or Beneficiary/Alternate Payee who were receiving benefits and died in prior year.

            //else if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE && ablnIsParticipantVested)
            //    lstrStatusCode = busConstant.VESTED_ACTIVE_PARTICIPANT;

            //else if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_ACTIVE && !ablnIsParticipantVested)
            //    lstrStatusCode = busConstant.NON_VESTED_ACTIVE_PARTICIPANT;

            //else if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE && !ablnIsParticipantVested)
            //    lstrStatusCode = busConstant.NON_VESTED_INACTIVE;

            //else if (astrPersonAccountStatus == busConstant.PERSON_ACCOUNT_STATUS_INACTIVE && ablnIsParticipantVested)
            //    lstrStatusCode = busConstant.VESTED_INACTIVE;

            #endregion


            return lstrStatusCode;
        }


        /// <summary>
        /// Set benefit option code
        /// </summary>
        /// <param name="astrBenefitOptionCodeValue"></param>
        /// <returns></returns>
        private string SetBenefitOptionCode(string astrBenefitOptionCodeValue)
        {
            string lstrBenefitCode = string.Empty;
            if (astrBenefitOptionCodeValue == busConstant.LIFE_ANNUTIY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_LIFE_ANNUITY;
            }
            else if (astrBenefitOptionCodeValue == busConstant.TWO_YEARS_CERTAIN_AND_LIFE_ANNUITY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_TWO_YR_CERTAIN_LIFE;
            }
            else if (astrBenefitOptionCodeValue == busConstant.THREE_YEARS_CERTAIN_AND_LIFE_ANNUITY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_THREE_YR_CERTAIN_LIFE;
            }
            else if (astrBenefitOptionCodeValue == busConstant.FIVE_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                lstrBenefitCode = busConstant.FIVE_YR_CERTAIN_LIFE;
            }
            else if (astrBenefitOptionCodeValue == busConstant.TEN_YEARS_TERM_CERTAIN)
            {
                lstrBenefitCode = busConstant.DATA_EXT_TEN_YR_CERTAIN;
            }
            else if (astrBenefitOptionCodeValue == busConstant.TEN_YEARS_CERTAIN_AND_LIFE_ANNUTIY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_TEN_YR_CERTAIN_LIFE;
            }
            else if (astrBenefitOptionCodeValue == busConstant.QJ50)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_50;
            }
            else if (astrBenefitOptionCodeValue == busConstant.JS66)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_66;
            }
            else if (astrBenefitOptionCodeValue == busConstant.JS75)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_75;
            }
            else if (astrBenefitOptionCodeValue == busConstant.J100)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_100;
            }
            else if (astrBenefitOptionCodeValue == busConstant.JP50)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_50_POP_UP;
            }
            else if (astrBenefitOptionCodeValue == busConstant.JOINT_75_PERCENT_POPUP_ANNUITY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_75_POP_UP;
            }
            else if (astrBenefitOptionCodeValue == busConstant.JOINT_100_PERCENT_POPUP_ANNUITY)
            {
                lstrBenefitCode = busConstant.DATA_EXT_JS_100_POP_UP;
            }

            return lstrBenefitCode;
        }

        /// <summary>
        /// Add rows in temp table
        /// </summary>
        /// <param name="adtTempTable"></param>
        /// <param name="aintPersonId"></param>
        /// <param name="acdoPerson"></param>
        /// <param name="lstrStatusCode"></param>
        /// <param name="lintTotalQualifiedYearCount"></param>
        /// <param name="lintQFYearBeforeBIS"></param>
        /// <param name="ldecAccruedBenefitForPreviousCompYr"></param>
        /// <param name="ldecTotalAccruedBenefit"></param>
        /// <param name="ldecEEContribution"></param>
        /// <param name="ldecEEInterest"></param>
        /// <param name="ldecUVHPContribution"></param>
        /// <param name="ldecUVHPInterest"></param>
        /// <param name="ldecHoursForLastCompYear"></param>
        /// <param name="ldecTotalHoursTillLastCompYear"></param>
        /// <param name="ldecHoursBeforeLastCompYear"></param>
        /// <param name="ldecGrossMonthlyAmt"></param>
        /// <param name="ldtDeterminationDate"></param>
        /// <param name="ldtBeneficiaryFirstPaymentDate"></param>
        /// <param name="ldtPensionStopDate"></param>
        /// <param name="lstrRetirementType"></param>
        /// <param name="ldtbLocalDataInfo"></param>
        private DataRow AddRowsInTempTable(DataTable adtTempTable, int aintHeaderID, int aintPersonId, DataRow acdoPerson, string lstrStatusCode, int lintTotalQualifiedYearCount,
                                         int lintQFYearBeforeBIS, decimal ldecAccruedBenefitForPreviousCompYr, decimal ldecTotalAccruedBenefit,
                                         decimal ldecEEContribution, decimal ldecEEInterest, decimal ldecUVHPContribution, decimal ldecUVHPInterest,
                                         decimal ldecHoursForLastCompYear, decimal ldecTotalHoursTillLastCompYear, decimal ldecTotalHoursBeforeLastCompYear,
                                         decimal ldecGrossMonthlyAmt, DateTime ldtDeterminationDate, DateTime ldtBeneficiaryFirstPaymentDate,
                                         DateTime ldtPensionStopDate, string lstrRetirementType, string astrLocal600Flag, decimal adecLocal600PremergerQualifiedYears,
                                         decimal adecLocal600PremergerBenefit, string astrLocal666Flag, decimal adecLocal666PremergerQualifiedYears, decimal adecLocal666PremergerBenefit,
                                         string astrLocal700Flag, decimal adecLocal700PremergerQualifiedYears, decimal adecLocal700PremergerBenefit,
                                         string astrLocal52Flag, decimal adecLocal52PremergerQualifiedYears, decimal adecLocal52PremergerBenefit,
                                         string astrLocal161Flag, decimal adecLocal161PremergerQualifiedYears, decimal adecLocal161PremergerBenefit,
                                         decimal adecPriorYearIAPAccBal, decimal adecAlloc1Amt, decimal adecAlloc2Amt, decimal adecAlloc4Amt,
                                         decimal adecIAPAlloc2ForfeitureAmt, decimal adecIAPAlloc4ForfeitureAmt, decimal adecIAPAlloc2InvestmentAmt,
                                         decimal adecIAPAlloc4InvestmentAmt, decimal adecPriorYrEEAmt, decimal adecPriorYrUVHPAmt,
                                        decimal aintTotalQFYrBeginingOfLastCompYr, decimal adecEEAmtCurrentYr, decimal adecUVHPAmtCurrentYr,
                                        decimal adecLocal52PensionCredits, decimal adecLocal52CreditedHours, decimal adecLocal600PensionCredits, decimal adecLocal600CreditedHours,
                                        decimal adecLocal666PensionCredits, decimal adecLocal666CreditedHours, decimal adecLocal700PensionCredits, decimal adecLocal700CreditedHours,
                                        decimal adecLocal161PensionCredits, decimal adecLocal161CreditedHours, decimal adecLateIAPAdjustmentAmt, decimal adecCurrentYearIAPPayment,
                                        decimal adecPriorYrLocal52AccBal, decimal adecLocal52Alloc1Amt, decimal adecLocal52Alloc2Amt, decimal adecLocal52Alloc4Amt,
                                        decimal adecPriorYrLocal161AccBal, decimal adecLocal161Alloc1Amt, decimal adecLocal161Alloc2Amt, decimal adecLocal161Alloc4Amt,
                                        decimal adecLocal52LateAdjustmentAmt, decimal adecLocal161LateAdjustmentAmt, decimal adecHrForYrBeforeLAstCompYr,
                                        decimal adecTotalQFHoursAtRet, int aintTotalQFYrsAtRet, string astrBenefitCode, decimal adecAccruedBenefitTillPreviousComputationYear,
                                        DataTable adtListOfAllBeneficiaries, int aintTotalQFYrEndOfLastCompYr, decimal adecDiffInAccruedBenefitForLateHours,
                                        decimal adecLateEEContributions, decimal ldecRemainingMinimumGuranteeAmount, decimal adecLifeAnnuityAmt, decimal adecNonEligibleBenefits, string astrMDFlag,
                                        int aintPlanId, int aintTotalVestedYears, decimal adecVestedHoursForLastCompYear, decimal adecMpiLateHoursInLastCompYearForPriorYears, string astr5500StatusCode, string astrEligibleActiveIncrFlag
                   , string strIsDisabilityConversion, string strIsConvertedFromPopup, string strDROModel  //RID 71411                
            ) //ChangeID: 57284
        {


            DataRow ldrTempTableDataRow = adtTempTable.NewRow();
            
            DataTable ldtbBeneficiaryInfo = new DataTable();
            DataRow[] ldrSpouseInfo = null;

            if (adtListOfAllBeneficiaries != null && adtListOfAllBeneficiaries.Rows.Count > 0)
            {
                if (adtListOfAllBeneficiaries.AsEnumerable().Where(item => item.Field<int>(
                                           enmPerson.person_id.ToString()) == aintPersonId).Count() > 0)
                {
                    ldtbBeneficiaryInfo = adtListOfAllBeneficiaries.AsEnumerable().Where(item => item.Field<int>(
                                               enmPerson.person_id.ToString()) == aintPersonId).CopyToDataTable();
                }

                if (ldtbBeneficiaryInfo.Rows.Count > 0)
                    ldrSpouseInfo = adtListOfAllBeneficiaries.FilterTable(utlDataType.Numeric, enmPerson.person_id.ToString(), aintPersonId);
            }

            //RID 71411
            ldrTempTableDataRow[enmDataExtractionBatchInfo.is_disability_conversion.ToString()] = strIsDisabilityConversion;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.is_converted_from_popup.ToString()] = strIsConvertedFromPopup;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.dro_model.ToString()] = strDROModel;

            ldrTempTableDataRow[enmDataExtractionBatchInfo.year_end_data_extraction_header_id.ToString()] = aintHeaderID;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.person_id.ToString()] = aintPersonId;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.md_flag.ToString()] = astrMDFlag;

            //PIR 787
            ldrTempTableDataRow[enmDataExtractionBatchInfo.plan_id.ToString()] = aintPlanId;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_vested_years.ToString()] = aintTotalVestedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.vested_hours_for_last_comp_year.ToString()] = adecVestedHoursForLastCompYear;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.mpi_late_hours_in_last_comp_year_for_prior_years.ToString()] = adecMpiLateHoursInLastCompYearForPriorYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.mpi_5500_status_code.ToString()] = astr5500StatusCode;
            //ChangeID: 57284
            ldrTempTableDataRow[enmDataExtractionBatchInfo.eligible_active_incr_flag.ToString()] = astrEligibleActiveIncrFlag;
                      
            ldrTempTableDataRow[enmDataExtractionBatchInfo.person_id.ToString()] = aintPersonId;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.person_name.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_name.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.person_ssn.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);

            ldrTempTableDataRow[enmDataExtractionBatchInfo.person_gender_id.ToString()] = 6014;
            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.person_gender_value.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.person_dob.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.participant_date_of_death.ToString()] =
                                                                Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]);
           

            #region set benficiary Info

            ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_flag.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_id.ToString()] = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]);

            ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_name.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_name.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]);
            ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_gender_id.ToString()] = 6014;
            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_gender_value.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_gender_value.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_dob.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()] = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]);
            #endregion
          
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_qualified_years.ToString()] = lintTotalQualifiedYearCount;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.participant_state_id.ToString()] = 150;

            if (Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]).IsNotNullOrEmpty())
                ldrTempTableDataRow[enmDataExtractionBatchInfo.participant_state_value.ToString()] = Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]);
            ldrTempTableDataRow[enmDataExtractionBatchInfo.last_qf_yr_before_bis.ToString()] = lintQFYearBeforeBIS;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.non_eligible_benefit.ToString()] = adecNonEligibleBenefits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.accrued_benefit_for_prior_year.ToString()] = ldecAccruedBenefitForPreviousCompYr;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.accrued_benefit_till_last_comp_year.ToString()] = ldecTotalAccruedBenefit;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_ee_contribution_amt.ToString()] = ldecEEContribution;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_uvhp_amt.ToString()] = ldecUVHPContribution;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_ee_interest_amt.ToString()] = ldecEEInterest;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_uvhp_interest_amt.ToString()] = ldecUVHPInterest;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.ytd_hours_for_last_comp_year.ToString()] = ldecHoursForLastCompYear;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_hours.ToString()] = ldecTotalHoursTillLastCompYear;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.ytd_hours_before_last_comp_year.ToString()] = ldecTotalHoursBeforeLastCompYear;

            #region Local Plan Info

            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_600_flag.ToString()] = astrLocal600Flag;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_600_premerger_total_qualified_years.ToString()] = adecLocal600PremergerQualifiedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_600_premerger_benefit.ToString()] = adecLocal600PremergerBenefit;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_666_flag.ToString()] = astrLocal666Flag;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_666_premerger_total_qualified_years.ToString()] = adecLocal666PremergerQualifiedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_666_premerger_benefit.ToString()] = adecLocal666PremergerBenefit;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_700_flag.ToString()] = astrLocal700Flag;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_700_premerger_total_qualified_years.ToString()] = adecLocal700PremergerQualifiedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_700_premerger_benefit.ToString()] = adecLocal700PremergerBenefit;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_52_flag.ToString()] = astrLocal52Flag;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_52_premerger_total_qualified_years.ToString()] = adecLocal52PremergerQualifiedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_52_premerger_benefit.ToString()] = adecLocal52PremergerBenefit;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_161_flag.ToString()] = astrLocal161Flag;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_161_premerger_total_qualified_years.ToString()] = adecLocal161PremergerQualifiedYears;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_161_premerger_benefit.ToString()] = adecLocal161PremergerBenefit;

            #endregion

            if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNullOrEmpty() ||
                (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                    Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                ldrTempTableDataRow[enmDataExtractionBatchInfo.monthly_benefit_amt.ToString()] = ldecGrossMonthlyAmt;

            ldrTempTableDataRow[enmDataExtractionBatchInfo.non_taxable_amt_left.ToString()] = ldecRemainingMinimumGuranteeAmount;

            if (Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]) == busConstant.FLAG_YES)
            {
                ldrTempTableDataRow[enmDataExtractionBatchInfo.return_to_work_flag.ToString()] = busConstant.FLAG_YES;
            }
            else
            {
                ldrTempTableDataRow[enmDataExtractionBatchInfo.return_to_work_flag.ToString()] = busConstant.FLAG_NO;
            }

            if (ldtDeterminationDate != DateTime.MinValue)
                ldrTempTableDataRow[enmDataExtractionBatchInfo.determination_date.ToString()] = ldtDeterminationDate;


            if (ldtBeneficiaryFirstPaymentDate != DateTime.MinValue)
                ldrTempTableDataRow[enmDataExtractionBatchInfo.beneficiary_first_payment_receive_date.ToString()] = ldtBeneficiaryFirstPaymentDate;


            if (ldtPensionStopDate != DateTime.MinValue)
                ldrTempTableDataRow[enmDataExtractionBatchInfo.pension_stop_date.ToString()] = ldtPensionStopDate;


            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_qualified_years_at_ret.ToString()] = aintTotalQFYrsAtRet;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_qualified_hours_at_ret.ToString()] = adecTotalQFHoursAtRet;

            if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
            {
                ldrTempTableDataRow[enmDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr.ToString()] = ldecGrossMonthlyAmt;
            }
            else
            {
                ldrTempTableDataRow[enmDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr.ToString()] = 0.0M;
            }
            ldrTempTableDataRow[enmDataExtractionBatchInfo.ee_amt_prior_year.ToString()] = adecPriorYrEEAmt;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.uvhp_amt_prior_year.ToString()] = adecPriorYrUVHPAmt;
           
            ldrTempTableDataRow["created_by"] = iobjPassInfo.istrUserID;
            ldrTempTableDataRow["modified_by"] = iobjPassInfo.istrUserID;
            ldrTempTableDataRow["created_date"] = DateTime.Now;
            ldrTempTableDataRow["modified_date"] = DateTime.Now;
            ldrTempTableDataRow["update_seq"] = 0;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.status_code_id.ToString()] = busConstant.DATA_EXT_STATUS_CODE_ID;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.status_code_value.ToString()] = lstrStatusCode;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.retirement_type_id.ToString()] = busConstant.DATA_EXT_RET_TYPE_ID;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.retirement_type_value.ToString()] = lstrRetirementType;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.benefit_option_code_id.ToString()] = 7056;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.benefit_option_code_value.ToString()] = astrBenefitCode;
            
            ldrTempTableDataRow[enmDataExtractionBatchInfo.ee_contribution_amt.ToString()] = adecEEAmtCurrentYr;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.mpi_person_id.ToString()] =
                                                Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.mpi_person_id.ToString()]);
            ldrTempTableDataRow[enmDataExtractionBatchInfo.uvhp_contribution_amt.ToString()] = adecUVHPAmtCurrentYr;

            #region Local Plan fields

            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_52_pension_credits.ToString()] = adecLocal52PensionCredits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_52_credited_hours.ToString()] = adecLocal52CreditedHours;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_600_pension_credits.ToString()] = adecLocal600PensionCredits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_600_credited_hours.ToString()] = adecLocal600CreditedHours;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_666_pension_credits.ToString()] = adecLocal666PensionCredits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_666_credited_hours.ToString()] = adecLocal666CreditedHours;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_700_pension_credits.ToString()] = adecLocal700PensionCredits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_700_credited_hours.ToString()] = adecLocal700CreditedHours;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_161_pension_credits.ToString()] = adecLocal161PensionCredits;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.local_161_credited_hours.ToString()] = adecLocal161CreditedHours;

            #endregion
            
            ldrTempTableDataRow[enmDataExtractionBatchInfo.ytd_hours_for_year_before_last_comp_year.ToString()] = adecHrForYrBeforeLAstCompYr;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.accrued_benefit_till_previous_year.ToString()] = adecAccruedBenefitTillPreviousComputationYear;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.total_qf_yr_end_of_last_comp_year.ToString()] = (aintTotalQFYrEndOfLastCompYr);
            ldrTempTableDataRow[enmDataExtractionBatchInfo.diff_accrued_benfit_for_late_hour.ToString()] = adecDiffInAccruedBenefitForLateHours;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.late_ee_contribution.ToString()] = adecLateEEContributions;
            ldrTempTableDataRow[enmDataExtractionBatchInfo.life_annuity_amt.ToString()] = adecLifeAnnuityAmt;

            return ldrTempTableDataRow;
        }

        private DataRow AddDataToTempWorkInfoTable(DataTable adtDataExtractionBatchHourInfo, int aintPersonId, string astrEmployerNo,
                                                string astrEmployerName, int aintComputationYear, decimal adecHours, decimal adecLateHours, string astrUnionCode,
                                                int aintHeaderId)
        {
            DataRow ldrTempTableDataRow = adtDataExtractionBatchHourInfo.NewRow();

            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.data_extraction_batch_info_id.ToString()] = 0;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.person_id.ToString()] = aintPersonId;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.employer_no.ToString()] = astrEmployerNo;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.employer_name.ToString()] = astrEmployerName;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.computation_year.ToString()] = aintComputationYear;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.hours_reported.ToString()] = adecHours;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.late_hour_reported.ToString()] = adecLateHours;

            ldrTempTableDataRow["created_by"] = "OPUSBatch";
            ldrTempTableDataRow["created_date"] = DateTime.Now;
            ldrTempTableDataRow["modified_by"] = "OPUSBatch";
            ldrTempTableDataRow["modified_date"] = DateTime.Now;
            ldrTempTableDataRow["update_seq"] = 0;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.union_code.ToString()] = astrUnionCode;
            ldrTempTableDataRow[enmDataExtractionBatchHourInfo.year_end_data_extraction_header_id.ToString()] = lintDataExtractionHeaderId;
            if (adecHours < 400)
            {
                ldrTempTableDataRow[enmDataExtractionBatchHourInfo.negative_qualified_years.ToString()] = -1;
            }
            else
            {
                ldrTempTableDataRow[enmDataExtractionBatchHourInfo.negative_qualified_years.ToString()] = 0;
            }
            
            return ldrTempTableDataRow;
        }


        #region Create Pension Actuary Change Report

        private void CreatePensionActuaryChangeReport()
        {

            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            busBase lobjBase = new busBase();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }

            utlPassInfo lobjPassInfo1 = new utlPassInfo();
            lobjPassInfo1.idictParams = ldictParams;
            lobjPassInfo1.iconFramework = DBFunction.GetDBConnection();
            utlPassInfo.iobjPassInfo = lobjPassInfo1;

            DataTable ldtbDataToGenerateReport = new DataTable();
            ArrayList larrPerosnID = new ArrayList();

            ldtbDataToGenerateReport.Columns.Add(new DataColumn("MPID", Type.GetType("System.String")));
            ldtbDataToGenerateReport.Columns.Add(new DataColumn("TYPE", Type.GetType("System.String")));
            ldtbDataToGenerateReport.Columns.Add(new DataColumn("OLD_VALUE", Type.GetType("System.String")));
            ldtbDataToGenerateReport.Columns.Add(new DataColumn("NEW_VALUE", Type.GetType("System.String")));

            //Sid Jain 01182013
            DataTable ldtbOldData = busBase.Select("cdoDataExtractionBatchInfo.GetDataForGivenYear", new object[1] { iintYear - 1 });
            DataTable ldtbNewData = busBase.Select("cdoDataExtractionBatchInfo.GetDataForLastYear", new object[1] { iintYear });

            foreach (DataRow ldrPersonID in ldtbOldData.Rows)
            {
                foreach (DataRow ldrNewPersonID in ldtbNewData.Rows)
                {
                    if (!larrPerosnID.Contains(Convert.ToInt32(ldrNewPersonID["PERSON_ID"])))
                    {
                        larrPerosnID.Add(larrPerosnID);

                        if (Convert.ToInt32(ldrPersonID["PERSON_ID"]) == Convert.ToInt32(ldrNewPersonID["PERSON_ID"]) &&
                               Convert.ToString(ldrNewPersonID["BENEFICIARY_FLAG"]).IsNullOrEmpty())
                        {
                            
                            string lstrMPID = Convert.ToString(ldrNewPersonID["MPI_PERSON_ID"]);
                            
                            if (Convert.ToString(ldrPersonID["PERSON_DOB"]) != Convert.ToString(ldrNewPersonID["PERSON_DOB"]))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Date of Birth";
                                ldrDataToGenerateReport["OLD_VALUE"] = String.Format("{0:d}", Convert.ToDateTime(ldrPersonID["PERSON_DOB"]));
                                ldrDataToGenerateReport["NEW_VALUE"] = String.Format("{0:d}", Convert.ToDateTime(ldrNewPersonID["PERSON_DOB"]));

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if (Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) != "I" && Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"]) == "I")
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Status Code(New Vested Active)";
                                ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["StatusCodeDescription"]);
                                ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["StatusCodeDescription"]);

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if (Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) == "I" &&
                                Convert.ToString(ldrPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty() &&
                                Convert.ToInt32(ldrPersonID["TOTAL_QUALIFIED_YEARS"]) >= 5)
                            {
                                if (Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"]) != "I" || Convert.ToString(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]).IsNullOrEmpty()
                                    || (Convert.ToString(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty() && Convert.ToInt32(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]) <= 5))
                                {
                                    DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                    ldrDataToGenerateReport["MPID"] = lstrMPID;
                                    ldrDataToGenerateReport["TYPE"] = "Status Code(Previously Vested Active)";
                                    ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["StatusCodeDescription"]);
                                    ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["StatusCodeDescription"]);

                                    ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                                }
                            }
                            if (Convert.ToString(ldrPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]).IsNotNullOrEmpty() && Convert.ToString(ldrNewPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]).IsNotNullOrEmpty()
                                && Convert.ToDecimal(ldrPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]) > Convert.ToDecimal(ldrNewPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Benefits less than Last Year";
                                
                                ldrDataToGenerateReport["OLD_VALUE"] = String.Format("{0:0.00}", Convert.ToDecimal(ldrPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]));
                                ldrDataToGenerateReport["NEW_VALUE"] = String.Format("{0:0.00}", Convert.ToDecimal(ldrNewPersonID["ACCRUED_BENEFIT_TILL_LAST_COMP_YEAR"]));
                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }

                            int lNewYears = 0, loldYears = 0;

                            if (Convert.ToString(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty())
                            {
                                lNewYears = Convert.ToInt32(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]);
                            }
                            if (Convert.ToString(ldrPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty())
                            {
                                loldYears = Convert.ToInt32(ldrPersonID["TOTAL_QUALIFIED_YEARS"]);
                            }
                            int lYearDifference = lNewYears - loldYears;

                            if ((Convert.ToString(ldrPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty() && Convert.ToString(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty())
                                && ((Convert.ToInt32(ldrPersonID["TOTAL_QUALIFIED_YEARS"]) > Convert.ToInt32(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"])) || lYearDifference >= 2))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Qualified Years Changed";
                                ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["TOTAL_QUALIFIED_YEARS"]);
                                ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]);

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if (Convert.ToString(ldrPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]).IsNotNullOrEmpty() && Convert.ToString(ldrNewPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]).IsNotNullOrEmpty() &&
                                Convert.ToDecimal(ldrPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]) > Convert.ToDecimal(ldrNewPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Total YTD Hours less than Last Year";
                                
                                ldrDataToGenerateReport["OLD_VALUE"] = String.Format("{0:0.00}", Convert.ToDecimal(ldrPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]));
                                ldrDataToGenerateReport["NEW_VALUE"] = String.Format("{0:0.00}", Convert.ToDecimal(ldrNewPersonID["YTD_HOURS_FOR_LAST_COMP_YEAR"]));

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if ((Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) == "A" || Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) == "I") &&
                                (Convert.ToString(ldrPersonID["TOTAL_QUALIFIED_YEARS"]).IsNotNullOrEmpty() && Convert.ToInt32(ldrPersonID["TOTAL_QUALIFIED_YEARS"]) >= 1))
                            {
                                if ((Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) != Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"])) || (Convert.ToInt32(ldrNewPersonID["TOTAL_QUALIFIED_YEARS"]) <= 1))
                                {
                                    DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                    ldrDataToGenerateReport["MPID"] = lstrMPID;
                                    ldrDataToGenerateReport["TYPE"] = "New Participant with more than 1 Qualified Year";
                                    ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["StatusCodeDescription"]);
                                    ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["StatusCodeDescription"]);

                                    ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                                }
                            }

                            if ((Convert.ToString(ldrPersonID["STATUS_CODE_VALUE"]) == "F") && (Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"]) == "A" || Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"]) == "B" || Convert.ToString(ldrNewPersonID["STATUS_CODE_VALUE"]) == "C"))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Status Change";
                                ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["StatusCodeDescription"]);
                                ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["StatusCodeDescription"]);

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if (Convert.ToString(ldrPersonID["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() && Convert.ToString(ldrNewPersonID["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                               Convert.ToString(ldrPersonID["BENEFIT_OPTION_CODE_VALUE"]) != Convert.ToString(ldrNewPersonID["BENEFIT_OPTION_CODE_VALUE"]) ||
                               (Convert.ToString(ldrPersonID["BENEFIT_OPTION_CODE_VALUE"]).IsNullOrEmpty() && Convert.ToString(ldrNewPersonID["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty()))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Benefit Option Change";
                                ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["BenefitOptionCodeDescription"]);
                                ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["BenefitOptionCodeDescription"]);

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                            if (Convert.ToString(ldrPersonID["BENEFICIARY_FLAG"]) != Convert.ToString(ldrNewPersonID["BENEFICIARY_FLAG"]))
                            {
                                DataRow ldrDataToGenerateReport = ldtbDataToGenerateReport.NewRow();
                                ldrDataToGenerateReport["MPID"] = lstrMPID;
                                ldrDataToGenerateReport["TYPE"] = "Change In QDRO";
                                ldrDataToGenerateReport["OLD_VALUE"] = Convert.ToString(ldrPersonID["BENEFICIARY_FLAG"]);
                                ldrDataToGenerateReport["NEW_VALUE"] = Convert.ToString(ldrNewPersonID["BENEFICIARY_FLAG"]);

                                ldtbDataToGenerateReport.Rows.Add(ldrDataToGenerateReport);
                            }
                        }
                    }
                }
            }

            ExecuteFinalReports(ldtbDataToGenerateReport, null);

            if (lobjPassInfo1.iconFramework.State == ConnectionState.Open)
            {
                lobjPassInfo1.iconFramework.Close();
            }

            lobjPassInfo1.iconFramework.Dispose();
            lobjPassInfo1.iconFramework = null;

        }


        private int ExecuteFinalReports(DataTable adtPersonData, string astrReportPrefixPaymentScheduleID)
        {
            int lintrtn = 0;
            string lstrReportPath = string.Empty;
            busCreateReports lobjCreateReports = new busCreateReports();
            List<string> llstGeneratedReports = new List<string>();
            string lstrReportPrefixPaymentScheduleID = string.Empty;

            try
            {
                idlgUpdateProcessLog("Pension Actuary Change Report", "INFO", istrProcessName);
                adtPersonData.TableName = "rptPensionActuaryChangeReport";
                if (adtPersonData.Rows.Count > 0)
                {
                    lstrReportPath = CreatePDFReport(adtPersonData, "rptPensionActuaryChangeReport", astrReportPrefixPaymentScheduleID);
                    llstGeneratedReports.Add(lstrReportPath);
                    idlgUpdateProcessLog("Pension Actuary Change Report generated succesfully", "INFO", istrProcessName);
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
                idlgUpdateProcessLog("Pension Actuary Change Report Failed.", "INFO", istrProcessName);
                return -1;
            }

            return lintrtn;
        }

        #endregion

        private void FormatDataForExportToExcel(DataTable ldtTempDataExtractionTable)
        {
            #region Format and Copy data to excel sheet

            DataTable ldtbExcelInfo = new DataTable();
            ldtbExcelInfo = ldtTempDataExtractionTable.Copy();
            ldtbExcelInfo.Columns.Remove("data_extraction_batch_info_id");
            ldtbExcelInfo.Columns.Remove("year_end_data_extraction_header_id");
            ldtbExcelInfo.Columns.Remove("person_id");
            ldtbExcelInfo.Columns.Remove("person_gender_id");
            ldtbExcelInfo.Columns.Remove("beneficiary_gender_id");
            ldtbExcelInfo.Columns.Remove("status_code_id");
            ldtbExcelInfo.Columns.Remove("participant_state_id");
            ldtbExcelInfo.Columns.Remove("created_by");
            ldtbExcelInfo.Columns.Remove("created_date");
            ldtbExcelInfo.Columns.Remove("modified_by");
            ldtbExcelInfo.Columns.Remove("modified_date");
            ldtbExcelInfo.Columns.Remove("update_seq");
            ldtbExcelInfo.Columns.Remove("benefit_option_code_id");
            ldtbExcelInfo.Columns.Remove("retirement_type_id");
            ldtbExcelInfo.AcceptChanges();

            DateTime ldtDefaultDate = new DateTime(1753, 01, 01);

            foreach (DataRow ldr in ldtbExcelInfo.Rows)
            {
                
                #region Set Beneficiary Description

                if (Convert.ToString(ldr["person_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["person_gender_value"]) == busConstant.FEMALE)
                {
                    ldr["person_gender_value"] = "Female";
                }
                else if (Convert.ToString(ldr["person_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["person_gender_value"]) == busConstant.MALE)
                {
                    ldr["person_gender_value"] = "Male";
                }
                else if (Convert.ToString(ldr["person_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["person_gender_value"]) == busConstant.UNKNOWN)
                {
                    ldr["person_gender_value"] = "Uknown";
                }

                if (Convert.ToString(ldr["beneficiary_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["beneficiary_gender_value"]) == busConstant.FEMALE)
                {
                    ldr["beneficiary_gender_value"] = "Female";
                }
                else if (Convert.ToString(ldr["beneficiary_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["beneficiary_gender_value"]) == busConstant.MALE)
                {
                    ldr["beneficiary_gender_value"] = "Male";
                }
                else if (Convert.ToString(ldr["beneficiary_gender_value"]).IsNotNullOrEmpty() && Convert.ToString(ldr["beneficiary_gender_value"]) == busConstant.UNKNOWN)
                {
                    ldr["beneficiary_gender_value"] = "Uknown";
                }

                #endregion
                            

                if (Convert.ToString(ldr[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]).IsNotNullOrEmpty() &&
                    Convert.ToDateTime(ldr[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]) == ldtDefaultDate)
                {
                    ldr[enmDataExtractionBatchInfo.participant_date_of_death.ToString()] = DBNull.Value;
                }
                if (Convert.ToString(ldr[enmDataExtractionBatchInfo.determination_date.ToString()]).IsNotNullOrEmpty() &&
                    Convert.ToDateTime(ldr[enmDataExtractionBatchInfo.determination_date.ToString()]) == ldtDefaultDate)
                {
                    ldr[enmDataExtractionBatchInfo.determination_date.ToString()] = DBNull.Value;
                }
                if (Convert.ToString(ldr[enmDataExtractionBatchInfo.beneficiary_first_payment_receive_date.ToString()]).IsNotNullOrEmpty() &&
                    Convert.ToDateTime(ldr[enmDataExtractionBatchInfo.beneficiary_first_payment_receive_date.ToString()]) == ldtDefaultDate)
                {
                    ldr[enmDataExtractionBatchInfo.beneficiary_first_payment_receive_date.ToString()] = DBNull.Value;
                }
                if (Convert.ToString(ldr[enmDataExtractionBatchInfo.pension_stop_date.ToString()]).IsNotNullOrEmpty() &&
                    Convert.ToDateTime(ldr[enmDataExtractionBatchInfo.pension_stop_date.ToString()]) == ldtDefaultDate)
                {
                    ldr[enmDataExtractionBatchInfo.pension_stop_date.ToString()] = DBNull.Value;
                }
                if (Convert.ToString(ldr[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]).IsNotNullOrEmpty() &&
                    Convert.ToDateTime(ldr[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]) == ldtDefaultDate)
                {
                    ldr[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()] = DBNull.Value;
                }
            }

            ExportExcel(ldtbExcelInfo);

            #endregion
        }

        private void ExportExcel(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return;
            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();

            if (xlApp == null)
            {
                return;
            }
            System.Globalization.CultureInfo CurrentCI = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range;
            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];
                range.Interior.ColorIndex = 15;
                range.Font.Bold = true;
            }
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dt.Rows[r][i].ToString();
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }

            string lstrFileName = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.ANNUAL_EXTRACTION_PATH_GENERATED) + "AnnualExtractionData" + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" +
                                       DateTime.Now.Year + "_" + DateTime.Now.Hour + "_" + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".xls";

            worksheet.SaveAs(@lstrFileName);
            workbooks.Close();
            xlApp.Quit();

        }

        private void LoadAnnualStatementRequest(string astrStatusValue)
        {
            ibusYearEndProcessRequest = new busYearEndProcessRequest { icdoYearEndProcessRequest = new cdoYearEndProcessRequest() };

            DataTable ldt1099rRequests = busBase.Select<cdoYearEndProcessRequest>
                (new string[2] { enmYearEndProcessRequest.status_value.ToString(), enmYearEndProcessRequest.year_end_process_value.ToString() },
                new object[2] { astrStatusValue, busConstant.YEAR_END_PROC_ANNUAL_STATEMENT },
                null, "YEAR desc");
            if (ldt1099rRequests.Rows.Count > 0)
            {
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.LoadData(ldt1099rRequests.Rows[0]);
            }
        }

        private void UpdateBatchRequest()
        {
            try
            {
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_value = busConstant.BatchRequest1099rStatusComplete;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.processed_date = iobjSystemManagement.icdoSystemManagement.batch_date; //asharma Changed on 12/3/2012 based on discussion with Vinovin
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.status_date = iobjSystemManagement.icdoSystemManagement.batch_date;
                ibusYearEndProcessRequest.icdoYearEndProcessRequest.Update();
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("Updating Annual 1099r Batch Request failed", "INFO", istrProcessName);
                ExceptionManager.Publish(ex);
                throw ex;
            }
        }

        //ChangeID: 57284
        private bool IsEligibleForActiveIncrease(busBenefitApplication lbusBenefitApplication)
        {
            bool iblnEligibleForActiveIncrease = false;

            if (lbusBenefitApplication.aclbPersonWorkHistory_MPI.Where(t => (t.idecBenefitRate == ldecBenefitRateupto10QY || t.idecBenefitRate == ldecBenefitRateafter10QY) && t.idecBenefitAmount > 0).Count() > 0)
            {
                iblnEligibleForActiveIncrease = true;
            }                
            
            return iblnEligibleForActiveIncrease;
        }

        #region Framework Upgrade changes
        /// <summary>
        /// Add rows in Database table
        /// </summary>
        private void AddRowsInDatabase(int aintHeaderID, int aintPersonId, DataRow acdoPerson, string lstrStatusCode, int lintTotalQualifiedYearCount,
                                        int lintQFYearBeforeBIS, decimal ldecAccruedBenefitForPreviousCompYr, decimal ldecTotalAccruedBenefit,
                                        decimal ldecEEContribution, decimal ldecEEInterest, decimal ldecUVHPContribution, decimal ldecUVHPInterest,
                                        decimal ldecHoursForLastCompYear, decimal ldecTotalHoursTillLastCompYear, decimal ldecTotalHoursBeforeLastCompYear,
                                        decimal ldecGrossMonthlyAmt, DateTime ldtDeterminationDate, DateTime ldtBeneficiaryFirstPaymentDate,
                                        DateTime ldtPensionStopDate, string lstrRetirementType, string astrLocal600Flag, decimal adecLocal600PremergerQualifiedYears,
                                        decimal adecLocal600PremergerBenefit, string astrLocal666Flag, decimal adecLocal666PremergerQualifiedYears, decimal adecLocal666PremergerBenefit,
                                        string astrLocal700Flag, decimal adecLocal700PremergerQualifiedYears, decimal adecLocal700PremergerBenefit,
                                        string astrLocal52Flag, decimal adecLocal52PremergerQualifiedYears, decimal adecLocal52PremergerBenefit,
                                        string astrLocal161Flag, decimal adecLocal161PremergerQualifiedYears, decimal adecLocal161PremergerBenefit,
                                        decimal adecPriorYearIAPAccBal, decimal adecAlloc1Amt, decimal adecAlloc2Amt, decimal adecAlloc4Amt,
                                        decimal adecIAPAlloc2ForfeitureAmt, decimal adecIAPAlloc4ForfeitureAmt, decimal adecIAPAlloc2InvestmentAmt,
                                        decimal adecIAPAlloc4InvestmentAmt, decimal adecPriorYrEEAmt, decimal adecPriorYrUVHPAmt,
                                       decimal aintTotalQFYrBeginingOfLastCompYr, decimal adecEEAmtCurrentYr, decimal adecUVHPAmtCurrentYr,
                                       decimal adecLocal52PensionCredits, decimal adecLocal52CreditedHours, decimal adecLocal600PensionCredits, decimal adecLocal600CreditedHours,
                                       decimal adecLocal666PensionCredits, decimal adecLocal666CreditedHours, decimal adecLocal700PensionCredits, decimal adecLocal700CreditedHours,
                                       decimal adecLocal161PensionCredits, decimal adecLocal161CreditedHours, decimal adecLateIAPAdjustmentAmt, decimal adecCurrentYearIAPPayment,
                                       decimal adecPriorYrLocal52AccBal, decimal adecLocal52Alloc1Amt, decimal adecLocal52Alloc2Amt, decimal adecLocal52Alloc4Amt,
                                       decimal adecPriorYrLocal161AccBal, decimal adecLocal161Alloc1Amt, decimal adecLocal161Alloc2Amt, decimal adecLocal161Alloc4Amt,
                                       decimal adecLocal52LateAdjustmentAmt, decimal adecLocal161LateAdjustmentAmt, decimal adecHrForYrBeforeLAstCompYr,
                                       decimal adecTotalQFHoursAtRet, int aintTotalQFYrsAtRet, string astrBenefitCode, decimal adecAccruedBenefitTillPreviousComputationYear,
                                       DataTable adtListOfAllBeneficiaries, int aintTotalQFYrEndOfLastCompYr, decimal adecDiffInAccruedBenefitForLateHours,
                                       decimal adecLateEEContributions, decimal ldecRemainingMinimumGuranteeAmount, decimal adecLifeAnnuityAmt, decimal adecNonEligibleBenefits, string astrMDFlag,
                                       int aintPlanId, int aintTotalVestedYears, decimal adecVestedHoursForLastCompYear, decimal adecMpiLateHoursInLastCompYearForPriorYears, string astr5500StatusCode, string astrEligibleActiveIncrFlag
                   , string strIsDisabilityConversion, string strIsConvertedFromPopup, string strDROModel  //RID 71411                
            ) //ChangeID: 57284
        {
            cdoDataExtractionBatchInfo lcdoDataExtractionBatchInfo = new cdoDataExtractionBatchInfo();

            DataTable ldtbBeneficiaryInfo = new DataTable();
            DataRow[] ldrSpouseInfo = null;

            if (adtListOfAllBeneficiaries != null && adtListOfAllBeneficiaries.Rows.Count > 0)
            {
                if (adtListOfAllBeneficiaries.AsEnumerable().Where(item => item.Field<int>(
                                           enmPerson.person_id.ToString()) == aintPersonId).Count() > 0)
                {
                    ldtbBeneficiaryInfo = adtListOfAllBeneficiaries.AsEnumerable().Where(item => item.Field<int>(
                                               enmPerson.person_id.ToString()) == aintPersonId).CopyToDataTable();
                }

                if (ldtbBeneficiaryInfo.Rows.Count > 0)
                    ldrSpouseInfo = adtListOfAllBeneficiaries.FilterTable(utlDataType.Numeric, enmPerson.person_id.ToString(), aintPersonId);
            }

            lcdoDataExtractionBatchInfo.year_end_data_extraction_header_id = aintHeaderID;
            lcdoDataExtractionBatchInfo.person_id = aintPersonId;
            lcdoDataExtractionBatchInfo.md_flag = astrMDFlag;

            //PIR 787
            lcdoDataExtractionBatchInfo.plan_id = aintPlanId;
            lcdoDataExtractionBatchInfo.total_vested_years = aintTotalVestedYears;
            lcdoDataExtractionBatchInfo.vested_hours_for_last_comp_year = adecVestedHoursForLastCompYear;
            lcdoDataExtractionBatchInfo.mpi_late_hours_in_last_comp_year_for_prior_years = adecMpiLateHoursInLastCompYearForPriorYears;
            lcdoDataExtractionBatchInfo.mpi_5500_status_code = astr5500StatusCode;
            lcdoDataExtractionBatchInfo.eligible_active_incr_flag = astrEligibleActiveIncrFlag;//ChangeID: 57284


            lcdoDataExtractionBatchInfo.person_id = aintPersonId;
            lcdoDataExtractionBatchInfo.person_name = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_name.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.person_ssn = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]);

            lcdoDataExtractionBatchInfo.person_gender_id = 6014;
            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.person_gender_value = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.person_dob = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.person_dob.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.participant_date_of_death =
                                                                Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.participant_date_of_death.ToString()]);
            #region set benficiary Info

            lcdoDataExtractionBatchInfo.beneficiary_flag = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_flag.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.beneficiary_id = Convert.ToInt32(acdoPerson[enmDataExtractionBatchInfo.beneficiary_id.ToString()]);

            lcdoDataExtractionBatchInfo.beneficiary_name = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_name.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_ssn.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.beneficiary_ssn = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_ssn.ToString()]);
            lcdoDataExtractionBatchInfo.beneficiary_gender_id = 6014;
            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.person_gender_value.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.beneficiary_gender_value = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_gender_value.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.beneficiary_dob = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.beneficiary_dob.ToString()]);

            if (Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.beneficiary_date_of_death = Convert.ToDateTime(acdoPerson[enmDataExtractionBatchInfo.beneficiary_date_of_death.ToString()]);
            #endregion

            lcdoDataExtractionBatchInfo.total_qualified_years = lintTotalQualifiedYearCount;
            lcdoDataExtractionBatchInfo.participant_state_id = 150;

            if (Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]).IsNotNullOrEmpty())
                lcdoDataExtractionBatchInfo.participant_state_value = Convert.ToString(acdoPerson[enmPersonAddress.addr_state_value.ToString()]);
            lcdoDataExtractionBatchInfo.last_qf_yr_before_bis = lintQFYearBeforeBIS;
            lcdoDataExtractionBatchInfo.non_eligible_benefit = adecNonEligibleBenefits;
            lcdoDataExtractionBatchInfo.accrued_benefit_for_prior_year = ldecAccruedBenefitForPreviousCompYr;
            lcdoDataExtractionBatchInfo.accrued_benefit_till_last_comp_year = ldecTotalAccruedBenefit;
            lcdoDataExtractionBatchInfo.total_ee_contribution_amt = ldecEEContribution;
            lcdoDataExtractionBatchInfo.total_uvhp_amt = ldecUVHPContribution;
            lcdoDataExtractionBatchInfo.total_ee_interest_amt = ldecEEInterest;
            lcdoDataExtractionBatchInfo.total_uvhp_interest_amt = ldecUVHPInterest;
            lcdoDataExtractionBatchInfo.ytd_hours_for_last_comp_year = ldecHoursForLastCompYear;
            lcdoDataExtractionBatchInfo.total_hours = ldecTotalHoursTillLastCompYear;
            lcdoDataExtractionBatchInfo.ytd_hours_before_last_comp_year = ldecTotalHoursBeforeLastCompYear;

            #region Local Plan Info
            lcdoDataExtractionBatchInfo.local_600_flag = astrLocal600Flag;
            lcdoDataExtractionBatchInfo.local_600_premerger_total_qualified_years = adecLocal600PremergerQualifiedYears;
            lcdoDataExtractionBatchInfo.local_600_premerger_benefit = adecLocal600PremergerBenefit;
            lcdoDataExtractionBatchInfo.local_666_flag = astrLocal666Flag;
            lcdoDataExtractionBatchInfo.local_666_premerger_total_qualified_years = adecLocal666PremergerQualifiedYears;
            lcdoDataExtractionBatchInfo.local_666_premerger_benefit = adecLocal666PremergerBenefit;
            lcdoDataExtractionBatchInfo.local_700_flag = astrLocal700Flag;
            lcdoDataExtractionBatchInfo.local_700_premerger_total_qualified_years = adecLocal700PremergerQualifiedYears;
            lcdoDataExtractionBatchInfo.local_700_premerger_benefit = adecLocal700PremergerBenefit;
            lcdoDataExtractionBatchInfo.local_52_flag = astrLocal52Flag;
            lcdoDataExtractionBatchInfo.local_52_premerger_total_qualified_years = adecLocal52PremergerQualifiedYears;
            lcdoDataExtractionBatchInfo.local_52_premerger_benefit = adecLocal52PremergerBenefit;
            lcdoDataExtractionBatchInfo.local_161_flag = astrLocal161Flag;
            lcdoDataExtractionBatchInfo.local_161_premerger_total_qualified_years = adecLocal161PremergerQualifiedYears;
            lcdoDataExtractionBatchInfo.local_161_premerger_benefit = adecLocal161PremergerBenefit;
            #endregion

            if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNullOrEmpty() ||
                (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() &&
                    Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) != busConstant.LUMP_SUM))
                lcdoDataExtractionBatchInfo.monthly_benefit_amt = ldecGrossMonthlyAmt;

            lcdoDataExtractionBatchInfo.non_taxable_amt_left = ldecRemainingMinimumGuranteeAmount;

            if (Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]).IsNotNullOrEmpty() &&
                        Convert.ToString(acdoPerson[enmPayeeAccount.reemployed_flag.ToString()]) == busConstant.FLAG_YES)
            {
                lcdoDataExtractionBatchInfo.return_to_work_flag = busConstant.FLAG_YES;
            }
            else
            {
                lcdoDataExtractionBatchInfo.return_to_work_flag = busConstant.FLAG_NO;
            }

            if (ldtDeterminationDate != DateTime.MinValue)
                lcdoDataExtractionBatchInfo.determination_date = ldtDeterminationDate;


            if (ldtBeneficiaryFirstPaymentDate != DateTime.MinValue)
                lcdoDataExtractionBatchInfo.beneficiary_first_payment_receive_date = ldtBeneficiaryFirstPaymentDate;


            if (ldtPensionStopDate != DateTime.MinValue)
                lcdoDataExtractionBatchInfo.pension_stop_date = ldtPensionStopDate;


            lcdoDataExtractionBatchInfo.total_qualified_years_at_ret = aintTotalQFYrsAtRet;
            lcdoDataExtractionBatchInfo.total_qualified_hours_at_ret = adecTotalQFHoursAtRet;

            if (Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]).IsNotNullOrEmpty() && Convert.ToString(acdoPerson["BENEFIT_OPTION_CODE_VALUE"]) == busConstant.LUMP_SUM)
            {
                lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = ldecGrossMonthlyAmt;
            }
            else
            {
                lcdoDataExtractionBatchInfo.lump_amt_taken_in_last_comp_yr = 0.0M;
            }
            lcdoDataExtractionBatchInfo.ee_amt_prior_year = adecPriorYrEEAmt;
            lcdoDataExtractionBatchInfo.uvhp_amt_prior_year = adecPriorYrUVHPAmt;

            lcdoDataExtractionBatchInfo.created_by = iobjPassInfo.istrUserID;
            lcdoDataExtractionBatchInfo.modified_by = iobjPassInfo.istrUserID;
            lcdoDataExtractionBatchInfo.created_date = DateTime.Now;
            lcdoDataExtractionBatchInfo.modified_date = DateTime.Now;
            lcdoDataExtractionBatchInfo.update_seq = 0;
            lcdoDataExtractionBatchInfo.status_code_id = busConstant.DATA_EXT_STATUS_CODE_ID;
            lcdoDataExtractionBatchInfo.status_code_value = lstrStatusCode;
            lcdoDataExtractionBatchInfo.retirement_type_id = busConstant.DATA_EXT_RET_TYPE_ID;
            lcdoDataExtractionBatchInfo.retirement_type_value = lstrRetirementType;
            lcdoDataExtractionBatchInfo.benefit_option_code_id = 7056;
            lcdoDataExtractionBatchInfo.benefit_option_code_value = astrBenefitCode;
            lcdoDataExtractionBatchInfo.ee_contribution_amt = adecEEAmtCurrentYr;
            lcdoDataExtractionBatchInfo.mpi_person_id = Convert.ToString(acdoPerson[enmDataExtractionBatchInfo.mpi_person_id.ToString()]);
            lcdoDataExtractionBatchInfo.uvhp_contribution_amt = adecUVHPAmtCurrentYr;

            #region Local Plan fields
            lcdoDataExtractionBatchInfo.local_52_pension_credits = adecLocal52PensionCredits;
            lcdoDataExtractionBatchInfo.local_52_credited_hours = adecLocal52CreditedHours;
            lcdoDataExtractionBatchInfo.local_600_pension_credits = adecLocal600PensionCredits;
            lcdoDataExtractionBatchInfo.local_600_credited_hours = adecLocal600CreditedHours;
            lcdoDataExtractionBatchInfo.local_666_pension_credits = adecLocal666PensionCredits;
            lcdoDataExtractionBatchInfo.local_666_credited_hours = adecLocal666CreditedHours;
            lcdoDataExtractionBatchInfo.local_700_pension_credits = adecLocal700PensionCredits;
            lcdoDataExtractionBatchInfo.local_700_credited_hours = adecLocal700CreditedHours;
            lcdoDataExtractionBatchInfo.local_161_pension_credits = adecLocal161PensionCredits;
            lcdoDataExtractionBatchInfo.local_161_credited_hours = adecLocal161CreditedHours;
            #endregion

            lcdoDataExtractionBatchInfo.ytd_hours_for_year_before_last_comp_year = adecHrForYrBeforeLAstCompYr;
            lcdoDataExtractionBatchInfo.accrued_benefit_till_previous_year = adecAccruedBenefitTillPreviousComputationYear; //Pension Actuary
            lcdoDataExtractionBatchInfo.total_qf_yr_end_of_last_comp_year = (aintTotalQFYrEndOfLastCompYr);
            lcdoDataExtractionBatchInfo.diff_accrued_benfit_for_late_hour = adecDiffInAccruedBenefitForLateHours;
            lcdoDataExtractionBatchInfo.late_ee_contribution = adecLateEEContributions;
            lcdoDataExtractionBatchInfo.life_annuity_amt = adecLifeAnnuityAmt;
            //RID 71411
            lcdoDataExtractionBatchInfo.is_disability_conversion = strIsDisabilityConversion; 
            lcdoDataExtractionBatchInfo.is_converted_from_popup = strIsConvertedFromPopup; 
            lcdoDataExtractionBatchInfo.dro_model = strDROModel; 

            lcdoDataExtractionBatchInfo.Insert();
        }
        #endregion

    }
}
