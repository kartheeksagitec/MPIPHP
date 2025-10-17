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
	/// Class MPIPHP.CustomDataObjects.cdoPayment1099r:
	/// Inherited from doPayment1099r, the class is used to customize the database object doPayment1099r.
	/// </summary>
    [Serializable]
	public class cdoPayment1099r : doPayment1099r
	{
		public cdoPayment1099r() : base()
		{
           
		}
        public decimal total_employee_contrib_amt { get; set; }

        public string id_suffix { get; set; }
        public string mpi_person_id { get; set; }
    } 
} 
