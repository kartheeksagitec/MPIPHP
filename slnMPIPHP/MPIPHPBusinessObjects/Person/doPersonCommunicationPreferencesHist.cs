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
	/// Class NeoSpin.DataObjects.doPersonCommunicationPreferencesHist:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doPersonCommunicationPreferencesHist : doBase
    {
         public doPersonCommunicationPreferencesHist() : base()
         {
         }
         public int person_communication_preferences_hist_id { get; set; }
         public int person_communication_preferences_id { get; set; }
         public int person_id { get; set; }
         public string registered_email_address { get; set; }
         public string is_paper_statement { get; set; }
         public string is_paper_spd { get; set; }
         public string log_by { get; set; }
         public DateTime log_date { get; set; }
    }
    [Serializable]
    public partial class enmPersonCommunicationPreferencesHist
    {
      public const string  person_communication_preferences_hist_id = "person_communication_preferences_hist_id";
      public const string  person_communication_preferences_id = "person_communication_preferences_id";
      public const string  person_id = "person_id";
      public const string  registered_email_address = "registered_email_address";
      public const string  is_paper_statement = "is_paper_statement";
      public const string  is_paper_spd = "is_paper_spd";
      public const string  log_by = "log_by";
      public const string  log_date = "log_date";
    }
}
