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
	/// Class MPIPHP.DataObjects.doDroApplication:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDroApplication : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDroApplication() : base()
         {
         }
         public int dro_application_id { get; set; }
         public int person_id { get; set; }
         public int alternate_payee_id { get; set; }
         public string case_number { get; set; }
         public DateTime received_date { get; set; }
         public DateTime date_of_marriage { get; set; }
         public DateTime date_of_divorce { get; set; }
         public DateTime order_date { get; set; }
         public string joinder_on_file { get; set; }
         public DateTime joinder_recv_date { get; set; }
         public string qualified_by_user { get; set; }
         public DateTime qualified_date { get; set; }
         public string approved_by_user { get; set; }
         public DateTime approved_date { get; set; }
         public int dro_status_id { get; set; }
         public string dro_status_description { get; set; }
         public string dro_status_value { get; set; }
         public string notes { get; set; }
         public string is_ammended_flag { get; set; }
         public string batch_90day_flag { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public string cancellation_reason { get; set; }
         public DateTime dro_commencement_date { get; set; }
         public string is_participant_disabled_flag { get; set; }
         public string life_conversion_factor_flag { get; set; }
         public string waived_disability_entitlement_flag { get; set; }
         public string eligible_for_continuance_flag { get; set; }
         public string is_disability_conversion { get; set; }
    }
    [Serializable]
    public enum enmDroApplication
    {
         dro_application_id ,
         person_id ,
         alternate_payee_id ,
         case_number ,
         received_date ,
         date_of_marriage ,
         date_of_divorce ,
         order_date ,
         joinder_on_file ,
         joinder_recv_date ,
         qualified_by_user ,
         qualified_date ,
         approved_by_user ,
         approved_date ,
         dro_status_id ,
         dro_status_description ,
         dro_status_value ,
         notes ,
         is_ammended_flag ,
         batch_90day_flag ,
         addr_state_id ,
         addr_state_description ,
         addr_state_value ,
         cancellation_reason ,
         dro_commencement_date ,
         is_participant_disabled_flag ,
         life_conversion_factor_flag ,
         waived_disability_entitlement_flag ,
         eligible_for_continuance_flag ,
         is_disability_conversion ,
    }
}

