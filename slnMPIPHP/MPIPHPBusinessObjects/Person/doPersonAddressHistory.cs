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
	/// Class MPIPHP.DataObjects.doPersonAddressHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAddressHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAddressHistory() : base()
         {
         }
         public int person_address_history_id { get; set; }
         public int address_id { get; set; }
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
         public string foreign_province { get; set; }
         public string foreign_postal_code { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string secured_flag { get; set; }
         public string addr_source_desc { get; set; }
         public string bad_address_flag { get; set; } 
    }
    [Serializable]
    public enum enmPersonAddressHistory
    {
         person_address_history_id ,
         address_id ,
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
         foreign_province ,
         foreign_postal_code ,
         start_date ,
         end_date ,
         secured_flag ,
         addr_source_desc ,
         bad_address_flag ,
    }
}

