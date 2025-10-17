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
	/// Class MPIPHP.DataObjects.doDroModel:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDroModel : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDroModel() : base()
         {
         }
         public int dro_model_id { get; set; }
         public string dro_model { get; set; }
    }
    [Serializable]
    public enum enmDroModel
    {
         dro_model_id ,
         dro_model ,
    }
}

