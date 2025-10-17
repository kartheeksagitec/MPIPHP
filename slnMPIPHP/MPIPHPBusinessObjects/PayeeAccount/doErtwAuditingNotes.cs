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
	/// Class NeoSpin.DataObjects.doErtwAuditingNotes:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public partial class doErtwAuditingNotes : doBase
    {
         public doErtwAuditingNotes() : base()
         {
         }
         public int note_id { get; set; }
         public int reemployment_notification_id { get; set; }
         public string notes { get; set; }
         public string complete { get; set; }
         public string response { get; set; }
         public string first_initial { get; set; }
         public string last_name { get; set; }
         public DateTime received_on { get; set; }
    }
    [Serializable]
    public partial class enmErtwAuditingNotes
    {
      public const string  note_id = "note_id";
      public const string  reemployment_notification_id = "reemployment_notification_id";
      public const string  notes = "notes";
      public const string  complete = "complete";
      public const string  response = "response";
      public const string  first_initial = "first_initial";
      public const string  last_name = "last_name";
      public const string  received_on = "received_on";
    }
}
