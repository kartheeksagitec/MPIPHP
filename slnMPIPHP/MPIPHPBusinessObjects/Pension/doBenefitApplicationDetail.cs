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
	/// Class MPIPHP.DataObjects.doBenefitApplicationDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplicationDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doBenefitApplicationDetail() : base()
         {
         }
         public int benefit_application_detail_id { get; set; }
         public int benefit_application_id { get; set; }
         public int plan_benefit_id { get; set; }
         public int joint_annuitant_id { get; set; }
         public string spousal_consent_flag { get; set; }
         public int survivor_id { get; set; }
         public int organization_id { get; set; }
         public int benefit_subtype_id { get; set; }
         public string benefit_subtype_description { get; set; }
         public string benefit_subtype_value { get; set; }
         public string ee_flag { get; set; }
         public string uvhp_flag { get; set; }
         public string l52_spl_acc_flag { get; set; }
         public string l161_spl_acc_flag { get; set; }
         public int dro_model_id { get; set; }
         public string dro_model_description { get; set; }
         public string dro_model_value { get; set; }
         public int application_detail_status_id { get; set; }
         public string application_detail_status_description { get; set; }
         public string application_detail_status_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplicationDetail
    {
         benefit_application_detail_id ,
         benefit_application_id ,
         plan_benefit_id ,
         joint_annuitant_id ,
         spousal_consent_flag ,
         survivor_id ,
         organization_id ,
         benefit_subtype_id ,
         benefit_subtype_description ,
         benefit_subtype_value ,
         ee_flag ,
         uvhp_flag ,
         l52_spl_acc_flag ,
         l161_spl_acc_flag ,
         dro_model_id ,
         dro_model_description ,
         dro_model_value ,
         application_detail_status_id ,
         application_detail_status_description ,
         application_detail_status_value ,
    }
}

