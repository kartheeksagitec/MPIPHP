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
	/// Class MPIPHP.CustomDataObjects.cdoPersonAccountOverlimitContribution:
	/// Inherited from doPersonAccountOverlimitContribution, the class is used to customize the database object doPersonAccountOverlimitContribution.
	/// </summary>
    [Serializable]
	public class cdoPersonAccountOverlimitContribution : doPersonAccountOverlimitContribution
	{
		public cdoPersonAccountOverlimitContribution() : base()
		{
		}
    } 
} 
