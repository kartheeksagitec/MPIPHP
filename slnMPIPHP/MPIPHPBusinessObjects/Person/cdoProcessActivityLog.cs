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
	/// Class MPIPHP.CustomDataObjects.cdoProcessActivityLog:
	/// Inherited from doProcessActivityLog, the class is used to customize the database object doProcessActivityLog.
	/// </summary>
    [Serializable]
	public class cdoProcessActivityLog : doProcessActivityLog
	{
		public cdoProcessActivityLog() : base()
		{
		}
    } 
} 
