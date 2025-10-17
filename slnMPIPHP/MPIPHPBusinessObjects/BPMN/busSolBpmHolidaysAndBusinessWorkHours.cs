using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoBase.Common;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using BPM = Sagitec.Common;
using Sagitec.Bpm;

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busSolBpmHolidaysAndBusinessWorkHours:
    /// Inherited from busBpmHolidaysAndBusinessWorkHours, For GetHolidays,GetDayStartTime,GetDayEndTime 
    /// </summary>
    [Serializable]
    public class busSolBpmHolidaysAndBusinessWorkHours : busBpmHolidaysAndBusinessWorkHours
    {
        #region Properties

        /// <summary>
        /// BusinessDayEndTime
        /// </summary>
        public override TimeSpan BusinessDayEndTime { get { return base.BusinessDayEndTime; } }
        
        /// <summary>
        /// BusinessDayStartTime
        /// </summary>
        public override TimeSpan BusinessDayStartTime { get { return base.BusinessDayStartTime; } }

        #endregion

        #region Override Methode

        /// <summary>
        /// GetHolidaysBetweenDates
        /// </summary>
        public override Collection<DateTime> GetHolidaysBetweenDates(DateTime adtFromDateTime, DateTime adtToDateTime)
        { return null; }

        /// <summary>
        /// GetHolidaysFromDate
        /// </summary>
        public override Collection<DateTime> GetHolidaysFromDate(DateTime adtFromDateTime)
        {
            DataTable ldtbHolidayList = DBFunction.DBSelect("entHoliday.GetHolidayDateList", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            Collection<DateTime> iclbHolidayList = new Collection<DateTime>();
            foreach (DataRow ldtrHoliday in ldtbHolidayList.Rows)
            {
                iclbHolidayList.Add(Convert.ToDateTime(ldtrHoliday[0]));
            }

            return iclbHolidayList;
        }

        /// <summary>
        ///GetHolidaysUptoDate
        /// </summary>
        public override Collection<DateTime> GetHolidaysUptoDate(DateTime adtToDateTime)
        { return null; }

        #endregion

    }
}
