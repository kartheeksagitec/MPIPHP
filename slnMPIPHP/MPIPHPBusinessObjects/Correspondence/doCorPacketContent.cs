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
	/// Class MPIPHP.DataObjects.doCorPacketContent:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorPacketContent : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doCorPacketContent() : base()
         {
         }
         public int cor_packet_content_id { get; set; }
         public int packet_template_id { get; set; }
         public int cor_template_id { get; set; }
    }
    [Serializable]
    public enum enmCorPacketContent
    {
         cor_packet_content_id ,
         packet_template_id ,
         cor_template_id ,
    }
}

