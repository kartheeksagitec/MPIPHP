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
	/// Class MPIPHP.CustomDataObjects.cdoBenefitFactorLocal:
	/// Inherited from doBenefitFactorLocal, the class is used to customize the database object doBenefitFactorLocal.
	/// </summary>
    [Serializable]
	public class cdoBenefitFactorLocal : doBenefitFactorLocal
	{
		public cdoBenefitFactorLocal() : base()
		{
		}
    } 
} 
