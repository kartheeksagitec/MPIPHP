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
	/// Class MPIPHP.CustomDataObjects.cdoAnnualStatementBatchData:
	/// Inherited from doAnnualStatementBatchData, the class is used to customize the database object doAnnualStatementBatchData.
	/// </summary>
    [Serializable]
	public class cdoAnnualStatementBatchData : doAnnualStatementBatchData
	{
		public cdoAnnualStatementBatchData() : base()
		{

		}

        public string MPI_PERSON_ID {get;set;}
        public DateTime PERSON_DOB {get;set;}
        public string PERSON_SSN {get;set;}
        public string VIP_FLAG { get; set; }
    } 
} 
