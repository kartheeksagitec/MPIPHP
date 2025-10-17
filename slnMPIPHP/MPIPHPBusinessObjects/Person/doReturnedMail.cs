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
	/// Class MPIPHP.DataObjects.doReturnedMail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doReturnedMail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doReturnedMail() : base()
         {
         }
         public int returned_mail_id { get; set; }
         public DateTime returned_mail_date { get; set; }
         public int doc_id { get; set; }
         public int doc_type_source_id { get; set; }
         public string doc_type_source_description { get; set; }
         public string doc_type_source_value { get; set; }
         public int address_id { get; set; }
         public int org_address_id { get; set; }
         public string returned_mail_addres_flag { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string notes { get; set; }
         public string other_document_type { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
    }
    [Serializable]
    public enum enmReturnedMail
    {
         returned_mail_id ,
         returned_mail_date ,
         doc_id ,
         doc_type_source_id ,
         doc_type_source_description ,
         doc_type_source_value ,
         address_id ,
         org_address_id ,
         returned_mail_addres_flag ,
         reason_id ,
         reason_description ,
         reason_value ,
         notes ,
         other_document_type ,
         person_id ,
         org_id ,
    }
}

