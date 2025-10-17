#region Using directives

using System;
using System.Data;
using Sagitec.BusinessObjects;
using MPIPHP.DataObjects;
using Sagitec.Common;
using Sagitec.ExceptionPub;
#endregion

namespace MPIPHP.CustomDataObjects
{
    /// <summary>
    /// Class MPIPHP.CustomDataObjects.cdoActivityInstance:
    /// Inherited from doActivityInstance, the class is used to customize the database object doActivityInstance.
    /// </summary>
    [Serializable]
    public class cdoActivityInstance : doActivityInstance
    {
        public cdoActivityInstance()
            : base()
        {
        }

        public busBase busObject { get; set; }

        public string UserId { get; set; }
        
        //PROPERTIES FOR PERSON OVERVIEW EXCLUSIVELY
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
        
        //PROPERTIES FOR PERSON OVERVIEW EXCLUSIVELY

        public int UserSerialId { get; set; }
        public bool iblnNeedHistory { get; set; }

        public string istrRelativeVipFlag { get; set; }

        public string istrProcessName { get; set; }
        public string istrActivityName { get; set; }

        public override int Update()
        {            

            if (!iblnNeedHistory)
            {
                if (ihstOldValues.Count > 0 && ihstOldValues["status_value"] as string != status_value)
                    iblnNeedHistory = true;
            }

            int lintResult = base.Update();
            if (iblnNeedHistory)
            {
                UpdateActivityInstanceHistory();
                iblnNeedHistory = false;
            }
            return lintResult;
        }

        public override int Insert()
        {
            int lintResult = base.Insert();
            //Inserting the New Activity Instance History
            cdoActivityInstanceHistory newActivityInstanceHistory = new cdoActivityInstanceHistory();
            newActivityInstanceHistory.activity_instance_id = activity_instance_id;
            newActivityInstanceHistory.start_time = DateTime.Now;
            newActivityInstanceHistory.status_value = status_value;
            newActivityInstanceHistory.action_user_id = iobjPassInfo.istrUserID;
            newActivityInstanceHistory.Insert();
            return lintResult;
        }

        private void UpdateActivityInstanceHistory()
        {
            //Updating the End Time of Previous History if exists
            DataTable ldtbLastActivityHistory = busBase.Select("cdoActivityInstance.GetMaxActivityInstanceHistoryByActivityInstance",
                                                               new object[1] { activity_instance_id });
            if (ldtbLastActivityHistory.Rows.Count > 0)
            {
                cdoActivityInstanceHistory lobjcdoActivityInstanceHistory = new cdoActivityInstanceHistory();
                lobjcdoActivityInstanceHistory.LoadData(ldtbLastActivityHistory.Rows[0]);
                lobjcdoActivityInstanceHistory.end_time = DateTime.Now;
                lobjcdoActivityInstanceHistory.Update();
            }

            //Inserting the New Activity Instance History
            cdoActivityInstanceHistory newActivityInstanceHistory = new cdoActivityInstanceHistory();
            newActivityInstanceHistory.activity_instance_id = activity_instance_id;
            newActivityInstanceHistory.start_time = DateTime.Now;
            if ((status_value == "PROC") || (status_value == "RETU") || (status_value == "REAU") || (status_value == "CANC"))
                newActivityInstanceHistory.end_time = DateTime.Now;
            newActivityInstanceHistory.status_value = status_value;
            newActivityInstanceHistory.action_user_id = iobjPassInfo.istrUserID;
            newActivityInstanceHistory.Insert();
        }
    }
}
