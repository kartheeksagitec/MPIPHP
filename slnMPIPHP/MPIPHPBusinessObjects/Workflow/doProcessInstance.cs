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
	/// Class MPIPHP.DataObjects.doProcessInstance:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessInstance : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcessInstance() : base()
         {
         }
         public int process_instance_id { get; set; }
         public int process_id { get; set; }
         public int request_id { get; set; }
         public Guid workflow_instance_guid { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int priority { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int contact_ticket_id { get; set; }
         public string additional_parameter1 { get; set; }
    }
    [Serializable]
    public enum enmProcessInstance
    {
         process_instance_id ,
         process_id ,
         request_id ,
         workflow_instance_guid ,
         person_id ,
         org_id ,
         priority ,
         status_id ,
         status_description ,
         status_value ,
         contact_ticket_id ,
         additional_parameter1 ,
    }
}

