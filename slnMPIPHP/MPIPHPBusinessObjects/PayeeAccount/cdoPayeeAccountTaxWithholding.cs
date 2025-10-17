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
	/// Class MPIPHP.CustomDataObjects.cdoPayeeAccountTaxWithholding:
	/// Inherited from doPayeeAccountTaxWithholding, the class is used to customize the database object doPayeeAccountTaxWithholding.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountTaxWithholding : doPayeeAccountTaxWithholding
	{
        public string istrMonthlyBenefitTaxtOption { get; set; }
        public decimal idecCalculatedTaxAmount { get; set; }

        public cdoPayeeAccountTaxWithholding() : base()
		{
		}
        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    } 
} 
