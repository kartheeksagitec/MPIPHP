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
	/// Class MPIPHP.DataObjects.doQdroCalculationOptions:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doQdroCalculationOptions : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doQdroCalculationOptions() : base()
         {
         }
         public int qdro_calculation_option_id { get; set; }
         public int qdro_calculation_detail_id { get; set; }
         public int plan_benefit_id { get; set; }
         public Decimal benefit_option_factor { get; set; }
         public Decimal alt_payee_benefit_amount { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
         public int alt_payee_relationship_id { get; set; }
         public string alt_payee_relationship_description { get; set; }
         public string alt_payee_relationship_value { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public Decimal life_conversion_factor { get; set; }
         public Decimal early_reduction_factor { get; set; }
    }
    [Serializable]
    public enum enmQdroCalculationOptions
    {
         qdro_calculation_option_id ,
         qdro_calculation_detail_id ,
         plan_benefit_id ,
         benefit_option_factor ,
         alt_payee_benefit_amount ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
         alt_payee_relationship_id ,
         alt_payee_relationship_description ,
         alt_payee_relationship_value ,
         ee_flag ,
         uvhp_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         life_conversion_factor ,
         early_reduction_factor ,
    }
}

