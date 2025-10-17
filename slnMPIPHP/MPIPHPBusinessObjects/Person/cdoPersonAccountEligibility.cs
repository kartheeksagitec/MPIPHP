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
	/// Class MPIPHP.CustomDataObjects.cdoPersonAccountEligibility:
	/// Inherited from doPersonAccountEligibility, the class is used to customize the database object doPersonAccountEligibility.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountEligibility : doPersonAccountEligibility
	{
		public cdoPersonAccountEligibility() : base()
		{
		}
    } 
} 
