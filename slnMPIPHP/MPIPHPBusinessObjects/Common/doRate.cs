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
	/// Class MPIPHP.DataObjects.doRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRate : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRate() : base()
         {
         }
         public int rate_id { get; set; }
         public int rate_type_id { get; set; }
         public string rate_type_description { get; set; }
         public string rate_type_value { get; set; }
         public string union_code { get; set; }
         public string employer_code { get; set; }
         public DateTime effective_date { get; set; }
    }
    [Serializable]
    public enum enmRate
    {
         rate_id ,
         rate_type_id ,
         rate_type_description ,
         rate_type_value ,
         union_code ,
         employer_code ,
         effective_date ,
    }
}

