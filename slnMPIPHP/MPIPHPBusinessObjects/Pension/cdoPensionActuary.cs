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
	/// Class MPIPHP.CustomDataObjects.cdoPensionActuary:
	/// Inherited from doPensionActuary, the class is used to customize the database object doPensionActuary.
	/// </summary>
    [Serializable]
	public class cdoPensionActuary : doPensionActuary
	{
		public cdoPensionActuary() : base()
		{
		}
    } 
} 
