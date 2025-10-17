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
	/// Class MPIPHP.DataObjects.doCorTracking:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorTracking : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCorTracking() : base()
         {
         }
         public int tracking_id { get; set; }
         public int template_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int plan_id { get; set; }
         public int contact_id { get; set; }
         public int org_contact_id { get; set; }
         public int cor_status_id { get; set; }
         public string cor_status_description { get; set; }
         public string cor_status_value { get; set; }
         public DateTime generated_date { get; set; }
         public DateTime print_on_date { get; set; }
         public DateTime printed_date { get; set; }
         public int imaging_serial_no { get; set; }
         public string comments { get; set; }
         public DateTime imaged_date { get; set; }
         public string converted_to_image_flag { get; set; }
    }
    [Serializable]
    public enum enmCorTracking
    {
         tracking_id ,
         template_id ,
         person_id ,
         org_id ,
         plan_id ,
         contact_id ,
         org_contact_id ,
         cor_status_id ,
         cor_status_description ,
         cor_status_value ,
         generated_date ,
         print_on_date ,
         printed_date ,
         imaging_serial_no ,
         comments ,
         imaged_date ,
         converted_to_image_flag ,
    }
}

