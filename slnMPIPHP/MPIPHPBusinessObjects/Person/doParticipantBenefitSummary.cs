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
	/// Class MPIPHP.DataObjects.doParticipantBenefitSummary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doParticipantBenefitSummary : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doParticipantBenefitSummary() : base()
         {
         }
         public int participant_benefit_summary_id { get; set; }
         public int person_id { get; set; }
         public string mpi_person_id { get; set; }
         public int plan_id { get; set; }
         public string plan_name { get; set; }
         public string plan_status { get; set; }
         public Decimal pension_hours { get; set; }
         public int qualified_years { get; set; }
         public string pension_credit { get; set; }
         public DateTime vested_date { get; set; }
         public string health_hours { get; set; }
         public string health_years { get; set; }
         public string monthly_benefit { get; set; }
         public string iap_balance { get; set; }
         public string allocation_as_of_yr_end { get; set; }
         public int update_benefit { get; set; }
    }
    [Serializable]
    public enum enmParticipantBenefitSummary
    {
         participant_benefit_summary_id ,
         person_id ,
         mpi_person_id ,
         plan_id ,
         plan_name ,
         plan_status ,
         pension_hours ,
         qualified_years ,
         pension_credit ,
         vested_date ,
         health_hours ,
         health_years ,
         monthly_benefit ,
         iap_balance ,
         allocation_as_of_yr_end ,
         update_benefit ,
    }
}

