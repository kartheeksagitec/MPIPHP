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
	/// Class MPIPHP.DataObjects.doRelationship:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRelationship : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doRelationship() : base()
         {
         }
         public int person_relationship_id { get; set; }
         public int person_id { get; set; }
         public int beneficiary_person_id { get; set; }
         public int beneficiary_org_id { get; set; }
         public int dependent_person_id { get; set; }
         public int relationship_id { get; set; }
         public string relationship_description { get; set; }
         public string relationship_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public string addr_same_as_participant_flag { get; set; }
         public DateTime date_of_marriage { get; set; }
         public int beneficiary_from_id { get; set; }
         public string beneficiary_from_description { get; set; }
         public string beneficiary_from_value { get; set; }
         public int beneficiary_of { get; set; }
    }
    [Serializable]
    public enum enmRelationship
    {
         person_relationship_id ,
         person_id ,
         beneficiary_person_id ,
         beneficiary_org_id ,
         dependent_person_id ,
         relationship_id ,
         relationship_description ,
         relationship_value ,
         effective_start_date ,
         effective_end_date ,
         addr_same_as_participant_flag ,
         date_of_marriage ,
         beneficiary_from_id ,
         beneficiary_from_description ,
         beneficiary_from_value ,
         beneficiary_of ,
    }
}

