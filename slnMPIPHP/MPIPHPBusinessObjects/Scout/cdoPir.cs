#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;
using MPIPHP.BusinessObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPir:
	/// Inherited from doPir, the class is used to customize the database object doPir.
	/// </summary>
    [Serializable]
    public class cdoPir : doPir
    {
        public cdoPir()
            : base()
        {

        }
        public string istrPriorityDate { get; set; }
        public string additional_notes { get; set; }


        public override int Update()
        {
            InsertToPirHistory();
            if (base.status_value == "RTST")// "FIXD" modified by "RTST"
            {
                base.date_resolved = DateTime.Now;
            }
            int lintResult = base.Update();

            return lintResult;
        }

        public override int Insert()
        {
            int lintResult = base.Insert();
            InsertToPirHistory();
            return lintResult;
        }

        public void InsertToPirHistory()
        {
            //Create a cdoPirHistory and keep it ready 
            //Insert into history
            cdoPirHistory lcdoPirHistory = new cdoPirHistory();
            lcdoPirHistory.pir_id = pir_id;

            if (additional_notes != null && this.additional_notes.Length > 2000)
            {
                lcdoPirHistory.long_description = additional_notes.Substring(0, 2000);
            }
            else
            {
                lcdoPirHistory.long_description = additional_notes;
            }
            //additional_notes = "";
            lcdoPirHistory.assigned_to_id = assigned_to_id;
            lcdoPirHistory.status_id = busConstant.PIR_STATUS_ID;
            lcdoPirHistory.status_value = status_value;
            lcdoPirHistory.created_by = created_by;
            lcdoPirHistory.modified_by = iobjPassInfo.istrUserID;
            lcdoPirHistory.created_date = created_date;
            lcdoPirHistory.modified_date = DateTime.Now;
            lcdoPirHistory.Insert();
        }
    }
} 
