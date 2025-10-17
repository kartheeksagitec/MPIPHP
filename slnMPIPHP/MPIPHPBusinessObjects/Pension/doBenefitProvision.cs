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
	/// Class MPIPHP.DataObjects.doBenefitProvision:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvision : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitProvision() : base()
         {
         }
         public int benefit_provision_id { get; set; }
         public string short_decription { get; set; }
         public string description { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvision
    {
         benefit_provision_id ,
         short_decription ,
         description ,
    }
}

