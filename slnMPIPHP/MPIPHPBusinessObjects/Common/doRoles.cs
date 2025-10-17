#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.BusinessObjects;
using Sagitec.DataObjects;
#endregion
namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doRoles:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRoles : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRoles() : base()
         {
         }
         public int role_id { get; set; }
         public string role_description { get; set; }
    }
    [Serializable]
    public enum enmRoles
    {
         role_id ,
         role_description ,
    }
}
