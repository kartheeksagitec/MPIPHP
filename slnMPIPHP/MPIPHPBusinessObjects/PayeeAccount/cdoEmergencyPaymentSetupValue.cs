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
	/// Class MPIPHP.CustomDataObjects.cdoEmergencyPaymentSetupValue:
	/// Inherited from doEmergencyPaymentSetupValue, the class is used to customize the database object doEmergencyPaymentSetupValue.
	/// </summary>
    [Serializable]
	public class cdoEmergencyPaymentSetupValue : doEmergencyPaymentSetupValue
	{
		public cdoEmergencyPaymentSetupValue() : base()
		{
		}
    } 
} 
