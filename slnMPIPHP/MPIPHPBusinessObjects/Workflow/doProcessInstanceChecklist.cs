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
	/// Class MPIPHP.DataObjects.doProcessInstanceChecklist:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessInstanceChecklist : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcessInstanceChecklist() : base()
         {
         }
         public int process_instance_checklist_id { get; set; }
         public int process_instance_id { get; set; }
         public int document_id { get; set; }
         public string required_flag { get; set; }
         public string approved_flag { get; set; }
         public DateTime received_date { get; set; }
    }
    [Serializable]
    public enum enmProcessInstanceChecklist
    {
         process_instance_checklist_id ,
         process_instance_id ,
         document_id ,
         required_flag ,
         approved_flag ,
         received_date ,
    }
}

