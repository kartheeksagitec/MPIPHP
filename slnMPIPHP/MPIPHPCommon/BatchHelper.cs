using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPIPHP.Common
{
    public abstract class JobServiceCodes
    {

       
    }
    public class BatchHelper
    {
        public delegate bool dlgBoolIntDelegate(int aint);
        #region Public Helper Methods
        public static int DateToInt32(DateTime adtCurrent)
        {
            int rv;
            StringBuilder lstrbString = new StringBuilder();

            lstrbString.Append(adtCurrent.Year.ToString("D4"));
            lstrbString.Append(adtCurrent.Month.ToString("D2"));
            lstrbString.Append(adtCurrent.Day.ToString("D2"));

            rv = Convert.ToInt32(lstrbString.ToString());

            return rv;
        }

        public static DateTime Int32ToDate(int current)
        {
            DateTime rv;

            int year;
            int month;
            int day;
            int remainder;

            year = Math.DivRem(current, 10000, out remainder);
            month = Math.DivRem(remainder, 100, out remainder);
            day = Math.DivRem(remainder, 1, out remainder);


            rv = new DateTime(year, month, day);

            return rv;
        }

        public static TimeSpan Int32ToTime(int current)
        {
            TimeSpan ltsTimeSpan;

            int hours;
            int minutes;
            int seconds;
            int remainder;

            hours = Math.DivRem(current, 10000, out remainder);
            minutes = Math.DivRem(remainder, 100, out remainder);
            seconds = Math.DivRem(remainder, 1, out remainder);


            ltsTimeSpan = new TimeSpan(hours, minutes, seconds);

            return ltsTimeSpan;
        }

        public static int TimeToInt32(DateTime adtCurrent)
        {
            int rv;
            StringBuilder lstrbString = new StringBuilder();

            lstrbString.Append(adtCurrent.TimeOfDay.Hours.ToString("D2"));
            lstrbString.Append(adtCurrent.TimeOfDay.Minutes.ToString("D2"));
            lstrbString.Append(adtCurrent.TimeOfDay.Seconds.ToString("D2"));

            rv = Convert.ToInt32(lstrbString.ToString());

            return rv;
        }

        public static bool IsDateBetween(int current, int start, int end)
        {
            return IsBetween(current, start, end);
        }

        public static bool IsTimeBetween(int current, int start, int end)
        {
            return IsBetween(current, start, end);
        }

        //Sid Jain 02222013
        public static bool IsDateBetween(DateTime current, DateTime start, DateTime end)
        {
            if (end == DateTime.MinValue)
                end = DateTime.MaxValue;

            return ((current >= start) && (current <= end));

        }

        public static bool IsBetween(int current, int start, int end)
        {
            bool rv = false;

            rv = ((current >= start) && (current <= end));

            return rv;
        }

        /// <summary>
        /// Get Service in Year and Months
        /// </summary>
        /// <param name="adecService">Service in Decimal Notation</param>
        /// <returns>Service in Year(s) and Month(s)</returns>
        public static string ServiceInYearsAndMonths(decimal adecService)
        {
            string lstrServiceInYearsAndMonths = string.Empty;

            int lintYears = (int)Math.Round(adecService / 12.00m);
            int lintMonths = (int)Math.Round(adecService % 12.00m);

            if (lintYears > 0 && lintMonths > 0)
                lstrServiceInYearsAndMonths = string.Format("{0} year(s) and {1} month(s)", lintYears, lintMonths);
            else if (lintYears > 0)
                lstrServiceInYearsAndMonths = string.Format("{0} year(s)", lintYears);
            else
                lstrServiceInYearsAndMonths = string.Format("{0} month(s)", lintMonths);

            return lstrServiceInYearsAndMonths;
        }

        #endregion

        #region constants for JobService
        // Prem : C# doesn't support datetime constants, so creating the max date as string value, 
        // it would be cast as datetime when needed
        //public const string MAX_DATE = "9999/12/31";
        public const string MAX_DATE = "";
        public const string JOB_HEADER_STATUS_REVIEW = "REVW";
        public const string JOB_HEADER_STATUS_VALID = "VALD";
        public const string JOB_HEADER_STATUS_SUBMIT_FOR_APPROVAL = "SFAP";
        public const string CODE_VALUE_BATCH_EXECUTION_WINDOW = "BEWN";
        public const int CODE_ID_BATCH_EXECUTION_WINDOW = 52;
        public const string JOB_HEADER_STATUS_PICKED_UP = "PICK";
        public const string JOB_HEADER_STATUS_QUEUED = "QUED";
        public const string JOB_HEADER_STATUS_PROCESSING = "PRCS";
        public const string JOB_HEADER_STATUS_PROCESSED_SUCCESSFULLY = "PRCD";
        public const string JOB_HEADER_STATUS_PROCESSED_WITH_ERRORS = "PRCE";
        public const string JOB_HEADER_STATUS_CANCEL_REQUESTED = "CNRQ";
        public const string JOB_HEADER_STATUS_CANCELLED = "CANC";

        public const string JOB_SCHEDULE_HEADER_STATUS_REVIEW = "REVW";
        public const string JOB_SCHEDULE_HEADER_STATUS_VALID = "VALD";
        public const string JOB_SCHEDULE_HEADER_STATUS_SUBMIT_FOR_APPROVAL = "SFAP";
        public const string JOB_SCHEDULE_HEADER_STATUS_APPROVED = "APPR";

        public const string JOB_SCHEDULE_DETAIL_STATUS_REVIEW = "REVW";
        public const string JOB_SCHEDULE_DETAIL_STATUS_VALID = "VALD";

        public const string JOB_DETAIL_STATUS_REVIEW = "REVW";
        public const string JOB_DETAIL_STATUS_VALID = "VALD";

        public const string JOB_DETAIL_STATUS_QUEUED = "QUED";
        public const string JOB_DETAIL_STATUS_PROCESSING = "PRCS";
        public const string JOB_DETAIL_STATUS_PROCESSED_SUCCESSFULLY = "PRSU";
        public const string JOB_DETAIL_STATUS_PROCESSED_WITH_ERRORS = "PRER";
        public const string JOB_DETAIL_STATUS_SKIPPED = "SKIP";
        public const string JOB_DETAIL_STATUS_CANCELLED = "CANC";
        public const string JOB_DETAIL_STATUS_CANCEL_REQUESTED = "CNRQ";

        public const int CODE_ID_FOR_JOB_HEADER_STATUS = 6018;
        public const int CODE_ID_FOR_JOB_DETAIL_STATUS = 6020;
        public const int CODE_ID_FOR_JOB_SCHEDULE_FREQUENCY_TYPE = 6017;
        public const int CODE_ID_FOR_JOB_SCHEDULE_STATUS = 6040;
        public const int CODE_ID_FOR_JOB_SCHEDULE_DETAIL_STATUS = 6018;

        public const string JOB_SCHEDULE_END_DATE_PRESENT = "ENDDATEPOPULATED";
        public const string JOB_SCHEDULE_NO_END_DATE_PRESENT = "ENDDATENOTPOPULATED";

        public const string JOB_SCHEDULE_SUBDAY_FREQUENCY_ONCE = "ONCE";
        public const string JOB_SCHEDULE_SUBDAY_FREQUENCY_EVERY = "EVERY";
        public const string JOB_SCHEDULE_SUBDAY_FREQUENCY_BATCH_WINDOW = "BATCHWINDOW";

        public const int JOB_SCHEDULE_FREQUENCY_TYPE_DAILY = 4;
        public const int JOB_SCHEDULE_FREQUENCY_TYPE_WEEKLY = 8;
        public const int JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY = 16;
        public const int JOB_SCHEDULE_FREQUENCY_TYPE_MONTHLY_RELATIVE = 32;
        public const int JOB_SCHEDULE_FREQUENCY_TYPE_IMMEDIATE = 64;

        public const int JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_MINUTES = 4;
        public const int JOB_SCHEDULE_FREQUENCY_SUBDAY_TYPE_IN_HOURS = 8;
        public const decimal CAP_FOR_JS_BASIC_BENEFIT_MULTIPLIER = 1.0m;
        public const decimal DIFFERENCE_IN_AGE_CHECK = 9.0m;
        public const string COMMA = ",";
        public const string SPACE = " ";
        public const int NUMBER_OF_DAYS_IN_MONTH = 30;
        public const int NUMBER_OF_MONTHS_IN_YEAR = 12;
        public const int NUMBER_OF_DAYS_IN_YEAR = 365;
        public const int NUMBER_OF_DAYS_IN_LEAP_YEAR = 366;
        public const string NUMBER_OF_DAYS_DIFFERENCE_FOR_MONTHLY_REPORTING_FREQUENCY = "DDMN";
        public const string NUMBER_OF_DAYS_DIFFERENCE_FOR_SEMIMONTHLY_REPORTING_FREQUENCY = "DDSM";
        public const string NUMBER_OF_DAYS_DIFFERENCE_FOR_BIWEEKLY_REPORTING_FREQUENCY = "DDBW";
        public const string REPORTING_FREQUENCY_BIWEEKLY = "BIWK";
        public const string REPORTING_FREQUENCY_SEMI_MONTHLY = "SEMI";
        public const string REPORTING_FREQUENCY_MONTHLY = "MNTH";
        public const int CODE_ID_SYSTEM_CONSTANTS = 52;
        // Constants used by Batch Process (i.e MPIPHPBatch)
        public const string BATCH_USER = "MPIPHP BATCH";
        public const string BATCH_MESSAGE_SUMMARY = "SUMM";
        public const string BATCH_MESSAGE_ERROR = "ERR";
        public const string BATCH_MESSAGE_INFO = "INFO";

        #endregion

    }
}
