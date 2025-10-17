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
	/// Class NeoSpin.DataObjects.doBenefitApplicationAuditingChecklist:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doBenefitApplicationAuditingChecklist : doBase
    {
         public doBenefitApplicationAuditingChecklist() : base()
         {
         }
         public int auditing_checklist_benefit_application_id { get; set; }
         public int benefit_application_id { get; set; }
         public string auditing_note { get; set; }
         public string auditing_note_completed { get; set; }
         public string auditing_note_response { get; set; }
         public DateTime auditing_note_received_on { get; set; }
         public string auditing_note_first_intial { get; set; }
         public string auditing_note_last_intial { get; set; }
    }
    [Serializable]
    public partial class enmBenefitApplicationAuditingChecklist
    {
      public const string  auditing_checklist_benefit_application_id = "auditing_checklist_benefit_application_id";
      public const string  benefit_application_id = "benefit_application_id";
      public const string  auditing_note = "auditing_note";
      public const string  auditing_note_completed = "auditing_note_completed";
      public const string  auditing_note_response = "auditing_note_response";
      public const string  auditing_note_received_on = "auditing_note_received_on";
      public const string  auditing_note_first_intial = "auditing_note_first_intial";
      public const string  auditing_note_last_intial = "auditing_note_last_intial";
    }
}
