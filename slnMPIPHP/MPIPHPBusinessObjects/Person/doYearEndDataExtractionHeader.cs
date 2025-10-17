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
	/// Class MPIPHP.DataObjects.doYearEndDataExtractionHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doYearEndDataExtractionHeader : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doYearEndDataExtractionHeader() : base()
         {
         }
         public int year_end_data_extraction_header_id { get; set; }
         public int year { get; set; }
         public string is_annual_statement_generated_flag { get; set; }
    }
    [Serializable]
    public enum enmYearEndDataExtractionHeader
    {
         year_end_data_extraction_header_id ,
         year ,
         is_annual_statement_generated_flag ,
    }
}

