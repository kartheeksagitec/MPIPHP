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
	/// Class MPIPHP.DataObjects.doWorkflowRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWorkflowRequest : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doWorkflowRequest() : base()
         {
         }
         public int workflow_request_id { get; set; }
         public string doc_type { get; set; }
         public int process_id { get; set; }
         public long reference_id { get; set; }
         public string filenet_document_type { get; set; }
         public string image_doc_category { get; set; }
         public int person_id { get; set; }
         public string org_code { get; set; }
         public int process_instance_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int source_id { get; set; }
         public string source_description { get; set; }
         public string source_value { get; set; }
         public DateTime initiated_date { get; set; }
         public int contact_ticket_id { get; set; }
         public string additional_parameter1 { get; set; }
         public int tracking_id { get; set; }
         public DateTime timestamp { get; set; }
         public string group_name { get; set; }
    }
    [Serializable]
    public enum enmWorkflowRequest
    {
         workflow_request_id ,
         doc_type ,
         process_id ,
         reference_id ,
         filenet_document_type ,
         image_doc_category ,
         person_id ,
         org_code ,
         process_instance_id ,
         status_id ,
         status_description ,
         status_value ,
         source_id ,
         source_description ,
         source_value ,
         initiated_date ,
         contact_ticket_id ,
         additional_parameter1 ,
         tracking_id ,
         timestamp ,
         group_name ,
    }
}

