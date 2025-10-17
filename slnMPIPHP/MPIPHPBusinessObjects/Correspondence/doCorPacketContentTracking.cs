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
	/// Class MPIPHP.DataObjects.doCorPacketContentTracking:
	/// Inherited from doNeoTrackBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorPacketContentTracking : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCorPacketContentTracking() : base()
         {
         }
         public int cor_packet_content_tracking_id { get; set; }
         public int cor_packet_content_id { get; set; }
         public int tracking_id { get; set; }
         public string mailed_by { get; set; }
         public DateTime mailed_date { get; set; }
         public string received_by { get; set; }
         public DateTime received_date { get; set; }
         public int benefit_appicaltion_id { get; set; }
         public int benefit_calculation_header_id { get; set; }
         public int qdro_appicaltion_id { get; set; }
         public int qdro_calculation_header_id { get; set; }
         public DateTime retirement_date { get; set; }
         public int packet_status_id { get; set; }
         public string packet_status_description { get; set; }
         public string packet_status_value { get; set; }
         public string notes { get; set; }
         public string access_template_name { get; set; }
         public string access_paket_status { get; set; }
         public DateTime access_monthly_cycle { get; set; }
         public string access_plan { get; set; }
    }
    [Serializable]
    public enum enmCorPacketContentTracking
    {
         cor_packet_content_tracking_id ,
         cor_packet_content_id ,
         tracking_id ,
         mailed_by ,
         mailed_date ,
         received_by ,
         received_date ,
         benefit_appicaltion_id ,
         benefit_calculation_header_id ,
         qdro_appicaltion_id ,
         qdro_calculation_header_id ,
         retirement_date ,
         packet_status_id ,
         packet_status_description ,
         packet_status_value ,
         notes ,
         access_template_name ,
         access_paket_status ,
         access_monthly_cycle ,
         access_plan ,
    }
}

