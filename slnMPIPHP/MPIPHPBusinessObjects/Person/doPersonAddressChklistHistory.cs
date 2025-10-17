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
	/// Class MPIPHP.DataObjects.doPersonAddressChklistHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAddressChklistHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAddressChklistHistory() : base()
         {
         }
         public int address_chklist_history_id { get; set; }
         public int person_address_history_id { get; set; }
         public int address_type_id { get; set; }
         public string address_type_description { get; set; }
         public string address_type_value { get; set; }
    }
    [Serializable]
    public enum enmPersonAddressChklistHistory
    {
         address_chklist_history_id ,
         person_address_history_id ,
         address_type_id ,
         address_type_description ,
         address_type_value ,
    }
}

