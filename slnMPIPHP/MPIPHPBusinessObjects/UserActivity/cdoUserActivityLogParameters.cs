#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace MPIPHP.CustomDataObjects
{
	/// <summary>
	/// Class MPIPHP.CustomDataObjects.cdoUserActivityLogParameters:
	/// Inherited from doUserActivityLogParameters, the class is used to customize the database object doUserActivityLogParameters.
	/// </summary>
    [Serializable]
	public class cdoUserActivityLogParameters : doUserActivityLogParameters
	{
		public cdoUserActivityLogParameters() : base()
		{
		}
    } 
} 
