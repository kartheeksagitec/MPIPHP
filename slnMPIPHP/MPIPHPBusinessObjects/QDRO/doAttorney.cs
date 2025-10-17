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
	/// Class MPIPHP.DataObjects.doAttorney:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doAttorney : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doAttorney() : base()
         {
         }
         public int attorney_id { get; set; }
         public string first_name { get; set; }
         public string last_name { get; set; }
         public string middle_name { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public int addr_zip_code { get; set; }
         public int addr_zip_4_code { get; set; }
         public string phone_nbr { get; set; }
         public string email_address { get; set; }
         public int addr_type_id { get; set; }
         public string addr_type_description { get; set; }
         public string addr_type_value { get; set; }
         public int contact_type_id { get; set; }
         public string contact_type_description { get; set; }
         public string contact_type_value { get; set; }
         public int attorney_type_id { get; set; }
         public string attorney_type_description { get; set; }
         public string attorney_type_value { get; set; }
         public DateTime date_eff_from { get; set; }
         public DateTime date_eff_to { get; set; }
         public int dro_application_id { get; set; }
    }
    [Serializable]
    public enum enmAttorney
    {
         attorney_id ,
         first_name ,
         last_name ,
         middle_name ,
         addr_line_1 ,
         addr_line_2 ,
         addr_city ,
         addr_state_id ,
         addr_state_description ,
         addr_state_value ,
         addr_country_id ,
         addr_country_description ,
         addr_country_value ,
         addr_zip_code ,
         addr_zip_4_code ,
         phone_nbr ,
         email_address ,
         addr_type_id ,
         addr_type_description ,
         addr_type_value ,
         contact_type_id ,
         contact_type_description ,
         contact_type_value ,
         attorney_type_id ,
         attorney_type_description ,
         attorney_type_value ,
         date_eff_from ,
         date_eff_to ,
         dro_application_id ,
    }
}

