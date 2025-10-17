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
	/// Class MPIPHP.DataObjects.doOrgBank:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgBank : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doOrgBank() : base()
         {
         }
         public int org_bank_id { get; set; }
         public int org_id { get; set; }
         public int bank_org_id { get; set; }
         public string account_no { get; set; }
         public int usage_id { get; set; }
         public string usage_description { get; set; }
         public string usage_value { get; set; }
         public int account_type_id { get; set; }
         public string account_type_description { get; set; }
         public string account_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmOrgBank
    {
         org_bank_id ,
         org_id ,
         bank_org_id ,
         account_no ,
         usage_id ,
         usage_description ,
         usage_value ,
         account_type_id ,
         account_type_description ,
         account_type_value ,
         status_id ,
         status_description ,
         status_value ,
    }
}

