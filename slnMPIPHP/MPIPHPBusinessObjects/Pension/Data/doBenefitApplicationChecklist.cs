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
	/// Class NeoSpin.DataObjects.doBenefitApplicationChecklist:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doBenefitApplicationChecklist : doBase
    {
         public doBenefitApplicationChecklist() : base()
         {
         }
         public int checklist_benefit_application_id { get; set; }
         public int benefit_application_id { get; set; }
         public string proof_of_birth { get; set; }
         public DateTime iap_wait_timer { get; set; }
         public string proof_of_birth_spouse { get; set; }
         public DateTime proof_of_birth_received_on { get; set; }
         public string verification_of_tin { get; set; }
         public string verification_of_tin_spouse { get; set; }
         public DateTime verification_of_tin_received_on { get; set; }
         public string death_certificate { get; set; }
         public string death_certificate_na { get; set; }
         public DateTime death_certificate_received_on { get; set; }
         public string divorce_document { get; set; }
         public string no_of_divorces_ex_spouse_name1 { get; set; }
         public string no_of_divorces_ex_spouse_name2 { get; set; }
         public string no_of_divorces_ex_spouse_name3 { get; set; }
         public string no_of_divorces_ex_spouse_name4 { get; set; }
         public int ex_spouse_id { get; set; }
         public string ex_spouse_name1_interest { get; set; }
         public string ex_spouse_name2_interest { get; set; }
         public string ex_spouse_name3_interest { get; set; }
         public string ex_spouse_name4_interest { get; set; }
         public string ssd_award { get; set; }
         public string ssd_award_na { get; set; }
         public string ssd_application { get; set; }
         public string ssd_application_na { get; set; }
         public DateTime ssd_application_received_on { get; set; }
         public DateTime ssd_award_received_on { get; set; }
         public string shared_note { get; set; }
         public string shared_note_aprroved { get; set; }
         public string shared_note_rework_needed { get; set; }
         public DateTime shared_note_rework_received_on { get; set; }
         public string shared_note_last_initial { get; set; }
         public string marriage_certificate { get; set; }
         public string marriage_certificate_na { get; set; }
         public DateTime marriage_certificate_received_on { get; set; }
         public DateTime ex_spouse_name1_received_on { get; set; }
         public DateTime ex_spouse_name2_received_on { get; set; }
         public DateTime ex_spouse_name3_received_on { get; set; }
         public DateTime ex_spouse_name4_received_on { get; set; }
         public string shared_note_first_initial { get; set; }
         public DateTime shared_note_received_on { get; set; }
         public string proof_of_birth_spouse_na { get; set; }
         public string verification_of_tin_spouse_na { get; set; }
    }
    [Serializable]
    public partial class enmBenefitApplicationChecklist
    {
      public const string  checklist_benefit_application_id = "checklist_benefit_application_id";
      public const string  benefit_application_id = "benefit_application_id";
      public const string  proof_of_birth = "proof_of_birth";
      public const string  iap_wait_timer = "iap_wait_timer";
      public const string  proof_of_birth_spouse = "proof_of_birth_spouse";
      public const string  proof_of_birth_received_on = "proof_of_birth_received_on";
      public const string  verification_of_tin = "verification_of_tin";
      public const string  verification_of_tin_spouse = "verification_of_tin_spouse";
      public const string  verification_of_tin_received_on = "verification_of_tin_received_on";
      public const string  death_certificate = "death_certificate";
      public const string  death_certificate_na = "death_certificate_na";
      public const string  death_certificate_received_on = "death_certificate_received_on";
      public const string  divorce_document = "divorce_document";
      public const string  no_of_divorces_ex_spouse_name1 = "no_of_divorces_ex_spouse_name1";
      public const string  no_of_divorces_ex_spouse_name2 = "no_of_divorces_ex_spouse_name2";
      public const string  no_of_divorces_ex_spouse_name3 = "no_of_divorces_ex_spouse_name3";
      public const string  no_of_divorces_ex_spouse_name4 = "no_of_divorces_ex_spouse_name4";
      public const string  ex_spouse_id = "ex_spouse_id";
      public const string  ex_spouse_name1_interest = "ex_spouse_name1_interest";
      public const string  ex_spouse_name2_interest = "ex_spouse_name2_interest";
      public const string  ex_spouse_name3_interest = "ex_spouse_name3_interest";
      public const string  ex_spouse_name4_interest = "ex_spouse_name4_interest";
      public const string  ssd_award = "ssd_award";
      public const string  ssd_award_na = "ssd_award_na";
      public const string  ssd_application = "ssd_application";
      public const string  ssd_application_na = "ssd_application_na";
      public const string  ssd_application_received_on = "ssd_application_received_on";
      public const string  ssd_award_received_on = "ssd_award_received_on";
      public const string  shared_note = "shared_note";
      public const string  shared_note_aprroved = "shared_note_aprroved";
      public const string  shared_note_rework_needed = "shared_note_rework_needed";
      public const string  shared_note_rework_received_on = "shared_note_rework_received_on";
      public const string  shared_note_last_initial = "shared_note_last_initial";
      public const string  marriage_certificate = "marriage_certificate";
      public const string  marriage_certificate_na = "marriage_certificate_na";
      public const string  marriage_certificate_received_on = "marriage_certificate_received_on";
      public const string  ex_spouse_name1_received_on = "ex_spouse_name1_received_on";
      public const string  ex_spouse_name2_received_on = "ex_spouse_name2_received_on";
      public const string  ex_spouse_name3_received_on = "ex_spouse_name3_received_on";
      public const string  ex_spouse_name4_received_on = "ex_spouse_name4_received_on";
      public const string  shared_note_first_initial = "shared_note_first_initial";
      public const string  shared_note_received_on = "shared_note_received_on";
      public const string  proof_of_birth_spouse_na = "proof_of_birth_spouse_na";
      public const string  verification_of_tin_spouse_na = "verification_of_tin_spouse_na";
    }
}
