#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using MPIPHP.Common;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;
using System.Collections.Specialized;

#endregion

namespace MPIPHP.BusinessObjects
{


    public class clsJobScheduleDetailsParameters
    {
        public int iintJobScheduleDetailID;
        public Collection<busJobParameters> iclbParameters;

        //property added to macth the job schedule info - because step no is the only common field between the SGS_JOB_SCHEDULE_DETAIL and clsJobScheduleDetailsParameters.
        public int iintStepNumber; 
    }

    [Serializable]
    public class busJobSchedule : busJobScheduleGen
    {
        #region Public Variables
        public Collection<busJobScheduleDetail> iclbJobScheduleDetail { get; set; }
        public Collection<busJobHeader> iclbJobHeader { get; set; }
        public string istrOverriddenStatus { get; set; }
        public Collection<busJobScheduleParams> iclbJobScheduleParametersChildGrid { get; set; }

        #endregion

        #region Public Methods
                
        public bool FindJobScheduleByScheduleCode(string astrScheduleCode)
        {
            bool lblnResult = false;
            DataTable ldtbJobSchedule = Select<cdoJobSchedule>(
                                new string[1] { enmJobSchedule.schedule_code.ToString() },
                                new object[1] { astrScheduleCode }, null, null);
            if (ldtbJobSchedule.Rows.Count > 0)
            {
                icdoJobSchedule = new cdoJobSchedule();
                icdoJobSchedule.LoadData(ldtbJobSchedule.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        public Collection<clsJobScheduleDetailsParameters> GetScheduleParameters()
        {
            Collection<clsJobScheduleDetailsParameters> lclsJobScheduleParameters = new Collection<clsJobScheduleDetailsParameters>();
            this.LoadJobScheduleDetails(true);
            foreach (busJobScheduleDetail lbusJobScheduleDetail in iclbJobScheduleDetail)
            {
                clsJobScheduleDetailsParameters lclsJobScheduleDetailsParameters = new clsJobScheduleDetailsParameters();
                lclsJobScheduleDetailsParameters.iintJobScheduleDetailID = lbusJobScheduleDetail.icdoJobScheduleDetail.job_schedule_detail_id;
                lclsJobScheduleDetailsParameters.iintStepNumber = lbusJobScheduleDetail.icdoJobScheduleDetail.step_no;

                lclsJobScheduleDetailsParameters.iclbParameters = new Collection<busJobParameters>();
                foreach (busJobScheduleParams lbusJobScheduleParams in lbusJobScheduleDetail.iclbJobScheduleParameters)
                {
                    lclsJobScheduleDetailsParameters.iclbParameters.Add(lbusJobScheduleParams.ConvertToJobDetailParams());
                }
                lclsJobScheduleParameters.Add(lclsJobScheduleDetailsParameters);
            }
            return lclsJobScheduleParameters;
        }


        public void LoadJobScheduleDetails()
        {
            LoadJobScheduleDetails(false);
        }

        // This method is the same LoadJobScheduleDetails(), but since the delete method in the Business object xml
        // doesnot take a method with same name, I have added a new method doing the same thing..
        // Have to work with framework team to get it fixed.
        public void LoadJobScheduleDetailsForDelete()
        {
            LoadJobScheduleDetails(false);

            // to avoid Null object Reference while deleting the Job without any Detail records
            if (iclbJobScheduleDetail == null)
                iclbJobScheduleDetail = new Collection<busJobScheduleDetail>();

            // Setting this object will ensure during delete the busJobSchedule doesn't get revalidated once again.
            foreach (busJobScheduleDetail lobjJobScheduleDetail in iclbJobScheduleDetail)
            {
                lobjJobScheduleDetail.iblnDeleteFromBusJobScheduleScreen = false;
            }
        }

        /// <summary>
        /// This method will load the all the Job Detail records for the appropriate Header Id
        /// and will also load the Parameters for the appropriate detail record if the parameter is passed as true.
        /// </summary>
        /// <param name="ablnLoadWithParameters"></param>
        public void LoadJobScheduleDetails(bool ablnLoadWithParameters)
        {
            // Get all the Job detail records from the SGS_Job_Detail table

            DataTable ldtbJobScheduleDetails = Select<cdoJobScheduleDetail>(
                                new string[1] { "job_schedule_id" },
                                new object[1] { icdoJobSchedule.job_schedule_id }, null, "order_number");

            if (ldtbJobScheduleDetails.Rows.Count > 0)
            {
                iclbJobScheduleDetail = GetCollection<busJobScheduleDetail>(ldtbJobScheduleDetails, "icdoJobScheduleDetail");
                // Load all the parameters corresponding to the particular Job schedule Detail as well.
                if (ablnLoadWithParameters)
                {
                    foreach (busJobScheduleDetail lobjJobScheduleDetail in iclbJobScheduleDetail)
                    {
                        lobjJobScheduleDetail.LoadJobParameters();
                    }
                }
            }
        }

        public void LoadJobHeaders()
        {
            DataTable ldtbJobHeader = Select<cdoJobHeader>(
                         new string[1] { "job_schedule_id" },
                         new object[1] { icdoJobSchedule.job_schedule_id }, null, "job_header_id desc");

            if (ldtbJobHeader.Rows.Count > 0)
            {
                iclbJobHeader = GetCollection<busJobHeader>(ldtbJobHeader, "icdoJobHeader");
            }

        }

        public busJobHeader ConvertToJobHeader()
        {
            busJobHeader lobjJobHeader = new busJobHeader();
            lobjJobHeader.icdoJobHeader = new cdoJobHeader();
            lobjJobHeader.icdoJobHeader.job_name = icdoJobSchedule.schedule_name;
            lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_VALID;
            lobjJobHeader.icdoJobHeader.job_schedule_id = icdoJobSchedule.job_schedule_id;
            lobjJobHeader.iclbJobDetail = new Collection<busJobDetail>();

            return lobjJobHeader;
        }

        /// <summary>
        /// This method will load the step information for all the detail records
        /// as mentioned in the SGS_Batch_Schedule table.
        /// </summary>
        public void LoadJobScheduleDetailStepInfo()
        {
            if (iclbJobScheduleDetail != null && iclbJobScheduleDetail.Count > 0)
            {
                // Load all the parameters corresponding to the particular Job Detail as well.
                foreach (busJobScheduleDetail lobjJobScheduleDetail in iclbJobScheduleDetail)
                {
                    lobjJobScheduleDetail.LoadStepInfo();
                    lobjJobScheduleDetail.LoadDependentStepInfo();
                }
            }
        }

        // Validation Methods
        public bool IsDuplicateStepsPresent()
        {
            DataTable ldtbJobDetails = Select("cdoJobScheduleDetail.CountOfStepsThatAreDuplicate",
                                                  new object[1] { icdoJobSchedule.job_schedule_id });

            if (ldtbJobDetails.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method would create a job object from the schedule object in hand
        /// </summary>
        public busJobHeader CloneJobFromSchedule()
        {
            // Job is initially created in pending status, then it would be updated to queued 
            // to avoid the thread picking up the job before detail and the parameters are written out.
            busJobHeader lobjJobHeader = ConvertToJobHeader();
            lobjJobHeader.icdoJobHeader.Insert();

            foreach (busJobScheduleDetail lobjJobScheduleDetail in iclbJobScheduleDetail)
            {
                busJobDetail lobjJobDetail = lobjJobScheduleDetail.ConvertToJobDetail();
                lobjJobDetail.icdoJobDetail.job_header_id = lobjJobHeader.icdoJobHeader.job_header_id;
                lobjJobDetail.icdoJobDetail.Insert();

                // Add this job detail to the jobheader collection
                lobjJobHeader.iclbJobDetail.Add(lobjJobDetail);

                if (lobjJobScheduleDetail.iclbJobScheduleParameters != null && lobjJobScheduleDetail.iclbJobScheduleParameters.Count > 0)
                {
                    foreach (
                        busJobScheduleParams lobjJobScheduleParameters in
                            lobjJobScheduleDetail.iclbJobScheduleParameters)
                    {
                        busJobParameters lobjJobParameters = lobjJobScheduleParameters.ConvertToJobDetailParams();
                        lobjJobParameters.icdoJobParameters.job_detail_id = lobjJobDetail.icdoJobDetail.job_detail_id;
                        lobjJobParameters.icdoJobParameters.Insert();
                    }
                }
            }

            // Finally go ahead and set up the job in queued status which means the job service can pick up the job now.
            lobjJobHeader.icdoJobHeader.status_value = BatchHelper.JOB_HEADER_STATUS_QUEUED;
            lobjJobHeader.icdoJobHeader.Update();
            return lobjJobHeader;
        }
        private void SetStartAndEndTime()
        {
            // If "Occurs Once" has been selected by the user, we need to change the freq_subday_type as 1 (at the specified time)
            // Also, we have separate text boxes for capturing start time for once and every.. so capture the 
            // appropriate value into start time, (which is the only value the Job service looks for)
            if (icdoJobSchedule.lstrSubdayFrequency == BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_ONCE)
            {
                icdoJobSchedule.freq_subday_type = 1;
                icdoJobSchedule.start_time = icdoJobSchedule.StartTime_For_Once;
            }
            else if (icdoJobSchedule.lstrSubdayFrequency == BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_BATCH_WINDOW)
            {
                icdoJobSchedule.freq_subday_type = 16;
            }
            else
            {
                // This code segment would be executed only when the user has selected "Occurs Every" option, now we have to see
                // what is the dropdown value he has selected and set the appropriate freq_subday_type value.
                if (icdoJobSchedule.Freq_SubDay_Type_On_Screen == 4)
                    icdoJobSchedule.freq_subday_type = 4;
                else
                    icdoJobSchedule.freq_subday_type = 8;

                icdoJobSchedule.start_time = icdoJobSchedule.StartTime_For_Every;
            }
        }

        private void SetRecurrenceFactor() //Rohan
        {
            // This method will set the frequency interval to the appropriate value based on the frequency type (daily, monthly, weekly or immediate)
            int lintFrequencyTypeValue = Convert.ToInt32(icdoJobSchedule.frequency_type_value);
            switch (lintFrequencyTypeValue)
            {
               
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY:
                    icdoJobSchedule.freq_recurance_factor = icdoJobSchedule.UI_Freq_Rec_Factor_Weekly;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY:
                    icdoJobSchedule.freq_recurance_factor = icdoJobSchedule.UI_Freq_Rec_Factor_Monthly;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE:
                    icdoJobSchedule.freq_recurance_factor = icdoJobSchedule.UI_Freq_Rec_Factor_Monthly_Relative;
                    break;
            }
        }

        private void GetRecurrenceFactor() //Kunal : 25/02
        {
            // This method will set the frequency interval to the appropriate value based on the frequency type (daily, monthly, weekly or immediate)
            int lintFrequencyTypeValue = Convert.ToInt32(icdoJobSchedule.frequency_type_value);
            switch (lintFrequencyTypeValue)
            {

                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY:
                    icdoJobSchedule.UI_Freq_Rec_Factor_Weekly = icdoJobSchedule.freq_recurance_factor;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY:
                    icdoJobSchedule.UI_Freq_Rec_Factor_Monthly = icdoJobSchedule.freq_recurance_factor;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE:
                    icdoJobSchedule.UI_Freq_Rec_Factor_Monthly_Relative = icdoJobSchedule.freq_recurance_factor; 
                    break;
            }
        }


        private void SetFrequencyInterval()
        {
            // This method will set the frequency interval to the appropriate value based on the frequency type (daily, monthly, weekly or immediate)
            int lintFrequencyTypeValue = Convert.ToInt32(icdoJobSchedule.frequency_type_value);
            switch (lintFrequencyTypeValue)
            {
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_DAILY:
                    icdoJobSchedule.frequency_interval = icdoJobSchedule.UI_Freq_Interval_For_Daily_Frequency;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY:
                    icdoJobSchedule.frequency_interval = icdoJobSchedule.UI_Freq_Interval_For_Weekly_Frequency;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY:
                    icdoJobSchedule.frequency_interval = icdoJobSchedule.UI_Freq_Interval_For_Monthly_Frequency;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE:
                    icdoJobSchedule.frequency_interval = icdoJobSchedule.UI_Freq_Interval_For_Monthly_Relative_Frequency;
                    break;
            }
        }

        private void GetFrequencyInterval()
        {
            // This method will get the frequency interval and set the appropriate UI controls 
            // based on the frequency type (daily, monthly, weekly or immediate)
            int lintFrequencyTypeValue = Convert.ToInt32(icdoJobSchedule.frequency_type_value);

            switch (lintFrequencyTypeValue)
            {
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_DAILY:
                    icdoJobSchedule.UI_Freq_Interval_For_Daily_Frequency = icdoJobSchedule.frequency_interval;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY:
                    SetCheckBoxesForWeeklyFrequency();
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY:
                    icdoJobSchedule.UI_Freq_Interval_For_Monthly_Frequency = icdoJobSchedule.frequency_interval;
                    break;
                case BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE:
                    icdoJobSchedule.UI_Freq_Interval_For_Monthly_Relative_Frequency = icdoJobSchedule.frequency_interval;
                    break;
            }
        }

        private void SetStartAndEndDate()
        {
            // This method will set the start and end dates to the appropriate values based on the frequency type (daily, monthly, weekly or immediate)
            if (Convert.ToInt32(icdoJobSchedule.frequency_type_value) != BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_IMMEDIATE)
            {
                // If the frequency type is not immediate
                // If "No end date" has been selected by the user , we have to default the end date to a max date in the future.
                if (icdoJobSchedule.lstrEndDatePresent == BatchHelper.JOB_SCHEDULE_NO_END_DATE_PRESENT)
                {
                    //icdoJobSchedule.end_date = null;// Convert.ToDateTime(BatchHelper.MAX_DATE);
                }
            }
        }

        private void GetStartAndEndDate()
        {
            // This method will set the other appropriate controls based on the end date.
            //if (icdoJobSchedule.end_date == Convert.ToDateTime(BatchHelper.MAX_DATE))
            if (icdoJobSchedule.end_date == DateTime.MinValue)
            {
                icdoJobSchedule.lstrEndDatePresent = BatchHelper.JOB_SCHEDULE_NO_END_DATE_PRESENT;
            }
            else
            {
                icdoJobSchedule.lstrEndDatePresent = BatchHelper.JOB_SCHEDULE_END_DATE_PRESENT;
            }
        }

        public void PrepareUIControlsBasedOnData()
        {
            GetFrequencyInterval();
            GetStartAndEndDate();
            GetStartAndEndTime();
            GetRecurrenceFactor();
        }

        private void GetStartAndEndTime()
        {
            switch (icdoJobSchedule.freq_subday_type)
            {
                case 1:
                    icdoJobSchedule.lstrSubdayFrequency = BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_ONCE;
                    icdoJobSchedule.StartTime_For_Once = icdoJobSchedule.start_time;
                    break;
                case 4:
                    icdoJobSchedule.lstrSubdayFrequency = BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_EVERY;
                    icdoJobSchedule.Freq_SubDay_Type_On_Screen = 4;
                    icdoJobSchedule.StartTime_For_Every = icdoJobSchedule.start_time;
                    break;
                case 8:
                    icdoJobSchedule.lstrSubdayFrequency = BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_EVERY;
                    icdoJobSchedule.Freq_SubDay_Type_On_Screen = 8;
                    icdoJobSchedule.StartTime_For_Every = icdoJobSchedule.start_time;
                    break;
                case 16:
                    icdoJobSchedule.lstrSubdayFrequency = BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_BATCH_WINDOW;
                    //icdoJobSchedule.Freq_SubDay_Type_On_Screen = 16;
                    //DateTime today=DateTime.Now;
                    //icdoJobSchedule.StartTime_For_Once = DateTime.Parse("21:00");
                    break;
            }
        }

        private void SetCheckBoxesForWeeklyFrequency()
        {
            WeeklyFrequencyInterval configuredDays = (WeeklyFrequencyInterval)icdoJobSchedule.frequency_interval;

            if (WeeklyFrequencyInterval.Sunday == (configuredDays & WeeklyFrequencyInterval.Sunday))
            {
                icdoJobSchedule.Sunday = Convert.ToInt32(WeeklyFrequencyInterval.Sunday);
            }

            if (WeeklyFrequencyInterval.Monday == (configuredDays & WeeklyFrequencyInterval.Monday))
            {
                icdoJobSchedule.Monday = Convert.ToInt32(WeeklyFrequencyInterval.Monday);
            }

            if (WeeklyFrequencyInterval.Tuesday == (configuredDays & WeeklyFrequencyInterval.Tuesday))
            {
                icdoJobSchedule.Tuesday = Convert.ToInt32(WeeklyFrequencyInterval.Tuesday);
            }

            if (WeeklyFrequencyInterval.Wednesday == (configuredDays & WeeklyFrequencyInterval.Wednesday))
            {
                icdoJobSchedule.Wednesday = Convert.ToInt32(WeeklyFrequencyInterval.Wednesday);
            }
            if (WeeklyFrequencyInterval.Thursday == (configuredDays & WeeklyFrequencyInterval.Thursday))
            {
                icdoJobSchedule.Thursday = Convert.ToInt32(WeeklyFrequencyInterval.Thursday);
            }

            if (WeeklyFrequencyInterval.Friday == (configuredDays & WeeklyFrequencyInterval.Friday))
            {
                icdoJobSchedule.Friday = Convert.ToInt32(WeeklyFrequencyInterval.Friday);
            }

            if (WeeklyFrequencyInterval.Saturday == (configuredDays & WeeklyFrequencyInterval.Saturday))
            {
                icdoJobSchedule.Saturday = Convert.ToInt32(WeeklyFrequencyInterval.Saturday);
            }
        }


        public void btn_SubmitForApproval_Click()
        {
            icdoJobSchedule.status_value = BatchHelper.JOB_SCHEDULE_HEADER_STATUS_SUBMIT_FOR_APPROVAL;
            icdoJobSchedule.Update();
        }

        public void btn_Approve_Click()
        {
            icdoJobSchedule.status_value = BatchHelper.JOB_SCHEDULE_HEADER_STATUS_APPROVED;
            icdoJobSchedule.Update();

            // If the schedule is of type "Immediate" we need to create a job in the sgs_job_header table
            // so the service can pick it up immediately.
            if (Convert.ToInt32(icdoJobSchedule.frequency_type_value) == BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_IMMEDIATE)
            {
                CloneJobFromSchedule();
            }
        }

        // Whenever someone clicks on "Run Immediate" button, we will go ahead and create 
        // a new Job in the SGS_Job_Header table, so our service can pick it up from there.
        public ArrayList btn_RunImmediate_Click()
        {
            utlError lobjError = null;
            
            if (this.iarrErrors.IsNull())
                this.iarrErrors = new ArrayList();

            if (icdoJobSchedule.job_schedule_id == busConstant.DATA_EXTRACTION_JOB_SCHEDULE_ID)
            {
                busSystemManagement lobjSystemManagement = new busSystemManagement();
                lobjSystemManagement.FindSystemManagement();

                DataTable ldtbAnnualStatementGenerated = busBase.Select<cdoYearEndDataExtractionHeader>(
                  new string[1] { enmYearEndDataExtractionHeader.year.ToString() },
                  new object[1] { lobjSystemManagement.icdoSystemManagement.batch_date.Year - 1 }, null, null);


                if (ldtbAnnualStatementGenerated.Rows.Count > 0 )
                {
                    if (Convert.ToString(ldtbAnnualStatementGenerated.Rows[0][enmYearEndDataExtractionHeader.is_annual_statement_generated_flag.ToString()]) ==
                        busConstant.FLAG_YES)
                    {
                        lobjError = AddError(6133, string.Empty);
                        this.iarrErrors.Add(lobjError);
                        return iarrErrors;
                    }                    
                }

                DataTable ldtbAnnualStatementNotGenerated = busBase.Select<cdoYearEndDataExtractionHeader>(
                  new string[2] { enmYearEndDataExtractionHeader.year.ToString(), enmYearEndDataExtractionHeader.is_annual_statement_generated_flag.ToString() },
                  new object[2] { lobjSystemManagement.icdoSystemManagement.batch_date.Year - 1, busConstant.FLAG_NO }, null, null);

                //RID 80207
                //if (ldtbAnnualStatementNotGenerated.Rows.Count > 0 && icdoJobSchedule.is_annual_statement_generated_flag != busConstant.FLAG_YES)
                //{
                //    lobjError = AddError(6135, string.Empty);
                //    this.iarrErrors.Add(lobjError);
                //    return iarrErrors;
                //}
            }

            //if (icdoJobSchedule.job_schedule_id == busConstant.HEALTH_ELIGIBILITY_ACTUARY_JOB_SCHEDULE_ID)
            //{
            //    busSystemManagement lobjSystemManagement = new busSystemManagement();
            //    lobjSystemManagement.FindSystemManagement();

            //    DataTable ldtbHealthEligibilityActuary = busBase.Select<cdoHealthEligibiltyActuaryData>(
            //  new string[1] { enmHealthEligibiltyActuaryData.plan_year.ToString() },
            //  new object[1] { lobjSystemManagement.icdoSystemManagement.batch_date.Year - 1 }, null, null);

            //    if (ldtbHealthEligibilityActuary.Rows.Count > 0 && icdoJobSchedule.is_annual_statement_generated_flag != busConstant.FLAG_YES)
            //    {
            //        lobjError = AddError(6136, string.Empty);
            //        this.iarrErrors.Add(lobjError);
            //        return iarrErrors;
            //    }

            //}
                       

            if (icdoJobSchedule.job_schedule_id == busConstant.HEALTH_ELIGIBILITY_ACTUARY_JOB_SCHEDULE_ID)
            {
                busSystemManagement lobjSystemManagement = new busSystemManagement();
                lobjSystemManagement.FindSystemManagement();

                DataTable ldtbHealthActuary = busBase.Select<cdoHealthEligibiltyActuaryData>(
                    new string[1] { enmHealthEligibiltyActuaryData.plan_year.ToString() },
                    new object[1] { lobjSystemManagement.icdoSystemManagement.batch_date.Year}, null, null);

                if (ldtbHealthActuary.Rows.Count > 0)
                {
                    lobjError = AddError(6165, "");
                    this.iarrErrors.Add(lobjError);
                    return iarrErrors;
                }
            }

            CloneJobFromSchedule();

            return iarrErrors;
        }

        // TODO:prem to move this validation code to xml business rule, for now I am writing this code in C#
        public bool DoesFrequencySubdayIntervalHaveValidValues()
        {
            if (icdoJobSchedule.lstrSubdayFrequency == BatchHelper.JOB_SCHEDULE_SUBDAY_FREQUENCY_EVERY)
            {
                if (icdoJobSchedule.Freq_SubDay_Type_On_Screen == BatchHelper.JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_MINUTES)
                {
                    if (icdoJobSchedule.freq_subday_interval > 1 && icdoJobSchedule.freq_subday_interval < 60)
                    {
                        return true;
                    }
                }
                else if (icdoJobSchedule.Freq_SubDay_Type_On_Screen == BatchHelper.JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_HOURS)
                {
                    if (icdoJobSchedule.freq_subday_interval >= 1 && icdoJobSchedule.freq_subday_interval < 24)
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }

        public ArrayList btnMoveJobScheduleUp(int aintJobScheduleDetailId)
        {
            // We will declare Arraylist to catch all the errors that will be reported during the
            // process of setting primary address
            ArrayList larrErrors = new ArrayList();

            // Let's first get the currently selected Job Schedule Detail Id from the jobscheduledetail collection.
            busJobScheduleDetail lobjCurrentJobScheduleDetail =
                (from lobjJobScheduleDetail in iclbJobScheduleDetail
                 where lobjJobScheduleDetail.icdoJobScheduleDetail.job_schedule_detail_id == aintJobScheduleDetailId
                 select lobjJobScheduleDetail).FirstOrDefault();

            // Flag to indicate whether the order number has been modified or not, 
            // if it has been modified then we need to validate the core object again, since now potentially the dependent 
            // step numbers could raise validation errors or it could fix those errors
            bool lblnOrderNumberModified = false;

            if (lobjCurrentJobScheduleDetail != null)
            {
                int lintOrderNumber = lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.order_number;
                // If the currently selected object is not the first order number in the collection, then we have to 
                // start manipulating things in the collection for the object just ahead of this order number.

                // Check to see if this is not the minimum order number..
                int lintMinOrderNumber = (from lobjJobScheduleDetail in iclbJobScheduleDetail
                                          select (lobjJobScheduleDetail.icdoJobScheduleDetail.order_number)).Min();

                if (lintOrderNumber != lintMinOrderNumber)
                {
                    // Get the immediately preceding JobScheduleDetail object
                    busJobScheduleDetail lobjPreviousJobScheduleDetail =
                        (from lobjJobScheduleDetail in iclbJobScheduleDetail
                         where lobjJobScheduleDetail.icdoJobScheduleDetail.order_number < lintOrderNumber
                         select lobjJobScheduleDetail).LastOrDefault();

                    // If we are able to find a previous record, then go ahead and increment the order number
                    // so it would move down the list.
                    if (lobjPreviousJobScheduleDetail != null)
                    {
                        lobjPreviousJobScheduleDetail.icdoJobScheduleDetail.order_number++;
                        lobjPreviousJobScheduleDetail.icdoJobScheduleDetail.Update();
                    }

                    // Now decrease the order number for the selected record
                    lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.order_number--;
                    lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.Update();


                    lblnOrderNumberModified = true;
                }
            }

            if (lblnOrderNumberModified)
            {
                ValidateAndUpdateStatus();
            }
            return larrErrors;
        }

        public ArrayList btnMoveJobScheduleDown(int aintJobScheduleDetailId)
        {
            // We will declare Arraylist to catch all the errors that will be reported during the
            // process of setting primary address
            ArrayList larrErrors = new ArrayList();

            // Let's first get the currently selected Job Schedule Detail Id from the jobscheduledetail collection.
            busJobScheduleDetail lobjCurrentJobScheduleDetail =
                (from lobjJobScheduleDetail in iclbJobScheduleDetail
                 where lobjJobScheduleDetail.icdoJobScheduleDetail.job_schedule_detail_id == aintJobScheduleDetailId
                 select lobjJobScheduleDetail).FirstOrDefault();

            // Flag to indicate whether the order number has been modified or not, 
            // if it has been modified then we need to validate the core object again, since now potentially the dependent 
            // step numbers could raise validation errors or it could fix those errors
            bool lblnOrderNumberModified = false;

            if (lobjCurrentJobScheduleDetail != null)
            {
                int lintOrderNumber = lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.order_number;
                // Check to see if this is not the maximum order number..
                int lintMaxOrderNumber = (from lobjJobScheduleDetail in iclbJobScheduleDetail
                                          select (lobjJobScheduleDetail.icdoJobScheduleDetail.order_number)).Max();

                // If the currently selected object is not the first order number in the collection, then we have to 
                // start manipulating things in the collection for the object just ahead of this order number.
                if (lintOrderNumber != lintMaxOrderNumber)
                {
                    // Get the immediately preceding JobScheduleDetail object
                    busJobScheduleDetail lobjPreviousJobScheduleDetail =
                        (from lobjJobScheduleDetail in iclbJobScheduleDetail
                         where lobjJobScheduleDetail.icdoJobScheduleDetail.order_number > lintOrderNumber
                         select lobjJobScheduleDetail).FirstOrDefault();

                    // If we are able to find a previous record, then go ahead and decrement the order number
                    // so it would move down the list.
                    if (lobjPreviousJobScheduleDetail != null)
                    {
                        lobjPreviousJobScheduleDetail.icdoJobScheduleDetail.order_number--;
                        lobjPreviousJobScheduleDetail.icdoJobScheduleDetail.Update();
                    }

                    // Now increase the order number for the selected record
                    lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.order_number++;
                    lobjCurrentJobScheduleDetail.icdoJobScheduleDetail.Update();


                    lblnOrderNumberModified = true;
                }
            }

            if (lblnOrderNumberModified)
            {
                ValidateAndUpdateStatus();
            }
            return larrErrors;
        }


        private void ValidateAndUpdateStatus()
        {
            ValidateSoftErrors();
            UpdateValidateStatus();
        }

        /// <summary>
        /// Creates job schedule request to invoke the job 
        /// </summary>
        /// <param name="aintStepNumber">step number to identify job schedule</param>
        public static void SubmitJobServiceRequest(int aintStepNumber, string strJobScheduleParamName, string strJobScheduleParamValue)
        {
            // Identifies job schedule by step number
            busJobSchedule lobjJobSchedule = GetJobScheduleByStepNo(aintStepNumber);
            if (lobjJobSchedule == null) return;

            // Loads job schedule detail and its parameters
            lobjJobSchedule.LoadJobScheduleDetails(true);

            // Suresh - Finds and repalce the editable job parameter value which is been passed as input
            if (strJobScheduleParamName != string.Empty)
            {
                if (lobjJobSchedule.iclbJobScheduleDetail.Count > 0 && lobjJobSchedule.iclbJobScheduleDetail[0].iclbEditableJobScheduleParameters != null
                    && lobjJobSchedule.iclbJobScheduleDetail[0].iclbEditableJobScheduleParameters.Count > 0)
                {
                    // Get the current job schedule parameter item for the given param name from the collection
                    busJobScheduleParams lobjJobScheduleParams =
                        lobjJobSchedule.iclbJobScheduleDetail[0].iclbEditableJobScheduleParameters
                            .Where(lobjJobScheduleParam => lobjJobScheduleParam.icdoJobScheduleParams.param_name == strJobScheduleParamName)
                            .FirstOrDefault();
                    // Sets the given param value for the job schedule parameter object if exists
                    if (lobjJobScheduleParams != null) lobjJobScheduleParams.icdoJobScheduleParams.param_value = strJobScheduleParamValue;
                }
            }

            // Creates job schedule request to invoke the job process (basically inserts a record in sgs_job_header table in Queued status)
            lobjJobSchedule.CloneJobFromSchedule();
        }

        /// <summary>
        /// Loads and returns job schedule based on given step number
        /// </summary>
        /// <param name="astrStepNumber">step number to identify job schedule</param>
        /// <returns></returns>
        public static busJobSchedule GetJobScheduleByStepNo(int aintStepNumber)
        {
            // Go ahead and get the Job Schedule for the given passed in step number
            // get only the schedules that are in approved status and of immediate frequency type.
            DataTable ldtbJobSchedule = busBase.Select("cdoJobSchedule.GetJobScheduleForStepNo",
                new object[3] { BatchHelper.JOB_SCHEDULE_HEADER_STATUS_APPROVED, BatchHelper.JOB_SCHEDULE_FREQUENCY_TYPE_IMMEDIATE, aintStepNumber });

            if (ldtbJobSchedule.Rows.Count <= 0)
                return null;

            busJobSchedule lobjJobSchedule = new busJobSchedule();
            lobjJobSchedule.icdoJobSchedule = new cdoJobSchedule();

            lobjJobSchedule.icdoJobSchedule.LoadData(ldtbJobSchedule.Rows[0]);
            return lobjJobSchedule;
        }

        public void RunJobInImmediateMode(Collection<clsJobScheduleDetailsParameters> aclbJobScheduleDetailParameters)
        {
            busJobHeader lbusJobHeader = CloneJobFromSchedule();
            if (aclbJobScheduleDetailParameters != null)
            {
                foreach (busJobDetail lbusJobDetail in lbusJobHeader.iclbJobDetail)
                {
                    foreach (clsJobScheduleDetailsParameters aclsJobScheduleDetailParameters in aclbJobScheduleDetailParameters)
                    {
                        //Matching on the Step no instead of job detail id.
                        if (aclsJobScheduleDetailParameters.iintStepNumber == lbusJobDetail.icdoJobDetail.step_no)
                        {
                            lbusJobDetail.LoadJobParameters();

                            if (lbusJobDetail.iclbJobParameters.IsNotNull())
                            {
                                foreach (busJobParameters lbusJobParameters in lbusJobDetail.iclbJobParameters)
                                {
                                    foreach (busJobParameters lbusInputJobParameters in aclsJobScheduleDetailParameters.iclbParameters)
                                    {
                                        //Matching on the param name instead of job_parameters_id.                                     
                                        if (lbusInputJobParameters.icdoJobParameters.param_name == lbusJobParameters.icdoJobParameters.param_name)
                                        {
                                            lbusJobParameters.icdoJobParameters.param_value = lbusInputJobParameters.icdoJobParameters.param_value;
                                            lbusJobParameters.icdoJobParameters.Update();
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Overridden Methods

        public override void BeforePersistChanges()
        {
            istrOverriddenStatus = this.icdoJobSchedule.status_value;
            SetStartAndEndDate();
            SetFrequencyInterval();
            SetRecurrenceFactor();
            SetStartAndEndTime();

            if(icdoJobSchedule.active_flag == busConstant.FLAG_YES 
                && (this.icdoJobSchedule.status_value != busConstant.MPIPHPBatch.JOB_STATUS_ON_DEMAND &&
                this.icdoJobSchedule.status_value != busConstant.MPIPHPBatch.JOB_STATUS_AD_HOC))
            {
                istrOverriddenStatus = busConstant.MPIPHPBatch.JOB_STATUS_ON_SCHEDULE;
                this.icdoJobSchedule.status_value = busConstant.MPIPHPBatch.JOB_STATUS_ON_SCHEDULE;
            }

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            this.icdoJobSchedule.status_value = istrOverriddenStatus;
            this.icdoJobSchedule.Update();
        }

        public override bool ValidateSoftErrors()
        {
            bool lblnRetVal;
            if (iclbJobScheduleDetail == null)
            {
                LoadJobScheduleDetails();
            }
            if (iclbJobScheduleDetail != null && iclbJobScheduleDetail.Count > 0)
            {
                foreach (busJobScheduleDetail lobjJobScheduleDetail in iclbJobScheduleDetail)
                {
                    lobjJobScheduleDetail.ibusJobSchedule = this;
                    lobjJobScheduleDetail.iblnHeaderValidating = true;
                    lobjJobScheduleDetail.ValidateSoftErrors();
                    lobjJobScheduleDetail.UpdateValidateStatus();
                    // set this back to false, since we want to do skip some processing in
                    // updatevalidatestatus() method based on this flag.
                    lobjJobScheduleDetail.iblnHeaderValidating = false;
                }
            }
            lblnRetVal = base.ValidateSoftErrors();
            UpdateValidateStatus();
            return lblnRetVal;
        }


        #endregion


    }
}
