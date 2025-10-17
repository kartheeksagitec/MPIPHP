#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.Common;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace MPIPHP.BusinessObjects
{
	[Serializable]
	public class busJobScheduleDetail : busJobScheduleDetailGen
	{
        #region Public Variables
        public Collection<busJobScheduleParams> iclbJobScheduleParameters { get; set; }

        // Fields added for setting up the JobScheduleParameters from the UI
        public Collection<busJobScheduleParams> iclbEditableJobScheduleParameters { get; set; }
        public Collection<busJobScheduleParams> iclbNonEditableJobScheduleParameters { get; set; }

        public busBatchSchedule ibusBatchSchedule { get; set; }
        public busBatchSchedule ibusDependentBatchSchedule { get; set; }
        public busJobSchedule ibusJobSchedule { get; set; }

        // Flag used to for initiating the validation from the parent object (JobScheduleHeader).
        public bool iblnHeaderValidating = false;

        public bool iblnDeleteFromBusJobScheduleScreen = true; 
        #endregion

        #region Public Methods
        public void LoadJobParameters()
        {
            // We can't get the records directly from the Job_Parameters table, since 
            // we need to know whether the parameter is read_only or not.
            DataTable ldtbList = Select("cdoJobScheduleParams.GetParametersForDetailId",
                            new object[1] { icdoJobScheduleDetail.job_schedule_detail_id });
            iclbJobScheduleParameters = new Collection<busJobScheduleParams>();
            if (ldtbList.Rows.Count > 0)
            {
                iclbEditableJobScheduleParameters = new Collection<busJobScheduleParams>();
                iclbNonEditableJobScheduleParameters = new Collection<busJobScheduleParams>();

                // This collection is populated purely for deleting the JobDetail cleanly through the grid
                iclbJobScheduleParameters = GetCollection<busJobScheduleParams>(ldtbList, "icdoJobScheduleParams");

            }
        }



        public void LoadJobSchedule()
        {
            LoadJobSchedule(false);
        }

        public void LoadJobSchedule(bool lblnForceLoad)
        {
            if (!lblnForceLoad)
            {
                if (ibusJobSchedule != null)
                {
                    return;
                }
            }
            DataTable ldtbList = Select<cdoJobSchedule>(
                new string[1] { "job_schedule_id" },
                new object[1] { icdoJobScheduleDetail.job_schedule_id }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                ibusJobSchedule = new busJobSchedule();
                ibusJobSchedule.icdoJobSchedule = new cdoJobSchedule();
                ibusJobSchedule.icdoJobSchedule.LoadData(ldtbList.Rows[0]);
            }
        }


        public void LoadStepInfo()
        {
            DataTable ldtbList = Select<cdoBatchSchedule>(
                new string[1] { "step_no" },
                new object[1] { icdoJobScheduleDetail.step_no }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ibusBatchSchedule = new busBatchSchedule();
                ibusBatchSchedule.icdoBatchSchedule = new cdoBatchSchedule();
                ibusBatchSchedule.icdoBatchSchedule.LoadData(ldtbList.Rows[0]);
            }
        }

        public void LoadDependentStepInfo()
        {
            if (icdoJobScheduleDetail.dependent_step_no > 0)
            {
                DataTable ldtbList = Select<cdoBatchSchedule>(
                    new string[1] { "step_no" },
                    new object[1] { icdoJobScheduleDetail.dependent_step_no }, null, null);

                if (ldtbList.Rows.Count > 0)
                {
                    ibusDependentBatchSchedule = new busBatchSchedule();
                    ibusDependentBatchSchedule.icdoBatchSchedule = new cdoBatchSchedule();
                    ibusDependentBatchSchedule.icdoBatchSchedule.LoadData(ldtbList.Rows[0]);
                }
            }
        }

        public busJobDetail ConvertToJobDetail()
        {
            busJobDetail lobjJobDetail = new busJobDetail();
            lobjJobDetail.icdoJobDetail = new cdoJobDetail();

            lobjJobDetail.icdoJobDetail.step_no = icdoJobScheduleDetail.step_no;
            lobjJobDetail.icdoJobDetail.status_value = BatchHelper.JOB_DETAIL_STATUS_QUEUED;
            lobjJobDetail.icdoJobDetail.order_number = icdoJobScheduleDetail.order_number;
            lobjJobDetail.icdoJobDetail.dependent_step_no = icdoJobScheduleDetail.dependent_step_no;
            lobjJobDetail.icdoJobDetail.operator_value = icdoJobScheduleDetail.operator_value;
            lobjJobDetail.icdoJobDetail.dependent_step_return_value = icdoJobScheduleDetail.dependent_step_return_value;

            return lobjJobDetail;
        }

        //public void LoadJobParametersinNewMode()
        //{
        //    DataTable ldtbList = Select<cdoBatchParameters>(
        //        new string[1] { "step_no" },
        //        new object[1] { icdoJobScheduleDetail.step_no }, null, null);
        //    if (ldtbList.Rows.Count > 0)
        //    {
        //        iclbEditableJobScheduleParameters = new Collection<busJobScheduleParams>();
        //        iclbNonEditableJobScheduleParameters = new Collection<busJobScheduleParams>();

        //        foreach (DataRow ldr in ldtbList.Rows)
        //        {
        //            busJobScheduleParams lobjJobScheduleParams = new busJobScheduleParams();
        //            lobjJobScheduleParams.icdoJobScheduleParams = new cdoJobScheduleParams();

        //            lobjJobScheduleParams.icdoJobScheduleParams.param_name = ldr["param_name"].ToString();
        //            lobjJobScheduleParams.icdoJobScheduleParams.param_value = ldr["param_value"].ToString();
        //            // Based on the readonly flag, ensure that the JobParameters is added to the appropriate collection
        //            if (ldr["readonly_flag"].ToString() == busConstant.Flag_Yes)
        //            {
        //                iclbNonEditableJobScheduleParameters.Add(lobjJobScheduleParams);
        //            }
        //            else
        //            {
        //                iclbEditableJobScheduleParameters.Add(lobjJobScheduleParams);
        //            }
        //        }
        //    }
        //}

        private int GetNextOrderNumber()
        {
            int lintOrderNumber = 1;
            // Execute a query to get the next order number from the table for the current schedule id.
            DataTable ldtbList = Select("cdoJobScheduleDetail.GetNextOrderNumber",
                            new object[1] { icdoJobScheduleDetail.job_schedule_id });

            if (ldtbList.Rows.Count > 0)
            {
                lintOrderNumber = Convert.ToInt32(ldtbList.Rows[0][0]);
            }
            return lintOrderNumber;
        }

        public Collection<cdoBatchSchedule> GetStepsForJobScheduleId()
        {
            Collection<cdoBatchSchedule> lclbBatchSchedule = new Collection<cdoBatchSchedule>();
            // Execute a query to get the next order number from the table for the current schedule id.
            DataTable ldtbList = Select("cdoJobScheduleDetail.GetStepsForJobScheduleId",
                            new object[2] { icdoJobScheduleDetail.job_schedule_id, icdoJobScheduleDetail.step_no });

            if (ldtbList.Rows.Count > 0)
            {
                foreach (DataRow ldr in ldtbList.Rows)
                {
                    cdoBatchSchedule lobjcdoBatchSchedule = new cdoBatchSchedule();
                    lobjcdoBatchSchedule.step_no = Convert.ToInt32(ldr["Step_No"]);
                    lobjcdoBatchSchedule.step_name = Convert.ToString(ldr["Step_Name"]);
                    lclbBatchSchedule.Add(lobjcdoBatchSchedule);
                }
            }
            return lclbBatchSchedule;
        } 
        #endregion

        #region Overridden Methods
       
        public override void BeforePersistChanges()
        {
            // Only in the Insert mode, we need to go ahead and populate the parameters table with the appropriate values
            // these values will not be displayed in the grid in "Insert" mode, but they would automatically get
            // persisted the moment we save it.
            if (icdoJobScheduleDetail.ienuObjectState == ObjectState.Insert)
            {
                //LoadJobParametersinNewMode();
                // Set the ordernumber of the object.
                icdoJobScheduleDetail.order_number = GetNextOrderNumber();
                // Go ahead and persist the core object, so we could use that identifier to save the other objects.
                icdoJobScheduleDetail.Insert();

                //if (iclbEditableJobScheduleParameters != null)
                //{
                //    // Now go ahead and insert the editable and non editable parameters collection to the database.
                //    foreach (busJobScheduleParams lobjJobScheduleParams in iclbEditableJobScheduleParameters)
                //    {
                //        lobjJobScheduleParams.icdoJobScheduleParams.job_schedule_detail_id =
                //            icdoJobScheduleDetail.job_schedule_detail_id;
                //        lobjJobScheduleParams.icdoJobScheduleParams.Insert();
                //    }
                //}

                //if (iclbNonEditableJobScheduleParameters != null)
                //{
                //    foreach (busJobScheduleParams lobjJobScheduleParams in iclbNonEditableJobScheduleParameters)
                //    {
                //        lobjJobScheduleParams.icdoJobScheduleParams.job_schedule_detail_id =
                //            icdoJobScheduleDetail.job_schedule_detail_id;
                //        lobjJobScheduleParams.icdoJobScheduleParams.Insert();
                //    }
                //}

            }
            //ChangeID: 59078
            if (ibusJobSchedule.icdoJobSchedule.job_schedule_id == busConstant.JobIAPYearEndAllocation)
            {
                if (iclbJobScheduleParameters != null && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamUnallocableOverlimitAmtFrmAccounting).Count() > 0
                    && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamUnallocableOverlimitAmtFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value.IsNotNullOrEmpty()
                    && Convert.ToDecimal(iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamUnallocableOverlimitAmtFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value) != decimal.Zero)
                {
                    decimal ldecTotalAssetsFromAccounting = decimal.Zero;
                    decimal ldecTotalInvestmentIncomeFromAccounting = decimal.Zero;
                    decimal ldecAdministrativeExpensesFromAccounting = decimal.Zero;
                    decimal ldecOverlimitInterest = decimal.Zero;
                    decimal ldecOtherMiscAdjustment = decimal.Zero;

                    if (iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalAssetsFrmAccounting).Count() > 0
                    && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalAssetsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value.IsNotNullOrEmpty())
                    {
                        ldecTotalAssetsFromAccounting = Convert.ToDecimal(iclbJobScheduleParameters.
                            Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalAssetsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value);
                    }

                    if (iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalInvstAmtFrmAccounting).Count() > 0
                    && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalInvstAmtFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value.IsNotNullOrEmpty())
                    {

                        ldecTotalInvestmentIncomeFromAccounting = Convert.ToDecimal(iclbJobScheduleParameters.
                            Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamTotalInvstAmtFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value);
                    }

                    if (iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamAdmExpFrmAccounting).Count() > 0
                    && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamAdmExpFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value.IsNotNullOrEmpty())
                    {

                        ldecAdministrativeExpensesFromAccounting = Convert.ToDecimal(iclbJobScheduleParameters.
                            Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamAdmExpFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value);
                    }
                    

                    if (ldecTotalAssetsFromAccounting != 0M && ldecTotalInvestmentIncomeFromAccounting != 0M && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInvIncomeOrLossFactor).Count() > 0
                    )
                    {
                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInvIncomeOrLossFactor).FirstOrDefault().icdoJobScheduleParams.param_value
                            = Convert.ToString(Math.Round((ldecTotalInvestmentIncomeFromAccounting + ldecAdministrativeExpensesFromAccounting) / ldecTotalAssetsFromAccounting,10));

                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInvIncomeOrLossFactor).FirstOrDefault().icdoJobScheduleParams.Update();
                    }

                    busIapAllocationSummary lbusIapAllocationSummary = new busIapAllocationSummary { icdoIapAllocationSummary = new cdoIapAllocationSummary() };
                    lbusIapAllocationSummary.LoadLatestAllocationSummary();

                    busIapOverlimitContributionsInterestDetails lbusIapOverlimitContributionsInterestDetails = new busIapOverlimitContributionsInterestDetails { icdoIapOverlimitContributionsInterestDetails = new cdoIapOverlimitContributionsInterestDetails() };
                    lbusIapOverlimitContributionsInterestDetails.LoadIAPOverLimitContributionsInterestDetails(lbusIapAllocationSummary.icdoIapAllocationSummary.computation_year);

                    if (iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInterest).Count() > 0)
                    {

                        ldecOverlimitInterest = Math.Round(((ldecTotalInvestmentIncomeFromAccounting + ldecAdministrativeExpensesFromAccounting) / ldecTotalAssetsFromAccounting)
                            * lbusIapOverlimitContributionsInterestDetails.icdoIapOverlimitContributionsInterestDetails.total_overlimit_contributions_interest_amount * -1,2);

                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInterest).FirstOrDefault().icdoJobScheduleParams.param_value
                            = Convert.ToString(ldecOverlimitInterest);

                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOverlimitInterest).FirstOrDefault().icdoJobScheduleParams.Update();
                    }

                    if (iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOtherMiscAdjustmentsFrmAccounting).Count() > 0
                        && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOtherMiscAdjustmentsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value.IsNotNullOrEmpty())
                    {
                        ldecOtherMiscAdjustment = Convert.ToDecimal(iclbJobScheduleParameters.
                           Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamOtherMiscAdjustmentsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value);
                    }

                        if (ldecTotalAssetsFromAccounting != 0M && ldecTotalInvestmentIncomeFromAccounting != 0M && iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamMiscAdjustemntsFrmAccounting).Count() > 0
                        )
                    {
                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamMiscAdjustemntsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.param_value
                            = Convert.ToString(Math.Round(ldecOverlimitInterest + ldecOtherMiscAdjustment,2));

                        iclbJobScheduleParameters.Where(t => t.icdoJobScheduleParams.param_name == busConstant.JobParamMiscAdjustemntsFrmAccounting).FirstOrDefault().icdoJobScheduleParams.Update();
                    }


                }
            }

            base.BeforePersistChanges();
        }

        public override bool ValidateSoftErrors()
        {
            bool lblnResult;
            if (iblnHeaderValidating)
            {
                lblnResult = base.ValidateSoftErrors();
            }
            else
            {
                lblnResult = ibusJobSchedule.ValidateSoftErrors();
            }

            return lblnResult;
        }

        //PIR 1024
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            utlError lobjError = null;
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            base.ValidateHardErrors(aenmPageMode);

            if (icdoJobScheduleDetail.step_no == busConstant.MPIPHPBatch.HEALTH_ELIGIBILITY_ACTUARY_BATCH)
            {
                if (iclbJobScheduleParameters != null && iclbJobScheduleParameters.Count > 0)
                {
                    if (iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value.IsNullOrEmpty() ||
                       (iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value.IsNotNullOrEmpty() &&
                       (!Regex.IsMatch(iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value, @"^\d+$") ||
                        iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value.Length != 4 ||
                        (Convert.ToInt32(iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value) < 1900
                        || Convert.ToInt32(iclbJobScheduleParameters[0].icdoJobScheduleParams.param_value) > 9999))))
                    {
                        lobjError = AddError(6273, " ");
                        this.iarrErrors.Add(lobjError);
                    }
                }
            }

        }

        public override void UpdateValidateStatus()
        {
            // We don't have to call UpdateValidateStatus() twice, since this is already handled in the busJobSchedule class 
            // for all the detail records, so if the iblnHeaderValidating flag is not set, we don't have to call the updatevalidatestatus()
            if (iblnHeaderValidating)
            {
                base.UpdateValidateStatus();
            }
        }

        public override void AfterPersistChanges()
        {
            // Load the Step information from the SGS_Batch_Schedule table, since we need to show the step name to the user
            LoadStepInfo();
            LoadJobSchedule(true);
            // FindJobScheduleDetail and LoadErrors() method are specific methods called only for this screen due to the functioanlity
            // of doing validation in the header object (busJobSchedule) rather than on the core object itself.
            FindJobScheduleDetail(icdoJobScheduleDetail.job_schedule_detail_id);
            LoadErrors();
            base.AfterPersistChanges();
        }

        public override int Delete()
        {
            // Get the JobScheduleId before the delete operation and once delete has happened
            // go ahead and fire the validation/updatevalidatestatus to get the header record to the appropriate status
            int lintJobScheduleId = icdoJobScheduleDetail.job_schedule_id;
            base.Delete();

            // Only if the busJobScheduleDetail is deleted from the JobSchedule (wfmJobScheduleMaintenance) screen, we would need to validate the 
            // busJobSchedule object once again, but if the whole busJobSchedule object itself is going to get deleted
            // from the wfmJobScheduleLookup screen, we would need to skip validating the header object.
            if (iblnDeleteFromBusJobScheduleScreen)
            {
                // Go ahead and load the job schedule record and try to validate it.
                busJobSchedule lobjJobSchedule = new busJobSchedule();
                lobjJobSchedule.FindJobSchedule(lintJobScheduleId);

                lobjJobSchedule.ValidateSoftErrors();
                lobjJobSchedule.UpdateValidateStatus();
            }
            return 0;
        } 
        #endregion
	}
}
