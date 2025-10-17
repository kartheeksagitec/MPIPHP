// -----------------------------------------------------------------------
// <copyright file="busSSADisabilityReCertificationBatch.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace MPIPHPJobService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using MPIPHPJobService;
    using Sagitec.BusinessObjects;
    using Sagitec.DBUtility;
    using System.Data;
    using System.Threading.Tasks;
    using MPIPHP.BusinessObjects;
    using System.Collections.ObjectModel;
    using MPIPHP.CustomDataObjects;
    using Sagitec.ExceptionPub;
    using System.Collections;
    using Sagitec.Common;
    using MPIPHP.DataObjects;
    using Sagitec.DataObjects;
    using Sagitec.CustomDataObjects;
    using Sagitec.Interface;
    using System.Data.SqlClient;
    //rid 80600
    using System.IO;
    using iTextSharp.text;
    using iTextSharp.text.pdf;
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class busPensionVerificationHistoryBatch : busBatchHandler
    {

        private object iobjLock = null;
        int iintCount = busConstant.ZERO_INT;
        int iintTotalCount = busConstant.ZERO_INT;


        #region public collections
        Collection<busPensionVerificationHistory> lclbPensionVerificationHistory { get; set; }
        #endregion
        public busPensionVerificationHistoryBatch()
        {
        }

        public override void Process()
        {
            busBase lobjBase = new busBase();
            base.Process();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;
                                                           
            DataSet idtbstPensionVerificationLetters = new DataSet();
            createTableNinetyDaysLetter();
            createTableSixtyDaysLetter();
            createTableThirtyDaysLetter();
            //WI 19555 - PBV Phase-1 Suspension Batch commenting to separate suspension feature
            //createTableSuspendPayeeAccounts();

            ProcessVerficationLetter(lobjBase, idtbstPensionVerificationLetters, ldictParams, lobjMainPassInfo);           
            
        }

        //WI 19555 - PBV Phase-1 Suspension Batch 
        public void SuspendProcess()
        {
            busBase lobjBase = new busBase();
            base.Process();
            Dictionary<string, object> ldictParams = new Dictionary<string, object>();
            foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
            {
                ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
            }
            iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
            utlPassInfo lobjMainPassInfo = iobjPassInfo;

            DataSet idtbstPensionVerificationLetters = new DataSet();
            createTableSuspendPayeeAccounts();

            SuspendPayeeAccountsAndCreateReport(lobjBase, idtbstPensionVerificationLetters, ldictParams, lobjMainPassInfo);

        }

        //WI 19555 - PBV Phase-1 Suspension Batch
        private void SuspendPayeeAccountsAndCreateReport(busBase lobjBase, DataSet idtbstPensionVerificationLetters, Dictionary<string, object> ldictParams, utlPassInfo lobjMainPassInfo)
        {
            Collection<busPayeeAccount> lclbPayeeAccounts;
            DataTable ldtblPayeeAccountsForSuspension = new DataTable();

            iobjLock = new object();

            ldtblPayeeAccountsForSuspension = busBase.Select("cdoBenefitApplication.GetDataToSuspendPayeeAccountForPensionVerification", new object[0] { });

            if (ldtblPayeeAccountsForSuspension.Rows.Count > 0)
            {
                lclbPayeeAccounts = new Collection<busPayeeAccount>();
                lclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldtblPayeeAccountsForSuspension, "icdoPayeeAccount");

                ParallelOptions lpoParallelOptions = new ParallelOptions();
                lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount ;

                Parallel.ForEach(lclbPayeeAccounts.AsEnumerable(), lpoParallelOptions, (lbusPayeeAccount, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "PensionBenefitVerificationBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    SuspendPayeeAccounts(lbusPayeeAccount, lobjPassInfo);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;
                });

                //foreach (var lbusPayeeAccount in lclbPayeeAccounts.AsEnumerable())
                //{
                //    SuspendPayeeAccounts(lbusPayeeAccount);
                //}
                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }

            //Excel Report
            idtbstPensionVerificationLetters.Tables.Add(idtSuspendPayeeAccounts.Copy());
            idtbstPensionVerificationLetters.Tables[0].TableName = "SUSPENSIONLETTERS";

            string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_PENSION_BENEFIT_VERIFICATION + ".xlsx";
            string lstrPensionBenefitVerificationReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_PENSION_BENEFIT_VERIFICATION_REPORT_PATH) + busConstant.REPORT_PENSION_BENEFIT_VERIFICATION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
            lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrPensionBenefitVerificationReportPath, "NINETYDAYSLETTERS", idtbstPensionVerificationLetters);
        }


        private void ProcessVerficationLetter(busBase lobjBase, DataSet idtbstPensionVerificationLetters, Dictionary<string, object> ldictParams, utlPassInfo lobjMainPassInfo)
        {            
            Collection<busPensionVerificationHistory> lclbPensionVerificationHistoryNinetyDays;
            Collection<busPensionVerificationHistory> lclbPensionVerificationHistorySixtyDays;
            Collection<busPensionVerificationHistory> lclbPensionVerificationHistoryThirtyDays;
            //WI 19555 - PBV Phase-1 Suspension Batch commenting to separate suspension feature
            //Collection<busPayeeAccount> lclbPayeeAccounts;
            //DataTable ldtblPayeeAccountsForSuspension = new DataTable();

            iobjLock = new object();

            DataTable ldtblPayeeAccountsforPensionVerificationHistory = busBase.Select("cdoBenefitApplication.GetPayeeAccountforPensionVerificationHistory", new object[1] {this.iobjPassInfo.istrUserID });
            if (ldtblPayeeAccountsforPensionVerificationHistory.Rows.Count > 0)
            {
                lclbPensionVerificationHistory = new Collection<busPensionVerificationHistory>();
                lclbPensionVerificationHistory = lobjBase.GetCollection<busPensionVerificationHistory>(ldtblPayeeAccountsforPensionVerificationHistory,"icdoPensionVerificationHistory");
                
                //Ninety Days Letters Collection
                lclbPensionVerificationHistoryNinetyDays = (from item in lclbPensionVerificationHistory
                                                            where item.icdoPensionVerificationHistory.ninety_days_letter_sent.IsNotNull() && item.icdoPensionVerificationHistory.ninety_days_letter_sent != DateTime.MinValue && item.icdoPensionVerificationHistory.ninety_days_letter_sent.Date == DateTime.Now.Date
                                                            select item).ToList().ToCollection<busPensionVerificationHistory>();

                //Sixty Days Letters Collection
                lclbPensionVerificationHistorySixtyDays = (from item in lclbPensionVerificationHistory
                                                           where item.icdoPensionVerificationHistory.sixty_days_letter_sent.IsNotNull() && item.icdoPensionVerificationHistory.sixty_days_letter_sent != DateTime.MinValue && item.icdoPensionVerificationHistory.sixty_days_letter_sent.Date == DateTime.Now.Date
                                                           select item).ToList().ToCollection<busPensionVerificationHistory>();

                //Thirty Days Letters Collection
                lclbPensionVerificationHistoryThirtyDays = (from item in lclbPensionVerificationHistory
                                                            where item.icdoPensionVerificationHistory.thirty_days_letter_sent.IsNotNull() && item.icdoPensionVerificationHistory.thirty_days_letter_sent != DateTime.MinValue && item.icdoPensionVerificationHistory.thirty_days_letter_sent.Date == DateTime.Now.Date
                                                            select item).ToList().ToCollection<busPensionVerificationHistory>();
                //WI 19555 - PBV Phase-1 Suspension Batch commenting to separate suspension feature
                //if(lclbPensionVerificationHistoryThirtyDays.Count > 0)
                //{
                //    ldtblPayeeAccountsForSuspension = busBase.Select("cdoBenefitApplication.GetDataToSuspendPayeeAccountForPensionVerification", new object[0] { });
                //}

                ParallelOptions lpoParallelOptions = new ParallelOptions();
                lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                if (lclbPensionVerificationHistoryNinetyDays.Count > 0)
                {
                    Parallel.ForEach(lclbPensionVerificationHistoryNinetyDays.AsEnumerable(), lpoParallelOptions, (lbusPensionVerificationHistory, loopState) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "PensionBenefitVerificationBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_NINETY_DAYS, lobjPassInfo);

                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }

                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;
                    });

                    //foreach (busPensionVerificationHistory lbusPensionVerificationHistory in lclbPensionVerificationHistoryNinetyDays)
                    //{
                    //    GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_NINETY_DAYS);
                    //}
                    MergePdfsBatchFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\", iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\PensionVerification\\");
                    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
                }
                
                if (lclbPensionVerificationHistorySixtyDays.Count > 0)
                {
                    lpoParallelOptions = new ParallelOptions();
                    lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                    Parallel.ForEach(lclbPensionVerificationHistorySixtyDays.AsEnumerable(), lpoParallelOptions, (lbusPensionVerificationHistory, loopState) =>
                    {
                        utlPassInfo lobjPassInfo = new utlPassInfo();
                        lobjPassInfo.idictParams = ldictParams;
                        lobjPassInfo.idictParams["ID"] = "PensionBenefitVerificationBatch";
                        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                        utlPassInfo.iobjPassInfo = lobjPassInfo;

                        GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_SIXTY_DAYS, lobjPassInfo);

                        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                        {
                            lobjPassInfo.iconFramework.Close();
                        }

                        lobjPassInfo.iconFramework.Dispose();
                        lobjPassInfo.iconFramework = null;
                    });


                    //foreach (busPensionVerificationHistory lbusPensionVerificationHistory in lclbPensionVerificationHistorySixtyDays)
                    //{
                    //    GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_SIXTY_DAYS);
                    //}
                    MergePdfsBatchFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\", iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\PensionVerification\\");
                    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
                }


                if (lclbPensionVerificationHistoryThirtyDays.Count > 0)
                {
                    lpoParallelOptions = new ParallelOptions();
                    lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                    Parallel.ForEach(lclbPensionVerificationHistoryThirtyDays.AsEnumerable(), lpoParallelOptions, (lbusPensionVerificationHistory, loopState) =>
                     {
                         utlPassInfo lobjPassInfo = new utlPassInfo();
                         lobjPassInfo.idictParams = ldictParams;
                         lobjPassInfo.idictParams["ID"] = "PensionBenefitVerificationBatch";
                         lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                         utlPassInfo.iobjPassInfo = lobjPassInfo;

                         GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_SUSPENSION, lobjPassInfo);

                         if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                         {
                             lobjPassInfo.iconFramework.Close();
                         }

                         lobjPassInfo.iconFramework.Dispose();
                         lobjPassInfo.iconFramework = null;
                     });
                    //foreach (busPensionVerificationHistory lbusPensionVerificationHistory in lclbPensionVerificationHistoryThirtyDays)
                    //{
                    //    GeneratePensionVerificationLetter(lbusPensionVerificationHistory, busConstant.PENSION_VERIFICATION_HISTORY_SUSPENSION);
                    //}
                    MergePdfsBatchFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + "Correspondence\\Generated\\", iobjSystemManagement.icdoSystemManagement.base_directory + "Reports\\PensionVerification\\");
                    //lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
                }

                //WI 19555 - PBV Phase-1 Suspension Batch commenting to separate suspension feature
                //if (ldtblPayeeAccountsForSuspension.Rows.Count > 0)
                //{
                //    lclbPayeeAccounts = new Collection<busPayeeAccount>();
                //    lclbPayeeAccounts = lobjBase.GetCollection<busPayeeAccount>(ldtblPayeeAccountsForSuspension, "icdoPayeeAccount");

                //    lpoParallelOptions = new ParallelOptions();
                //    lpoParallelOptions.MaxDegreeOfParallelism = System.Environment.ProcessorCount * 4;

                //    Parallel.ForEach(lclbPayeeAccounts.AsEnumerable(), lpoParallelOptions, (lbusPayeeAccount, loopState) =>
                //    {
                //        utlPassInfo lobjPassInfo = new utlPassInfo();
                //        lobjPassInfo.idictParams = ldictParams;
                //        lobjPassInfo.idictParams["ID"] = "PensionBenefitVerificationBatch";
                //        lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                //        utlPassInfo.iobjPassInfo = lobjPassInfo;

                //        SuspendPayeeAccounts(lbusPayeeAccount,lobjPassInfo);

                //        if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                //        {
                //            lobjPassInfo.iconFramework.Close();
                //        }

                //        lobjPassInfo.iconFramework.Dispose();
                //        lobjPassInfo.iconFramework = null;
                //    });

                //    //foreach (var lbusPayeeAccount in lclbPayeeAccounts.AsEnumerable())
                //    //{
                //    //    SuspendPayeeAccounts(lbusPayeeAccount);
                //    //}
                //    lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                //    utlPassInfo.iobjPassInfo = lobjMainPassInfo;
                //}                
            }

            //Excel Report
            idtbstPensionVerificationLetters.Tables.Add(idtbNinetyDaysLetters.Copy());
                idtbstPensionVerificationLetters.Tables[0].TableName = "NINETYDAYSLETTERS";                              
                idtbstPensionVerificationLetters.Tables.Add(idtSixtyDaysLetters.Copy());
                idtbstPensionVerificationLetters.Tables[1].TableName = "SIXTYDAYSLETTERS";
                idtbstPensionVerificationLetters.Tables.Add(idtThirtyDaysLetters.Copy());
                idtbstPensionVerificationLetters.Tables[2].TableName = "THIRTYDAYSLETTERS";
            //WI 19555 - PBV Phase-1 Suspension Batch commenting to separate suspension feature
            //idtbstPensionVerificationLetters.Tables.Add(idtSuspendPayeeAccounts.Copy());
            //idtbstPensionVerificationLetters.Tables[3].TableName = "SUSPENSIONLETTERS";            

            string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_PENSION_BENEFIT_VERIFICATION + ".xlsx";
            string lstrPensionBenefitVerificationReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.GENERATED_PENSION_BENEFIT_VERIFICATION_REPORT_PATH) + busConstant.REPORT_PENSION_BENEFIT_VERIFICATION + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
            lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrPensionBenefitVerificationReportPath, "NINETYDAYSLETTERS", idtbstPensionVerificationLetters);
        }

        
        private void GeneratePensionVerificationLetter(busPensionVerificationHistory abusPensionVerificationHistory, int LetterType, utlPassInfo autlPassInfo) //LetterType should be 90,60 or 30 only
        {
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;
            busPersonOverview lbusPersonOverview = new busPersonOverview { icdoPerson = new cdoPerson() };
                        
            autlPassInfo.BeginTransaction();
            try
            {
                ArrayList aarrResult = new ArrayList();
                Hashtable ahtbQueryBkmarks = new Hashtable();
                var lIsValidAddress = "";
                
                #region For report changes
                lock (iobjLock)
                {
                    lbusPersonOverview.FindPerson(abusPensionVerificationHistory.icdoPensionVerificationHistory.person_id);
                    lbusPersonOverview.lbusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lbusPersonOverview.LoadActiveAddressOfMember();
                    bool provinceRequired = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == busConstant.AUSTRALIA.ToString() ||
                                            lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == busConstant.CANADA.ToString() ||
                                            lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == busConstant.MEXICO.ToString() ||
                                            lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == busConstant.NewZealand.ToString() ||
                                            lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == busConstant.OTHER_PROVINCE.ToString();
                    //rid 83291
                    if (lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value == "0001")
                    {
                        if (lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() && lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_city.IsNotNullOrEmpty() &&
                            lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value.IsNotNullOrEmpty() && lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code.IsNotNullOrEmpty() &&
                            (lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.end_date.ToString().IsNullOrEmpty()|| lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.end_date == DateTime.MinValue))
                        {
                            lIsValidAddress = "Y";
                        }else
                        {
                            lIsValidAddress = "N";
                        }
                    }
                    else {
                        if (lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1.IsNotNullOrEmpty() && lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_city.IsNotNullOrEmpty() &&
                            (provinceRequired && lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.foreign_province.IsNotNullOrEmpty() || 
                                !provinceRequired && lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.foreign_province.IsNullOrEmpty()) &&
                            (lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.end_date.ToString().IsNullOrEmpty() ||
                                lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.end_date == DateTime.MinValue))
                        {
                            lIsValidAddress = "Y";
                        }
                        else
                        {
                            lIsValidAddress = "N";
                        }
                    }
                    
                                                    
                    lbusPersonOverview.ldtResumptionDate = Convert.ToDateTime(DateTime.Now.Month.ToString() + "/15/" + DateTime.Now.Year.ToString());  //rid 80600
                    aarrResult.Add(lbusPersonOverview);

                    if (LetterType == 90)
                    {

                        this.CreateCorrespondence(busConstant.PENSION_VERIFICATION_HISTORY_NINETY_DAYS_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);

                        //For Report Purpose
                        DataRow dr = idtbNinetyDaysLetters.NewRow();
                        dr["MPI_PERSON_ID"] = lbusPersonOverview.icdoPerson.mpi_person_id;
                        dr["FIRST_NAME"] = lbusPersonOverview.icdoPerson.first_name;
                        dr["LAST_NAME"] = lbusPersonOverview.icdoPerson.last_name;
                        dr["BENEFIT_DATE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_date;
                        dr["BENEFIT_ACCOUNT_TYPE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_account_type;
                        dr["VALID_ADDRESS"] = lIsValidAddress;
                        idtbNinetyDaysLetters.Rows.Add(dr);

                        String lstrMsg = "Pension Benefit Verification First Letter for Member with MPID : " + lbusPersonOverview.icdoPerson.mpi_person_id;
                        PostInfoMessage(lstrMsg);

                    }
                    else if (LetterType == 60)
                    {
                        //lock (iobjLock)
                        //{
                            this.CreateCorrespondence(busConstant.PENSION_VERIFICATION_HISTORY_SIXTY_DAYS_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);

                            //For Report Purpose
                            DataRow dr = idtSixtyDaysLetters.NewRow();
                            dr["MPI_PERSON_ID"] = lbusPersonOverview.icdoPerson.mpi_person_id;
                            dr["FIRST_NAME"] = lbusPersonOverview.icdoPerson.first_name;
                            dr["LAST_NAME"] = lbusPersonOverview.icdoPerson.last_name;
                            dr["BENEFIT_DATE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_date;
                            dr["BENEFIT_ACCOUNT_TYPE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_account_type;
                            dr["VALID_ADDRESS"] = lIsValidAddress;
                            idtSixtyDaysLetters.Rows.Add(dr);

                            String lstrMsg = "Pension Benefit Verification Second Letter for Member with MPID : " + lbusPersonOverview.icdoPerson.mpi_person_id;
                            PostInfoMessage(lstrMsg);
                        //}
                    }
                    else if (LetterType == 30)
                    {
                        //lock (iobjLock)
                        //{
                            this.CreateCorrespondence(busConstant.PENSION_VERIFICATION_HISTORY_THIRTY_DAYS_LETTER, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, busConstant.BOOL_TRUE);

                        //For Report Purpose
                        //rid 80600
                        string astrSSN = lbusPersonOverview.icdoPerson.istrSSNNonEncrypted;
                        if (!String.IsNullOrEmpty(astrSSN))
                        {
                            busPerson lbusPerson = new busPerson();
                            lbusPersonOverview.icdoPerson.UnionCode = lbusPerson.GetTrueUnionCodeBySSN(astrSSN);
                        }
                        DataRow dr = idtThirtyDaysLetters.NewRow();
                        dr["MPI_PERSON_ID"] = lbusPersonOverview.icdoPerson.mpi_person_id;
                        dr["FIRST_NAME"] = lbusPersonOverview.icdoPerson.first_name;
                        dr["LAST_NAME"] = lbusPersonOverview.icdoPerson.last_name;
                        dr["ADDRESS1"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_1; //rid 80600
                        dr["ADDRESS2"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_line_2; //rid 80600
                        dr["CITY"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_city;  //rid 80600
                        dr["STATE"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_state_value;  //rid 80600
                        dr["ZIPCODE"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_zip_code;  //rid 80600
                        dr["COUNTRY"] = lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_value != "0001" ? lbusPersonOverview.ibusPersonAddressForCorr.icdoPersonAddress.addr_country_description : string.Empty ;  //rid 83291
                        dr["BENEFIT_DATE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_date;
                        dr["BENEFIT_ACCOUNT_TYPE"] = abusPensionVerificationHistory.icdoPensionVerificationHistory.benefit_account_type;
                        dr["VALID_ADDRESS"] = lIsValidAddress;
                        dr["HOME_PHONE"] = lbusPersonOverview.icdoPerson.home_phone_no;   //rid 80600
                        dr["MOBILE_PHONE"] = lbusPersonOverview.icdoPerson.cell_phone_no; //rid 80600
                        dr["WORK_PHONE"] = lbusPersonOverview.icdoPerson.work_phone_no; //rid 80600
                        dr["EMAIL_ADDRESS"] = lbusPersonOverview.icdoPerson.email_address_1; //rid 80600
                        dr["UNION_CODE"] = lbusPersonOverview.icdoPerson.UnionCode; //rid 80600
                        idtThirtyDaysLetters.Rows.Add(dr);
                        String lstrMsg = "Pension Benefit Verification Suspension Letter for Member with MPID : " + lbusPersonOverview.icdoPerson.mpi_person_id;
                            PostInfoMessage(lstrMsg);
                        //}
                    }

                    iintCount++;
                    iintTotalCount++;
                    if (iintCount == 10)
                    {
                        String lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                        PostInfoMessage(lstrMsg);
                        iintCount = 0;
                    }
                }
                #endregion
                autlPassInfo.Commit();                
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + lbusPersonOverview.ibusPerson.icdoPerson.mpi_person_id + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();               
            }
        }

        private void SuspendPayeeAccounts(busPayeeAccount abusPayeeAccount, utlPassInfo autlPassInfo)
        {

            autlPassInfo.BeginTransaction();
            try
            {
                lock (iobjLock)
                {
                    busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
                    Hashtable ahtbQueryBkmarks = new Hashtable();

                    lbusPerson.FindPerson(abusPayeeAccount.icdoPayeeAccount.person_id);
                    abusPayeeAccount.LoadPayeeAccountStatuss();


                    string lstrMsg = string.Empty;
                    //Create Workflow. 
                    //busWorkflowHelper.InitializeWorkflow(busConstant.PROCESS_PAYEE_ACCOUNT, lbusPerson.icdoPerson.person_id, 0, abusPayeeAccount.icdoPayeeAccount.payee_account_id, ahtbQueryBkmarks);
                    // if (abusPayeeAccount.istrPayeeStatus != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED)
                    if (abusPayeeAccount.iclbPayeeAccountStatus.Count() > 0 && abusPayeeAccount.iclbPayeeAccountStatus != null &&
                        abusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.status_value != busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED
                        && abusPayeeAccount.iclbPayeeAccountStatus.FirstOrDefault().icdoPayeeAccountStatus.suspension_status_reason_value != busConstant.Suspension_Reason_For_Disability)
                    {
                        cdoPayeeAccountStatus lcdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                        lcdoPayeeAccountStatus.payee_account_id = Convert.ToInt32(abusPayeeAccount.icdoPayeeAccount.payee_account_id);
                        lcdoPayeeAccountStatus.status_value = busConstant.PAYEE_ACCOUNT_STATUS_SUSPENDED;
                        lcdoPayeeAccountStatus.suspension_status_reason_value = busConstant.Suspension_Reason_For_Pension_Verification;
                        lcdoPayeeAccountStatus.status_effective_date = DateTime.Now;
                        lcdoPayeeAccountStatus.Insert();
                        lstrMsg = "Payee Account with Payee Account Id :" + abusPayeeAccount.icdoPayeeAccount.payee_account_id + " suspended.";
                        PostInfoMessage(lstrMsg);

                        iintCount++;
                        iintTotalCount++;
                        if (iintCount == 10)
                        {
                            lstrMsg = iintTotalCount + " : " + " Records Has Been Processed";
                            PostInfoMessage(lstrMsg);
                            iintCount = 0;
                        }
                    }

                    #region For report changes
                    DataRow dr = idtSuspendPayeeAccounts.NewRow();
                    dr["MPI_PERSON_ID"] = abusPayeeAccount.icdoPayeeAccount.istrMPID;
                    dr["FIRST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrFirsttName;
                    dr["LAST_NAME"] = abusPayeeAccount.icdoPayeeAccount.istrLastName;
                    dr["PAYEE_ACCOUNT_ID"] = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
                    idtSuspendPayeeAccounts.Rows.Add(dr);
                    #endregion
                }
                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                ExceptionManager.Publish(e);
                String lstrMsg = "Error while Executing Batch,Suspend Payee - Error Message For Payee Account Id  " + abusPayeeAccount.icdoPayeeAccount.payee_account_id + ":" + e.ToString();
                PostErrorMessage(lstrMsg);
                autlPassInfo.Rollback();
            }
        }
        //rid 80600
        private void MergePdfsBatchFromPath(string astrGeneratedFilePath, string astrOutFilePath = "")
        {
            MergeBatchPDFs(astrGeneratedFilePath, "DMST", astrOutFilePath);
            MergeBatchPDFs(astrGeneratedFilePath, "INTR", astrOutFilePath);
        }

        //rid 80600
        private void MergeBatchPDFs(string generatedpath, string astrpostfix, string astrOutFilePath = "")
        {
            DirectoryInfo dir = new DirectoryInfo(generatedpath);
            List<string> filesPath = new List<string>();

            //sorting based on filename happen here...
            IOrderedEnumerable<FileInfo> lenumFiles = dir.GetFiles("*.pdf").Where(x => x.CreationTime >= ibusJobHeader.icdoJobHeader.start_time).OrderBy(x => x.Name);
            foreach (FileInfo fi in lenumFiles)
            {                
                filesPath.Add(fi.FullName);
            }
            
            //Get All Files with current postfix
            List<string> AllPostFixs = (from obj in filesPath
                                        where obj.Contains(astrpostfix)
                                        select obj).ToList();
            

            if (AllPostFixs != null && AllPostFixs.Count() > 0)
            {
                AllPostFixs = AllPostFixs.Select(o => o.Replace(".pdf", "")).Distinct().ToList();

                List<PdfReader> readerList = new List<PdfReader>();
                string outPutFilePath = string.Empty;
                string outPutFilePathVested = string.Empty;
                string outPutFilePathNonVested = string.Empty;
                string lstrVested = string.Empty;
                
                outPutFilePath = astrOutFilePath + astrpostfix + "\\" + astrpostfix + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".pdf";

                List<string> SelectedfilesPath = new List<string>();
                SelectedfilesPath = filesPath.Where(obj => obj.EndsWith(astrpostfix + ".pdf")).ToList();

                foreach (string filePath in SelectedfilesPath)
                {
                    PdfReader pdfReader = new PdfReader(filePath);
                    readerList.Add(pdfReader);
                }
                //Define a new output document and its size, type
                Document document = new Document(PageSize.LETTER, 0, 0, 0, 0);
                //Create blank output pdf file and get the stream to write on it.
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outPutFilePath, FileMode.Create));
                document.Open();
                Console.WriteLine("Merging Files");
                foreach (PdfReader reader in readerList)
                {
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        document.Add(iTextSharp.text.Image.GetInstance(page));
                    }
                }
                Console.WriteLine("Closing Files");
                document.Close();
                   
                // Delete file if not required
                foreach (string filePath in SelectedfilesPath)
                {
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// Create Table for Report which contains
        /// Members for the letters and suspend payee accounts
        /// </summary>
        public DataTable createTableNinetyDaysLetter()
        {
            idtbNinetyDaysLetters = new DataTable();
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("BENEFIT_DATE", typeof(DateTime)));
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("BENEFIT_ACCOUNT_TYPE", typeof(string)));
            idtbNinetyDaysLetters.Columns.Add(new DataColumn("VALID_ADDRESS", typeof(string)));
            return idtbNinetyDaysLetters;
        }

        public DataTable createTableSixtyDaysLetter()
        {
            idtSixtyDaysLetters = new DataTable();
            idtSixtyDaysLetters.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtSixtyDaysLetters.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtSixtyDaysLetters.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtSixtyDaysLetters.Columns.Add(new DataColumn("BENEFIT_DATE", typeof(DateTime)));
            idtSixtyDaysLetters.Columns.Add(new DataColumn("BENEFIT_ACCOUNT_TYPE", typeof(string)));
            idtSixtyDaysLetters.Columns.Add(new DataColumn("VALID_ADDRESS", typeof(string)));
            return idtSixtyDaysLetters;
        }

        public DataTable createTableThirtyDaysLetter()
        {
            idtThirtyDaysLetters = new DataTable();
            idtThirtyDaysLetters.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("ADDRESS1", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("ADDRESS2", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("CITY", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("STATE", typeof(string))); //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("ZIPCODE", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("COUNTRY", typeof(string)));  //rid 83291
            idtThirtyDaysLetters.Columns.Add(new DataColumn("BENEFIT_DATE", typeof(DateTime)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("BENEFIT_ACCOUNT_TYPE", typeof(string)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("VALID_ADDRESS", typeof(string)));
            idtThirtyDaysLetters.Columns.Add(new DataColumn("HOME_PHONE", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("MOBILE_PHONE", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("WORK_PHONE", typeof(string)));  //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("EMAIL_ADDRESS", typeof(string))); //rid 80600
            idtThirtyDaysLetters.Columns.Add(new DataColumn("UNION_CODE", typeof(string))); //rid 80600

            return idtThirtyDaysLetters;
        }

        public DataTable createTableSuspendPayeeAccounts()
        {
            idtSuspendPayeeAccounts = new DataTable();
            idtSuspendPayeeAccounts.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtSuspendPayeeAccounts.Columns.Add(new DataColumn("FIRST_NAME", typeof(string)));
            idtSuspendPayeeAccounts.Columns.Add(new DataColumn("LAST_NAME", typeof(string)));
            idtSuspendPayeeAccounts.Columns.Add(new DataColumn("PAYEE_ACCOUNT_ID", typeof(int)));
            return idtSuspendPayeeAccounts;
        }

        public DataTable idtbNinetyDaysLetters { get; set; }
        public DataTable idtSixtyDaysLetters { get; set; }

        public DataTable idtThirtyDaysLetters { get; set; }
        public DataTable idtSuspendPayeeAccounts { get; set; }
    }
}
