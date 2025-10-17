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
	/// Class MPIPHP.CustomDataObjects.cdoProcess:
	/// Inherited from doProcess, the class is used to customize the database object doProcess.
	/// </summary>
    [Serializable]
	public class cdoProcess : doProcess
	{
		public cdoProcess() : base()
		{
		}

        public string istrWorkflowFullPath { get; set; }
    } 
} 
