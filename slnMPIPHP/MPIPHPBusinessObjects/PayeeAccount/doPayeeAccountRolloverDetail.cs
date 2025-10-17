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
	/// Class MPIPHP.DataObjects.doPayeeAccountRolloverDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRolloverDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountRolloverDetail() : base()
         {
         }
         public int payee_account_rollover_detail_id { get; set; }
         public int payee_account_id { get; set; }
         public int rollover_org_id { get; set; }
         public int rollover_option_id { get; set; }
         public string rollover_option_description { get; set; }
         public string rollover_option_value { get; set; }
         public Decimal percent_of_taxable { get; set; }
         public Decimal amount { get; set; }
         public string account_number { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int rollover_type_id { get; set; }
         public string rollover_type_description { get; set; }
         public string rollover_type_value { get; set; }
         public int old_rollover_dtl_id { get; set; }
         public string org_address { get; set; }
         public string contact_name { get; set; }
         public string send_to_participant { get; set; }
         public string participant_pickup { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string city { get; set; }
         public int state_id { get; set; }
         public string state_description { get; set; }
         public string state_value { get; set; }
         public int country_id { get; set; }
         public string country_description { get; set; }
         public string country_value { get; set; }
         public string zip_code { get; set; }
         public string zip_4_code { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRolloverDetail
    {
         payee_account_rollover_detail_id ,
         payee_account_id ,
         rollover_org_id ,
         rollover_option_id ,
         rollover_option_description ,
         rollover_option_value ,
         percent_of_taxable ,
         amount ,
         account_number ,
         status_id ,
         status_description ,
         status_value ,
         rollover_type_id ,
         rollover_type_description ,
         rollover_type_value ,
         old_rollover_dtl_id ,
         org_address ,
         contact_name ,
         send_to_participant ,
         participant_pickup ,
         addr_line_1 ,
         addr_line_2 ,
         city ,
         state_id ,
         state_description ,
         state_value ,
         country_id ,
         country_description ,
         country_value ,
         zip_code ,
         zip_4_code ,
    }
}

