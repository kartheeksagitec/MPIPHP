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
	/// Class MPIPHP.DataObjects.doCorTemplates:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorTemplates : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCorTemplates() : base()
         {
         }
         public int template_id { get; set; }
         public string template_name { get; set; }
         public string template_desc { get; set; }
         public int template_group_id { get; set; }
         public string template_group_description { get; set; }
         public string template_group_value { get; set; }
         public string active_flag { get; set; }
         public int destination_id { get; set; }
         public string destination_description { get; set; }
         public string destination_value { get; set; }
         public string associated_forms { get; set; }
         public string filter_object_id { get; set; }
         public string filter_object_description { get; set; }
         public string filter_object_field { get; set; }
         public string filter_object_value { get; set; }
         public int contact_role_id { get; set; }
         public string contact_role_description { get; set; }
         public string contact_role_value { get; set; }
         public string batch_flag { get; set; }
         public string online_flag { get; set; }
         public string auto_print_flag { get; set; }
         public int printer_name_id { get; set; }
         public string printer_name_description { get; set; }
         public string printer_name_value { get; set; }
         public int image_doc_category_id { get; set; }
         public string image_doc_category_description { get; set; }
         public string image_doc_category_value { get; set; }
         public int filenet_document_type_id { get; set; }
         public string filenet_document_type_description { get; set; }
         public string filenet_document_type_value { get; set; }
         public string doc_type { get; set; }
         public int template_type_id { get; set; }
         public string template_type_description { get; set; }
         public string template_type_value { get; set; }
         public string track_details_flag { get; set; }
    }
    [Serializable]
    public enum enmCorTemplates
    {
         template_id ,
         template_name ,
         template_desc ,
         template_group_id ,
         template_group_description ,
         template_group_value ,
         active_flag ,
         destination_id ,
         destination_description ,
         destination_value ,
         associated_forms ,
         filter_object_id ,
         filter_object_description ,
         filter_object_field ,
         filter_object_value ,
         contact_role_id ,
         contact_role_description ,
         contact_role_value ,
         batch_flag ,
         online_flag ,
         auto_print_flag ,
         printer_name_id ,
         printer_name_description ,
         printer_name_value ,
         image_doc_category_id ,
         image_doc_category_description ,
         image_doc_category_value ,
         filenet_document_type_id ,
         filenet_document_type_description ,
         filenet_document_type_value ,
         doc_type ,
         template_type_id ,
         template_type_description ,
         template_type_value ,
         track_details_flag ,
    }
}

