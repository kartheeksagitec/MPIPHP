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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitProvisionBenefitOption:
	/// Inherited from doBenefitProvisionBenefitOption, the class is used to customize the database object doBenefitProvisionBenefitOption.
	/// </summary>
    [Serializable]
	public class cdoBenefitProvisionBenefitOption : doBenefitProvisionBenefitOption
	{
		public cdoBenefitProvisionBenefitOption() : base()
		{
		}
    } 
} 
