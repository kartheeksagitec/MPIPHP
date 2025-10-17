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
	/// Class NeoSpin.DataObjects.doPersonCommunicationPreferences:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doPersonCommunicationPreferences : doBase
    {
         public doPersonCommunicationPreferences() : base()
         {
         }
         public int person_communication_preferences_id { get; set; }
         public int person_id { get; set; }
         public string registered_email_address { get; set; }
         public string is_paper_statement { get; set; }
         public string is_paper_spd { get; set; }
    }
    [Serializable]
    public partial class enmPersonCommunicationPreferences
    {
      public const string  person_communication_preferences_id = "person_communication_preferences_id";
      public const string  person_id = "person_id";
      public const string  registered_email_address = "registered_email_address";
      public const string  is_paper_statement = "is_paper_statement";
      public const string  is_paper_spd = "is_paper_spd";
    }
}
