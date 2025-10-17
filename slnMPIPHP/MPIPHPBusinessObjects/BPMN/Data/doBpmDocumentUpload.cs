#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NeoBase.Common.DataObjects;
using Sagitec.Common;
using Sagitec.DataObjects;
#endregion
namespace NeoBase.BPMDataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doBpmDocumentUpload:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBpmDocumentUpload : doNeoBase
    {
         public doBpmDocumentUpload() : base()
         {
         }
         public int bpm_document_upload_id { get; set; }
         public int bpm_process_instance_id { get; set; }
         public string bpm_document_name { get; set; }
    }
    [Serializable]
    public enum enmBpmDocumentUpload
    {
         bpm_document_upload_id ,
         bpm_process_instance_id ,
         bpm_document_name ,
    }
}
