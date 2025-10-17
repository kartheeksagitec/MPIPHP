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
	/// Class MPIPHP.DataObjects.doDocumentProcessCrossref:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDocumentProcessCrossref : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDocumentProcessCrossref() : base()
         {
         }
         public int document_process_crossref_id { get; set; }
         public int document_id { get; set; }
         public int process_id { get; set; }
         public int document_type_action_id { get; set; }
         public string document_type_action_description { get; set; }
         public string document_type_action_value { get; set; }
         public string required_flag { get; set; }
    }
    [Serializable]
    public enum enmDocumentProcessCrossref
    {
         document_process_crossref_id ,
         document_id ,
         process_id ,
         document_type_action_id ,
         document_type_action_description ,
         document_type_action_value ,
         required_flag ,
    }
}

