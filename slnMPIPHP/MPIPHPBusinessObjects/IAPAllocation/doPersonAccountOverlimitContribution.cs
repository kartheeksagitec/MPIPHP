#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace MPIPHP.DataObjects
{
	/// <summary>
	/// Class MPIPHP.DataObjects.doPersonAccountOverlimitContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountOverlimitContribution : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccountOverlimitContribution() : base()
         {
         }
         public int person_account_overlimit_id { get; set; }
         public int computation_year { get; set; }
         public int person_account_id { get; set; }
         public int emp_account_no { get; set; }
         public Decimal total_contribution_amount { get; set; }
         public Decimal excess_contribution_amount { get; set; }
         public DateTime processed_date { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountOverlimitContribution
    {
         person_account_overlimit_id ,
         computation_year ,
         person_account_id ,
         emp_account_no ,
         total_contribution_amount ,
         excess_contribution_amount ,
         processed_date ,
    }
}

