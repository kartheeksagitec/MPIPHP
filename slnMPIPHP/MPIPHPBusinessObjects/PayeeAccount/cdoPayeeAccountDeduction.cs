#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using MPIPHP.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountDeduction:
	/// Inherited from doPayeeAccountDeduction, the class is used to customize the database object doPayeeAccountDeduction.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountDeduction : doPayeeAccountDeduction
	{
		public cdoPayeeAccountDeduction() : base()
		{
		}
        public string istrOrgMPID { get; set; }
        public string istrOrgName { get; set; }
        public string istrPersonMPID { get; set; }
        public string istrPersonName { get; set; }
        public string istrVendorOrgMPID { get;set;}
        public string istrVendorOrgName { get; set; }

        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    } 
} 
