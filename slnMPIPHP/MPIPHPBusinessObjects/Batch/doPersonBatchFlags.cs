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
	/// Class MPIPHP.DataObjects.doPersonBatchFlags:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonBatchFlags : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonBatchFlags() : base()
         {
         }
         public int person_batch_flag_id { get; set; }
         public int person_id { get; set; }
         public string pre_notification_bis_flag { get; set; }
         public string bis_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonBatchFlags
    {
         person_batch_flag_id ,
         person_id ,
         pre_notification_bis_flag ,
         bis_flag ,
    }
}

