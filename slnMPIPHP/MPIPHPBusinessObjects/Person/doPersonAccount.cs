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
	/// Class MPIPHP.DataObjects.doPersonAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccount : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPersonAccount() : base()
         {
         }
         public int person_account_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string special_account { get; set; }
         public string uvhp { get; set; }
         public string ee_contr { get; set; }
         public int benefeciary_person_id { get; set; }
         public int benefeciary_of_person_id { get; set; }
         public string recalculate_rule1_flag { get; set; }
         public string application_form_sent { get; set; }
         public string qdro_legal_review_required { get; set; }
         public string election_packet_sent { get; set; }
         public string election_packet_received { get; set; }
         public DateTime signed_application_forn_received_date { get; set; }
         public DateTime qdro_review_completed_date { get; set; }
         public string qdro_auditor_name { get; set; }
    }
    [Serializable]
    public enum enmPersonAccount
    {
         person_account_id ,
         person_id ,
         plan_id ,
         start_date ,
         end_date ,
         status_id ,
         status_description ,
         status_value ,
         special_account ,
         uvhp ,
         ee_contr ,
         benefeciary_person_id ,
         benefeciary_of_person_id ,
         recalculate_rule1_flag ,
         application_form_sent ,
         qdro_legal_review_required ,
         election_packet_sent ,
         election_packet_received ,
         signed_application_forn_received_date ,
         qdro_review_completed_date ,
         qdro_auditor_name ,
    }
}
