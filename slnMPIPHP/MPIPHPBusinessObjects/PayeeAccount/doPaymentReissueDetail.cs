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
	/// Class MPIPHP.DataObjects.doPaymentReissueDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentReissueDetail : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPaymentReissueDetail() : base()
         {
         }
         public int payment_reissue_detail_id { get; set; }
         public int payment_history_distribution_id { get; set; }
         public int recipient_person_id { get; set; }
         public int recipient_rollover_org_id { get; set; }
         public int reissue_reason_id { get; set; }
         public string reissue_reason_description { get; set; }
         public string reissue_reason_value { get; set; }
         public int reissue_payment_type_id { get; set; }
         public string reissue_payment_type_description { get; set; }
         public string reissue_payment_type_value { get; set; }
         public int recipient_org_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentReissueDetail
    {
         payment_reissue_detail_id ,
         payment_history_distribution_id ,
         recipient_person_id ,
         recipient_rollover_org_id ,
         reissue_reason_id ,
         reissue_reason_description ,
         reissue_reason_value ,
         reissue_payment_type_id ,
         reissue_payment_type_description ,
         reissue_payment_type_value ,
         recipient_org_id ,
    }
}

