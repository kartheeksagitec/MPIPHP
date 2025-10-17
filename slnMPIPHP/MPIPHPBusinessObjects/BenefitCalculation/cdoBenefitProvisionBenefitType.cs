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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitProvisionBenefitType:
	/// Inherited from doBenefitProvisionBenefitType, the class is used to customize the database object doBenefitProvisionBenefitType.
	/// </summary>
    [Serializable]
	public class cdoBenefitProvisionBenefitType : doBenefitProvisionBenefitType
	{
		public cdoBenefitProvisionBenefitType() : base()
		{
		}
    } 
} 
