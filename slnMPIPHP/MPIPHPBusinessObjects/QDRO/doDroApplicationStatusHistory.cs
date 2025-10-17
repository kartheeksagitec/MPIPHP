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
	/// Class MPIPHP.DataObjects.doDroApplicationStatusHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDroApplicationStatusHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDroApplicationStatusHistory() : base()
         {
         }
         public int dro_application_status_history_id { get; set; }
         public int dro_application_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime status_date { get; set; }
    }
    [Serializable]
    public enum enmDroApplicationStatusHistory
    {
         dro_application_status_history_id ,
         dro_application_id ,
         status_id ,
         status_description ,
         status_value ,
         status_date ,
    }
}

