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
    /// Class MPIPHP.DataObjects.doPerson:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public class doPerson : doBase
    {
        [NonSerialized]
        public static Hashtable ihstFields = null;
        public doPerson() : base()
        {
        }
        public int person_id { get; set; }
        public string mpi_person_id { get; set; }
        public string first_name { get; set; }
        public string middle_name { get; set; }
        public string last_name { get; set; }
        public int name_prefix_id { get; set; }
        public string name_prefix_description { get; set; }
        public string name_prefix_value { get; set; }
        public DateTime date_of_birth { get; set; }
        public DateTime date_of_death { get; set; }
        public string ssn { get; set; }
        public string ethnicity { get; set; }
        public int gender_id { get; set; }
        public string gender_description { get; set; }
        public string gender_value { get; set; }
        public int marital_status_id { get; set; }
        public string marital_status_description { get; set; }
        public string marital_status_value { get; set; }
        public string home_phone_no { get; set; }
        public string work_phone_no { get; set; }
        public string cell_phone_no { get; set; }
        public string extension { get; set; }
        public string fax_no { get; set; }
        public string pager { get; set; }
        public string email_address_1 { get; set; }
        public string email_address_2 { get; set; }
        public int communication_preference_id { get; set; }
        public string communication_preference_description { get; set; }
        public string communication_preference_value { get; set; }
        public int status_id { get; set; }
        public string status_description { get; set; }
        public string status_value { get; set; }
        public string vip_flag { get; set; }
        public string name_suffix { get; set; }
        public string recalculate_vesting_flag { get; set; }
        public string health_eligible_flag { get; set; }
        public string pension_eligible_notification_flag { get; set; }
        public string is_person_deleted_flag { get; set; }
        public string suspension_of_benefits_notification_flag { get; set; }
        public string new_participant_letter_send_flag { get; set; }
        public DateTime retirement_health_date { get; set; }
        public int md_age_opt_id { get; set; }
        public string document_upload_flag { get; set; }
        public string adverse_interest_flag { get; set; }
    }
    [Serializable]
    public enum enmPerson
    {
        person_id,
        mpi_person_id,
        first_name,
        middle_name,
        last_name,
        name_prefix_id,
        name_prefix_description,
        name_prefix_value,
        date_of_birth,
        date_of_death,
        ssn,
        ethnicity,
        gender_id,
        gender_description,
        gender_value,
        marital_status_id,
        marital_status_description,
        marital_status_value,
        home_phone_no,
        work_phone_no,
        cell_phone_no,
        extension,
        fax_no,
        pager,
        email_address_1,
        email_address_2,
        communication_preference_id,
        communication_preference_description,
        communication_preference_value,
        status_id,
        status_description,
        status_value,
        vip_flag,
        name_suffix,
        recalculate_vesting_flag,
        health_eligible_flag,
        pension_eligible_notification_flag,
        is_person_deleted_flag,
        suspension_of_benefits_notification_flag,
        new_participant_letter_send_flag,
        retirement_health_date,
        md_age_opt_id,
        document_upload_flag,
        adverse_interest_flag,
    }
}

