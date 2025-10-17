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
	/// Class MPIPHP.DataObjects.doSsnMergeHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSsnMergeHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSsnMergeHistory() : base()
         {
         }
         public int ssn_merge_history_id { get; set; }
         public int old_person_id { get; set; }
         public int new_person_id { get; set; }
         public string new_ssn { get; set; }
         public DateTime merge_date { get; set; }
         public string merged_by { get; set; }
         public string old_mpi_person_id { get; set; }
         public string new_mpi_person_id { get; set; }
         public string old_ssn { get; set; }
    }
    [Serializable]
    public enum enmSsnMergeHistory
    {
         ssn_merge_history_id ,
         old_person_id ,
         new_person_id ,
         new_ssn ,
         merge_date ,
         merged_by ,
         old_mpi_person_id ,
         new_mpi_person_id ,
         old_ssn ,
    }
}

