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
	/// Class NeoSpin.DataObjects.doOnetimeRetireePaymentContract:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doOnetimeRetireePaymentContract : doBase
    {
         public doOnetimeRetireePaymentContract() : base()
         {
         }
         public int onetime_retiree_payment_contract_id { get; set; }
         public int plan_year { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public DateTime retirement_date_from { get; set; }
         public DateTime retirement_date_to { get; set; }
         public int percent_increase_id { get; set; }
         public string percent_increase_description { get; set; }
         public string percent_increase_value { get; set; }
         public int contract_status_id { get; set; }
         public string contract_status_description { get; set; }
         public string contract_status_value { get; set; }
         public string approved_by { get; set; }
         public string notes { get; set; }
    }
    [Serializable]
    public partial class enmOnetimeRetireePaymentContract
    {
      public const string  onetime_retiree_payment_contract_id = "onetime_retiree_payment_contract_id";
      public const string  plan_year = "plan_year";
      public const string  effective_start_date = "effective_start_date";
      public const string  effective_end_date = "effective_end_date";
      public const string  retirement_date_from = "retirement_date_from";
      public const string  retirement_date_to = "retirement_date_to";
      public const string  percent_increase_id = "percent_increase_id";
      public const string  percent_increase_description = "percent_increase_description";
      public const string  percent_increase_value = "percent_increase_value";
      public const string  contract_status_id = "contract_status_id";
      public const string  contract_status_description = "contract_status_description";
      public const string  contract_status_value = "contract_status_value";
      public const string  approved_by = "approved_by";
      public const string  notes = "notes";
    }
}
