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
	/// Class MPIPHP.DataObjects.doDummyWorkData:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDummyWorkData : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDummyWorkData() : base()
         {
         }
         public int year { get; set; }
         public string computation_year { get; set; }
         public Decimal age { get; set; }
         public Decimal qualified_hours { get; set; }
         public Decimal vested_hours { get; set; }
         public int qualified_years_count { get; set; }
         public int vested_years_count { get; set; }
         public int anniversary_years_count { get; set; }
         public int bis_years_count { get; set; }
         public string comments { get; set; }
    }
    [Serializable]
    public enum enmDummyWorkData
    {
         year ,
         computation_year ,
         age ,
         qualified_hours ,
         vested_hours ,
         qualified_years_count ,
         vested_years_count ,
         anniversary_years_count ,
         bis_years_count ,
         comments ,
    }
}

