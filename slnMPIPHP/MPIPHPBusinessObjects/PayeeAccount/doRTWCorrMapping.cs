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
    /// Class NeoSpin.DataObjects.doRTWCorrMapping:
    /// Inherited from doBase, the class is used to create a wrapper of database table object.
    /// Each property of an instance of this class represents a column of database table object.  
    /// </summary>
    [Serializable]
    public partial class doRTWCorrMapping : doBase
    {
         public doRTWCorrMapping() : base()
         {
         }
         public int mapping_id { get; set; }
         public int reemployment_notification_id { get; set; }
         public int corr_tracking_id { get; set; }
         public string data1 { get; set; }
         public string data2 { get; set; }
    }
    [Serializable]
    public partial class enmRTWCorrMapping
    {
      public const string  mapping_id = "mapping_id";
      public const string  reemployment_notification_id = "reemployment_notification_id";
      public const string  corr_tracking_id = "corr_tracking_id";
      public const string  data1 = "data1";
      public const string  data2 = "data2";
    }
}
