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
    /// Class MPIPHP.DataObjects.doPayeeAccountTaxWithholding:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doPayeeAccountTaxWithholding : doBase
    {
		 [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountTaxWithholding() : base()
         {
         }
         public int payee_account_tax_withholding_id { get; set; }
         public int payee_account_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public int benefit_distribution_type_id { get; set; }
         public string benefit_distribution_type_description { get; set; }
         public string benefit_distribution_type_value { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int tax_option_id { get; set; }
         public string tax_option_description { get; set; }
         public string tax_option_value { get; set; }
         public int tax_allowance { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public Decimal additional_tax_amount { get; set; }
         public Decimal tax_percentage { get; set; }
         public Decimal step_2_b_3 { get; set; }
         public Decimal step_3_amount { get; set; }
         public Decimal step_4_a { get; set; }
         public Decimal step_4_b { get; set; }
         public Decimal step_4_c { get; set; }
         public int personal_exemptions { get; set; }
         public int age_and_blindness_exemptions { get; set; }
         public Decimal voluntary_withholding { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountTaxWithholding
    {
         payee_account_tax_withholding_id ,
         payee_account_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         benefit_distribution_type_id ,
         benefit_distribution_type_description ,
         benefit_distribution_type_value ,
         start_date ,
         end_date ,
         tax_option_id ,
         tax_option_description ,
         tax_option_value ,
         tax_allowance ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         additional_tax_amount ,
         tax_percentage ,
         step_2_b_3 ,
         step_3_amount ,
         step_4_a ,
         step_4_b ,
         step_4_c ,
         personal_exemptions ,
         age_and_blindness_exemptions ,
         voluntary_withholding ,
    }
}

