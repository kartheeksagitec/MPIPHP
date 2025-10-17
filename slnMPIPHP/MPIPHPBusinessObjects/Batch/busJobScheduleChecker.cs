using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using MPIPHP.CustomDataObjects;
using MPIPHP.Common;
using MPIPHP.DataObjects;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Summary description for Shedule.
    /// </summary>
    [Serializable]
    public class busJobScheduleChecker : busJobSchedule
    {
        #region Private Constants

        private const
            WeeklyFrequencyInterval _iWeekDays = WeeklyFrequencyInterval.Monday |
            WeeklyFrequencyInterval.Tuesday | WeeklyFrequencyInterval.Wednesday | WeeklyFrequencyInterval.Thursday |
            WeeklyFrequencyInterval.Friday;
        private const
            WeeklyFrequencyInterval _iWeekEndDays = WeeklyFrequencyInterval.Saturday | WeeklyFrequencyInterval.Sunday;
        private const
            WeeklyFrequencyInterval _iDays = _iWeekDays | _iWeekEndDays;

        #endregion

        // Helper methods
        /// <summary>
        /// Gets all the active scheduled jobs from the SGS_Job_Schedule table.
        /// </summary>
        /// <returns></returns>
        public static Collection<busJobScheduleChecker> GetActiveScheduledJobs()
        {
            Collection<busJobScheduleChecker> lclbJobSchedule = null;

            //DataTable ldtbJobSchedule = Select<cdoJobSchedule>(
            //        new string[2] { enmJobSchedule.active_flag.ToString(), enmJobSchedule.status_value.ToString() },
            //        new object[2] { busConstant.Flag_Yes, BatchHelper.JOB_SCHEDULE_HEADER_STATUS_VALID }, null, "priority_value asc");


            DataTable ldtbJobSchedule = busBase.Select("cdoJobSchedule.GetActiveSchedules", new object[] { });


            if (ldtbJobSchedule.Rows.Count > 0)
            {
                lclbJobSchedule = new busJobSchedule().GetCollection<busJobScheduleChecker>(ldtbJobSchedule, "icdoJobSchedule");
            }

            DataTable ldtbJobHeader = busBase.Select("cdoJobHeader.GetExclusionListForScheduling", new object[] { });
            /*DataTable ldtbJobHeader = Select<cdoJobHeader>
            (
                new string[1] { enmJobHeader.status_value.ToString() },
                new object[1] { BatchHelper.JOB_HEADER_STATUS_QUEUED }, new object[1] { BatchHelper.JOB_HEADER_STATUS_PROCESSING.ToString() },
                enmJobHeader.job_schedule_id.ToString() + " asc"
            );*/
            Collection<busJobHeader> lclbJobHeader = null;
            if (ldtbJobHeader.Rows.Count > 0)
            {
                lclbJobHeader = new busJobHeader().GetCollection<busJobHeader>(ldtbJobHeader, "icdoJobHeader");
            }
            Collection<busJobScheduleChecker> lclbSchedulesNotReady = new Collection<busJobScheduleChecker>();
            if (lclbJobHeader != null)
            {
                foreach (busJobScheduleChecker lbusJobScheduleChecker in lclbJobSchedule)
                {
                    foreach (busJobHeader lbusJobHeader in lclbJobHeader)
                    {
                        if (lbusJobScheduleChecker.icdoJobSchedule.job_schedule_id == lbusJobHeader.icdoJobHeader.job_schedule_id)
                        {
                            lclbSchedulesNotReady.Add(lbusJobScheduleChecker);
                            break;
                        }
                    }
                }
            }
            if (lclbSchedulesNotReady != null && lclbSchedulesNotReady.Count > 0)
            {
                foreach (busJobScheduleChecker lbusJobScheduleCheckerToBeRemoved in lclbSchedulesNotReady)
                {
                    foreach (busJobScheduleChecker lbusJobScheduleChecker in lclbJobSchedule)
                    {
                        lclbJobSchedule.Remove(lbusJobScheduleCheckerToBeRemoved);
                        break;
                    }
                }
            }
            return lclbJobSchedule;
        }

        // Methods only for identifying whether a Schedule should be run for a given time or not.

        #region ISchedule Members

        /// <summary>
        /// Have taken off the holiday logic as discussed with Vinovin : 14/03/2013
        /// </summary>
        /// <param name="adtmCurrent"></param>
        /// <returns></returns>
        public bool CanRun(DateTime adtmCurrent)
        {
            //bool lblnReturnValue = false;
            //if (adtmCurrent.Date.IsHoliday())
            //{
            //    lblnReturnValue = (icdoJobSchedule.execute_on_org_holiday_flag.ToLower() == busConstant.Flag_Yes.ToLower());
            //}
            //else
            //{
            //    lblnReturnValue = true;
            //}

            //if (lblnReturnValue == true)
            //{
                bool lblnReturnValue = false;
                int lintCurrent = BatchHelper.DateToInt32(adtmCurrent);
                int lintCurrentTime = BatchHelper.TimeToInt32(adtmCurrent);

                // the current date should be within the start and end dates
                //if (!BatchHelper.IsDateBetween(lintCurrent, icdoJobSchedule.ActiveStartDate, icdoJobSchedule.ActiveEndDate))
                //    return false;

                if (!BatchHelper.IsDateBetween(adtmCurrent, icdoJobSchedule.start_date, icdoJobSchedule.end_date))
                    return false;

                lblnReturnValue = CheckInterval(lintCurrent, lintCurrentTime);
            //}
            return lblnReturnValue;
        }

        #endregion

        #region Private Helper Methods
        private bool CheckInterval(int adtmCurrentDate, int aintCurrentTime)
        {
            bool lblnReturnValue = false;

            switch (Convert.ToInt32(icdoJobSchedule.frequency_type_value))
            {
                case (int)FrequencyType.Once:
                    lblnReturnValue = CheckOnce(adtmCurrentDate, aintCurrentTime);
                    break;

                case (int)FrequencyType.Daily:
                    lblnReturnValue = CheckDaily(adtmCurrentDate) && CheckSubDayInterval(adtmCurrentDate, aintCurrentTime);
                    break;

                case (int)FrequencyType.Weekly:
                    lblnReturnValue = CheckWeekly(adtmCurrentDate) && CheckSubDayInterval(adtmCurrentDate, aintCurrentTime);
                    break;

                case (int)FrequencyType.Monthly:
                    lblnReturnValue = CheckMonthly(adtmCurrentDate) && CheckSubDayInterval(adtmCurrentDate, aintCurrentTime);
                    break;

                case (int)FrequencyType.MonthlyRelative:
                    lblnReturnValue = CheckMonthlyRelative(adtmCurrentDate) && CheckSubDayInterval(adtmCurrentDate, aintCurrentTime);
                    break;

                default:
                    break;

            }

            return lblnReturnValue;

        }

        private bool CheckSubDayInterval(int adtmCurrentDate, int aintCurrentTime)
        {
            bool lblnReturnValue = false;

            switch (icdoJobSchedule.freq_subday_type)
            {
                case (int)FrequencySubDayType.AtTheSpecifiedTime:
                    lblnReturnValue = (aintCurrentTime>=icdoJobSchedule.ActiveStartTime) && (aintCurrentTime <= (icdoJobSchedule.ActiveStartTime + 1));
                    break;

                case (int)FrequencySubDayType.Hours:
                    lblnReturnValue = (BatchHelper.IsTimeBetween(aintCurrentTime, icdoJobSchedule.ActiveStartTime, icdoJobSchedule.ActiveEndTime) &&
                        CheckHours(aintCurrentTime));
                    break;

                case (int)FrequencySubDayType.Minutes:
                    lblnReturnValue = (BatchHelper.IsTimeBetween(aintCurrentTime, icdoJobSchedule.ActiveStartTime, icdoJobSchedule.ActiveEndTime) &&
                        CheckMinutes(aintCurrentTime));
                    break;

                case (int)FrequencySubDayType.Seconds:
                    lblnReturnValue = (BatchHelper.IsTimeBetween(aintCurrentTime, icdoJobSchedule.ActiveStartTime, icdoJobSchedule.ActiveEndTime) &&
                        CheckSeconds(aintCurrentTime));
                    break;
                case (int)FrequencySubDayType.DuringBatchWindow:
                    busCode lbusCode = new busCode();
                    cdoCodeValue lcdoCodeValue = lbusCode.GetCodeValue(BatchHelper.CODE_ID_BATCH_EXECUTION_WINDOW, BatchHelper.CODE_VALUE_BATCH_EXECUTION_WINDOW);
                    DateTime ldtBatchWindowStartTime = DateTime.Parse(lcdoCodeValue.data1);
                    DateTime ldtBatchWindowEndTime = DateTime.Parse(lcdoCodeValue.data2);
                    DateTime ldtmBatchWindowStartForToday = BatchHelper.Int32ToDate(adtmCurrentDate);

                    DateTime ldtmBatchWindowEndForToday;

                    if (ldtBatchWindowStartTime > ldtBatchWindowEndTime)//crosses date boundary
                    {
                        ldtmBatchWindowEndForToday = ldtmBatchWindowStartForToday.AddDays(1);
                    }
                    else
                    {
                        ldtmBatchWindowEndForToday = ldtmBatchWindowStartForToday;//on same day
                    }

                    ldtmBatchWindowStartForToday = new DateTime(ldtmBatchWindowStartForToday.Year, ldtmBatchWindowStartForToday.Month, ldtmBatchWindowStartForToday.Day, ldtBatchWindowStartTime.Hour, ldtBatchWindowStartTime.Minute, ldtBatchWindowStartTime.Second);
                    ldtmBatchWindowEndForToday = new DateTime(ldtmBatchWindowEndForToday.Year, ldtmBatchWindowEndForToday.Month, ldtmBatchWindowEndForToday.Day, ldtBatchWindowEndTime.Hour, ldtBatchWindowEndTime.Minute, ldtBatchWindowEndTime.Second);
                    if ((BatchHelper.IsTimeBetween(aintCurrentTime, BatchHelper.TimeToInt32(ldtBatchWindowStartTime), BatchHelper.TimeToInt32(ldtBatchWindowEndTime))))
                    {
                        //if this job has already executed in batch window today, then return false
                        lblnReturnValue = !HasJobExecutedInTodaysBatchWindow(ldtmBatchWindowStartForToday, ldtmBatchWindowEndForToday);
                    }
                    else
                    {
                        lblnReturnValue = false;
                    }
                    lbusCode = null;
                    lcdoCodeValue = null;
                    break;
                default:
                    break;
            }

            return lblnReturnValue;
        }

        private bool HasJobExecutedInTodaysBatchWindow(DateTime adtmBatchWindowStartForToday, DateTime adtmBatchWindowEndForToday)
        {
            /*
            DataTable ldtbJobHeader = SelectWithOperator<cdoJobHeader>(
            new string[3] { enmJobHeader.job_schedule_id.ToString(), enmJobHeader.start_time.ToString(), enmJobHeader.start_time.ToString() },
            new string[3] { busConstant.DBOperatorEquals, busConstant.DBOperatorGreaterThanEquals, busConstant.DBOperatorLessThanEquals },
            new object[3] { icdoJobSchedule.job_schedule_id, ldtmBatchWindowStartForToday, ldtmBatchWindowEndForToday }, null);
            */
            DataTable ldtbJobHeader = busBase.Select("cdoJobHeader.GetJobsExecutedInTimeFrame",
                                                          new object[3] { icdoJobSchedule.job_schedule_id, adtmBatchWindowStartForToday, adtmBatchWindowEndForToday });

            return (ldtbJobHeader != null && ldtbJobHeader.Rows.Count > 0);
        }

        private bool CheckOnce(int adtmCurrentDate, int aintCurrentTime)
        {
            bool lblnReturnValue = false;

            lblnReturnValue = (adtmCurrentDate == icdoJobSchedule.ActiveStartDate) && (aintCurrentTime == icdoJobSchedule.ActiveStartTime);

            return lblnReturnValue;
        }

        private bool CheckDaily(int aintDate)
        {
            bool lblnReturnValue = false;
            DateTime ltsCurrent = BatchHelper.Int32ToDate(aintDate);
            DateTime ltsStart = BatchHelper.Int32ToDate(icdoJobSchedule.ActiveStartDate);
            TimeSpan ltsTim = ltsCurrent - ltsStart;

            lblnReturnValue = (ltsTim.TotalDays % icdoJobSchedule.frequency_interval) == 0;

            return lblnReturnValue;
        }

        private bool CheckWeekly(int aintDate)
        {
            bool lblnReturnValue = false;
            DateTime ldtmCurrent = BatchHelper.Int32ToDate(aintDate);
            DateTime ldtmStart = BatchHelper.Int32ToDate(icdoJobSchedule.ActiveStartDate);
            TimeSpan ltsTimeSpan = ldtmCurrent - ldtmStart;

            WeeklyFrequencyInterval currentDay = (WeeklyFrequencyInterval)
                Enum.Parse(typeof(WeeklyFrequencyInterval), ldtmCurrent.DayOfWeek.ToString("G"), true);
            WeeklyFrequencyInterval configuredDay = (WeeklyFrequencyInterval)icdoJobSchedule.frequency_interval;

            bool tv = ((int)(ltsTimeSpan.TotalDays / 7) % icdoJobSchedule.freq_recurance_factor) == 0;

            lblnReturnValue = tv && (currentDay == (currentDay & configuredDay));

            return lblnReturnValue;
        }

        private bool CheckMonthly(int aintDate)
        {
            bool lblnReturnValue = false;
            DateTime ldtmCurrent = BatchHelper.Int32ToDate(aintDate);
            DateTime ldtmStart = BatchHelper.Int32ToDate(icdoJobSchedule.ActiveStartDate);

            bool lblnRecurrenceValue = ((ldtmCurrent.Month - ldtmStart.Month) % icdoJobSchedule.freq_recurance_factor) == 0;

            lblnReturnValue = lblnRecurrenceValue && (ldtmCurrent.Day == icdoJobSchedule.frequency_interval);

            return lblnReturnValue;
        }

        private bool CheckMonthlyRelative(int aintDate)
        {
            bool lblnReturnValue = false;
            DateTime ldtmCurrent = BatchHelper.Int32ToDate(aintDate);
            DateTime ldtmStart = BatchHelper.Int32ToDate(icdoJobSchedule.ActiveStartDate);

            WeeklyFrequencyInterval lenmCurrentDay = (WeeklyFrequencyInterval)
                Enum.Parse(typeof(WeeklyFrequencyInterval), ldtmCurrent.DayOfWeek.ToString("G"), true);

            int[] lintNumDays = { 31, (DateTime.IsLeapYear(ldtmCurrent.Year)) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

            switch (icdoJobSchedule.frequency_interval)
            {
                case (int)MonthlyRelativeFrequencyInterval.Day:

                    switch (icdoJobSchedule.freq_relative_interval)
                    {
                        case (int)FrequencyRelativeInterval.First:
                            if (ldtmCurrent.Day == 1)
                                lblnReturnValue = true;
                            break;

                        case (int)FrequencyRelativeInterval.Second:
                            if (ldtmCurrent.Day == 2)
                                lblnReturnValue = true;
                            break;

                        case (int)FrequencyRelativeInterval.Third:
                            if (ldtmCurrent.Day == 3)
                                lblnReturnValue = true;
                            break;

                        case (int)FrequencyRelativeInterval.Fourth:
                            if (ldtmCurrent.Day == 4)
                                lblnReturnValue = true;
                            break;

                        case (int)FrequencyRelativeInterval.Last:
                            if (lintNumDays[ldtmCurrent.Month - 1] == ldtmCurrent.Day)
                                lblnReturnValue = true;
                            break;
                    }

                    break;

                case (int)MonthlyRelativeFrequencyInterval.Weekday:

                    if (lenmCurrentDay != (lenmCurrentDay & _iWeekDays))
                    {
                        lblnReturnValue = false;
                    }
                    else
                    {
                        lblnReturnValue = (ldtmCurrent.Day == GetDayByOccurrence(_iWeekDays,
                            (FrequencyRelativeInterval)icdoJobSchedule.freq_relative_interval, ldtmCurrent));
                    }

                    break;

                case (int)MonthlyRelativeFrequencyInterval.WeekendDay:

                    if (lenmCurrentDay != (lenmCurrentDay & _iWeekEndDays))
                    {
                        lblnReturnValue = false;
                    }
                    else
                    {
                        lblnReturnValue = (ldtmCurrent.Day == GetDayByOccurrence(_iWeekEndDays,
                            (FrequencyRelativeInterval)icdoJobSchedule.freq_relative_interval, ldtmCurrent));
                    }

                    break;

                case (int)MonthlyRelativeFrequencyInterval.Sunday:
                case (int)MonthlyRelativeFrequencyInterval.Monday:
                case (int)MonthlyRelativeFrequencyInterval.Tuesday:
                case (int)MonthlyRelativeFrequencyInterval.Wednesday:
                case (int)MonthlyRelativeFrequencyInterval.Thursday:
                case (int)MonthlyRelativeFrequencyInterval.Friday:
                case (int)MonthlyRelativeFrequencyInterval.Saturday:

                    if (lenmCurrentDay != (lenmCurrentDay & _iDays))
                    {
                        lblnReturnValue = false;
                    }
                    else
                    {
                        lblnReturnValue = (ldtmCurrent.Day == GetDayByOccurrence((MonthlyRelativeFrequencyInterval)icdoJobSchedule.frequency_interval,
                            (FrequencyRelativeInterval)icdoJobSchedule.freq_relative_interval, ldtmCurrent));
                    }

                    break;

                default:
                    break;

            }

            bool tv = ((ldtmCurrent.Month - ldtmStart.Month) % icdoJobSchedule.freq_recurance_factor) == 0;

            lblnReturnValue = lblnReturnValue && tv;

            return lblnReturnValue;
        }

        private bool CheckHours(int aintTime)
        {
            bool lblnReturnValue = false;
            TimeSpan ltsCurrent = BatchHelper.Int32ToTime(aintTime);
            TimeSpan ltsStart = BatchHelper.Int32ToTime(icdoJobSchedule.ActiveStartTime);
            TimeSpan ltsDifference = ltsCurrent - ltsStart;

            lblnReturnValue = (((int)ltsDifference.TotalHours) % icdoJobSchedule.freq_subday_interval) == 0;

            return lblnReturnValue;
        }

        private bool CheckMinutes(int aintTime)
        {
            bool lblnReturnValue = false;
            TimeSpan ltsCurrent = BatchHelper.Int32ToTime(aintTime);
            TimeSpan ltsStart = BatchHelper.Int32ToTime(icdoJobSchedule.ActiveStartTime);
            TimeSpan ltsDifference = ltsCurrent - ltsStart;

            lblnReturnValue = (((int)ltsDifference.TotalMinutes) % icdoJobSchedule.freq_subday_interval) == 0;

            return lblnReturnValue;
        }

        private bool CheckSeconds(int aintTime)
        {
            bool lblnReturnValue = false;
            TimeSpan ltsCurrent = BatchHelper.Int32ToTime(aintTime);
            TimeSpan ltsStart = BatchHelper.Int32ToTime(icdoJobSchedule.ActiveStartTime);
            TimeSpan ltsDifference = ltsCurrent - ltsStart;

            lblnReturnValue = (((int)ltsDifference.TotalSeconds) % icdoJobSchedule.freq_subday_interval) == 0;

            return lblnReturnValue;
        }

        private int GetDayByOccurrence(WeeklyFrequencyInterval aenmWeeklyFrequencyInterval,
            FrequencyRelativeInterval aenmFrequencyRelativeInterval,
            DateTime adtmCurrent)
        {
            int lintReturnValue = 0;
            int lintFactor = 0;

            switch (aenmFrequencyRelativeInterval)
            {
                case FrequencyRelativeInterval.First:
                    lintFactor = 1;
                    break;

                case FrequencyRelativeInterval.Second:
                    lintFactor = 2;
                    break;

                case FrequencyRelativeInterval.Third:
                    lintFactor = 3;
                    break;

                case FrequencyRelativeInterval.Fourth:
                    lintFactor = 4;
                    break;

                case FrequencyRelativeInterval.Last:
                    lintFactor = 31;
                    break;

                default:
                    break;
            }

            DateTime ldtmCurrent = new DateTime(adtmCurrent.Year, adtmCurrent.Month, 1);
            WeeklyFrequencyInterval lenmCurrentDay;

            int i = 0;
            int lintFoundAt = 0;

            while (true)
            {
                lenmCurrentDay = (WeeklyFrequencyInterval)
                    Enum.Parse(typeof(WeeklyFrequencyInterval), ldtmCurrent.DayOfWeek.ToString("G"), true);

                if (lenmCurrentDay == (lenmCurrentDay & aenmWeeklyFrequencyInterval))
                {
                    i++;
                    lintFoundAt = ldtmCurrent.Day;
                }

                if (i < lintFactor && ldtmCurrent.AddDays(1).Month == adtmCurrent.Month)
                {
                    ldtmCurrent = ldtmCurrent.AddDays(1);
                }
                else
                    break;
            }

            lintReturnValue = lintFoundAt;

            return lintReturnValue;
        }

        private int GetDayByOccurrence(MonthlyRelativeFrequencyInterval aenmMonthkyRelativeFrequencyInterval,
            FrequencyRelativeInterval aenmFrequencyRelativeInterval,
            DateTime adtmCurrent)
        {
            int lintReturnValue = 0;
            int lintFactor = 0;

            switch (aenmFrequencyRelativeInterval)
            {
                case FrequencyRelativeInterval.First:
                    lintFactor = 1;
                    break;

                case FrequencyRelativeInterval.Second:
                    lintFactor = 2;
                    break;

                case FrequencyRelativeInterval.Third:
                    lintFactor = 3;
                    break;

                case FrequencyRelativeInterval.Fourth:
                    lintFactor = 4;
                    break;

                case FrequencyRelativeInterval.Last:
                    lintFactor = 31;
                    break;

                default:
                    break;
            }

            DateTime ldtmCurrent = new DateTime(adtmCurrent.Year, adtmCurrent.Month, 1);
            MonthlyRelativeFrequencyInterval lenmCurrentDay;

            int i = 0;
            int lintFoundAt = 0;

            while (true)
            {
                lenmCurrentDay = (MonthlyRelativeFrequencyInterval)
                    Enum.Parse(typeof(MonthlyRelativeFrequencyInterval), ldtmCurrent.DayOfWeek.ToString("G"), true);

                if (lenmCurrentDay == aenmMonthkyRelativeFrequencyInterval)
                {
                    i++;
                    lintFoundAt = ldtmCurrent.Day;
                }

                if (i < lintFactor && ldtmCurrent.AddDays(1).Month == adtmCurrent.Month)
                {
                    ldtmCurrent = ldtmCurrent.AddDays(1);
                }
                else
                    break;
            }

            lintReturnValue = lintFoundAt;

            return lintReturnValue;
        }


        #endregion


    }
}
