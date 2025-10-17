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
	/// Class MPIPHP.DataObjects.doPersonBridgeHoursDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonBridgeHoursDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonBridgeHoursDetail() : base()
         {
         }
         public int person_bridge_hours_detail_id { get; set; }
         public int person_bridge_id { get; set; }
         public int computation_year { get; set; }
         public Decimal hours { get; set; }
         public DateTime from_date { get; set; }
         public DateTime to_date { get; set; }
    }
    [Serializable]
    public enum enmPersonBridgeHoursDetail
    {
         person_bridge_hours_detail_id ,
         person_bridge_id ,
         computation_year ,
         hours ,
         from_date ,
         to_date ,
    }
}

