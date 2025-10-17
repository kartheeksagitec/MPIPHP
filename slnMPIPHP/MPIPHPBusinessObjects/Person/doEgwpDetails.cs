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
	/// Class MPIPHP.DataObjects.doEgwpDetails:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doEgwpDetails : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doEgwpDetails() : base()
         {
         }
         public int egwp_details_id { get; set; }
         public string mpi_person_id { get; set; }
         public DateTime retirement_date { get; set; }
         public string retirement_status { get; set; }
         public DateTime status_date { get; set; }
         public string disabled_flag { get; set; }
    }
    [Serializable]
    public enum enmEgwpDetails
    {
         egwp_details_id ,
         mpi_person_id ,
         retirement_date ,
         retirement_status ,
         status_date ,
         disabled_flag ,
    }
}

