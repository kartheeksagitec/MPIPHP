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
	/// Class MPIPHP.DataObjects.doPayeeAccountWireDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountWireDetail : doBase
    {
		 [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPayeeAccountWireDetail() : base()
         {
         }
         public int payee_account_wire_detail_id { get; set; }
         public int payee_account_id { get; set; }
         public int bank_org_id { get; set; }
         public int bank_account_type_id { get; set; }
         public string bank_account_type_description { get; set; }
         public string bank_account_type_value { get; set; }
         public string beneficiary_account_number { get; set; }
         public string call_back_flag { get; set; }
         public DateTime wire_start_date { get; set; }
         public DateTime wire_end_date { get; set; }
         public DateTime call_back_completion_date { get; set; }
         public string call_back_comments { get; set; }
         public int inter_bank_org_id { get; set; }
         public string inter_beneficiary_account_number { get; set; }
         public DateTime inter_wire_start_date { get; set; }
         public DateTime inter_wire_end_date { get; set; }

        public string istrOrgName { get; set; }

        public string istrOrgMPID { get; set; }

        public string istrInterOrgName { get; set; }

        public string istrinterOrgMPID { get; set; }

        public int iintAbaSwiftBankCode { get; set; }
        public string istrAbaSwiftBankCode { get; set; }

        public int iintInterAbaSwiftBankCode { get; set; }
        public string istrInterAbaSwiftBankCode { get; set; }

        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    }
    [Serializable]
    public enum enmPayeeAccountWireDetail
    {
         payee_account_wire_detail_id ,
         payee_account_id ,
         bank_org_id ,
         bank_account_type_id ,
         bank_account_type_description ,
         bank_account_type_value ,
         beneficiary_account_number ,
         call_back_flag ,
         wire_start_date ,
         wire_end_date ,
         call_back_completion_date ,
         call_back_comments ,
         inter_bank_org_id ,
         inter_beneficiary_account_number ,
         inter_wire_start_date ,
         inter_wire_end_date ,
    }
}
