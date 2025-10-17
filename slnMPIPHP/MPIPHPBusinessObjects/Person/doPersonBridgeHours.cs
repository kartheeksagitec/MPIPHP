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
	/// Class MPIPHP.DataObjects.doPersonBridgeHours:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonBridgeHours : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonBridgeHours() : base()
         {
         }
         public int person_bridge_id { get; set; }
         public int person_id { get; set; }
         public int bridge_type_id { get; set; }
         public string bridge_type_description { get; set; }
         public string bridge_type_value { get; set; }
         public Decimal hours_reported { get; set; }
         public DateTime bridge_start_date { get; set; }
         public DateTime bridge_end_date { get; set; }
    }
    [Serializable]
    public enum enmPersonBridgeHours
    {
         person_bridge_id ,
         person_id ,
         bridge_type_id ,
         bridge_type_description ,
         bridge_type_value ,
         hours_reported ,
         bridge_start_date ,
         bridge_end_date ,
    }
}

