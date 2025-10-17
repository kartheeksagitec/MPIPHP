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
	/// Class MPIPHP.DataObjects.doQdroCalculationHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroCalculationHeader : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroCalculationHeader() : base()
         {
         }
         public int qdro_calculation_header_id { get; set; }
         public int qdro_application_id { get; set; }
         public int person_id { get; set; }
         public int alternate_payee_id { get; set; }
         public string alternate_payee_name { get; set; }
         public DateTime alternate_payee_date_of_birth { get; set; }
         public int calculation_type_id { get; set; }
         public string calculation_type_description { get; set; }
         public string calculation_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime retirement_date { get; set; }
         public Decimal age { get; set; }
         public DateTime qdro_commencement_date { get; set; }
         public DateTime date_of_marriage { get; set; }
         public DateTime date_of_seperation { get; set; }
         public string is_alt_payee_eligible_for_iap { get; set; }
         public string is_participant_disabled { get; set; }
         public DateTime benefit_comencement_date { get; set; }
         public string is_participant_dead_flag { get; set; }
         public string is_final_estimate { get; set; }
         public string mss_flag { get; set; }
    }
    [Serializable]
    public enum enmQdroCalculationHeader
    {
         qdro_calculation_header_id ,
         qdro_application_id ,
         person_id ,
         alternate_payee_id ,
         alternate_payee_name ,
         alternate_payee_date_of_birth ,
         calculation_type_id ,
         calculation_type_description ,
         calculation_type_value ,
         status_id ,
         status_description ,
         status_value ,
         retirement_date ,
         age ,
         qdro_commencement_date ,
         date_of_marriage ,
         date_of_seperation ,
         is_alt_payee_eligible_for_iap ,
         is_participant_disabled ,
         benefit_comencement_date ,
         is_participant_dead_flag ,
         is_final_estimate ,
         mss_flag ,
    }
}

