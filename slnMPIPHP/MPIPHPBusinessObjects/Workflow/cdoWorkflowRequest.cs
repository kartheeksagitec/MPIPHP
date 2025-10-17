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
	/// Class MPIPHP.CustomDataObjects.cdoWorkflowRequest:
	/// Inherited from doWorkflowRequest, the class is used to customize the database object doWorkflowRequest.
	/// </summary>
    [Serializable]
	public class cdoWorkflowRequest : doWorkflowRequest
	{
		public cdoWorkflowRequest() : base()
		{
		}
    } 
} 
