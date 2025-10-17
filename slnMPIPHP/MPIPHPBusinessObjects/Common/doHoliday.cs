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
	/// Class MPIPHP.DataObjects.doHoliday:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doHoliday : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doHoliday() : base()
         {
         }
         public int holiday_id { get; set; }
         public string description { get; set; }
         public DateTime holiday_date { get; set; }
    }
    [Serializable]
    public enum enmHoliday
    {
         holiday_id ,
         description ,
         holiday_date ,
    }
}

