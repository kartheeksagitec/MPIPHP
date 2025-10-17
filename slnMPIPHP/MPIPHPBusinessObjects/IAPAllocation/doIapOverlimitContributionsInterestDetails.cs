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
	/// Class MPIPHP.DataObjects.doIapOverlimitContributionsInterestDetails:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapOverlimitContributionsInterestDetails : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapOverlimitContributionsInterestDetails() : base()
         {
         }
         public int iap_overlimit_contributions_interest_details_id { get; set; }
         public int computation_year { get; set; }
         public Decimal overlimit_amount { get; set; }
         public Decimal alloc1_factor { get; set; }
         public Decimal interest { get; set; }
         public Decimal total_overlimit_contributions_interest_amount { get; set; }
    }
    [Serializable]
    public enum enmIapOverlimitContributionsInterestDetails
    {
         iap_overlimit_contributions_interest_details_id ,
         computation_year ,
         overlimit_amount ,
         alloc1_factor ,
         interest ,
         total_overlimit_contributions_interest_amount ,
    }
}

