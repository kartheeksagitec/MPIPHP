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
	/// Class MPIPHP.DataObjects.doNotes:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doNotes : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doNotes() : base()
         {
         }
         public int note_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int form_id { get; set; }
         public string form_description { get; set; }
         public string form_value { get; set; }
         public string notes { get; set; }
         public int process_instance_id { get; set; }
         public long reference_id { get; set; }
    }
    [Serializable]
    public enum enmNotes
    {
         note_id ,
         person_id ,
         org_id ,
         form_id ,
         form_description ,
         form_value ,
         notes ,
         process_instance_id ,
		 reference_id,
    }
}
