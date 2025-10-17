using System;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MPIPHP.CustomDataObjects;
using MPIPHP.BusinessObjects;
using MPIPHP.DataObjects;
using MPIPHP.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using System.Linq;
using MPIPHPJobService;
using Sagitec.CustomDataObjects;
using System.Data.SqlClient;
using System.Data.Sql;
using Sagitec.Common;
using System.Threading.Tasks;
using Sagitec.ExceptionPub;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MPIPHPJobService
{
    class busUpdateTaxAmount : busBatchHandler
    {
        private int BatchSheduleID = 0;

        private object iobjLock = null;

        private DateTime _last_benefit_date;
        public DateTime last_benefit_date
        {
            get { return _last_benefit_date; }
            set { _last_benefit_date = value; }
        }

        public void LoadLastBenefitDate()
        {
            _last_benefit_date = busPayeeAccountHelper.GetLastBenefitPaymentDate();
        }

        private DateTime _next_benefit_date;
        public DateTime next_benefit_date
        {
            get
            {
                return _next_benefit_date;
            }
            set
            {
                _next_benefit_date = value;
            }
        }






        public DataTable idtTax { get; set; }

        public DataTable idtPAPIT { get; set; }

        public DataTable idtTaxWithholding { get; set; }
        public void UpdateFederalAndStateTaxAmount()
        {
            istrProcessName = "Update Federal and State Tax Amount Batch";

            DateTime ldtNextPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            // Load all Payee Account other than Refund
            DataTable ldtbResult = busBase.Select("cdoPayeeAccount.LoadPayeeAccountUpdateTaxRateBatch", new object[1] { ldtNextPaymentDate });

            idtPAPIT = busBase.Select("cdoPayeeAccount.LoadPAPITForRecalculatingTax", new object[1] { ldtNextPaymentDate });

            idtTaxWithholding = busBase.Select("cdoPayeeAccount.LoadTaxWithHoldingForRecalculatingTax", new object[1] { ldtNextPaymentDate });



            //set the next payment date
            if (last_benefit_date == DateTime.MinValue)
                LoadLastBenefitDate();
            next_benefit_date = last_benefit_date.AddMonths(1);

            //set the batch schedule id
            BatchSheduleID = 58;

            ReCalculateTax(ldtbResult, false);
            if (idtTax.Rows.Count > 0)
                CreateFederalTaxRateChangeReport();
            // Create Federal Tax Rate Change Report
            //if (idtFederalTaxChanges.Rows.Count > 0)
            //    CreateFederalTaxRateChangeReport();

            //// Create State Tax Rate Change Report
            //if (idtStateTaxChanges.Rows.Count > 0)
            //    CreateStateTaxRateChangeReport();

            //// Create Tax Exception Report
            //if (idtTaxExceptions.Rows.Count > 0)
            //    CreateExceptionReport();

            DataSet idtbstFedStateTaxWithholding = new DataSet();

            idtbstFedStateTaxWithholding.Tables.Add(idtTax.AsEnumerable().CopyToDataTable());
            idtbstFedStateTaxWithholding.Tables[0].TableName = "ReportTable01";
            idtbstFedStateTaxWithholding.DataSetName = "ReportTable01";

            string lstrTemplatePath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_DEFINITION) + busConstant.REPORT_ANNUAL_RECALCULATE_TAXES + ".xlsx";
            string lstrStateTaxWithholdingReportPath = "";
            lstrStateTaxWithholdingReportPath = iobjPassInfo.isrvDBCache.GetPathInfo(busConstant.MPIPHPBatch.REPORT_PATH_GENERATED) + busConstant.REPORT_ANNUAL_RECALCULATE_TAXES + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".xlsx";
            busExcelReportGenerator lbusExcelReportGenerator = new busExcelReportGenerator();
            lbusExcelReportGenerator.CreateExcelReport(lstrTemplatePath, lstrStateTaxWithholdingReportPath, "ReportTable01", idtbstFedStateTaxWithholding);


        }
        private void CreateFederalTaxRateChangeReport()
        {
            idlgUpdateProcessLog("Creating Federal Tax Rate Change Report", "INFO", istrProcessName);
            CreatePDFReport(idtTax, "rptAnnualRecalculateTaxes");
            idlgUpdateProcessLog("Successfully Created Federal Tax Rate Change Report", "INFO", istrProcessName);
        }


        public void ReCalculateTax(DataTable adtbResult, bool ablnPayrollBatch)
        {
            if (ablnPayrollBatch)
            {
                //set the next payment date
                if (last_benefit_date == DateTime.MinValue)
                    LoadLastBenefitDate();
                next_benefit_date = last_benefit_date.AddMonths(2);

            }

            idtTax = CreateNewDataTable();

            foreach (DataRow ldtrPayeeAccount in adtbResult.Rows)
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.icdoPayeeAccount = new cdoPayeeAccount();
                lobjPayeeAccount.icdoPayeeAccount.LoadData(ldtrPayeeAccount);
                if (lobjPayeeAccount.icdoPayeeAccount.person_id != 0)
                {
                    lobjPayeeAccount.ibusPayee = new busPerson();
                    lobjPayeeAccount.ibusPayee.icdoPerson = new cdoPerson();
                    lobjPayeeAccount.ibusPayee.icdoPerson.LoadData(ldtrPayeeAccount);
                }
                if (lobjPayeeAccount.icdoPayeeAccount.org_id != 0)
                {
                    lobjPayeeAccount.ibusOrganization = new busOrganization();
                    lobjPayeeAccount.ibusOrganization.icdoOrganization = new cdoOrganization();
                    lobjPayeeAccount.ibusOrganization.icdoOrganization.LoadData(ldtrPayeeAccount);
                }
                if (!ablnPayrollBatch)
                {
                    busBase lobjBase = new busBase();
                    // Calculate New Federal Tax Amount
                    DataTable ldtbPayeePAPIT = idtPAPIT.AsEnumerable().Where(o =>
                        o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();

                    lobjPayeeAccount.iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();

                    foreach (DataRow ldr in ldtbPayeePAPIT.Rows)
                    {
                        busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.LoadData(ldr);
                        //Load data will laod the first occurence of column name into CDO property
                        //while UPDATE is called, framework looks into ihstOldValues and current value then update, so if wrong data is loaded into CDO property
                        //audit columns wont be updated properly; so assigning the correct values explicitly
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.update_seq = Convert.ToInt32(ldr["papit_update_seq"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.created_by = Convert.ToString(ldr["PAPIT_CREATED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.created_date = Convert.ToDateTime(ldr["PAPIT_CREATED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.modified_by = Convert.ToString(ldr["PAPIT_MODIFIED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.modified_date = Convert.ToDateTime(ldr["PAPIT_MODIFIED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["update_seq"] = Convert.ToInt32(ldr["papit_update_seq"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["created_by"] = Convert.ToString(ldr["PAPIT_CREATED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["created_date"] = Convert.ToDateTime(ldr["PAPIT_CREATED_DATE"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["modified_by"] = Convert.ToString(ldr["PAPIT_MODIFIED_BY"]);
                        lobjPAPIT.icdoPayeeAccountPaymentItemType.ihstOldValues["modified_date"] = Convert.ToDateTime(ldr["PAPIT_MODIFIED_DATE"]);

                        lobjPAPIT.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                        lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldr);

                        lobjPayeeAccount.iclbPayeeAccountPaymentItemType.Add(lobjPAPIT);
                        lobjPAPIT = null;
                    }

                    DataTable ldtbPayeeFedTaxWithholding = idtTaxWithholding.AsEnumerable().Where(o =>
                      o.Field<int>("payee_account_id") == lobjPayeeAccount.icdoPayeeAccount.payee_account_id).AsDataTable();

                    lobjPayeeAccount.iclbPayeeAccountTaxWithholding = lobjBase.GetCollection<busPayeeAccountTaxWithholding>(ldtbPayeeFedTaxWithholding, "icdoPayeeAccountTaxWithholding");

                    if (lobjPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                    if (lobjPayeeAccount.idecTotalTaxableAmountForVariableTax == 0.00M)
                        lobjPayeeAccount.LoadTaxableAmountForVariableTax(next_benefit_date);
                    if (lobjPayeeAccount.idecGrossAmount == 0.00M)
                        lobjPayeeAccount.LoadGrossAmount();
                    if (lobjPayeeAccount.iclbPayeeAccountTaxWithholding == null)
                        lobjPayeeAccount.LoadPayeeAccountTaxWithholdings();
                    // PROD PIR 560 : Wasim -- change For active tax withholding details
                    lobjPayeeAccount.iclbPayeeAccountTaxWithholding = (from item in lobjPayeeAccount.iclbPayeeAccountTaxWithholding
                                                                       where busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                       item.icdoPayeeAccountTaxWithholding.start_date, item.icdoPayeeAccountTaxWithholding.end_date)
                                                                       select item).ToList().ToCollection<busPayeeAccountTaxWithholding>();

                    lobjPayeeAccount.icdoPayeeAccount.idecLastFederalTax = (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                            where
                                                                            busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                             obj.icdoPayeeAccountPaymentItemType.start_date, obj.icdoPayeeAccountPaymentItemType.end_date) &&
                                                                            obj.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == "FEDX"
                                                                            select obj.icdoPayeeAccountPaymentItemType.amount).Sum();
                    lobjPayeeAccount.icdoPayeeAccount.idecLastStateTax = (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemTypeActive.AsEnumerable()
                                                                          where
                                                                          busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                           obj.icdoPayeeAccountPaymentItemType.start_date, obj.icdoPayeeAccountPaymentItemType.end_date) &&
                                                                           obj.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == "STTX"
                                                                          select obj.icdoPayeeAccountPaymentItemType.amount).Sum();

                    foreach (busPayeeAccountTaxWithholding lobjTaxWithholding in lobjPayeeAccount.iclbPayeeAccountTaxWithholding)
                    {
                        //PROD PIR 842
                        if ((lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FedTaxOptionFedTaxBasedOnIRS) ||
                            (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.FedTaxOptionFedTaxBasedOnIRSAndAdditional) ||
                            (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.StateTaxOptionFedTaxBasedOnIRS) ||
                            (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.StateTaxOptionFedTaxBasedOnIRSAndAdditional))
                        {
                            if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.start_date > lobjPayeeAccount.idtNextBenefitPaymentDate)
                                lobjPayeeAccount.idtBenefitPaymentDateForTaxWithholding = lobjTaxWithholding.icdoPayeeAccountTaxWithholding.start_date;
                            else
                                lobjPayeeAccount.idtBenefitPaymentDateForTaxWithholding = lobjPayeeAccount.idtNextBenefitPaymentDate;

                            if (!lobjTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail.IsNullOrEmpty())
                            {
                                if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.start_date < DateTime.Now)
                                {
                                    busPayeeAccountPaymentItemType lbusPayeeAccountPaymentItemType = (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                                                      where
                                                                                                        obj.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id ==
                                                                                                        lobjTaxWithholding.iclbPayeeAccountTaxWithholdingItemDetail.FirstOrDefault().icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id
                                                                                                      select obj).FirstOrDefault();
                                    if (lbusPayeeAccountPaymentItemType != null)
                                    {
                                        lbusPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date = lobjTaxWithholding.icdoPayeeAccountTaxWithholding.end_date;
                                    }
                                }
                                else if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.start_date >= DateTime.Now)
                                {
                                    lobjPayeeAccount.CalculateTaxForBenefit(lobjTaxWithholding, busConstant.BOOL_TRUE); //PROD PIR 842
                                    if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX)
                                        lobjPayeeAccount.icdoPayeeAccount.idecAddFlatFederalTax = lobjTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount;
                                    if (lobjTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.FEDRAL_STATE_TAX)
                                        lobjPayeeAccount.icdoPayeeAccount.idecAddFlatStateTax = lobjTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount;
                                }
                            }
                            else
                                lobjPayeeAccount.CalculateTaxForBenefit(lobjTaxWithholding, busConstant.BOOL_TRUE); //PROD PIR 842
                        }
                    }
                    lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                    lobjPayeeAccount.icdoPayeeAccount.idecCurrentFederalTax = (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                               where
                                                                               busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                               obj.icdoPayeeAccountPaymentItemType.start_date, obj.icdoPayeeAccountPaymentItemType.end_date) &&
                                                                               obj.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == "FEDX"
                                                                               select obj.icdoPayeeAccountPaymentItemType.amount).Sum();

                    lobjPayeeAccount.icdoPayeeAccount.idecCurrentStateTax = (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                             where
                                                                             busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                              obj.icdoPayeeAccountPaymentItemType.start_date, obj.icdoPayeeAccountPaymentItemType.end_date) &&
                                                                              obj.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == "STTX"
                                                                             select obj.icdoPayeeAccountPaymentItemType.amount).Sum();
                    //PROD PIR 842
                    lobjPayeeAccount.idecNextGrossPaymentACH = ((from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                 where (obj.ibusPaymentItemType.icdoPaymentItemType.base_amount_flag == busConstant.FLAG_YES
                                                                 && obj.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1
                                                                 && busGlobalFunctions.CheckDateOverlapping(lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                              obj.icdoPayeeAccountPaymentItemType.start_date, obj.icdoPayeeAccountPaymentItemType.end_date))
                                                                || obj.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM21
                                                                || obj.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM22
                                                                || obj.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ITEM48
                                                                 select obj.icdoPayeeAccountPaymentItemType.amount).Sum()
                                                            +
                                                                (from obj in lobjPayeeAccount.iclbPayeeAccountPaymentItemType.AsEnumerable()
                                                                 where
                                                                 obj.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == "RRED"
                                                                 select obj.icdoPayeeAccountPaymentItemType.amount * obj.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum());

                    CreateNewDataRow(lobjPayeeAccount, false, true);
                }
            }
        }
        private DataTable CreateNewDataTable()
        {
            DataTable ldtbTaxRateChange = new DataTable();
            DataColumn ldtcFullName = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldtcPERSLinkID = new DataColumn("PERSONID", Type.GetType("System.String"));
            DataColumn ldtcBeforeRate = new DataColumn("OldFederal", Type.GetType("System.Decimal"));
            DataColumn ldtcAfterRate = new DataColumn("NewFederal", Type.GetType("System.Decimal"));
            DataColumn ldtcDifferenceFed = new DataColumn("DifferenceFed", Type.GetType("System.Decimal"));
            DataColumn ldtcBeforeRateState = new DataColumn("OldState", Type.GetType("System.Decimal"));
            DataColumn ldtcAfterRateState = new DataColumn("NewState", Type.GetType("System.Decimal"));
            DataColumn ldtcDifferenceState = new DataColumn("DifferenceState", Type.GetType("System.Decimal"));
            DataColumn ldtcAdditionalState = new DataColumn("AdditionalState", Type.GetType("System.Decimal"));
            DataColumn ldtcAdditionalFed = new DataColumn("AdditionalFed", Type.GetType("System.Decimal"));
            //PROD PIR 842
            DataColumn ldtcNextGrossPayment = new DataColumn("NextGrossPayment", Type.GetType("System.Decimal"));
            DataColumn ldtcPayeeAccountID = new DataColumn("PayeeAccountID", Type.GetType("System.Int32"));
            //DataColumn ldtcMessage = new DataColumn("Message", Type.GetType("System.String"));
            //DataColumn ldtcTypeOfMessage = new DataColumn("TypeOfMessage", Type.GetType("System.String"));
            ldtbTaxRateChange.Columns.Add(ldtcFullName);
            ldtbTaxRateChange.Columns.Add(ldtcPERSLinkID);
            ldtbTaxRateChange.Columns.Add(ldtcBeforeRate);
            ldtbTaxRateChange.Columns.Add(ldtcAfterRate);
            ldtbTaxRateChange.Columns.Add(ldtcDifferenceFed);
            ldtbTaxRateChange.Columns.Add(ldtcAdditionalFed);
            ldtbTaxRateChange.Columns.Add(ldtcBeforeRateState);
            ldtbTaxRateChange.Columns.Add(ldtcAfterRateState);
            ldtbTaxRateChange.Columns.Add(ldtcDifferenceState);
            ldtbTaxRateChange.Columns.Add(ldtcAdditionalState);
            //PROD PIR 842
            ldtbTaxRateChange.Columns.Add(ldtcPayeeAccountID);
            ldtbTaxRateChange.Columns.Add(ldtcNextGrossPayment);
            //ldtbTaxRateChange.Columns.Add(ldtcMessage);
            //ldtbTaxRateChange.Columns.Add(ldtcTypeOfMessage);
            ldtbTaxRateChange.TableName = "rpIAnnualRecalculateTaxes";
            return ldtbTaxRateChange;
        }
        public void CreateNewDataRow(busPayeeAccount AobjbusPayeeAccount, bool AblnIsStateTax, bool AblnIsException)
        {
            DataRow ldtrRow;
            if (AblnIsException)
                ldtrRow = idtTax.NewRow();
            else if (AblnIsStateTax)
                ldtrRow = idtTax.NewRow();
            else
                ldtrRow = idtTax.NewRow();
            if (AobjbusPayeeAccount.icdoPayeeAccount.person_id != 0)
            {
                ldtrRow["FullName"] = AobjbusPayeeAccount.ibusPayee.icdoPerson.istrFullName;
                ldtrRow["PERSONID"] = AobjbusPayeeAccount.ibusPayee.icdoPerson.mpi_person_id;
            }
            if (AobjbusPayeeAccount.icdoPayeeAccount.org_id != 0)
            {
                ldtrRow["FullName"] = AobjbusPayeeAccount.ibusOrganization.icdoOrganization.org_name;
                ldtrRow["PERSONID"] = Convert.ToInt32(AobjbusPayeeAccount.ibusOrganization.icdoOrganization.org_id);
            }
            ldtrRow["OldFederal"] = AobjbusPayeeAccount.icdoPayeeAccount.idecLastFederalTax;
            ldtrRow["NewFederal"] = AobjbusPayeeAccount.icdoPayeeAccount.idecCurrentFederalTax;
            ldtrRow["DifferenceFed"] = AobjbusPayeeAccount.icdoPayeeAccount.idecCurrentFederalTax
                                           - AobjbusPayeeAccount.icdoPayeeAccount.idecLastFederalTax;
            ldtrRow["OldState"] = AobjbusPayeeAccount.icdoPayeeAccount.idecLastStateTax;
            ldtrRow["NewState"] = AobjbusPayeeAccount.icdoPayeeAccount.idecCurrentStateTax;
            ldtrRow["AdditionalState"] = AobjbusPayeeAccount.icdoPayeeAccount.idecAddFlatStateTax;
            ldtrRow["AdditionalFed"] = AobjbusPayeeAccount.icdoPayeeAccount.idecAddFlatFederalTax;
            ldtrRow["DifferenceState"] = AobjbusPayeeAccount.icdoPayeeAccount.idecCurrentStateTax - AobjbusPayeeAccount.icdoPayeeAccount.idecLastStateTax;
            //PROD PIT 842
            ldtrRow["NextGrossPayment"] = AobjbusPayeeAccount.idecNextGrossPaymentACH;
            ldtrRow["PayeeAccountID"] = AobjbusPayeeAccount.icdoPayeeAccount.payee_account_id;
            idtTax.Rows.Add(ldtrRow);
        }

        #region State Tax Update Batch

        public void UpdateStateTaxBatch()
        {
            if (Convert.ToString(MPIPHP.Common.ApplicationSettings.Instance.StateTaxBatchFutureFlag) != "Y")
            {
                int lintrtn;
                string lRunForSpecificProcessDate = string.Empty;
                string lRunFromTempTable = string.Empty;
                if (ibusJobHeader != null)
                {
                    if (ibusJobHeader.iclbJobDetail == null)
                    {
                        ibusJobHeader.LoadJobDetail(true);
                    }
                    lRunForSpecificProcessDate = ibusJobHeader.iclbJobDetail[0].iclbJobParameters[0].icdoJobParameters.param_value.ToString();
                    lRunFromTempTable = ibusJobHeader.iclbJobDetail[0].iclbJobParameters[1].icdoJobParameters.param_value.ToString();
                }

                //Get all payee account tax withholding records
                int lintCount = 0;
                int lintTotalCount = 0;
                DataSet idtbstStateTaxWithholding = new DataSet();
                createTableDesignForStateTaxWithholding();
                Dictionary<string, object> ldictParams = new Dictionary<string, object>();
                foreach (string lstrParam in iobjPassInfo.idictParams.Keys)
                {
                    ldictParams[lstrParam] = iobjPassInfo.idictParams[lstrParam];
                }

                //Make a copy of original connection before starting the parallel loop which would be again used after completion of parallel loop
                iobjPassInfo.idictParams["ID"] = "Batch PassInfo";
                utlPassInfo lobjMainPassInfo = iobjPassInfo;
                iobjLock = new object();
                DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate(2).AddMonths(1);
                DataTable ldtPayeeStateTaxInformation = busBase.Select("cdoPayeeAccountTaxWithholding.GetStateTaxWithholding", new object[2] { lRunForSpecificProcessDate, lRunFromTempTable });
                //Initialize the parallel processing options, especially the max number of thread to be used for parallel processing
                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 1;//System.Environment.ProcessorCount * 4;

                Parallel.ForEach(ldtPayeeStateTaxInformation.AsEnumerable(), po, (acdoPayeeStateTaxInformation, loopState) =>
                {
                    utlPassInfo lobjPassInfo = new utlPassInfo();
                    lobjPassInfo.idictParams = ldictParams;
                    lobjPassInfo.idictParams["ID"] = "UpdateStateTaxBatch";
                    lobjPassInfo.iconFramework = DBFunction.GetDBConnection();
                    utlPassInfo.iobjPassInfo = lobjPassInfo;

                    GetStateTaxwithholding(acdoPayeeStateTaxInformation, ldteNextBenefitPaymentDate, lobjPassInfo, lintCount, lintTotalCount);

                    if (lobjPassInfo.iconFramework.State == ConnectionState.Open)
                    {
                        lobjPassInfo.iconFramework.Close();
                    }

                    lobjPassInfo.iconFramework.Dispose();
                    lobjPassInfo.iconFramework = null;


                });

                MergePdfsFromPath(iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Correspondence_Path,
                               iobjSystemManagement.icdoSystemManagement.base_directory + busConstant.Report_path_State_Tax_Update_Batch, busConstant.BOOL_FALSE, busConstant.BOOL_FALSE);

                idtbstStateTaxWithholding.Tables.Add(idtbStateTaxWithholding.AsEnumerable().CopyToDataTable());
                idtbstStateTaxWithholding.Tables[0].TableName = "ReportTable01";
                idtbstStateTaxWithholding.DataSetName = "ReportTable01";

                if (idtbstStateTaxWithholding.Tables[0].Rows.Count > 0)
                {
                    this.CreatePDFReport(idtbstStateTaxWithholding, "rptStateTaxWithholding", busConstant.MPIPHPBatch.GENERATE_STATE_TAX_UPDATE_BATCH_PATH);
                }

                if (lRunFromTempTable == "Y")
                {

                    lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountTaxWithholding.DeleteTempTableForStateTax",
                         new object[0] { },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                }

                lobjMainPassInfo.iconFramework = DBFunction.GetDBConnection();
                utlPassInfo.iobjPassInfo = lobjMainPassInfo;
            }

        }


        public void GetStateTaxwithholding(DataRow acdoPayeeStateTaxInformation, DateTime adteNextBenefitPaymentDate, utlPassInfo autlPassInfo, int aintCount, int aintTotalCount)
        {

            utlConnection utlLegacyDBConnetion = HelperFunction.GetDBConnectionProperties("Legacy");
            string astrLegacyDBConnetion = utlLegacyDBConnetion.istrConnectionString;
            string istrBisPartFlag = null;
            busBase lbusBase = new busBase();
            this.ibusJobHeader.icdoJobHeader.iblnGeneratePdfFlag = busConstant.BOOL_TRUE;

            lock (iobjLock)
            {
                aintCount++;
                aintTotalCount++;
                if (aintCount == 100)
                {
                    String lstrMsg = aintTotalCount + " : " + " Records Has Been Processed";
                    PostInfoMessage(lstrMsg);
                    aintCount = 0;
                }
            }
            autlPassInfo.BeginTransaction();
            try
            {
                ArrayList aarrResult;
                Hashtable ahtbQueryBkmarks;
                Boolean lflgInsert = false;
                Boolean lflgMultipleFDRL = false;

                aarrResult = new ArrayList();
                ahtbQueryBkmarks = new Hashtable();
                DataRow dr = idtbStateTaxWithholding.NewRow();
                dr["MPI_PERSON_ID"] = acdoPayeeStateTaxInformation["MPI_PERSON_ID"].ToString();
                dr["PAYEE_ACCOUNT_ID"] = Convert.ToInt32(acdoPayeeStateTaxInformation["PAYEE_ACCOUNT_ID"]);
                dr["PAYEE_NAME"] = acdoPayeeStateTaxInformation["PAYEE_NAME"].ToString();
                dr["MANDATORY_WITHHOLDING_STATE"] = acdoPayeeStateTaxInformation["MANDATORY_WITHHOLDING_STATE"].ToString();
                dr["PREV_MANDATORY_WITHHOLDING_STATE"] = acdoPayeeStateTaxInformation["PREV_MANDATORY_WITHHOLDING_STATE"].ToString();
                dr["Previous_Address"] = acdoPayeeStateTaxInformation["Previous_Address"].ToString();
                dr["New_Address"] = acdoPayeeStateTaxInformation["New_Address"].ToString();
                dr["TAX_IDENTIFIER_VALUE"] = acdoPayeeStateTaxInformation["TAX_IDENTIFIER_VALUE"].ToString();
                dr["PREV_TAX_IDENTIFIER_VALUE"] = acdoPayeeStateTaxInformation["PREV_TAX_IDENTIFIER_VALUE"].ToString();
                dr["Previous_State"] = acdoPayeeStateTaxInformation["Previous_State"].ToString();
                dr["New_State"] = acdoPayeeStateTaxInformation["New_State"].ToString();

                idtbStateTaxWithholding.Rows.Add(dr);


                busPayeeAccount lbusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lbusPayeeAccount.FindPayeeAccount(Convert.ToInt32(acdoPayeeStateTaxInformation["PAYEE_ACCOUNT_ID"]));

                lbusPayeeAccount.ibusParticipant = new busPerson { icdoPerson = new cdoPerson() };
                lbusPayeeAccount.ibusParticipant.FindPerson(Convert.ToInt32(acdoPayeeStateTaxInformation["PERSON_ID"]));

                lbusPayeeAccount.istrMandatoryState = acdoPayeeStateTaxInformation["MANDATORY_WITHHOLDING_STATE"].ToString();
                lbusPayeeAccount.istrNonMandatoryState = acdoPayeeStateTaxInformation["PREV_MANDATORY_WITHHOLDING_STATE"].ToString();
                lbusPayeeAccount.istrNewState = acdoPayeeStateTaxInformation["New_State"].ToString();
                lbusPayeeAccount.istrPrevState = acdoPayeeStateTaxInformation["Previous_State"].ToString();

                lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();

                if (Convert.ToString(acdoPayeeStateTaxInformation["MANDATORY_WITHHOLDING_STATE"]) == "Y")
                {

                    if (lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Where(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == "FDRL" && (i.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue || i.icdoPayeeAccountTaxWithholding.end_date > DateTime.Now.AddDays(1))).Count() > 0)
                    {
                        lflgMultipleFDRL = true;

                    }

                    if (!lflgMultipleFDRL && (Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]) == busConstant.VA_STATE_TAX ||
                                                Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]) == busConstant.OR_STATE_TAX))
                    {
                        return;

                    }

                    foreach (busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding in lbusPayeeAccount.iclbPayeeAccountTaxWithholding)
                    {

                        if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != "FDRL")
                        {
                            if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]))
                            {
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.Now.AddDays(-1);
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
                                lflgInsert = true;
                                
                            }
                            else if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]))
                            {
                                lflgInsert = false;
                                break;
                            }
                            else if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date > DateTime.Now.AddDays(-1) && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]))
                            {
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
                                lflgInsert = false;
                                break;


                            }
                            else if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]))
                            {
                                
                                lflgInsert = true;


                            }
                            else if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]))
                            {
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.Now.AddDays(-1);
                                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
                                lflgInsert = true;
                                

                            }
                        }



                    }
                    //if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != "FDRL")
                    //{
                    //    if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date == DateTime.MinValue && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value != lbusPayeeAccount.istrNewState)
                    //    {
                    //        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date = DateTime.Now.AddDays(-1);
                    //        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.Update();
                    //    }
                    // //if(Convert.ToString(acdoPayeeStateTaxInformation["MANDATORY_WITHHOLDING_STATE"]) == "Y" && (lbusPayeeAccount.istrPrevState != lbusPayeeAccount.istrNewState && lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue))
                    // //   {
                    // //       lflgInsert = true;
                    // //   }

                    //}
                }



                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholdingState = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                //lbusPayeeAccountTaxWithholdingState =
                //    lbusPayeeAccountTaxWithholdingState.LoadTaxWithHoldingByPayeeAccountIdAndTaxType(Convert.ToInt32(acdoPayeeStateTaxInformation["PAYEE_ACCOUNT_ID"]), acdoPayeeStateTaxInformation["NewTaxIdent"].ToString());

                if (lbusPayeeAccountTaxWithholdingState != null)
                {
                    if (lflgInsert)
                    {

                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.payee_account_id = Convert.ToInt32(acdoPayeeStateTaxInformation["PAYEE_ACCOUNT_ID"]);
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_identifier_value = acdoPayeeStateTaxInformation["NewTaxIdent"].ToString();
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = "MNBF";
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_option_value = busConstant.StateTaxOptionFedTaxBasedOnIRS;

                        if (Convert.ToString(acdoPayeeStateTaxInformation["NewTaxIdent"]) == busConstant.CA_STATE_TAX)
                        {
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.marital_status_value = busConstant.MARITAL_STATUS_MARRIED;
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_allowance = 3;
                        }
                        else
                        {
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.marital_status_value = Convert.ToString(acdoPayeeStateTaxInformation["MARITAL_STATUS"]);
                            lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.tax_allowance = 0;
                        }
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.start_date = DateTime.Now;
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.end_date = DateTime.MinValue;
                        lbusPayeeAccountTaxWithholdingState.icdoPayeeAccountTaxWithholding.Insert();

                    }

                }

                lbusPayeeAccount.iclbPayeeAccountTaxWithholding.Add(lbusPayeeAccountTaxWithholdingState);
                lbusPayeeAccount.idtNextBenefitPaymentDate = adteNextBenefitPaymentDate;
                lbusPayeeAccount.ProcessTaxWithHoldingDetails(true);
                lbusPayeeAccount.LoadPayeeAccountTaxWithholdings();
                lbusPayeeAccount.LoadBreakDownDetails();

                aarrResult.Add(lbusPayeeAccount);
                if (lflgInsert)
                {
                    this.CreateCorrespondence(busConstant.STATE_TAX_ADDRESS_CHANGE, this.iobjPassInfo.istrUserID, this.iobjPassInfo.iintUserSerialID, aarrResult, ahtbQueryBkmarks, ablnIsPDF: true);

                }



                autlPassInfo.Commit();
            }
            catch (Exception e)
            {
                lock (iobjLock)
                {
                    ExceptionManager.Publish(e);
                    String lstrMsg = "Error while Executing Batch,Error Message For MPID " + acdoPayeeStateTaxInformation["MPI_PERSON_ID"].ToString() + ":" + e.ToString();
                    PostErrorMessage(lstrMsg);
                }
                autlPassInfo.Rollback();

            }
        }

        public DataTable createTableDesignForStateTaxWithholding()
        {
            idtbStateTaxWithholding = new DataTable();
            idtbStateTaxWithholding.Columns.Add(new DataColumn("MPI_PERSON_ID", typeof(string)));
            idtbStateTaxWithholding.Columns.Add(new DataColumn("PAYEE_ACCOUNT_ID", typeof(int)));
            idtbStateTaxWithholding.Columns.Add(new DataColumn("PAYEE_NAME", typeof(string)));
            idtbStateTaxWithholding.Columns.Add(new DataColumn("MANDATORY_WITHHOLDING_STATE", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("PREV_MANDATORY_WITHHOLDING_STATE", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("Previous_Address", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("New_Address", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("TAX_IDENTIFIER_VALUE", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("PREV_TAX_IDENTIFIER_VALUE", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("Previous_State", typeof(string))); //rid 124432
            idtbStateTaxWithholding.Columns.Add(new DataColumn("New_State", typeof(string))); //rid 124432
            return idtbStateTaxWithholding;





        }
        public DataTable idtbStateTaxWithholding { get; set; }

        #endregion

    }
}
