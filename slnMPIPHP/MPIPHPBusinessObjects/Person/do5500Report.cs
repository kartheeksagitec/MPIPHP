#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class MPIPHP.DataObjects.do5500Report:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class do5500Report : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public do5500Report() : base()
         {
         }
         public int report_5500_id { get; set; }
         public int plan_id { get; set; }
         public int computation_year { get; set; }
         public int total_count { get; set; }
         public int active_participants_6a1_count { get; set; }
         public int active_participants_6a2_count { get; set; }
         public int retired_seperated_ptp_count { get; set; }
         public int other_retired_seperated_ptp_count { get; set; }
         public int deceased_ptp_count { get; set; }
         public int employers_count { get; set; }
    }
    [Serializable]
    public enum enm5500Report
    {
         report_5500_id ,
         plan_id ,
         computation_year ,
         total_count ,
         active_participants_6a1_count ,
         active_participants_6a2_count ,
         retired_seperated_ptp_count ,
         other_retired_seperated_ptp_count ,
         deceased_ptp_count ,
         employers_count ,
    }
}

