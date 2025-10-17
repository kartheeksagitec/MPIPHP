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
	/// Class MPIPHP.DataObjects.doSecurity:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSecurity : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSecurity() : base()
         {
         }
         public int role_id { get; set; }
         public int resource_id { get; set; }
         public int security_id { get; set; }
         public string security_description { get; set; }
         public int security_value { get; set; }
    }
    [Serializable]
    public enum enmSecurity
    {
         role_id ,
         resource_id ,
         security_id ,
         security_description ,
         security_value ,
    }
}

