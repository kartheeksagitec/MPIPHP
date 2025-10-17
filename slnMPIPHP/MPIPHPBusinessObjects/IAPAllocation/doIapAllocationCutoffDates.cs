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
	/// Class MPIPHP.DataObjects.doIapAllocationCutoffDates:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapAllocationCutoffDates : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapAllocationCutoffDates() : base()
         {
         }
         public int iap_allocation_cutoff_dates_id { get; set; }
         public int computational_year { get; set; }
         public DateTime first_day { get; set; }
         public DateTime last_day { get; set; }
         public DateTime ea_cutoff_date_from { get; set; }
         public DateTime ea_cutoff_date_to { get; set; }
         public DateTime adj_cutoff_date_from { get; set; }
         public DateTime adj_cutoff_date_to { get; set; }
    }

    [Serializable]
    public enum enmIapAllocationCutoffDates
    {
         iap_allocation_cutoff_dates_id ,
         computational_year ,
         first_day ,
         last_day ,
         ea_cutoff_date_from ,
         ea_cutoff_date_to ,
         adj_cutoff_date_from ,
         adj_cutoff_date_to ,
    }
}

