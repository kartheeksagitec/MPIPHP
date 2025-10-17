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
	/// Class MPIPHP.DataObjects.doSsa5500ReportDetails:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doSsa5500ReportDetails : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doSsa5500ReportDetails() : base()
         {
         }
         public string code { get; set; }
         public string ssn { get; set; }
         public string name { get; set; }
         public string type_of_annuity { get; set; }
         public string payment_frequency { get; set; }
         public Decimal gross_payment { get; set; }
         public int units_shares { get; set; }
         public Decimal total_value_account { get; set; }
         public int plan_year { get; set; }
         public string vested { get; set; }
         public string plan_identifier { get; set; }
         public int ssa_5500_report_id { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
    }
    [Serializable]
    public enum enmSsa5500ReportDetails
    {
         code ,
         ssn ,
         name ,
         type_of_annuity ,
         payment_frequency ,
         gross_payment ,
         units_shares ,
         total_value_account ,
         plan_year ,
         vested ,
         plan_identifier ,
         ssa_5500_report_id ,
         first_name ,
         middle_name ,
         last_name ,
    }
}

