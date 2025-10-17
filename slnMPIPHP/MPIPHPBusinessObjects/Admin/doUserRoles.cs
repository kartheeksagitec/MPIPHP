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
	/// Class MPIPHP.DataObjects.doUserRoles:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserRoles : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doUserRoles() : base()
         {
         }
         public int user_serial_id { get; set; }
         public int role_id { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
    }
    [Serializable]
    public enum enmUserRoles
    {
         user_serial_id ,
         role_id ,
         effective_start_date ,
         effective_end_date ,
    }
}

