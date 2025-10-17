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
	/// Class MPIPHP.CustomDataObjects.cdoBatchNotification:
	/// Inherited from doBatchNotification, the class is used to customize the database object doBatchNotification.
	/// </summary>
    [Serializable]
	public class cdoBatchNotification : doBatchNotification
	{
		public cdoBatchNotification() : base()
		{
		}
    } 
} 
