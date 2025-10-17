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
	/// Class MPIPHP.DataObjects.doProcessInstanceImageData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doProcessInstanceImageData : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doProcessInstanceImageData() : base()
         {
         }
         public int process_instance_image_data_id { get; set; }
         public int process_instance_id { get; set; }
         public int filenet_document_type_id { get; set; }
         public string filenet_document_type_description { get; set; }
         public string filenet_document_type_value { get; set; }
         public int image_doc_category_id { get; set; }
         public string image_doc_category_description { get; set; }
         public string image_doc_category_value { get; set; }
         public string doc_type { get; set; }
         public DateTime initiated_date { get; set; }
    }
    [Serializable]
    public enum enmProcessInstanceImageData
    {
         process_instance_image_data_id ,
         process_instance_id ,
         filenet_document_type_id ,
         filenet_document_type_description ,
         filenet_document_type_value ,
         image_doc_category_id ,
         image_doc_category_description ,
         image_doc_category_value ,
         doc_type ,
         initiated_date ,
    }
}

