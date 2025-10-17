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
	/// Class MPIPHP.DataObjects.doPersonT79hours:
	/// Inherited from doNeoTrackBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonT79hours : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonT79hours() : base()
         {
         }
         public int person_t97_id { get; set; }
         public int person_account_id { get; set; }
         public Decimal t79_hours { get; set; }
         public string approved_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonT79hours
    {
         person_t97_id ,
         person_account_id ,
         t79_hours ,
         approved_flag ,
    }
}

