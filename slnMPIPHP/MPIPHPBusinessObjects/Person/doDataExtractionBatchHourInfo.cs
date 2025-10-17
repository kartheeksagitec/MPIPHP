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
	/// Class MPIPHP.DataObjects.doDataExtractionBatchHourInfo:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDataExtractionBatchHourInfo : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doDataExtractionBatchHourInfo() : base()
         {
         }
         public int data_extraction_batch_hour_info_id { get; set; }
         public int data_extraction_batch_info_id { get; set; }
         public int person_id { get; set; }
         public string employer_no { get; set; }
         public string employer_name { get; set; }
         public int computation_year { get; set; }
         public Decimal hours_reported { get; set; }
         public Decimal late_hour_reported { get; set; }
         public string union_code { get; set; }
         public int negative_qualified_years { get; set; }
         public int year_end_data_extraction_header_id { get; set; }
    }
    [Serializable]
    public enum enmDataExtractionBatchHourInfo
    {
         data_extraction_batch_hour_info_id ,
         data_extraction_batch_info_id ,
         person_id ,
         employer_no ,
         employer_name ,
         computation_year ,
         hours_reported ,
         late_hour_reported ,
         union_code ,
         negative_qualified_years ,
         year_end_data_extraction_header_id ,
    }
}

