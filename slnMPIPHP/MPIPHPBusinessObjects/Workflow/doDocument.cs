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
	/// Class MPIPHP.DataObjects.doDocument:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDocument : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDocument() : base()
         {
         }
         public int document_id { get; set; }
         public string doc_type { get; set; }
         public string document_name { get; set; }
         public string ignore_process_flag { get; set; }
         public string app_name { get; set; }
         public string group_name { get; set; }
         public string doc_description { get; set; }
    }
    [Serializable]
    public enum enmDocument
    {
         document_id ,
         doc_type ,
         document_name ,
         ignore_process_flag ,
         app_name ,
         group_name ,
         doc_description ,
    }
}

