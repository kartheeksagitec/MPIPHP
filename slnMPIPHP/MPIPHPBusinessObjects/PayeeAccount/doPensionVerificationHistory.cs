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
	/// Class MPIPHP.DataObjects.doPensionVerificationHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPensionVerificationHistory : doBase
    {
         [NonSerialized]
         public static Hashtable ihstFields = null;
         public doPensionVerificationHistory() : base()
         {
         }
         public int pension_verification_history_id { get; set; }
         public int cycle_year { get; set; }
         public DateTime run_date { get; set; }
         public int person_id { get; set; }
         public DateTime verification_confirmation_letter_sent { get; set; }
         public DateTime received_date { get; set; }
         public DateTime ninety_days_letter_sent { get; set; }
         public DateTime sixty_days_letter_sent { get; set; }
         public DateTime thirty_days_letter_sent { get; set; }
         public DateTime resumption_letter_sent { get; set; }
         public DateTime benefit_date { get; set; }
         public string benefit_account_type { get; set; }
    }
    [Serializable]
    public enum enmPensionVerificationHistory
    {
         pension_verification_history_id ,
         cycle_year ,
         run_date ,
         person_id ,
         verification_confirmation_letter_sent ,
         received_date ,
         ninety_days_letter_sent ,
         sixty_days_letter_sent ,
         thirty_days_letter_sent ,
         resumption_letter_sent ,
         benefit_date ,
         benefit_account_type ,
    }
}

