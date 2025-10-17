using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPIPHP.BusinessObjects
{
    public class busReturnToWorkPayrollDetails
    {
        public string istrCurrentMonth { get; set; }
        public DateTime idtPayrollStartDate { get; set; }
        public DateTime idtPayrollEndDate { get; set; }
        public DateTime idtReferenceDate { get; set; }
        public string istrPayrollMonth { get; set; }
        public DateTime idtSuspensionDate { get; set; }
        public DateTime idtThresholdStartDate { get; set; }
        public DateTime idtThresholdEndDate { get; set; }

        public busReturnToWorkPayrollDetails GetPayrollPeriod(DateTime adtInputDate)
        {
            int lintYear = adtInputDate.Year;
            int lintMonth = adtInputDate.Month;

            DateTime ldtPayrollStart, ldtPayrollEnd;
            GetPayrollPeriodDates(lintYear, lintMonth, out ldtPayrollStart, out ldtPayrollEnd);

            if (adtInputDate < ldtPayrollStart)
            {
                GetPreviousMonth(ref lintYear, ref lintMonth);
                GetPayrollPeriodDates(lintYear, lintMonth, out ldtPayrollStart, out ldtPayrollEnd);
            }

            DateTime ldtThresholdStart, ldtThresholdEnd;
            GetThresholdWindow(lintYear, lintMonth, adtInputDate >= ldtPayrollStart, out ldtThresholdStart, out ldtThresholdEnd);

            return new busReturnToWorkPayrollDetails
            {
                istrCurrentMonth = new DateTime(lintYear, lintMonth, 1).ToString("MMMM"),
                idtPayrollStartDate = ldtPayrollStart,
                idtPayrollEndDate = ldtPayrollEnd,
                idtThresholdStartDate = ldtThresholdStart,
                idtThresholdEndDate = ldtThresholdEnd,
                idtReferenceDate = new DateTime(lintYear, lintMonth, 1),
                istrPayrollMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(ldtPayrollEnd.Month),
                idtSuspensionDate = ldtThresholdStart
            };
        }

        private void GetPayrollPeriodDates(int aYear, int aMonth, out DateTime adtPayrollStart, out DateTime adtPayrollEnd)
        {
            DateTime ldtLastThursday = GetLastThursdayOfMonth(aYear, aMonth);
            adtPayrollStart = ldtLastThursday.AddDays(-4);

            int lintNextMonth = aMonth == 12 ? 1 : aMonth + 1;
            int lintNextYear = aMonth == 12 ? aYear + 1 : aYear;
            DateTime ldtNextLastThursday = GetLastThursdayOfMonth(lintNextYear, lintNextMonth);
            adtPayrollEnd = ldtNextLastThursday.AddDays(-((int)ldtNextLastThursday.DayOfWeek + 1) % 7);
        }

        private void GetThresholdWindow(int aYear, int aMonth, bool ablnIsCurrent, out DateTime adtThresholdStart, out DateTime adtThresholdEnd)
        {
            int lintOffsetMonth = ablnIsCurrent ? aMonth + 1 : aMonth;
            int lintThresholdStartMonth = lintOffsetMonth <= 2 ? lintOffsetMonth + 10 : lintOffsetMonth - 2;
            int lintThresholdStartYear = lintOffsetMonth <= 2 ? aYear - 1 : aYear;

            int lintThresholdEndMonth = lintOffsetMonth == 1 ? 12 : lintOffsetMonth - 1;
            int lintThresholdEndYear = lintOffsetMonth == 1 ? aYear - 1 : aYear;

            adtThresholdStart = new DateTime(lintThresholdStartYear, lintThresholdStartMonth, 24);
            adtThresholdEnd = new DateTime(lintThresholdEndYear, lintThresholdEndMonth, 19);
        }

        private void GetPreviousMonth(ref int aYear, ref int aMonth)
        {
            if (aMonth == 1)
            {
                aMonth = 12;
                aYear -= 1;
            }
            else
            {
                aMonth -= 1;
            }
        }

        private DateTime GetLastThursdayOfMonth(int aYear, int aMonth)
        {
            DateTime ldtLastDay = new DateTime(aYear, aMonth, DateTime.DaysInMonth(aYear, aMonth));
            while (ldtLastDay.DayOfWeek != DayOfWeek.Thursday)
            {
                ldtLastDay = ldtLastDay.AddDays(-1);
            }
            return ldtLastDay;
        }
    }
}
