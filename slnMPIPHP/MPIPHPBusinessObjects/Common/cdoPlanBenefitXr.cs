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
	/// Class MPIPHP.CustomDataObjects.cdoPlanBenefitXr:
	/// Inherited from doPlanBenefitXr, the class is used to customize the database object doPlanBenefitXr.
	/// </summary>
    [Serializable]
	public class cdoPlanBenefitXr : doPlanBenefitXr
	{
		public cdoPlanBenefitXr() : base()
		{
		}
    } 
} 
