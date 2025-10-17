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
	/// Class MPIPHP.DataObjects.doPir:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPir : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPir() : base()
         {
         }
         public int pir_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string pir_description { get; set; }
         public int priority_id { get; set; }
         public string priority_description { get; set; }
         public string priority_value { get; set; }
         public int severity_id { get; set; }
         public string severity_description { get; set; }
         public string severity_value { get; set; }
         public int reported_by_id { get; set; }
         public int assigned_to_id { get; set; }
         public string screen_affected { get; set; }
         public string otherui_objects { get; set; }
         public string testcase_scenario { get; set; }
         public int defect_cause_id { get; set; }
         public string defect_cause_description { get; set; }
         public string defect_cause_value { get; set; }
         public DateTime date_resolved { get; set; }
         public int environment_id { get; set; }
         public string environment_description { get; set; }
         public string environment_value { get; set; }
         public int test_phase_id { get; set; }
         public string test_phase_description { get; set; }
         public string test_phase_value { get; set; }
         public int process_id { get; set; }
         public string process_description { get; set; }
         public string process_value { get; set; }
         public string defect_resolution { get; set; }
         public string release_info { get; set; }
         public DateTime due_date { get; set; }
         public string long_description { get; set; }
         public Decimal hours_worked { get; set; }
    }
    [Serializable]
    public enum enmPir
    {
         pir_id ,
         status_id ,
         status_description ,
         status_value ,
         pir_description ,
         priority_id ,
         priority_description ,
         priority_value ,
         severity_id ,
         severity_description ,
         severity_value ,
         reported_by_id ,
         assigned_to_id ,
         screen_affected ,
         otherui_objects ,
         testcase_scenario ,
         defect_cause_id ,
         defect_cause_description ,
         defect_cause_value ,
         date_resolved ,
         environment_id ,
         environment_description ,
         environment_value ,
         test_phase_id ,
         test_phase_description ,
         test_phase_value ,
         process_id ,
         process_description ,
         process_value ,
         defect_resolution ,
         release_info ,
         due_date ,
         long_description ,
         hours_worked ,
    }
}

