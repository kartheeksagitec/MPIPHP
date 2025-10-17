#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
using MPIPHP.CustomDataObjects;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
    /// <summary>
    /// Class MPIPHP.BusinessObjects.busHoliday:
    /// Inherited from busHolidayGen, the class is used to customize the business object busHolidayGen.
    /// </summary>
    [Serializable]
    public class busHoliday : busHolidayGen
    {
        #region [Static Methods]

        /// <summary>
        /// Get Number of working days between two days.
        /// </summary>
        /// <param name="adtStartDate">From Date</param>
        /// <param name="adtEndDate">To Date</param>
        /// <returns>No of working days</returns>
        public static int GetNoOfWorkingDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            // Perform Swap
            if (adtStartDate > adtEndDate)
            {
                DateTime ldtTempDate = adtStartDate;
                adtStartDate = adtEndDate;
                adtEndDate = ldtTempDate;
            }

            // Get Total No of Days
            int lintTotalNoOfDays = (adtEndDate - adtStartDate).Days;
            int lintNoOfHolidays = 0;

            // Get Total No of Holidays falling between the days.            
            DataTable ldtbHolidays = busMPIPHPBase.SelectWithOperator<cdoHoliday>(
                new string[2] { enmHoliday.holiday_date.ToString(), enmHoliday.holiday_date.ToString() },
                new string[2] { ">=", "<=" },
                new object[2] { adtStartDate, adtEndDate }, enmHoliday.holiday_date.ToString());

            busMPIPHPBase lbusBase = new busMPIPHPBase();
            Collection<busHoliday> lclbHolidays = lbusBase.GetCollection<busHoliday>(ldtbHolidays, "icdoHoliday");

            int lintNoOfSaturdays = CountOfDayInRange(DayOfWeek.Saturday, adtStartDate, adtEndDate);
            int lintNoOfSundays = CountOfDayInRange(DayOfWeek.Sunday, adtStartDate, adtEndDate);
            lintNoOfHolidays = lclbHolidays.Count + lintNoOfSaturdays + lintNoOfSundays;

            foreach (busHoliday lbusHoliday in lclbHolidays)
            {
                // Negate as the day is added twice now.
                if (lbusHoliday.icdoHoliday.holiday_date.DayOfWeek == DayOfWeek.Saturday
                    || lbusHoliday.icdoHoliday.holiday_date.DayOfWeek == DayOfWeek.Sunday)
                    lintNoOfHolidays--;
            }

            return lintTotalNoOfDays - lintNoOfHolidays;
        }


        /// <summary>
        /// Get number of holidays/weekend days between two dates.
        /// </summary>
        /// <param name="adtStartDate">From Date</param>
        /// <param name="adtEndDate">To Date</param>
        /// <returns>No of non-working days</returns>
        public static int GetNoOfNonWorkingDays(DateTime adtStartDate, DateTime adtEndDate)
        {
            // Perform Swap
            if (adtStartDate > adtEndDate)
            {
                DateTime ldtTempDate = adtStartDate;
                adtStartDate = adtEndDate;
                adtEndDate = ldtTempDate;
            }

            // Get Total No of Days
            int lintTotalNoOfDays = (adtEndDate - adtStartDate).Days;
            int lintNoOfHolidays = 0;

            // Get Total No of Holidays falling between the days.            
            DataTable ldtbHolidays = busMPIPHPBase.SelectWithOperator<cdoHoliday>(
                new string[2] { enmHoliday.holiday_date.ToString(), enmHoliday.holiday_date.ToString() },
                new string[2] { ">=", "<=" },
                new object[2] { adtStartDate, adtEndDate }, enmHoliday.holiday_date.ToString());

            busMPIPHPBase lbusBase = new busMPIPHPBase();
            Collection<busHoliday> lclbHolidays = lbusBase.GetCollection<busHoliday>(ldtbHolidays, "icdoHoliday");

            int lintNoOfSaturdays = CountOfDayInRange(DayOfWeek.Saturday, adtStartDate, adtEndDate);
            int lintNoOfSundays = CountOfDayInRange(DayOfWeek.Sunday, adtStartDate, adtEndDate);
            lintNoOfHolidays = lclbHolidays.Count + lintNoOfSaturdays + lintNoOfSundays;

            return lintNoOfHolidays;
        }

        /// <summary>
        /// Count the no. of given day within a date range
        /// </summary>
        /// <param name="aenmDay">Day of Week</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>No of Days</returns>
        public static int CountOfDayInRange(DayOfWeek aenmDay, DateTime adtStartDate, DateTime adtEndDate)
        {
            TimeSpan ltsSpan = adtEndDate - adtStartDate;
            int lintWeeks = (int)Math.Floor(ltsSpan.TotalDays / 7);

            // Set the no of days as no of weeks.
            int lintNoOfDays = lintWeeks;

            // Account for Remainders
            int lintRemainingDays = (int)(ltsSpan.TotalDays % 7);
            int lintAfterLastDay = (int)(adtEndDate.DayOfWeek - aenmDay);

            if (lintAfterLastDay < 0)
                lintAfterLastDay += 7;

            if (lintRemainingDays >= lintAfterLastDay)
                lintNoOfDays++;

            return lintNoOfDays;
        }

        /// <summary>
        /// Check if the given date is a work date
        /// </summary>
        /// <param name="adtGivenDate">Given Date</param>
        /// <returns>True if work day; False if not</returns>
        public static bool IsDateAWeekWorkDay(DateTime adtGivenDate)
        {
            return !(IsWeekend(adtGivenDate) == true || IsHoliday(adtGivenDate)==true);
        }
            
        public static bool IsHoliday(DateTime adtGivenDate)
        {
            // check if the given date is a holiday
            return (busMPIPHPBase.SelectCount<cdoHoliday>(
                new string[1] { enmHoliday.holiday_date.ToString() },
                new object[1] { adtGivenDate }, null) > 0);
            
        }
        
        public static bool IsWeekend(DateTime adtGivenDate)
        {
            // Initialize to true; Means all days are work days.
            bool lblnWeekend = false;

            // return non work day if saturday or sunday
            if (adtGivenDate.DayOfWeek == DayOfWeek.Saturday || adtGivenDate.DayOfWeek == DayOfWeek.Sunday)
                lblnWeekend = true;
            return lblnWeekend;
        }

        #endregion

        #region [Validation Methods]

        /// <summary>
        /// Check if the Holiday has already been added.
        /// </summary>
        /// <returns>True if already added; False if not.</returns>
        public bool IsHolidayAlreadyAdded()
        {
            DataTable ldtbHolidays = SelectWithOperator<cdoHoliday>(
                new string[2] { enmHoliday.holiday_date.ToString(), enmHoliday.holiday_id.ToString() },
                new string[2] { "=", "<>" },
                new object[2] { icdoHoliday.holiday_date, icdoHoliday.holiday_id }, null);

            return (ldtbHolidays != null && ldtbHolidays.Rows.Count > 0);
        }

        #endregion
    }
}
