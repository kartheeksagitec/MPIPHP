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
	/// Class MPIPHP.DataObjects.doPayeeAccountAchDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountAchDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountAchDetail() : base()
         {
         }
         public int payee_account_ach_detail_id { get; set; }
         public int payee_account_id { get; set; }
         public int bank_org_id { get; set; }
         public int bank_account_type_id { get; set; }
         public string bank_account_type_description { get; set; }
         public string bank_account_type_value { get; set; }
         public string account_number { get; set; }
         public string pre_note_flag { get; set; }
         public string primary_account_flag { get; set; }
         public DateTime ach_start_date { get; set; }
         public DateTime ach_end_date { get; set; }
         public DateTime pre_note_completion_date { get; set; }
         public Decimal percentage_of_net_amount { get; set; }
         public Decimal partial_amount { get; set; }
         public string joint_account_flag { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountAchDetail
    {
         payee_account_ach_detail_id ,
         payee_account_id ,
         bank_org_id ,
         bank_account_type_id ,
         bank_account_type_description ,
         bank_account_type_value ,
         account_number ,
         pre_note_flag ,
         primary_account_flag ,
         ach_start_date ,
         ach_end_date ,
         pre_note_completion_date ,
         percentage_of_net_amount ,
         partial_amount ,
         joint_account_flag ,
    }
}

