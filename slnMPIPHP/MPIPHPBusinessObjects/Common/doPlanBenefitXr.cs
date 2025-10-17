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
	/// Class MPIPHP.DataObjects.doPlanBenefitXr:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlanBenefitXr : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPlanBenefitXr() : base()
         {
         }
         public int plan_benefit_id { get; set; }
         public int plan_id { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public string retirement_flag { get; set; }
         public string withdrawal_flag { get; set; }
         public string disability_flag { get; set; }
         public string death_pre_retirement_flag { get; set; }
         public string qdro_flag { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public string death_pre_retirement_post_election_flag { get; set; }
    }
    [Serializable]
    public enum enmPlanBenefitXr
    {
         plan_benefit_id ,
         plan_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         retirement_flag ,
         withdrawal_flag ,
         disability_flag ,
         death_pre_retirement_flag ,
         qdro_flag ,
         ee_flag ,
         uvhp_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         death_pre_retirement_post_election_flag ,
    }
}

