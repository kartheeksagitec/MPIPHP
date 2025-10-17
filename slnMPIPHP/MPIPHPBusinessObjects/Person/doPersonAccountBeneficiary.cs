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
	/// Class MPIPHP.DataObjects.doPersonAccountBeneficiary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountBeneficiary : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccountBeneficiary() : base()
         {
         }
         public int person_account_beneficiary_id { get; set; }
         public int person_relationship_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public Decimal dist_percent { get; set; }
         public int beneficiary_type_id { get; set; }
         public string beneficiary_type_description { get; set; }
         public string beneficiary_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountBeneficiary
    {
         person_account_beneficiary_id ,
         person_relationship_id ,
         person_account_id ,
         start_date ,
         end_date ,
         dist_percent ,
         beneficiary_type_id ,
         beneficiary_type_description ,
         beneficiary_type_value ,
         status_id ,
         status_description ,
         status_value ,
    }
}

