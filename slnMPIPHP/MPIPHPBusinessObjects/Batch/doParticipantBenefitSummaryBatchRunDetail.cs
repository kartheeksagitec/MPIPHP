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
	/// Class MPIPHP.DataObjects.doParticipantBenefitSummaryBatchRunDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doParticipantBenefitSummaryBatchRunDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doParticipantBenefitSummaryBatchRunDetail() : base()
         {
         }
         public int participant_benefit_summary_batch_run_detail_id { get; set; }
         public DateTime last_run_date { get; set; }
         public string batch_status_flag { get; set; }
         public string import_status_flag { get; set; }
    }
    [Serializable]
    public enum enmParticipantBenefitSummaryBatchRunDetail
    {
         participant_benefit_summary_batch_run_detail_id ,
         last_run_date ,
         batch_status_flag ,
         import_status_flag ,
    }
}

