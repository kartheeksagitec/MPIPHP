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
using NeoSpin.DataObjects;

#endregion

namespace MPIPHP.BusinessObjects
{
	/// <summary>
	/// Class MPIPHP.BusinessObjects.busUserActivityLog:
	/// Inherited from busUserActivityLogGen, the class is used to customize the business object busUserActivityLogGen.
	/// </summary>
	[Serializable]
	public class busUserActivityLog : busUserActivityLogGen
	{
        public bool updateLogoffTime(string session_id)
        {
            busUserActivityLog lobjbusUserActivityLog = new busUserActivityLog();

            DataTable ldtbList = Select<cdoUserActivityLog>(
               new string[1] { enmUserActivityLog.session_id.ToString() },
               new object[1] { session_id }, null, null);

            if (ldtbList != null && ldtbList.Rows != null && ldtbList.Rows.Count > 0)
            {
                lobjbusUserActivityLog.icdoUserActivityLog = new cdoUserActivityLog();
                lobjbusUserActivityLog.icdoUserActivityLog.LoadData(ldtbList.Rows[0]);

                lobjbusUserActivityLog.icdoUserActivityLog.logoff_time = DateTime.Now;
                lobjbusUserActivityLog.icdoUserActivityLog.Update();
                return true;
            }

            return false;
        }

        public override bool FindUserActivityLog(int aintUserActivityLogId)
        {
            bool lblnReult= base.FindUserActivityLog(aintUserActivityLogId);

            icdoUserActivityLog.activity_count = SelectCount<cdoUserActivityLogDetail>(new string[1] { enmUserActivityLogDetail.user_activity_log_id.ToString() },
                new object[1] { icdoUserActivityLog.user_activity_log_id }, null);

            return lblnReult;
        }
	}
}
