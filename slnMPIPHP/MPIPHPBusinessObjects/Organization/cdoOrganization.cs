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
	/// Class MPIPHP.CustomDataObjects.cdoOrganization:
	/// Inherited from doOrganization, the class is used to customize the database object doOrganization.
	/// </summary>
    [Serializable]
	public class cdoOrganization : doOrganization
	{
		public cdoOrganization() : base()
		{
		}

        public string istrSelectedFlag { get; set; }
        public string istrStatusValue { get; set; }
    } 
} 
