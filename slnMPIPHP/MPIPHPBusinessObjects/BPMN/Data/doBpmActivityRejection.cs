#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;
using NeoBase.Common.DataObjects;
#endregion
namespace NeoBase.BPMDataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doBpmActivityRejection:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBpmActivityRejection : doNeoBase
    {
         public doBpmActivityRejection() : base()
         {
         }
         public int bpm_activity_rejection_id { get; set; }
         public int activity_id { get; set; }
         public int rejection_reason_id { get; set; }
         public string rejection_reason_description { get; set; }
         public string rejection_reason_value { get; set; }
    }
    [Serializable]
    public enum enmBpmActivityRejection
    {
         bpm_activity_rejection_id ,
         activity_id ,
         rejection_reason_id ,
         rejection_reason_description ,
         rejection_reason_value ,
    }
}
