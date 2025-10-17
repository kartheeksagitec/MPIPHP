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
	/// Class MPIPHP.DataObjects.doBenefitInterestRate:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitInterestRate : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitInterestRate() : base()
         {
         }
         public int benefit_interest_rate_id { get; set; }
         public int year { get; set; }
         public Decimal rate_of_interest { get; set; }
    }
    [Serializable]
    public enum enmBenefitInterestRate
    {
         benefit_interest_rate_id ,
         year ,
         rate_of_interest ,
    }
}

