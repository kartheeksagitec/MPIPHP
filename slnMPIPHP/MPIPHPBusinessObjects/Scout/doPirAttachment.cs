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
	/// Class MPIPHP.DataObjects.doPirAttachment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPirAttachment : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPirAttachment() : base()
         {
         }
         public int pir_attachment_id { get; set; }
         public int pir_id { get; set; }
         public byte[] attachment_content { get; set; }
         public Guid attachment_guid { get; set; }
         public string attachment_file_name { get; set; }
         public string attachment_mime_type { get; set; }
    }
    [Serializable]
    public enum enmPirAttachment
    {
         pir_attachment_id ,
         pir_id ,
         attachment_content ,
         attachment_guid ,
         attachment_file_name ,
         attachment_mime_type ,
    }
}

