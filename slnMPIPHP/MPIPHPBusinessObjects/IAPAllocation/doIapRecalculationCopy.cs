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
	/// Class MPIPHP.DataObjects.doIapRecalculationCopy:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIapRecalculationCopy : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doIapRecalculationCopy() : base()
         {
         }
         public int iap_recalculation_copy_id { get; set; }
         public int person_id { get; set; }
         public string mpi_person_id { get; set; }
         public string file_name { get; set; }
         public DateTime recalc_date { get; set; }
         public string path_code { get; set; }
    }
    [Serializable]
    public enum enmIapRecalculationCopy
    {
         iap_recalculation_copy_id ,
         person_id ,
         mpi_person_id ,
         file_name ,
         recalc_date ,
         path_code ,
    }
}

