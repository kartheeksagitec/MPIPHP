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
	/// Class MPIPHP.DataObjects.doCorPacketContentTrackingHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorPacketContentTrackingHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCorPacketContentTrackingHistory() : base()
         {
         }
         public int cor_packet_content_tracking_history_id { get; set; }
         public int cor_packet_content_tracking_id { get; set; }
         public string mailed_by { get; set; }
         public DateTime mailed_date { get; set; }
         public string received_by { get; set; }
         public DateTime received_date { get; set; }
         public int packet_status_id { get; set; }
         public string packet_status_description { get; set; }
         public string packet_status_value { get; set; }
         public string notes { get; set; }
    }
    [Serializable]
    public enum enmCorPacketContentTrackingHistory
    {
         cor_packet_content_tracking_history_id ,
         cor_packet_content_tracking_id ,
         mailed_by ,
         mailed_date ,
         received_by ,
         received_date ,
         packet_status_id ,
         packet_status_description ,
         packet_status_value ,
         notes ,
    }
}

