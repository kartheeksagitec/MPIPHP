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
	/// Class MPIPHP.DataObjects.doPersonContact:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonContact : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonContact() : base()
         {
         }
         public int person_contact_id { get; set; }
         public int person_id { get; set; }
         public string contact_name { get; set; }
         public string contact_phone_no { get; set; }
         public string email_address { get; set; }
         public int contact_type_id { get; set; }
         public string contact_type_description { get; set; }
         public string contact_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public string addr_same_as_person { get; set; }
         public string addr_line_1 { get; set; }
         public string addr_line_2 { get; set; }
         public string addr_city { get; set; }
         public int addr_state_id { get; set; }
         public string addr_state_description { get; set; }
         public string addr_state_value { get; set; }
         public int addr_country_id { get; set; }
         public string addr_country_description { get; set; }
         public string addr_country_value { get; set; }
         public string addr_zip_code { get; set; }
         public string addr_zip_4_code { get; set; }
         public string foreign_addr_flag { get; set; }
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public string correspondence_addr_flag { get; set; }
         public string pension_contact_flag { get; set; }
         public string health_contact_flag { get; set; }
         public int attorney_for_id { get; set; }
         public string attorney_for_description { get; set; }
         public string attorney_for_value { get; set; }
         public string county { get; set; }
    }
    [Serializable]
    public enum enmPersonContact
    {
         person_contact_id ,
         person_id ,
         contact_name ,
         contact_phone_no ,
         email_address ,
         contact_type_id ,
         contact_type_description ,
         contact_type_value ,
         status_id ,
         status_description ,
         status_value ,
         effective_start_date ,
         effective_end_date ,
         addr_same_as_person ,
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
         foreign_addr_flag ,
         foreign_province ,
         foreign_postal_code ,
         correspondence_addr_flag ,
         pension_contact_flag ,
         health_contact_flag ,
         attorney_for_id ,
         attorney_for_description ,
         attorney_for_value ,
         county ,
    }
}

