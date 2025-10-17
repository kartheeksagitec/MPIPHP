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
	/// Class MPIPHP.DataObjects.doPayeeBenefitAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeBenefitAccount : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeBenefitAccount() : base()
         {
         }
         public int payee_benefit_account_id { get; set; }
         public Decimal starting_taxable_amount { get; set; }
         public Decimal starting_nontaxable_amount { get; set; }
         public int person_account_id { get; set; }
         public Decimal gross_amount { get; set; }
         public int person_id { get; set; }
         public int funds_type_id { get; set; }
         public string funds_type_description { get; set; }
         public string funds_type_value { get; set; }
    }
    [Serializable]
    public enum enmPayeeBenefitAccount
    {
         payee_benefit_account_id ,
         starting_taxable_amount ,
         starting_nontaxable_amount ,
         person_account_id ,
         gross_amount ,
         person_id ,
         funds_type_id ,
         funds_type_description ,
         funds_type_value ,
    }
}

