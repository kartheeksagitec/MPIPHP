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
	/// Class MPIPHP.CustomDataObjects.cdoWithholdingInformation:
	/// Inherited from doWithholdingInformation, the class is used to customize the database object doWithholdingInformation.
	/// </summary>
    [Serializable]
	public class cdoWithholdingInformation : doWithholdingInformation
	{
		public cdoWithholdingInformation() : base()
		{
		}
        protected override bool DonotSaveObjectJustForAuditFields(bool ablnOnlyAuditFieldsAreBeingModified)
        {
            return true && ablnOnlyAuditFieldsAreBeingModified;
        }
    } 
} 
