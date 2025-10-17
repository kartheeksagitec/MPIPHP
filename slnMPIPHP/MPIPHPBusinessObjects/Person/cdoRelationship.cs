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
	/// Class MPIPHP.CustomDataObjects.cdoRelationship:
	/// Inherited from doRelationship, the class is used to customize the database object doRelationship.
	/// </summary>
    [Serializable]
	public class cdoRelationship : doRelationship
	{
		public cdoRelationship() : base()
		{
		}

        public string istrFullName { get; set; }
		//Ticket - 68547
        public string istrMpiPersonID { get; set; }
        //public long iintAPrimaryKey { get;  set; }
    } 
} 
