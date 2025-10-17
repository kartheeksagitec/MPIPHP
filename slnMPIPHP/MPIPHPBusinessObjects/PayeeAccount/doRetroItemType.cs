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
	/// Class MPIPHP.DataObjects.doRetroItemType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRetroItemType : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRetroItemType() : base()
         {
         }
         public int retro_item_type_id { get; set; }
         public string from_item_type { get; set; }
         public string to_item_type { get; set; }
         public int retro_payment_type_id { get; set; }
         public string retro_payment_type_description { get; set; }
         public string retro_payment_type_value { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
    }
    [Serializable]
    public enum enmRetroItemType
    {
         retro_item_type_id ,
         from_item_type ,
         to_item_type ,
         retro_payment_type_id ,
         retro_payment_type_description ,
         retro_payment_type_value ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
    }
}

