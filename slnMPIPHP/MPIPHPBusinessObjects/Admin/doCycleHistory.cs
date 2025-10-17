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
	/// Class MPIPHP.DataObjects.doCycleHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCycleHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCycleHistory() : base()
         {
         }
         public int cycle_no { get; set; }
         public DateTime start_time { get; set; }
         public DateTime end_time { get; set; }
    }
    [Serializable]
    public enum enmCycleHistory
    {
         cycle_no ,
         start_time ,
         end_time ,
    }
}

